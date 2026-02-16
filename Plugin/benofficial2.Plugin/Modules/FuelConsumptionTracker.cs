/*
    benofficial2's Official Overlays
    Copyright (C) 2025-2026 benofficial2

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

namespace benofficial2.Plugin
{
    public class FuelConsumptionTracker
    {
        private const double LapPositionEpsilon = 1e-5;
        private const double FullLapThreshold = 0.95;

        private double _lapPositionStart = -1.0;
        private double _lapFuelStart = -1.0;
        private int _lapIncidentCount = 0;
        private bool _wasInvalidated = false;

        private double _lastLapPosition = -1.0;

        private bool _previousLapValid = false;

        private readonly List<double> _allConsumptions = new List<double>();

        public void Update(double lapPosition, double fuelLevel, bool invalidate, int incidentCount)
        {
            lapPosition = Math.Max(0.0, lapPosition);
            fuelLevel = Math.Max(0.0, fuelLevel);
            incidentCount = Math.Max(0, incidentCount);

            // First update → initialize
            if (_lastLapPosition < 0.0)
            {
                _lapPositionStart = lapPosition;
                _lastLapPosition = lapPosition;
                _lapFuelStart = fuelLevel;
                _wasInvalidated = invalidate;
                _lapIncidentCount = incidentCount;
                return;
            }

            // Detect lap completion (lapPosition wrapped around)
            if (lapPosition + LapPositionEpsilon < _lastLapPosition)
            {
                double lapFuelConsumed = _lapFuelStart - fuelLevel;
                bool incidentHappened = (incidentCount > _lapIncidentCount);
                bool fullLapCompleted = (_lastLapPosition - _lapPositionStart) > FullLapThreshold;

                if (fullLapCompleted && !_wasInvalidated && !incidentHappened && lapFuelConsumed > Constants.FuelEpsilon)
                {
                    _allConsumptions.Add(lapFuelConsumed);
                    _previousLapValid = true;
                }
                else
                {
                    _previousLapValid = false;
                }

                // Reset for next lap
                _lapPositionStart = lapPosition;
                _lapFuelStart = fuelLevel;
                _wasInvalidated = false;
                _lapIncidentCount = incidentCount;
            }

            // Update flags during current lap
            if (invalidate)
                _wasInvalidated = true;

            _lastLapPosition = lapPosition;
        }

        /// <summary>
        /// Average consumption over the last N valid laps.
        /// </summary>
        public double GetRecentConsumption(int lastLaps)
        {
            if (_allConsumptions.Count == 0 || lastLaps <= 0)
                return 0.0;

            int count = Math.Min(lastLaps, _allConsumptions.Count);
            int startIndex = _allConsumptions.Count - count;

            double sum = 0.0;
            for (int i = startIndex; i < _allConsumptions.Count; i++)
            {
                sum += _allConsumptions[i];
            }

            return sum / count;
        }

        /// <summary>
        /// Consumption at the given percentile (0–100) across ALL valid laps ever tracked.
        /// Example: 50 = median, 90 = 90th percentile.
        /// </summary>
        public double GetConsumption(int percentile)
        {
            if (_allConsumptions.Count == 0)
                return 0.0;

            percentile = Math.Max(0, Math.Min(100, percentile));

            var sorted = _allConsumptions.OrderBy(x => x).ToList();
            double index = (percentile / 100.0) * (sorted.Count - 1);
            int lower = (int)Math.Floor(index);
            int upper = (int)Math.Ceiling(index);

            if (lower == upper)
                return sorted[lower];

            double fraction = index - lower;
            return sorted[lower] + (sorted[upper] - sorted[lower]) * fraction;
        }

        public int GetValidLapCount() => _allConsumptions.Count;

        public bool IsPreviousLapValid() => _previousLapValid;

        public void Reset()
        {
            _lapPositionStart = -1.0;
            _lastLapPosition = -1.0;
            _previousLapValid = false;
            _wasInvalidated = false;
            _lapIncidentCount = 0;
            _lapFuelStart = -1.0;
            _allConsumptions.Clear();
        }
    }
}
