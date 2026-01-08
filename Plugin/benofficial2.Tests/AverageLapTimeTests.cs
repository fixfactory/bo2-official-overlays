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
    public sealed class AverageLapTimeTests
    {
        [TestMethod]
        public void Average_3Laps()
        {
            var averageLapTime = new AverageLapTime(3);
            Assert.AreEqual(TimeSpan.FromSeconds(0), averageLapTime.GetAverageLapTime());

            averageLapTime.AddLapTime(1, TimeSpan.FromSeconds(90));
            Assert.AreEqual(TimeSpan.FromSeconds(90), averageLapTime.GetAverageLapTime());

            averageLapTime.AddLapTime(1, TimeSpan.FromSeconds(90));
            Assert.AreEqual(TimeSpan.FromSeconds(90), averageLapTime.GetAverageLapTime());

            averageLapTime.AddLapTime(2, TimeSpan.FromSeconds(91));
            Assert.AreEqual(TimeSpan.FromSeconds(90.5), averageLapTime.GetAverageLapTime());

            averageLapTime.AddLapTime(2, TimeSpan.FromSeconds(91));
            Assert.AreEqual(TimeSpan.FromSeconds(90.5), averageLapTime.GetAverageLapTime());

            averageLapTime.AddLapTime(3, TimeSpan.FromSeconds(92));
            Assert.AreEqual(TimeSpan.FromSeconds(91), averageLapTime.GetAverageLapTime());

            averageLapTime.AddLapTime(3, TimeSpan.FromSeconds(92));
            Assert.AreEqual(TimeSpan.FromSeconds(91), averageLapTime.GetAverageLapTime());

            averageLapTime.AddLapTime(4, TimeSpan.FromSeconds(93));
            Assert.AreEqual(TimeSpan.FromSeconds(92), averageLapTime.GetAverageLapTime());

            averageLapTime.AddLapTime(4, TimeSpan.FromSeconds(93));
            Assert.AreEqual(TimeSpan.FromSeconds(92), averageLapTime.GetAverageLapTime());
        }

        [TestMethod]
        public void InvalidateLap()
        {
            var averageLapTime = new AverageLapTime(3);
            Assert.AreEqual(TimeSpan.FromSeconds(0), averageLapTime.GetAverageLapTime());

            averageLapTime.AddLapTime(1, TimeSpan.FromSeconds(90));
            Assert.AreEqual(TimeSpan.FromSeconds(90), averageLapTime.GetAverageLapTime());

            // Can't invalidate a lap we already added.
            averageLapTime.InvalidateLap(1);
            Assert.AreEqual(TimeSpan.FromSeconds(90), averageLapTime.GetAverageLapTime());

            averageLapTime.InvalidateLap(2);
            Assert.AreEqual(TimeSpan.FromSeconds(90), averageLapTime.GetAverageLapTime());

            // This lap will be ignored because it was invalidated.
            averageLapTime.AddLapTime(2, TimeSpan.FromSeconds(91));
            Assert.AreEqual(TimeSpan.FromSeconds(90), averageLapTime.GetAverageLapTime());

            averageLapTime.AddLapTime(3, TimeSpan.FromSeconds(92));
            Assert.AreEqual(TimeSpan.FromSeconds(91), averageLapTime.GetAverageLapTime());
        }

        [TestMethod]
        public void DelayedLapTimeChange()
        {
            var averageLapTime = new AverageLapTime(3);
            Assert.AreEqual(TimeSpan.FromSeconds(0), averageLapTime.GetAverageLapTime());

            averageLapTime.AddLapTime(1, TimeSpan.FromSeconds(90));
            Assert.AreEqual(TimeSpan.FromSeconds(90), averageLapTime.GetAverageLapTime());

            // Lap incremented, but lap time hasn't been updated yet.
            averageLapTime.AddLapTime(2, TimeSpan.FromSeconds(90));
            Assert.AreEqual(TimeSpan.FromSeconds(90), averageLapTime.GetAverageLapTime());

            // Lap time finally updated.
            averageLapTime.AddLapTime(2, TimeSpan.FromSeconds(91));
            Assert.AreEqual(TimeSpan.FromSeconds(90.5), averageLapTime.GetAverageLapTime());
        }
    }
}
