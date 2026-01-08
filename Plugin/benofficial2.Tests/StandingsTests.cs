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

using benofficial2.Plugin;

namespace benofficial2.Tests
{
    [TestClass]
    public sealed class StandingsTests
    {
        [TestMethod]
        public void EstimateTotalLaps_LapRace_EnoughTime()
        {
            int estimatedTotalLaps;
            bool extraLap;
            estimatedTotalLaps = StandingsModule.EstimateTotalLaps(/*currentLapHighPrecision*/ -1.0, /*sessionTotalLaps*/ 30, /*sessionTimeRemain*/ 3600.0, /*avgLapTime*/ 90.0, out extraLap);
            Assert.AreEqual(30, estimatedTotalLaps);
            Assert.IsFalse(extraLap);

            estimatedTotalLaps = StandingsModule.EstimateTotalLaps(/*currentLapHighPrecision*/ 0.0, /*sessionTotalLaps*/ 30, /*sessionTimeRemain*/ 3600.0, /*avgLapTime*/ 90.0, out extraLap);
            Assert.AreEqual(30, estimatedTotalLaps);
            Assert.IsFalse(extraLap);

            estimatedTotalLaps = StandingsModule.EstimateTotalLaps(/*currentLapHighPrecision*/ 15.0, /*sessionTotalLaps*/ 30, /*sessionTimeRemain*/ 3600.0, /*avgLapTime*/ 90.0, out extraLap);
            Assert.AreEqual(30, estimatedTotalLaps);
            Assert.IsFalse(extraLap);

            estimatedTotalLaps = StandingsModule.EstimateTotalLaps(/*currentLapHighPrecision*/ 30.0, /*sessionTotalLaps*/ 30, /*sessionTimeRemain*/ 3600.0, /*avgLapTime*/ 90.0, out extraLap);
            Assert.AreEqual(30, estimatedTotalLaps);
            Assert.IsFalse(extraLap);

            estimatedTotalLaps = StandingsModule.EstimateTotalLaps(/*currentLapHighPrecision*/ 31.0, /*sessionTotalLaps*/ 30, /*sessionTimeRemain*/ 3600.0, /*avgLapTime*/ 90.0, out extraLap);
            Assert.AreEqual(30, estimatedTotalLaps);
            Assert.IsFalse(extraLap);
        }

        [TestMethod]
        public void EstimateTotalLaps_TimeRace_ExtraLap_HitZeroFlush()
        {
            int estimatedTotalLaps;
            double avgLapTime = 90.0;
            bool extraLap;

            estimatedTotalLaps = StandingsModule.EstimateTotalLaps(/*currentLapHighPrecision*/ 0.0, /*sessionTotalLaps*/ 0, /*sessionTimeRemain*/ 900.0, avgLapTime, out extraLap);
            Assert.AreEqual(11, estimatedTotalLaps);
            Assert.IsTrue(extraLap);

            estimatedTotalLaps = StandingsModule.EstimateTotalLaps(/*currentLapHighPrecision*/ 8.7, /*sessionTotalLaps*/ 0, /*sessionTimeRemain*/ 117.0, avgLapTime, out extraLap);
            Assert.AreEqual(11, estimatedTotalLaps);
            Assert.IsTrue(extraLap);

            // Not getting the white flag here because the timer would hit zero just as would cross the line on the next lap. So we get an extra lap.
            estimatedTotalLaps = StandingsModule.EstimateTotalLaps(/*currentLapHighPrecision*/ 8.9, /*sessionTotalLaps*/ 0, /*sessionTimeRemain*/ 99.0, avgLapTime, out extraLap);
            Assert.AreEqual(11, estimatedTotalLaps);
            Assert.IsTrue(extraLap);

            estimatedTotalLaps = StandingsModule.EstimateTotalLaps(/*currentLapHighPrecision*/ 9.1, /*sessionTotalLaps*/ 0, /*sessionTimeRemain*/ 81.0, avgLapTime, out extraLap);
            Assert.AreEqual(11, estimatedTotalLaps);
            Assert.IsTrue(extraLap);

            estimatedTotalLaps = StandingsModule.EstimateTotalLaps(/*currentLapHighPrecision*/ 9.7, /*sessionTotalLaps*/ 0, /*sessionTimeRemain*/ 27.0, avgLapTime, out extraLap);
            Assert.AreEqual(11, estimatedTotalLaps);
            Assert.IsTrue(extraLap);

            // Getting the white flag here because the timer will hit zero just as we cross the line.
            estimatedTotalLaps = StandingsModule.EstimateTotalLaps(/*currentLapHighPrecision*/ 9.9, /*sessionTotalLaps*/ 0, /*sessionTimeRemain*/ 9.0, avgLapTime, out extraLap);
            Assert.AreEqual(11, estimatedTotalLaps);
            Assert.IsTrue(extraLap);

            // Should have had the white flag by now. Timer reached zero.
            estimatedTotalLaps = StandingsModule.EstimateTotalLaps(/*currentLapHighPrecision*/ 10.1, /*sessionTotalLaps*/ 0, /*sessionTimeRemain*/ 0.0, avgLapTime, out extraLap);
            Assert.AreEqual(11, estimatedTotalLaps);
            Assert.IsFalse(extraLap);

            estimatedTotalLaps = StandingsModule.EstimateTotalLaps(/*currentLapHighPrecision*/ 10.7, /*sessionTotalLaps*/ 0, /*sessionTimeRemain*/ 0.0, avgLapTime, out extraLap);
            Assert.AreEqual(11, estimatedTotalLaps);
            Assert.IsFalse(extraLap);

            // Estimate keeps going up at this point as if we did not get the white flag.
            estimatedTotalLaps = StandingsModule.EstimateTotalLaps(/*currentLapHighPrecision*/ 10.9, /*sessionTotalLaps*/ 0, /*sessionTimeRemain*/ 0.0, avgLapTime, out extraLap);
            //Assert.AreEqual(11, laps);
        }

        [TestMethod]
        public void EstimateTotalLaps_TimeRace_NoExtraLap_HitZeroEarly()
        {
            int estimatedTotalLaps;
            int sessionTotalLaps = 0;
            double currentLapHighPrecision, sessionTimeRemain;
            double avgLapTime = 89.0;
            double sessionTimeTotal = 900.0;
            bool extraLap;

            currentLapHighPrecision = 0.0;
            sessionTimeRemain = Math.Max(0.0, sessionTimeTotal - (avgLapTime * currentLapHighPrecision));
            estimatedTotalLaps = StandingsModule.EstimateTotalLaps(currentLapHighPrecision, sessionTotalLaps, sessionTimeRemain, avgLapTime, out extraLap);
            Assert.AreEqual(11, estimatedTotalLaps);
            Assert.IsFalse(extraLap);

            currentLapHighPrecision = 8.7;
            sessionTimeRemain = Math.Max(0.0, sessionTimeTotal - (avgLapTime * currentLapHighPrecision));
            estimatedTotalLaps = StandingsModule.EstimateTotalLaps( currentLapHighPrecision, sessionTotalLaps, sessionTimeRemain, avgLapTime, out extraLap);
            Assert.AreEqual(11, estimatedTotalLaps);
            Assert.IsFalse(extraLap);

            currentLapHighPrecision = 8.99;
            sessionTimeRemain = Math.Max(0.0, sessionTimeTotal - (avgLapTime * currentLapHighPrecision));
            estimatedTotalLaps = StandingsModule.EstimateTotalLaps(currentLapHighPrecision, sessionTotalLaps, sessionTimeRemain, avgLapTime, out extraLap);
            Assert.AreEqual(11, estimatedTotalLaps);
            Assert.IsFalse(extraLap);

            currentLapHighPrecision = 9.1;
            sessionTimeRemain = Math.Max(0.0, sessionTimeTotal - (avgLapTime * currentLapHighPrecision));
            estimatedTotalLaps = StandingsModule.EstimateTotalLaps(currentLapHighPrecision, sessionTotalLaps, sessionTimeRemain, avgLapTime, out extraLap);
            Assert.AreEqual(11, estimatedTotalLaps);
            Assert.IsFalse(extraLap);

            currentLapHighPrecision = 9.7;
            sessionTimeRemain = Math.Max(0.0, sessionTimeTotal - (avgLapTime * currentLapHighPrecision));
            estimatedTotalLaps = StandingsModule.EstimateTotalLaps(currentLapHighPrecision, sessionTotalLaps, sessionTimeRemain, avgLapTime, out extraLap);
            Assert.AreEqual(11, estimatedTotalLaps);
            Assert.IsFalse(extraLap);

            // Getting the white flag here because the timer will hit zero early in the next lap.
            currentLapHighPrecision = 9.99;
            sessionTimeRemain = Math.Max(0.0, sessionTimeTotal - (avgLapTime * currentLapHighPrecision));
            estimatedTotalLaps = StandingsModule.EstimateTotalLaps(currentLapHighPrecision, sessionTotalLaps, sessionTimeRemain, avgLapTime, out extraLap);
            Assert.AreEqual(11, estimatedTotalLaps);
            Assert.IsFalse(extraLap);

            // Got the white flag, timer almost zero.
            currentLapHighPrecision = 10.1;
            sessionTimeRemain = Math.Max(0.0, sessionTimeTotal - (avgLapTime * currentLapHighPrecision));
            estimatedTotalLaps = StandingsModule.EstimateTotalLaps(currentLapHighPrecision, sessionTotalLaps, sessionTimeRemain, avgLapTime, out extraLap);
            Assert.AreEqual(11, estimatedTotalLaps);
            Assert.IsFalse(extraLap);

            // Timer reached zero. 
            currentLapHighPrecision = 10.3;
            sessionTimeRemain = Math.Max(0.0, sessionTimeTotal - (avgLapTime * currentLapHighPrecision));
            estimatedTotalLaps = StandingsModule.EstimateTotalLaps(currentLapHighPrecision, sessionTotalLaps, sessionTimeRemain, avgLapTime, out extraLap);
            Assert.AreEqual(11, estimatedTotalLaps);
            Assert.IsFalse(extraLap);

            currentLapHighPrecision = 10.7;
            sessionTimeRemain = Math.Max(0.0, sessionTimeTotal - (avgLapTime * currentLapHighPrecision));
            estimatedTotalLaps = StandingsModule.EstimateTotalLaps(currentLapHighPrecision, sessionTotalLaps, sessionTimeRemain, avgLapTime, out extraLap);
            Assert.AreEqual(11, estimatedTotalLaps);
            Assert.IsFalse(extraLap);

            // Estimate keeps going up at this point as if we did not get the white flag.
            currentLapHighPrecision = 10.99;
            sessionTimeRemain = Math.Max(0.0, sessionTimeTotal - (avgLapTime * currentLapHighPrecision));
            estimatedTotalLaps = StandingsModule.EstimateTotalLaps(currentLapHighPrecision, sessionTotalLaps, sessionTimeRemain, avgLapTime, out extraLap);
            //Assert.AreEqual(11, laps);
        }

        [TestMethod]
        public void EstimateTotalLaps_TimeRace_ExtraLap_HitZeroLate()
        {
            int estimatedTotalLaps;
            int sessionTotalLaps = 0;
            double currentLapHighPrecision, sessionTimeRemain;
            double avgLapTime = 90.1;
            double sessionTimeTotal = 900.0;
            bool extraLap;

            currentLapHighPrecision = 0.0;
            sessionTimeRemain = Math.Max(0.0, sessionTimeTotal - (avgLapTime * currentLapHighPrecision));
            estimatedTotalLaps = StandingsModule.EstimateTotalLaps(currentLapHighPrecision, sessionTotalLaps, sessionTimeRemain, avgLapTime, out extraLap);
            Assert.AreEqual(11, estimatedTotalLaps);
            Assert.IsTrue(extraLap);

            currentLapHighPrecision = 8.7;
            sessionTimeRemain = Math.Max(0.0, sessionTimeTotal - (avgLapTime * currentLapHighPrecision));
            estimatedTotalLaps = StandingsModule.EstimateTotalLaps(currentLapHighPrecision, sessionTotalLaps, sessionTimeRemain, avgLapTime, out extraLap);
            Assert.AreEqual(11, estimatedTotalLaps);
            Assert.IsTrue(extraLap);

            // Not getting white flag here because timer could run out very close to crossing the line on next lap.
            currentLapHighPrecision = 8.99;
            sessionTimeRemain = Math.Max(0.0, sessionTimeTotal - (avgLapTime * currentLapHighPrecision));
            estimatedTotalLaps = StandingsModule.EstimateTotalLaps(currentLapHighPrecision, sessionTotalLaps, sessionTimeRemain, avgLapTime, out extraLap);
            Assert.AreEqual(11, estimatedTotalLaps);
            Assert.IsTrue(extraLap);

            currentLapHighPrecision = 9.1;
            sessionTimeRemain = Math.Max(0.0, sessionTimeTotal - (avgLapTime * currentLapHighPrecision));
            estimatedTotalLaps = StandingsModule.EstimateTotalLaps(currentLapHighPrecision, sessionTotalLaps, sessionTimeRemain, avgLapTime, out extraLap);
            Assert.AreEqual(11, estimatedTotalLaps);
            Assert.IsTrue(extraLap);

            currentLapHighPrecision = 9.7;
            sessionTimeRemain = Math.Max(0.0, sessionTimeTotal - (avgLapTime * currentLapHighPrecision));
            estimatedTotalLaps = StandingsModule.EstimateTotalLaps(currentLapHighPrecision, sessionTotalLaps, sessionTimeRemain, avgLapTime, out extraLap);
            Assert.AreEqual(11, estimatedTotalLaps);
            Assert.IsTrue(extraLap);

            // Timer hit zero, but we still have one more lap since we did not get the white flag on the previous lap.
            currentLapHighPrecision = 9.99;
            sessionTimeRemain = Math.Max(0.0, sessionTimeTotal - (avgLapTime * currentLapHighPrecision));
            estimatedTotalLaps = StandingsModule.EstimateTotalLaps(currentLapHighPrecision, sessionTotalLaps, sessionTimeRemain, avgLapTime, out extraLap);
            Assert.AreEqual(11, estimatedTotalLaps);
            Assert.IsTrue(extraLap);

            // Should have had the white flag by now.
            currentLapHighPrecision = 10.1;
            sessionTimeRemain = Math.Max(0.0, sessionTimeTotal - (avgLapTime * currentLapHighPrecision));
            estimatedTotalLaps = StandingsModule.EstimateTotalLaps(currentLapHighPrecision, sessionTotalLaps, sessionTimeRemain, avgLapTime, out extraLap);
            Assert.AreEqual(11, estimatedTotalLaps);
            Assert.IsFalse(extraLap);

            currentLapHighPrecision = 10.7;
            sessionTimeRemain = Math.Max(0.0, sessionTimeTotal - (avgLapTime * currentLapHighPrecision));
            estimatedTotalLaps = StandingsModule.EstimateTotalLaps(currentLapHighPrecision, sessionTotalLaps, sessionTimeRemain, avgLapTime, out extraLap);
            Assert.AreEqual(11, estimatedTotalLaps);
            Assert.IsFalse(extraLap);

            // Estimate keeps going up to 12 laps at this point as if we did not get the white flag.
            currentLapHighPrecision = 10.99;
            sessionTimeRemain = Math.Max(0.0, sessionTimeTotal - (avgLapTime * currentLapHighPrecision));
            estimatedTotalLaps = StandingsModule.EstimateTotalLaps(currentLapHighPrecision, sessionTotalLaps, sessionTimeRemain, avgLapTime, out extraLap);
            //Assert.AreEqual(11, laps);
        }

        [TestMethod]
        public void EstimateTotalLaps_TimeRace_NoExtraLap_HitZeroLate()
        {
            int estimatedTotalLaps;
            int sessionTotalLaps = 0;
            double currentLapHighPrecision, sessionTimeRemain;
            double avgLapTime = 92.0;
            double sessionTimeTotal = 900.0;
            bool extraLap;

            currentLapHighPrecision = 0.0;
            sessionTimeRemain = Math.Max(0.0, sessionTimeTotal - (avgLapTime * currentLapHighPrecision));
            estimatedTotalLaps = StandingsModule.EstimateTotalLaps(currentLapHighPrecision, sessionTotalLaps, sessionTimeRemain, avgLapTime, out extraLap);
            Assert.AreEqual(10, estimatedTotalLaps);
            Assert.IsFalse(extraLap);

            currentLapHighPrecision = 8.7;
            sessionTimeRemain = Math.Max(0.0, sessionTimeTotal - (avgLapTime * currentLapHighPrecision));
            estimatedTotalLaps = StandingsModule.EstimateTotalLaps(currentLapHighPrecision, sessionTotalLaps, sessionTimeRemain, avgLapTime, out extraLap);
            Assert.AreEqual(10, estimatedTotalLaps);
            Assert.IsFalse(extraLap);

            // Getting the white flag here because the timer would run out on the next lap.
            currentLapHighPrecision = 8.99;
            sessionTimeRemain = Math.Max(0.0, sessionTimeTotal - (avgLapTime * currentLapHighPrecision));
            estimatedTotalLaps = StandingsModule.EstimateTotalLaps(currentLapHighPrecision, sessionTotalLaps, sessionTimeRemain, avgLapTime, out extraLap);
            Assert.AreEqual(10, estimatedTotalLaps);
            Assert.IsFalse(extraLap);

            // Should have had the white flag here.
            currentLapHighPrecision = 9.1;
            sessionTimeRemain = Math.Max(0.0, sessionTimeTotal - (avgLapTime * currentLapHighPrecision));
            estimatedTotalLaps = StandingsModule.EstimateTotalLaps(currentLapHighPrecision, sessionTotalLaps, sessionTimeRemain, avgLapTime, out extraLap);
            Assert.AreEqual(10, estimatedTotalLaps);
            Assert.IsFalse(extraLap);

            currentLapHighPrecision = 9.7;
            sessionTimeRemain = Math.Max(0.0, sessionTimeTotal - (avgLapTime * currentLapHighPrecision));
            estimatedTotalLaps = StandingsModule.EstimateTotalLaps(currentLapHighPrecision, sessionTotalLaps, sessionTimeRemain, avgLapTime, out extraLap);
            Assert.AreEqual(10, estimatedTotalLaps);
            Assert.IsFalse(extraLap);

            // Timer hit zero
            // Estimate keeps going up at this point as if we did not get the white flag.
            currentLapHighPrecision = 9.99;
            sessionTimeRemain = Math.Max(0.0, sessionTimeTotal - (avgLapTime * currentLapHighPrecision));
            estimatedTotalLaps = StandingsModule.EstimateTotalLaps(currentLapHighPrecision, sessionTotalLaps, sessionTimeRemain, avgLapTime, out extraLap);
            //Assert.AreEqual(10, laps);

            currentLapHighPrecision = 10.1;
            sessionTimeRemain = Math.Max(0.0, sessionTimeTotal - (avgLapTime * currentLapHighPrecision));
            estimatedTotalLaps = StandingsModule.EstimateTotalLaps(currentLapHighPrecision, sessionTotalLaps, sessionTimeRemain, avgLapTime, out extraLap);
            //Assert.AreEqual(10, laps);

            currentLapHighPrecision = 10.7;
            sessionTimeRemain = Math.Max(0.0, sessionTimeTotal - (avgLapTime * currentLapHighPrecision));
            estimatedTotalLaps = StandingsModule.EstimateTotalLaps(currentLapHighPrecision, sessionTotalLaps, sessionTimeRemain, avgLapTime, out extraLap);
            //Assert.AreEqual(10, laps);

            currentLapHighPrecision = 10.99;
            sessionTimeRemain = Math.Max(0.0, sessionTimeTotal - (avgLapTime * currentLapHighPrecision));
            estimatedTotalLaps = StandingsModule.EstimateTotalLaps(currentLapHighPrecision, sessionTotalLaps, sessionTimeRemain, avgLapTime, out extraLap);
            //Assert.AreEqual(10, laps);
        }

        [TestMethod]
        public void EstimateTotalLaps_RealExamples()
        {
            int estimatedTotalLaps, sessionTotalLaps;
            double currentLapHighPrecision, sessionTimeRemain, avgLapTime;
            bool extraLap;

            // 10 min AI race at Mugello.
            currentLapHighPrecision = 4.9627079367637634;
            sessionTotalLaps = 0;
            sessionTimeRemain = 99.150000000000034;
            avgLapTime = 99.322666599999991;
            estimatedTotalLaps = StandingsModule.EstimateTotalLaps(currentLapHighPrecision, sessionTotalLaps, sessionTimeRemain, avgLapTime, out extraLap);
            Assert.AreEqual(6, estimatedTotalLaps);
            Assert.IsFalse(extraLap);
        }
    }
}
