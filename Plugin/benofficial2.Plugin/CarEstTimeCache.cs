/*
    benofficial2's Official Overlays
    Copyright (C) 2026 benofficial2

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace benofficial2.Plugin
{
    public class CarEstTimeCache
    {
        // Length of track in meters.
        private double _trackLength;

        // Length of each bucket in meters. This determines the granularity of our cache.
        private const double _bucketLength = 50.0;

        // Number of buckets calculated from _trackLength. Recomputed on Initialize(trackLength).
        private int _bucketCount;

        // Cache per carId: array of bucket entries (PosPct == NaN means not cached yet).
        // Keep as a Dictionary to minimize allocations on the hot path (TryGetValue + array access).
        private struct BucketEntry
        {
            public double PosPct; // exact track position percentage used when storing this bucket
            public double EstTime; // cached estTime
        }

        private readonly Dictionary<string, BucketEntry[]> _cache = new Dictionary<string, BucketEntry[]>();
        
        // Latest estimated lap time (time to reach PosPct == 1.0) per car
        private readonly Dictionary<string, double> _estLapTime = new Dictionary<string, double>();
        
        public CarEstTimeCache() { }


        public void Initialize(double trackLength)
        {
            _trackLength = trackLength;
            _bucketCount = Math.Max(1, (int)Math.Ceiling(_trackLength / _bucketLength));
            _cache.Clear();
            _estLapTime.Clear();
        }

        public void Clear()
        {
            _trackLength = 0.0;
            _bucketCount = 0;
            _cache.Clear();
            _estLapTime.Clear();
        }

        public bool IsInitialized => _trackLength > 0.0 && _bucketCount > 0;

        private void Initialize(string carId)
        {
            if (!IsInitialized)
                return;

            // Pre-allocate cache for this carId. Fill with NaN to indicate "empty" buckets.
            var arr = new BucketEntry[_bucketCount];
            for (int i = 0; i < _bucketCount; i++)
            {
                arr[i].PosPct = double.NaN;
                arr[i].EstTime = double.NaN;
            }

            _cache[carId] = arr;
        }

        public void AddEstTime(string carId, double trackPosPct, double estTime, double estLapTime)
        {
            if (!IsInitialized)
                return;

            // Fast path: get or create bucket array for this car.
            if (!_cache.TryGetValue(carId, out var arr))
            {
                Initialize(carId);
                arr = _cache[carId];
            }

            // Normalize trackPosPct into [0,1]
            var pct = trackPosPct;
            if (pct < 0.0) 
                pct = 0.0;
            else if (pct > 1.0) 
                pct = 1.0;

            // Compute bucket index and store value
            var meters = pct * _trackLength;
            var idx = (int)(meters / _bucketLength);

            if (idx < 0) idx = 0;
            if (idx >= _bucketCount) idx = _bucketCount - 1;

            arr[idx].EstTime = estTime;
            arr[idx].PosPct = pct;

            // Store latest estLapTime for this car (overwrite previous)
            _estLapTime[carId] = estLapTime;
        }

        public double GetEstTime(string carId, double trackPosPct)
        {
            // Fast-path lookups. If we don't have a cache for this car, return 0.0
            if (!_cache.TryGetValue(carId, out var arr))
                return 0.0;

            // Normalize trackPosPct into [0,1]
            var pct = trackPosPct;
            if (pct < 0.0) pct = 0.0;
            else if (pct > 1.0) pct = 1.0;

            var meters = pct * _trackLength;
            var idx = (int)(meters / _bucketLength);

            // Clamp idx into valid range
            if (idx < 0) idx = 0;
            if (idx >= _bucketCount) idx = _bucketCount - 1;

            // Choose candidate low bucket.
            int lowIdx = idx;
            var bucket0 = arr[lowIdx];

            if (double.IsNaN(bucket0.PosPct))
                return 0.0;

            if (pct < bucket0.PosPct)
            {
                // Special case when pct is before the first cached bucket. We can interpolate between start of track and the first bucket if it's valid.
                if (lowIdx == 0)
                {
                    var denom = bucket0.PosPct;
                    if (denom <= 0.0)
                        return 0.0;

                    var t = pct / denom;
                    if (t < 0.0) t = 0.0;
                    else if (t > 1.0) t = 1.0;

                    return bucket0.EstTime * (1.0 - t);
                }

                // Use previous bucket.
                lowIdx = lowIdx - 1;
                bucket0 = arr[lowIdx];

                if (double.IsNaN(bucket0.PosPct))
                    return 0.0;
            }

            int highIdx = lowIdx + 1;

            if (highIdx >= _bucketCount)
            {
                // Special case when highIdx would be out of range. We can interpolate between the last bucket and the lap end if valid.
                if (!_estLapTime.TryGetValue(carId, out var lapTime))
                    return 0.0;

                var denom = 1.0 - bucket0.PosPct;
                if (denom <= 0.0)
                    return 0.0;

                var t = (pct - bucket0.PosPct) / denom;
                if (t < 0.0) t = 0.0;
                else if (t > 1.0) t = 1.0;

                return bucket0.EstTime * (1.0 - t) + lapTime * t;
            }  
                       
            var bucket1 = arr[highIdx];

            if (double.IsNaN(bucket1.PosPct))
                return 0.0;

            // Use the exact stored PosPct values to compute interpolation fraction.
            var pos0 = bucket0.PosPct;
            var pos1 = bucket1.PosPct;

            if (pos1 <= pos0)
                return 0.0;

            var denom2 = pos1 - pos0;
            if (denom2 <= 0.0)
                return 0.0;

            var t2 = (pct - pos0) / denom2;
            if (t2 < 0.0) t2 = 0.0;
            else if (t2 > 1.0) t2 = 1.0;

            return bucket0.EstTime * (1.0 - t2) + bucket1.EstTime * t2;
        }
    }
}
