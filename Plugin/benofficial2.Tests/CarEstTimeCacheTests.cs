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

using benofficial2.Plugin;

namespace benofficial2.Tests
{
    [TestClass]
    public sealed class CarEstTimeCacheTests
    {
        [TestMethod]
        public void MiddleInterpolation()
        {
            var cache = new CarEstTimeCache();
            cache.Initialize(1000); // 1000m track -> 20 buckets

            // Put two adjacent buckets with known pos and est times
            // Suppose bucket index 4 (~200m-250m) and 5 (~250m-300m)
            cache.AddEstTime("car1", 0.21, 10.0, 999.0); // inside bucket 4
            cache.AddEstTime("car1", 0.26, 20.0, 999.0); // inside bucket 5

            // Query halfway between the two stored pos
            var midPct = (0.21 + 0.26) / 2.0;
            var est = cache.GetEstTime("car1", midPct);

            // Expect interpolation -> halfway between 10 and 20
            Assert.AreEqual(15.0, est, 1e-6);
        }

        [TestMethod]
        public void PrecedingBucketIsUsedWhenPosBeforeBucket()
        {
            var cache = new CarEstTimeCache();
            cache.Initialize(1000);

            // Prepare three buckets: index 3,4,5
            cache.AddEstTime("car3", 0.16, 10.0, 999.0);
            cache.AddEstTime("car3", 0.23, 30.0, 999.0);
            cache.AddEstTime("car3", 0.28, 50.0, 999.0);

            // Query a position just before bucket 4's stored PosPct
            var queryPct = 0.22;
            var est = cache.GetEstTime("car3", queryPct);

            // According to current logic, lowIdx will step back to bucket 3 and highIdx will be bucket 4.
            Assert.AreEqual(27.142857142857139, est, 1e-6);
        }

        [TestMethod]
        public void EdgeInterpolation()
        {
            var cache = new CarEstTimeCache();
            cache.Initialize(1000);

            // Put a value near the end of the track and a value near the start
            // For wrap tests we provide an estLapTime used to interpolate from last bucket to lap end (1.0)
            cache.AddEstTime("car2", 0.98, 100.0, 140.0); // near end, estLapTime=140
            cache.AddEstTime("car2", 0.02, 10.0, 140.0); // near start

            // Query just past the last bucket pos, 0.99 is halfway between 0.98 and 1.0 -> expect average of 100 and 140
            var estNearEnd = cache.GetEstTime("car2", 0.99);
            Assert.AreEqual(120.0, estNearEnd, 1e-6);

            // Query just before the first bucket pos, e.g., 0.01 is halfway between 0.0 and 0.02 -> expect half of 10
            var estNearStart = cache.GetEstTime("car2", 0.01);
            Assert.AreEqual(5.0, estNearStart, 1e-6);
        }
    }
}
