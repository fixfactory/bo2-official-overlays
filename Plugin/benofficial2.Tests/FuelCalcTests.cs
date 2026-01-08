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
    public sealed class FuelCalcTests
    {
        [TestMethod]
        public void UnknownConsumption()
        {
            FuelCalcModule.CalculateFuel(/*fuelLevel*/ 0.0,
                /*consumptionPerLapAvg*/ 0.0,
                /*consumptionPerLapRecent*/ 0.0,
                /*currentLapHighPrecision*/ 1.0,
                /*estimatedTotalLaps*/ 20,
                /*isRace*/ false,
                /*isOval*/ false,
                /*trackLength*/ 3.0,
                /*maxFuelAllowed*/ 70,
                /*fuelReserve*/ 0.5,
                /*extraConsumptionPct*/ 1.0,
                /*extraRaceLaps*/ 0.0,
                /*extraRaceLapsOval*/ 3.0,
                /*extraFuelPerStop*/ 0.0,
                /*extraDistance*/ 0.0,
                /*evenFuelStints*/ false,
                out double remainingLaps,
                out int pitLap,
                out int pitWindowLap,
                out int pitStopsNeeded,
                out double refuelNeeded,
                out bool pitIndicatorOn,
                out bool pitWindowIndicatorOn,
                out double extraFuelAtFinish,
                out double consumptionTargetForExtraLap);

            Assert.AreEqual(remainingLaps, 0.0, Constants.LapEpsilon);
            Assert.AreEqual(pitLap, 0);
            Assert.AreEqual(pitWindowLap, 0);
            Assert.AreEqual(pitStopsNeeded, 0);
            Assert.AreEqual(refuelNeeded, 0.0, Constants.FuelEpsilon);
            Assert.AreEqual(pitIndicatorOn, false);
            Assert.AreEqual(pitWindowIndicatorOn, false);
            Assert.AreEqual(extraFuelAtFinish, 0.0, Constants.FuelEpsilon);
            Assert.AreEqual(consumptionTargetForExtraLap, 0.0, Constants.FuelEpsilon);
        }

        [TestMethod]
        public void RaceOverFueled()
        {
            FuelCalcModule.CalculateFuel(/*fuelLevel*/ 70.0,
                /*consumptionPerLapAvg*/ 0.5,
                /*consumptionPerLapRecent*/ 0.5,
                /*currentLapHighPrecision*/ 0.0,
                /*estimatedTotalLaps*/ 20,
                /*isRace*/ true,
                /*isOval*/ false,
                /*trackLength*/ 3.0,
                /*maxFuelAllowed*/ 70,
                /*fuelReserve*/ 0.5,
                /*extraConsumptionPct*/ 1.0,
                /*extraRaceLaps*/ 0.0,
                /*extraRaceLapsOval*/ 3.0,
                /*extraFuelPerStop*/ 0.0,
                /*extraDistance*/ 0.0,
                /*evenFuelStints*/ false,
                out double remainingLaps,
                out int pitLap,
                out int pitWindowLap,
                out int pitStopsNeeded,
                out double refuelNeeded,
                out bool pitIndicatorOn,
                out bool pitWindowIndicatorOn,
                out double extraFuelAtFinish,
                out double consumptionTargetForExtraLap);

            Assert.AreEqual(remainingLaps, 139.0, Constants.FuelEpsilon);
            Assert.AreEqual(pitLap, 139);
            Assert.AreEqual(pitWindowLap, 0);
            Assert.AreEqual(pitStopsNeeded, 0);
            Assert.AreEqual(refuelNeeded, 0.0, Constants.FuelEpsilon);
            Assert.AreEqual(pitIndicatorOn, false);
            Assert.AreEqual(pitWindowIndicatorOn, false);
            Assert.AreEqual(extraFuelAtFinish, 59.5, Constants.FuelEpsilon);
            Assert.AreEqual(consumptionTargetForExtraLap, 0.49642857142857144, Constants.FuelEpsilon);
        }

        [TestMethod]
        public void RaceOneStop()
        {
            FuelCalcModule.CalculateFuel(/*fuelLevel*/ 70.0,
                /*consumptionPerLapAvg*/ 0.5,
                /*consumptionPerLapRecent*/ 0.5,
                /*currentLapHighPrecision*/ 0.0,
                /*estimatedTotalLaps*/ 200,
                /*isRace*/ true,
                /*isOval*/ false,
                /*trackLength*/ 3.0,
                /*maxFuelAllowed*/ 70,
                /*fuelReserve*/ 0.5,
                /*extraConsumptionPct*/ 1.0,
                /*extraRaceLaps*/ 0.0,
                /*extraRaceLapsOval*/ 3.0,
                /*extraFuelPerStop*/ 0.0,
                /*extraDistance*/ 0.0,
                /*evenFuelStints*/ false,
                out double remainingLaps,
                out int pitLap,
                out int pitWindowLap,
                out int pitStopsNeeded,
                out double refuelNeeded,
                out bool pitIndicatorOn,
                out bool pitWindowIndicatorOn,
                out double extraFuelAtFinish,
                out double consumptionTargetForExtraLap);

            Assert.AreEqual(remainingLaps, 139.0, Constants.FuelEpsilon);
            Assert.AreEqual(pitLap, 139);
            Assert.AreEqual(pitWindowLap, 63);
            Assert.AreEqual(pitStopsNeeded, 1);
            Assert.AreEqual(refuelNeeded, 30.805, Constants.FuelEpsilon);
            Assert.AreEqual(pitIndicatorOn, false);
            Assert.AreEqual(pitWindowIndicatorOn, false);
            Assert.AreEqual(extraFuelAtFinish, 0.0, Constants.FuelEpsilon);
            Assert.AreEqual(consumptionTargetForExtraLap, 0.49642857142857144, Constants.FuelEpsilon);
        }

        [TestMethod]
        public void RaceOneStop_ExtraFuelPerStop()
        {
            FuelCalcModule.CalculateFuel(/*fuelLevel*/ 70.0,
                /*consumptionPerLapAvg*/ 0.5,
                /*consumptionPerLapRecent*/ 0.5,
                /*currentLapHighPrecision*/ 0.0,
                /*estimatedTotalLaps*/ 200,
                /*isRace*/ true,
                /*isOval*/ false,
                /*trackLength*/ 3.0,
                /*maxFuelAllowed*/ 70,
                /*fuelReserve*/ 0.5,
                /*extraConsumptionPct*/ 1.0,
                /*extraRaceLaps*/ 0.0,
                /*extraRaceLapsOval*/ 3.0,
                /*extraFuelPerStop*/ 0.3,
                /*extraDistance*/ 0.0,
                /*evenFuelStints*/ false,
                out double remainingLaps,
                out int pitLap,
                out int pitWindowLap,
                out int pitStopsNeeded,
                out double refuelNeeded,
                out bool pitIndicatorOn,
                out bool pitWindowIndicatorOn,
                out double extraFuelAtFinish,
                out double consumptionTargetForExtraLap);

            Assert.AreEqual(remainingLaps, 139.0, Constants.FuelEpsilon);
            Assert.AreEqual(pitLap, 139);
            Assert.AreEqual(pitWindowLap, 63);
            Assert.AreEqual(pitStopsNeeded, 1);
            Assert.AreEqual(refuelNeeded, 31.105, Constants.FuelEpsilon);
            Assert.AreEqual(pitIndicatorOn, false);
            Assert.AreEqual(pitWindowIndicatorOn, false);
            Assert.AreEqual(extraFuelAtFinish, 0.0, Constants.FuelEpsilon);
            Assert.AreEqual(consumptionTargetForExtraLap, 0.49642857142857144, Constants.FuelEpsilon);
        }

        [TestMethod]
        public void RaceOneStop_ExtraDistance()
        {
            FuelCalcModule.CalculateFuel(/*fuelLevel*/ 70.0,
                /*consumptionPerLapAvg*/ 0.5,
                /*consumptionPerLapRecent*/ 0.5,
                /*currentLapHighPrecision*/ 0.0,
                /*estimatedTotalLaps*/ 200,
                /*isRace*/ true,
                /*isOval*/ false,
                /*trackLength*/ 3.0,
                /*maxFuelAllowed*/ 70,
                /*fuelReserve*/ 0.5,
                /*extraConsumptionPct*/ 1.0,
                /*extraRaceLaps*/ 0.0,
                /*extraRaceLapsOval*/ 3.0,
                /*extraFuelPerStop*/ 0.3,
                /*extraDistance*/ 1.5,
                /*evenFuelStints*/ false,
                out double remainingLaps,
                out int pitLap,
                out int pitWindowLap,
                out int pitStopsNeeded,
                out double refuelNeeded,
                out bool pitIndicatorOn,
                out bool pitWindowIndicatorOn,
                out double extraFuelAtFinish,
                out double consumptionTargetForExtraLap);

            Assert.AreEqual(remainingLaps, 139.0, Constants.FuelEpsilon);
            Assert.AreEqual(pitLap, 139);
            Assert.AreEqual(pitWindowLap, 63);
            Assert.AreEqual(pitStopsNeeded, 1);
            Assert.AreEqual(refuelNeeded, 31.3575, Constants.FuelEpsilon);
            Assert.AreEqual(pitIndicatorOn, false);
            Assert.AreEqual(pitWindowIndicatorOn, false);
            Assert.AreEqual(extraFuelAtFinish, 0.0, Constants.FuelEpsilon);
            Assert.AreEqual(consumptionTargetForExtraLap, 0.49642857142857144, Constants.FuelEpsilon);
        }

        [TestMethod]
        public void RaceOneStopAtWindow()
        {
            FuelCalcModule.CalculateFuel(/*fuelLevel*/ 39.0,
                /*consumptionPerLapAvg*/ 0.5,
                /*consumptionPerLapRecent*/ 0.5,
                /*currentLapHighPrecision*/ 62.0,
                /*estimatedTotalLaps*/ 200,
                /*isRace*/ true,
                /*isOval*/ false,
                /*trackLength*/ 3.0,
                /*maxFuelAllowed*/ 70,
                /*fuelReserve*/ 0.5,
                /*extraConsumptionPct*/ 1.0,
                /*extraRaceLaps*/ 0.0,
                /*extraRaceLapsOval*/ 3.0,
                /*extraFuelPerStop*/ 0.0,
                /*extraDistance*/ 0.0,
                /*evenFuelStints*/ false,
                out double remainingLaps,
                out int pitLap,
                out int pitWindowLap,
                out int pitStopsNeeded,
                out double refuelNeeded,
                out bool pitIndicatorOn,
                out bool pitWindowIndicatorOn,
                out double extraFuelAtFinish,
                out double consumptionTargetForExtraLap);

            Assert.AreEqual(remainingLaps, 77.0, Constants.FuelEpsilon);
            Assert.AreEqual(pitLap, 139);
            Assert.AreEqual(pitWindowLap, 63);
            Assert.AreEqual(pitStopsNeeded, 1);
            Assert.AreEqual(refuelNeeded, 30.805, Constants.FuelEpsilon);
            Assert.AreEqual(pitIndicatorOn, false);
            Assert.AreEqual(pitWindowIndicatorOn, true);
            Assert.AreEqual(extraFuelAtFinish, 0.0, Constants.FuelEpsilon);
            Assert.AreEqual(consumptionTargetForExtraLap, 0.49358974358974361, Constants.FuelEpsilon);
        }

        [TestMethod]
        public void RaceOneStopAtPitLap()
        {
            FuelCalcModule.CalculateFuel(/*fuelLevel*/ 1.0,
                /*consumptionPerLapAvg*/ 0.5,
                /*consumptionPerLapRecent*/ 0.5,
                /*currentLapHighPrecision*/ 138.0,
                /*estimatedTotalLaps*/ 200,
                /*isRace*/ true,
                /*isOval*/ false,
                /*trackLength*/ 3.0,
                /*maxFuelAllowed*/ 70,
                /*fuelReserve*/ 0.5,
                /*extraConsumptionPct*/ 1.0,
                /*extraRaceLaps*/ 0.0,
                /*extraRaceLapsOval*/ 3.0,
                /*extraFuelPerStop*/ 0.0,
                /*extraDistance*/ 0.0,
                /*evenFuelStints*/ false,
                out double remainingLaps,
                out int pitLap,
                out int pitWindowLap,
                out int pitStopsNeeded,
                out double refuelNeeded,
                out bool pitIndicatorOn,
                out bool pitWindowIndicatorOn,
                out double extraFuelAtFinish,
                out double consumptionTargetForExtraLap);

            Assert.AreEqual(remainingLaps, 1.0, Constants.FuelEpsilon);
            Assert.AreEqual(pitLap, 139);
            Assert.AreEqual(pitWindowLap, 63);
            Assert.AreEqual(pitStopsNeeded, 1);
            Assert.AreEqual(refuelNeeded, 30.805, Constants.FuelEpsilon);
            Assert.AreEqual(pitIndicatorOn, true);
            Assert.AreEqual(pitWindowIndicatorOn, true);
            Assert.AreEqual(extraFuelAtFinish, 0.0, Constants.FuelEpsilon);
            Assert.AreEqual(consumptionTargetForExtraLap, 0.25, Constants.FuelEpsilon);
        }

        [TestMethod]
        public void RaceTwoStopEvenStints()
        {
            FuelCalcModule.CalculateFuel(/*fuelLevel*/ 70.0,
                /*consumptionPerLapAvg*/ 0.5,
                /*consumptionPerLapRecent*/ 0.5,
                /*currentLapHighPrecision*/ 0.0,
                /*estimatedTotalLaps*/ 300,
                /*isRace*/ true,
                /*isOval*/ false,
                /*trackLength*/ 3.0,
                /*maxFuelAllowed*/ 70,
                /*fuelReserve*/ 0.5,
                /*extraConsumptionPct*/ 1.0,
                /*extraRaceLaps*/ 0.0,
                /*extraRaceLapsOval*/ 3.0,
                /*extraFuelPerStop*/ 0.0,
                /*extraDistance*/ 0.0,
                /*evenFuelStints*/ true,
                out double remainingLaps,
                out int pitLap,
                out int pitWindowLap,
                out int pitStopsNeeded,
                out double refuelNeeded,
                out bool pitIndicatorOn,
                out bool pitWindowIndicatorOn,
                out double extraFuelAtFinish,
                out double consumptionTargetForExtraLap);

            Assert.AreEqual(remainingLaps, 139.0, Constants.FuelEpsilon);
            Assert.AreEqual(pitLap, 139);
            Assert.AreEqual(pitWindowLap, 24);
            Assert.AreEqual(pitStopsNeeded, 2);
            Assert.AreEqual(refuelNeeded, 40.4025, Constants.FuelEpsilon);
            Assert.AreEqual(pitIndicatorOn, false);
            Assert.AreEqual(pitWindowIndicatorOn, false);
            Assert.AreEqual(extraFuelAtFinish, 0.0, Constants.FuelEpsilon);
            Assert.AreEqual(consumptionTargetForExtraLap, 0.49642857142857144, Constants.FuelEpsilon);
        }

        [TestMethod]
        public void RaceOneStopOval()
        {
            FuelCalcModule.CalculateFuel(/*fuelLevel*/ 70.0,
                /*consumptionPerLapAvg*/ 0.5,
                /*consumptionPerLapRecent*/ 0.5,
                /*currentLapHighPrecision*/ 0.0,
                /*estimatedTotalLaps*/ 200,
                /*isRace*/ true,
                /*isOval*/ true,
                /*trackLength*/ 3.0,
                /*maxFuelAllowed*/ 70,
                /*fuelReserve*/ 0.5,
                /*extraConsumptionPct*/ 1.0,
                /*extraRaceLaps*/ 0.0,
                /*extraRaceLapsOval*/ 3.0,
                /*extraFuelPerStop*/ 0.0,
                /*extraDistance*/ 0.0,
                /*evenFuelStints*/ false,
                out double remainingLaps,
                out int pitLap,
                out int pitWindowLap,
                out int pitStopsNeeded,
                out double refuelNeeded,
                out bool pitIndicatorOn,
                out bool pitWindowIndicatorOn,
                out double extraFuelAtFinish,
                out double consumptionTargetForExtraLap);

            Assert.AreEqual(remainingLaps, 139.0, Constants.FuelEpsilon);
            Assert.AreEqual(pitLap, 139);
            Assert.AreEqual(pitWindowLap, 66);
            Assert.AreEqual(pitStopsNeeded, 1);
            Assert.AreEqual(refuelNeeded, 32.32, Constants.FuelEpsilon);
            Assert.AreEqual(pitIndicatorOn, false);
            Assert.AreEqual(pitWindowIndicatorOn, false);
            Assert.AreEqual(extraFuelAtFinish, 0.0, Constants.FuelEpsilon);
            Assert.AreEqual(consumptionTargetForExtraLap, 0.49642857142857144, Constants.FuelEpsilon);
        }

        [TestMethod]
        public void SFL_Mugello_RaceOneStop_PitNextLap()
        {
            // 11 min SFL AI race at Mugello, 10.5 L starting fuel.
            // Was suggesting a pit on 6/7
            FuelCalcModule.CalculateFuel(/*fuelLevel*/ 4.43250846862793,
                /*consumptionPerLapAvg*/ 1.4391992902796378,
                /*consumptionPerLapRecent*/ 1.4391992902796378,
                /*currentLapHighPrecision*/ 4.1070254445075989,
                /*estimatedTotalLaps*/ 7,
                /*isRace*/ true,
                /*isOval*/ false,
                /*trackLength*/ 3.0,
                /*maxFuelAllowed*/ 46,
                /*fuelReserve*/ 0.5,
                /*extraConsumptionPct*/ 1.0,
                /*extraRaceLaps*/ 0.0,
                /*extraRaceLapsOval*/ 3.0,
                /*extraFuelPerStop*/ 0.0,
                /*extraDistance*/ 0.0,
                /*evenFuelStints*/ false,
                out double remainingLaps,
                out int pitLap,
                out int pitWindowLap,
                out int pitStopsNeeded,
                out double refuelNeeded,
                out bool pitIndicatorOn,
                out bool pitWindowIndicatorOn,
                out double extraFuelAtFinish,
                out double consumptionTargetForExtraLap);

            Assert.AreEqual(2.7324280210448406, remainingLaps, Constants.FuelEpsilon);
            Assert.AreEqual(6, pitLap);
            Assert.AreEqual(0, pitWindowLap);
            Assert.AreEqual(1, pitStopsNeeded);
            Assert.AreEqual(0.24545045133658094, refuelNeeded, Constants.FuelEpsilon);
            Assert.AreEqual(false, pitIndicatorOn);
            Assert.AreEqual(true, pitWindowIndicatorOn);
            Assert.AreEqual(0.0, extraFuelAtFinish, Constants.FuelEpsilon);
            Assert.AreEqual(1.3593304722165431, consumptionTargetForExtraLap, Constants.FuelEpsilon);
        }

        [TestMethod]
        public void SFL_Mugello_RaceOneStop_PitThisLap()
        {
            // 11 min SFL AI race at Mugello, 10.5 L starting fuel.
            // Was suggesting a pit on 6/7
            FuelCalcModule.CalculateFuel(/*fuelLevel*/ 2.9937918186187744,
                /*consumptionPerLapAvg*/ 1.4374254413629475,
                /*consumptionPerLapRecent*/ 1.4374254413629475,
                /*currentLapHighPrecision*/ 5.1090390831232071,
                /*estimatedTotalLaps*/ 7,
                /*isRace*/ true,
                /*isOval*/ false,
                /*trackLength*/ 3.0,
                /*maxFuelAllowed*/ 46,
                /*fuelReserve*/ 0.5,
                /*extraConsumptionPct*/ 1.0,
                /*extraRaceLaps*/ 0.0,
                /*extraRaceLapsOval*/ 3.0,
                /*extraFuelPerStop*/ 0.0,
                /*extraDistance*/ 0.0,
                /*evenFuelStints*/ false,
                out double remainingLaps,
                out int pitLap,
                out int pitWindowLap,
                out int pitStopsNeeded,
                out double refuelNeeded,
                out bool pitIndicatorOn,
                out bool pitWindowIndicatorOn,
                out double extraFuelAtFinish,
                out double consumptionTargetForExtraLap);

            Assert.AreEqual(1.734901683842603, remainingLaps, Constants.FuelEpsilon);
            Assert.AreEqual(6, pitLap);
            Assert.AreEqual(0, pitWindowLap);
            Assert.AreEqual(1, pitStopsNeeded);
            Assert.AreEqual(0.23869776633656281, refuelNeeded, Constants.FuelEpsilon);
            Assert.AreEqual(true, pitIndicatorOn);
            Assert.AreEqual(true, pitWindowIndicatorOn);
            Assert.AreEqual(0.0, extraFuelAtFinish, Constants.FuelEpsilon);
            Assert.AreEqual(1.3187960662548477, consumptionTargetForExtraLap, Constants.FuelEpsilon);
        }

        [TestMethod]
        public void SFL_Mugello_RaceOneStop_MissedPitLap_EarlyInLap()
        {
            // 11 min SFL AI race at Mugello, 10.5 L starting fuel.
            // Was suggesting a pit on 6/7 for +0.2, but continued.
            FuelCalcModule.CalculateFuel(/*fuelLevel*/ 1.5842245817184448,
                /*consumptionPerLapAvg*/ 1.4420972638619318,
                /*consumptionPerLapRecent*/ 1.4420972638619318,
                /*currentLapHighPrecision*/ 6.08769016712904,
                /*estimatedTotalLaps*/ 7,
                /*isRace*/ true,
                /*isOval*/ false,
                /*trackLength*/ 3.0,
                /*maxFuelAllowed*/ 46,
                /*fuelReserve*/ 0.5,
                /*extraConsumptionPct*/ 1.0,
                /*extraRaceLaps*/ 0.0,
                /*extraRaceLapsOval*/ 3.0,
                /*extraFuelPerStop*/ 0.0,
                /*extraDistance*/ 0.0,
                /*evenFuelStints*/ false,
                out double remainingLaps,
                out int pitLap,
                out int pitWindowLap,
                out int pitStopsNeeded,
                out double refuelNeeded,
                out bool pitIndicatorOn,
                out bool pitWindowIndicatorOn,
                out double extraFuelAtFinish,
                out double consumptionTargetForExtraLap);

            Assert.AreEqual(0.75183873438251658, remainingLaps, Constants.FuelEpsilon);
            Assert.AreEqual(7, pitLap);
            Assert.AreEqual(7, pitWindowLap);
            Assert.AreEqual(1, pitStopsNeeded);
            Assert.AreEqual(0.2445713271968788, refuelNeeded, Constants.FuelEpsilon);
            Assert.AreEqual(true, pitIndicatorOn);
            Assert.AreEqual(true, pitWindowIndicatorOn);
            Assert.AreEqual(0.0, extraFuelAtFinish, Constants.FuelEpsilon);
            Assert.AreEqual(0.5669711900663571, consumptionTargetForExtraLap, Constants.FuelEpsilon);
        }

        [TestMethod]
        public void SFL_Mugello_RaceOneStop_MissedPitLap_LateInLap()
        {
            // 11 min SFL AI race at Mugello, 10.5 L starting fuel.
            // Was suggesting a pit on 6/7 for +0.2, but continued.
            FuelCalcModule.CalculateFuel(/*fuelLevel*/ 0.32208803296089172,
                /*consumptionPerLapAvg*/ 1.4496734014665857,
                /*consumptionPerLapRecent*/ 1.4496734014665857,
                /*currentLapHighPrecision*/ 6.9722132682800293,
                /*estimatedTotalLaps*/ 7,
                /*isRace*/ true,
                /*isOval*/ false,
                /*trackLength*/ 3.0,
                /*maxFuelAllowed*/ 46,
                /*fuelReserve*/ 0.5,
                /*extraConsumptionPct*/ 1.0,
                /*extraRaceLaps*/ 0.0,
                /*extraRaceLapsOval*/ 3.0,
                /*extraFuelPerStop*/ 0.0,
                /*extraDistance*/ 0.0,
                /*evenFuelStints*/ false,
                out double remainingLaps,
                out int pitLap,
                out int pitWindowLap,
                out int pitStopsNeeded,
                out double refuelNeeded,
                out bool pitIndicatorOn,
                out bool pitWindowIndicatorOn,
                out double extraFuelAtFinish,
                out double consumptionTargetForExtraLap);

            Assert.AreEqual(0.0, remainingLaps, Constants.FuelEpsilon);
            Assert.AreEqual(7, pitLap);
            Assert.AreEqual(7, pitWindowLap);
            Assert.AreEqual(1, pitStopsNeeded);
            Assert.AreEqual(0.21859646978611902, refuelNeeded, Constants.FuelEpsilon);
            Assert.AreEqual(true, pitIndicatorOn);
            Assert.AreEqual(true, pitWindowIndicatorOn);
            Assert.AreEqual(0.0, extraFuelAtFinish, Constants.FuelEpsilon);
            Assert.AreEqual(0.0, consumptionTargetForExtraLap, Constants.FuelEpsilon);
        }

        [TestMethod]
        public void SFL_Mugello_RaceNoStop_FinalLap()
        {
            // 10 min SFL AI race at Mugello, 10.5 L starting fuel.
            FuelCalcModule.CalculateFuel(/*fuelLevel*/ 3.0424654483795166,
                /*consumptionPerLapAvg*/ 1.4285655021667481,
                /*consumptionPerLapRecent*/ 1.4288019339243572,
                /*currentLapHighPrecision*/ 5.0918047651648521,
                /*estimatedTotalLaps*/ 6,
                /*isRace*/ true,
                /*isOval*/ false,
                /*trackLength*/ 3.0,
                /*maxFuelAllowed*/ 46,
                /*fuelReserve*/ 0.5,
                /*extraConsumptionPct*/ 1.0,
                /*extraRaceLaps*/ 0.0,
                /*extraRaceLapsOval*/ 3.0,
                /*extraFuelPerStop*/ 0.0,
                /*extraDistance*/ 0.0,
                /*evenFuelStints*/ false,
                out double remainingLaps,
                out int pitLap,
                out int pitWindowLap,
                out int pitStopsNeeded,
                out double refuelNeeded,
                out bool pitIndicatorOn,
                out bool pitWindowIndicatorOn,
                out double extraFuelAtFinish,
                out double consumptionTargetForExtraLap);

            Assert.AreEqual(1.7794386947646155, remainingLaps, Constants.FuelEpsilon);
            Assert.AreEqual(6, pitLap);
            Assert.AreEqual(0, pitWindowLap);
            Assert.AreEqual(0, pitStopsNeeded);
            Assert.AreEqual(0.0, refuelNeeded, Constants.FuelEpsilon);
            Assert.AreEqual(false, pitIndicatorOn);
            Assert.AreEqual(false, pitWindowIndicatorOn);
            Assert.AreEqual(1.2448343404661717, extraFuelAtFinish, Constants.FuelEpsilon);
            Assert.AreEqual(1.3323927247932597, consumptionTargetForExtraLap, Constants.FuelEpsilon);
        }

        [TestMethod]
        public void LateModel_SouthNational_Underfueled_Grid()
        {
            // 30 laps Late Model AI race at South National, gridding underfueled.
            FuelCalcModule.CalculateFuel(/*fuelLevel*/ 7.5616664886474609,
                /*consumptionPerLapAvg*/ 0.31981949806213378,
                /*consumptionPerLapRecent*/ 0.31924629211425781,
                /*currentLapHighPrecision*/ -0.10911405086517334,
                /*estimatedTotalLaps*/ 30,
                /*isRace*/ true,
                /*isOval*/ true,
                /*trackLength*/ 3.0,
                /*maxFuelAllowed*/ 83.867,
                /*fuelReserve*/ 0.5,
                /*extraConsumptionPct*/ 1.0,
                /*extraRaceLaps*/ 0.0,
                /*extraRaceLapsOval*/ 3.0,
                /*extraFuelPerStop*/ 0.0,
                /*extraDistance*/ 0.0,
                /*evenFuelStints*/ false,
                out double remainingLaps,
                out int pitLap,
                out int pitWindowLap,
                out int pitStopsNeeded,
                out double refuelNeeded,
                out bool pitIndicatorOn,
                out bool pitWindowIndicatorOn,
                out double extraFuelAtFinish,
                out double consumptionTargetForExtraLap);

            Assert.AreEqual(22.119807380942422, remainingLaps, Constants.FuelEpsilon);
            Assert.AreEqual(22, pitLap);
            Assert.AreEqual(0, pitWindowLap);
            Assert.AreEqual(1, pitStopsNeeded);
            Assert.AreEqual(3.54978081749279, refuelNeeded, Constants.FuelEpsilon);
            Assert.AreEqual(false, pitIndicatorOn);
            Assert.AreEqual(true, pitWindowIndicatorOn);
            Assert.AreEqual(0.0, extraFuelAtFinish, Constants.FuelEpsilon);
            Assert.AreEqual(0.30557928240364896, consumptionTargetForExtraLap, Constants.FuelEpsilon);
        }
    }
}
