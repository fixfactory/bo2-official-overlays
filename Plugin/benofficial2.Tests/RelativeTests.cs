/*
    benofficial2's Official Overlays
    Copyright (C) 2025 benofficial2

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
    public sealed class RelativeTests
    {
        [TestMethod]
        public void RelativeTrackDistance()
        {
            // Ahead
            Assert.AreEqual(-0.2, RelativeModule.GetRelativeTrackDistance(0.1, 0.3), Constants.LapEpsilon);
            Assert.AreEqual(-0.2, RelativeModule.GetRelativeTrackDistance(0.4, 0.6), Constants.LapEpsilon);
            Assert.AreEqual(-0.2, RelativeModule.GetRelativeTrackDistance(0.7, 0.9), Constants.LapEpsilon);
            Assert.AreEqual(-0.2, RelativeModule.GetRelativeTrackDistance(0.9, 0.1), Constants.LapEpsilon);

            // Behind
            Assert.AreEqual(0.2, RelativeModule.GetRelativeTrackDistance(0.3, 0.1), Constants.LapEpsilon);
            Assert.AreEqual(0.2, RelativeModule.GetRelativeTrackDistance(0.6, 0.4), Constants.LapEpsilon);
            Assert.AreEqual(0.2, RelativeModule.GetRelativeTrackDistance(0.9, 0.7), Constants.LapEpsilon);
            Assert.AreEqual(0.2, RelativeModule.GetRelativeTrackDistance(0.1, 0.9), Constants.LapEpsilon);

            // Equal
            Assert.AreEqual(0.0, RelativeModule.GetRelativeTrackDistance(0.0, 0.0), Constants.LapEpsilon);
            Assert.AreEqual(0.0, RelativeModule.GetRelativeTrackDistance(0.5, 0.5), Constants.LapEpsilon);
            Assert.AreEqual(0.0, RelativeModule.GetRelativeTrackDistance(1.0, 1.0), Constants.LapEpsilon);

            // Edge cases
            Assert.AreEqual(0.5, RelativeModule.GetRelativeTrackDistance(0.1, 0.6), Constants.LapEpsilon);
            Assert.AreEqual(0.5, RelativeModule.GetRelativeTrackDistance(0.6, 0.1), Constants.LapEpsilon);
        }
    }
}
