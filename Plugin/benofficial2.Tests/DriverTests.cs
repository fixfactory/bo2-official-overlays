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
    public sealed class DriverTests
    {
        [TestMethod]
        public void IsAheadOnTrack()
        {
            Assert.IsTrue(DriverModule.IsAheadOnTrack(0.1, 0.3));
            Assert.IsTrue(DriverModule.IsAheadOnTrack(0.4, 0.6));
            Assert.IsTrue(DriverModule.IsAheadOnTrack(0.7, 0.9));
            Assert.IsTrue(DriverModule.IsAheadOnTrack(0.9, 0.1));
            Assert.IsFalse(DriverModule.IsAheadOnTrack(0.3, 0.1));
            Assert.IsFalse(DriverModule.IsAheadOnTrack(0.6, 0.4));
            Assert.IsFalse(DriverModule.IsAheadOnTrack(0.9, 0.7));
            Assert.IsFalse(DriverModule.IsAheadOnTrack(0.1, 0.9));
            Assert.IsFalse(DriverModule.IsAheadOnTrack(0.1, 0.1));
        }
    }
}
