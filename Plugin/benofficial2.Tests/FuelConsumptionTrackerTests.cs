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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using benofficial2.Plugin;

namespace benofficial2.Tests
{
    [TestClass]
    public class FuelConsumptionTrackerTests
    {
        [TestMethod]
        public void TestSingleValidLap()
        {
            var tracker = new FuelConsumptionTracker();

            // Lap 1: fuel goes from 10.0 → 9.0 (consume 1.0)
            tracker.Update(0.01, 10.0, false, 0);
            tracker.Update(0.99, 9.0, false, 0);
            tracker.Update(0.01, 9.0, false, 0); // wrap → lap ends

            Assert.AreEqual(1, tracker.GetValidLapCount());
            Assert.AreEqual(true, tracker.IsPreviousLapValid());
            Assert.AreEqual(1.0, tracker.GetRecentConsumption(1), 1e-6);
            Assert.AreEqual(1.0, tracker.GetRecentConsumption(3), 1e-6);
        }

        [TestMethod]
        public void TestInvalidLap_InvalidateFlag()
        {
            var tracker = new FuelConsumptionTracker();

            tracker.Update(0.01, 10.0, true, 0); // invalidated
            tracker.Update(0.99, 9.0, false, 0);
            tracker.Update(0.01, 9.0, false, 0); // lap ends

            Assert.AreEqual(0, tracker.GetValidLapCount());
            Assert.AreEqual(false, tracker.IsPreviousLapValid());
        }

        [TestMethod]
        public void TestInvalidLap_Incident()
        {
            var tracker = new FuelConsumptionTracker();

            tracker.Update(0.01, 10.0, false, 0);
            tracker.Update(0.99, 9.0, false, 1); // incident happened
            tracker.Update(0.01, 9.0, false, 1); // lap ends

            Assert.AreEqual(0, tracker.GetValidLapCount());
            Assert.AreEqual(false, tracker.IsPreviousLapValid());
        }

        [TestMethod]
        public void TestInvalidLap_Incomplete()
        {
            var tracker = new FuelConsumptionTracker();

            tracker.Update(0.01, 10.0, false, 0);
            tracker.Update(0.50, 9.5, false, 0); 
            tracker.Update(0.01, 9.0, false, 0); // teleport, incomplete lap

            Assert.AreEqual(0, tracker.GetValidLapCount());
            Assert.AreEqual(false, tracker.IsPreviousLapValid());
        }

        [TestMethod]
        public void TestInvalidLap_NegativePosition_Incomplete()
        {
            var tracker = new FuelConsumptionTracker();

            tracker.Update(0.01, 10.0, false, 0);
            tracker.Update(0.50, 9.5, false, 0);
            tracker.Update(-1, 9.0, false, 0); // negative position, incomplete lap

            Assert.AreEqual(0, tracker.GetValidLapCount());
            Assert.AreEqual(false, tracker.IsPreviousLapValid());
        }

        [TestMethod]
        public void TestInvalidLap_NegativePosition_Complete()
        {
            var tracker = new FuelConsumptionTracker();

            tracker.Update(0.01, 10.0, false, 0);
            tracker.Update(0.99, 9.0, false, 0);
            tracker.Update(-0.1, 9.0, false, 0); // negative position, complete lap

            Assert.AreEqual(1, tracker.GetValidLapCount());
            Assert.AreEqual(true, tracker.IsPreviousLapValid());
        }

        [TestMethod]
        public void TestInvalidLap_Reverse()
        {
            var tracker = new FuelConsumptionTracker();

            tracker.Update(0.01, 10.0, false, 0);
            tracker.Update(0.50, 9.5, false, 0);
            tracker.Update(0.49, 9.5, false, 0); // reverse on track
            tracker.Update(0.99, 9.0, false, 0);
            tracker.Update(0.01, 9.0, false, 0);

            Assert.AreEqual(0, tracker.GetValidLapCount());
            Assert.AreEqual(false, tracker.IsPreviousLapValid());
        }

        [TestMethod]
        public void TestMultipleValidLap()
        {
            var tracker = new FuelConsumptionTracker();

            // Lap 1: consume 1.0
            tracker.Update(0.01, 10.0, false, 0);
            tracker.Update(0.99, 9.0, false, 0);
            tracker.Update(0.01, 9.0, false, 0);

            // Lap 2: consume 1.1
            tracker.Update(0.2, 9.0, false, 0);
            tracker.Update(0.99, 7.9, false, 0);
            tracker.Update(0.01, 7.9, false, 0);

            // Lap 3: consume 1.2
            tracker.Update(0.3, 7.9, false, 0);
            tracker.Update(0.99, 6.7, false, 0);
            tracker.Update(0.01, 6.7, false, 0);

            // Lap 4: consume 1.3
            tracker.Update(0.4, 6.7, false, 0);
            tracker.Update(0.99, 5.4, false, 0);
            tracker.Update(0.01, 5.4, false, 0);

            // Lap 5: consume 1.4
            tracker.Update(0.5, 5.4, false, 0);
            tracker.Update(0.99, 4.0, false, 0);
            tracker.Update(0.01, 4.0, false, 0);

            Assert.AreEqual(5, tracker.GetValidLapCount(), "Should have 5 valid laps total");
            Assert.AreEqual(true, tracker.IsPreviousLapValid());

            // Check average of the most recent 3 laps (Laps 3, 4, 5 → 1.2, 1.3, 1.4)
            double expected = (1.2 + 1.3 + 1.4) / 3.0;
            double actual = tracker.GetRecentConsumption(3);

            Assert.AreEqual(expected, actual, 1e-6, "Recent 3 lap consumption average should match");
        }

        [TestMethod]
        public void TestPercentileConsumption()
        {
            var tracker = new FuelConsumptionTracker();

            // Lap 1: 1.0
            tracker.Update(0.01, 10.0, false, 0);
            tracker.Update(0.99, 9.0, false, 0);
            tracker.Update(0.01, 9.0, false, 0);

            // Lap 2: 2.0
            tracker.Update(0.01, 9.0, false, 0);
            tracker.Update(0.99, 7.0, false, 0);
            tracker.Update(0.01, 7.0, false, 0);

            // Lap 3: 3.0
            tracker.Update(0.01, 7.0, false, 0);
            tracker.Update(0.99, 4.0, false, 0);
            tracker.Update(0.01, 4.0, false, 0);

            Assert.AreEqual(3, tracker.GetValidLapCount());
            Assert.AreEqual(true, tracker.IsPreviousLapValid());

            // Median (50th percentile) should be 2.0
            Assert.AreEqual(2.0, tracker.GetConsumption(50), 1e-6);

            // 100th percentile should be max = 3.0
            Assert.AreEqual(3.0, tracker.GetConsumption(100), 1e-6);

            // 0th percentile should be min = 1.0
            Assert.AreEqual(1.0, tracker.GetConsumption(0), 1e-6);
        }

        [TestMethod]
        public void TestNegativePositionAtStartFinish()
        {
            var tracker = new FuelConsumptionTracker();

            // Lap 1: 1.0
            tracker.Update(0.01, 10.0, false, 0);
            tracker.Update(0.99, 9.0, false, 0);
            tracker.Update(0.01, 9.0, false, 0);

            // Lap 2: 2.0
            tracker.Update(0.01, 9.0, false, 0);
            tracker.Update(0.99, 7.0, false, 0);
            tracker.Update(0.01, 7.0, false, 0);

            // Lap 3: 3.0
            tracker.Update(0.01, 7.0, false, 0);
            tracker.Update(0.99, 4.0, false, 0);
            tracker.Update(0.01, 4.0, false, 0);

            // Lap 4: box
            tracker.Update(0.01, 7.0, false, 0);
            tracker.Update(0.99, 4.0, true, 0);
            tracker.Update(-0.01, 4.0, true, 0); // negative position at finish
            tracker.Update(0.01, 4.0, true, 0);

            Assert.AreEqual(3, tracker.GetValidLapCount());
            Assert.AreEqual(false, tracker.IsPreviousLapValid());

            // Median (50th percentile) should be 2.0
            Assert.AreEqual(2.0, tracker.GetConsumption(50), 1e-6);
        }
    }
}
