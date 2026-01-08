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

using GameReaderCommon;
using SimHub.Plugins;
using SimHub.Plugins.DataPlugins.DataCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime;
using static benofficial2.Plugin.BroadcastMessage;

namespace benofficial2.Plugin
{
    public class FuelCalcSettings : ModuleSettings
    {
        public int BackgroundOpacity { get; set; } = 60;
        public ModuleSettingFloat FuelReserveLiters { get; set; } = new ModuleSettingFloat(0.5f);
        public ModuleSettingFloat ExtraRaceLaps { get; set; } = new ModuleSettingFloat(0.0f);
        public ModuleSettingFloat ExtraRaceLapsOval { get; set; } = new ModuleSettingFloat(3.0f);
        public ModuleSettingFloat ExtraConsumption { get; set; } = new ModuleSettingFloat(1.0f);
        public ModuleSettingFloat ExtraFuelPerStopLiters { get; set; } = new ModuleSettingFloat(0.3f);
        public ModuleSettingFloat ExtraDistance { get; set; } = new ModuleSettingFloat(0.5f);
        public bool FuelRemainingInfoVisible { get; set; } = true;
        public bool PitStopInfoVisible { get; set; } = true;
        public bool ConsumptionInfoVisible { get; set; } = true;
        public bool EnablePreRaceWarning { get; set; } = true;
        public bool EvenFuelStints { get; set; } = false;
        public bool AutoFuelEnabled { get; set; } = false;
        public int ConsumptionRecentLapCount { get; set; } = 3;
        public int ConsumptionPercentile { get; set; } = 75;

        // Legacy properties for backwards compatibility (saved pre 3.0)
        public string FuelReserveString { get => FuelReserveLiters.ValueString; set => FuelReserveLiters.ValueString = value; }
        public string ExtraLapsString { get => ExtraRaceLaps.ValueString; set => ExtraRaceLaps.ValueString = value; }
        public string ExtraConsumptionPctString { get => ExtraConsumption.ValueString; set => ExtraConsumption.ValueString = value; }
    }

    public class FuelCalcSession
    {
        public string TypeName { get; set; } = string.Empty;
        public string SubTypeName { get; set; } = string.Empty;
        public int Laps { get; set; } = 0;
        public TimeSpan Time { get; set; } = TimeSpan.Zero;
        public bool LimitedByTime { get; set; } = false;
        public double FuelNeeded { get; set; } = 0.0;
        public int StopsNeeded { get; set; } = 0;
    }

    public class FuelCalcModule : PluginModuleBase
    {
        private DriverModule _driverModule = null;
        private SessionModule _sessionModule = null;
        private TrackModule _trackModule = null;
        private StandingsModule _standingsModule = null;
        private CarModule _carModule = null;

        private DateTime _lastUpdateTime = DateTime.MinValue;
        private TimeSpan _updateInterval = TimeSpan.FromMilliseconds(1000);

        private string _lastCarTrackCombo = string.Empty;
        private bool _lastIsInPitLane = false;
        private bool _lastIsTowing = false;

        private FuelConsumptionTracker _consumptionTracker = new FuelConsumptionTracker();

        public FuelCalcSettings Settings { get; set; }
        public bool StartFuelCalculatorVisible { get; internal set; } = true;
        public TimeSpan BestLapTime { get; internal set; } = TimeSpan.Zero;
        public double SetupFuelLevel { get; internal set; } = 0.0;
        public double FuelLevel { get; internal set; } = 0.0;
        public double MaxFuel { get; internal set; } = 0.0;
        public double MaxFuelAllowed { get; internal set; } = 0.0;
        public string Units { get; internal set; } = "L";
        public double ConvertFromLiters { get; internal set; } = 1.0;
        public double ConvertFromSimHubUnits { get; internal set; } = 1.0;
        public double ConsumptionPerLapSafe { get; internal set; } = 0.0;
        public double ConsumptionPerLapAvg { get; internal set; } = 0.0;
        public double ConsumptionPerLapRecent { get; internal set; } = 0.0;
        public double ConsumptionLastLap { get; internal set; } = 0.0;
        public double ConsumptionTargetForExtraLap { get; internal set; } = 0.0;
        public double RemainingLaps { get; internal set; } = 0.0;
        public int EstimatedTotalLaps { get; internal set; } = 0;
        public int PitLap { get; internal set; } = 0;
        public bool PitIndicatorOn { get; internal set; } = false;
        public int PitWindowLap { get; internal set; } = 0;
        public bool PitWindowIndicatorOn { get; internal set; } = false;
        public int PitStopsNeeded { get; internal set; } = 0;
        public double RefuelNeeded { get; internal set; } = 0.0;
        public double ExtraFuelAtFinish { get; internal set; } = 0.0;
        public bool WarningVisible { get; internal set; } = false;
        public int TrackerValidLapCount { get { return _consumptionTracker.GetValidLapCount(); } }
        public bool TrackerLastLapValid { get { return _consumptionTracker.IsPreviousLapValid(); } }
        public double TrackerMedianConsumption { get; internal set; } = 0.0;
        public double TrackerRecentConsumption { get; internal set; } = 0.0;
        public double TrackerMinConsumption { get; internal set; } = 0.0;
        public double TrackerMaxConsumption { get; internal set; } = 0.0;

        public const int MaxSessions = 6;
        public List<FuelCalcSession> Sessions { get; internal set; }

        public override void Init(PluginManager pluginManager, benofficial2 plugin)
        {
            _driverModule = plugin.GetModule<DriverModule>();
            _sessionModule = plugin.GetModule<SessionModule>();
            _trackModule = plugin.GetModule<TrackModule>();
            _standingsModule = plugin.GetModule<StandingsModule>();
            _carModule = plugin.GetModule<CarModule>();

            Settings = plugin.ReadCommonSettings<FuelCalcSettings>("FuelCalcSettings", () => new FuelCalcSettings());
            plugin.AttachDelegate(name: "FuelCalc.BackgroundOpacity", valueProvider: () => Settings.BackgroundOpacity);
            plugin.AttachDelegate(name: "FuelCalc.FuelReserveLiters", valueProvider: () => Settings.FuelReserveLiters.Value);
            plugin.AttachDelegate(name: "FuelCalc.ExtraLaps", valueProvider: () => Settings.ExtraRaceLaps.Value);
            plugin.AttachDelegate(name: "FuelCalc.ExtraLapsOval", valueProvider: () => Settings.ExtraRaceLapsOval.Value);
            plugin.AttachDelegate(name: "FuelCalc.ExtraConsumptionPct", valueProvider: () => Settings.ExtraConsumption.Value);
            plugin.AttachDelegate(name: "FuelCalc.FuelRemainingInfoVisible", valueProvider: () => Settings.FuelRemainingInfoVisible);
            plugin.AttachDelegate(name: "FuelCalc.PitStopInfoVisible", valueProvider: () => Settings.PitStopInfoVisible);
            plugin.AttachDelegate(name: "FuelCalc.ConsumptionInfoVisible", valueProvider: () => Settings.ConsumptionInfoVisible);
            plugin.AttachDelegate(name: "FuelCalc.ConsumptionPercentile", valueProvider: () => Settings.ConsumptionPercentile);
            plugin.AttachDelegate(name: "FuelCalc.EnablePreRaceWarning", valueProvider: () => Settings.EnablePreRaceWarning);
            plugin.AttachDelegate(name: "FuelCalc.EvenFuelStints", valueProvider: () => Settings.EvenFuelStints);
            plugin.AttachDelegate(name: "FuelCalc.AutoFuelEnabled", valueProvider: () => Settings.AutoFuelEnabled);
            plugin.AttachDelegate(name: "FuelCalc.Visible", valueProvider: () => StartFuelCalculatorVisible);
            plugin.AttachDelegate(name: "FuelCalc.BestLapTime", valueProvider: () => BestLapTime);
            plugin.AttachDelegate(name: "FuelCalc.SetupFuelLevel", valueProvider: () => SetupFuelLevel);
            plugin.AttachDelegate(name: "FuelCalc.Fuel", valueProvider: () => FuelLevel);
            plugin.AttachDelegate(name: "FuelCalc.MaxFuel", valueProvider: () => MaxFuel);
            plugin.AttachDelegate(name: "FuelCalc.MaxFuelAllowed", valueProvider: () => MaxFuelAllowed);
            plugin.AttachDelegate(name: "FuelCalc.Units", valueProvider: () => Units);
            plugin.AttachDelegate(name: "FuelCalc.ConsumptionPerLap", valueProvider: () => ConsumptionPerLapRecent);
            plugin.AttachDelegate(name: "FuelCalc.ConsumptionPerLapAvg", valueProvider: () => ConsumptionPerLapAvg);
            plugin.AttachDelegate(name: "FuelCalc.ConsumptionLastLap", valueProvider: () => ConsumptionLastLap);
            plugin.AttachDelegate(name: "FuelCalc.ConsumptionTargetForExtraLap", valueProvider: () => ConsumptionTargetForExtraLap);
            plugin.AttachDelegate(name: "FuelCalc.RemainingLaps", valueProvider: () => RemainingLaps);
            plugin.AttachDelegate(name: "FuelCalc.EstimatedTotalLaps", valueProvider: () => EstimatedTotalLaps);
            plugin.AttachDelegate(name: "FuelCalc.PitLap", valueProvider: () => PitLap);
            plugin.AttachDelegate(name: "FuelCalc.PitIndicatorOn", valueProvider: () => PitIndicatorOn);
            plugin.AttachDelegate(name: "FuelCalc.PitWindowLap", valueProvider: () => PitWindowLap);
            plugin.AttachDelegate(name: "FuelCalc.PitWindowIndicatorOn", valueProvider: () => PitWindowIndicatorOn);
            plugin.AttachDelegate(name: "FuelCalc.PitStopsNeeded", valueProvider: () => PitStopsNeeded);
            plugin.AttachDelegate(name: "FuelCalc.RefuelNeeded", valueProvider: () => RefuelNeeded);
            plugin.AttachDelegate(name: "FuelCalc.ExtraFuelAtFinish", valueProvider: () => ExtraFuelAtFinish);
            plugin.AttachDelegate(name: "FuelCalc.WarningVisible", valueProvider: () => WarningVisible);
            plugin.AttachDelegate(name: "FuelCalc.Tracker.ValidLapCount", valueProvider: () => TrackerValidLapCount);
            plugin.AttachDelegate(name: "FuelCalc.Tracker.LastLapValid", valueProvider: () => TrackerLastLapValid);
            plugin.AttachDelegate(name: "FuelCalc.Tracker.MedianConsumption", valueProvider: () => TrackerMedianConsumption);
            plugin.AttachDelegate(name: "FuelCalc.Tracker.RecentConsumption", valueProvider: () => TrackerRecentConsumption);
            plugin.AttachDelegate(name: "FuelCalc.Tracker.MinConsumption", valueProvider: () => TrackerMinConsumption);
            plugin.AttachDelegate(name: "FuelCalc.Tracker.MaxConsumption", valueProvider: () => TrackerMaxConsumption);

            Sessions = new List<FuelCalcSession>(Enumerable.Range(0, MaxSessions).Select(x => new FuelCalcSession()));
            for (int sessionIdx = 0; sessionIdx < MaxSessions; sessionIdx++)
            {
                FuelCalcSession session = Sessions[sessionIdx];
                plugin.AttachDelegate(name: $"FuelCalc.Session{sessionIdx:00}.TypeName", valueProvider: () => session.TypeName);
                plugin.AttachDelegate(name: $"FuelCalc.Session{sessionIdx:00}.SubTypeName", valueProvider: () => session.SubTypeName);
                plugin.AttachDelegate(name: $"FuelCalc.Session{sessionIdx:00}.Laps", valueProvider: () => session.Laps);
                plugin.AttachDelegate(name: $"FuelCalc.Session{sessionIdx:00}.Time", valueProvider: () => session.Time);
                plugin.AttachDelegate(name: $"FuelCalc.Session{sessionIdx:00}.LimitedByTime", valueProvider: () => session.LimitedByTime);
                plugin.AttachDelegate(name: $"FuelCalc.Session{sessionIdx:00}.FuelNeeded", valueProvider: () => session.FuelNeeded);
                plugin.AttachDelegate(name: $"FuelCalc.Session{sessionIdx:00}.StopsNeeded", valueProvider: () => session.StopsNeeded);
            }
        }

        public override void DataUpdate(PluginManager pluginManager, benofficial2 plugin, ref GameData data)
        {
            // Update consumption tracker every frame for precise new lap detection.
            UpdateConsumptionTracker(ref data);

            RawDataHelper.TryGetTelemetryData<double>(ref data, out double fuelLevel, "FuelLevel");
            FuelLevel = fuelLevel * ConvertFromLiters;

            if (data.FrameTime - _lastUpdateTime < _updateInterval) return;
            _lastUpdateTime = data.FrameTime;

            RawDataHelper.TryGetTelemetryData<bool>(ref data, out bool isGarageVisible, "IsGarageVisible");

            StartFuelCalculatorVisible = isGarageVisible;
            UpdateWarningVisible(ref data);

            // Remember the last valid consumption & time for this car/track combo
            // So if we immediately come back to the same car/track combo, we can reuse the values
            string carTrackCombo = data.NewData.CarId + "_" + data.NewData.TrackId;
            bool carTrackComboValid = data.NewData.CarId.Length > 0 && data.NewData.TrackId.Length > 0;
            if (carTrackComboValid && carTrackCombo != _lastCarTrackCombo)
            {
                _consumptionTracker.Reset();
                ConsumptionPerLapAvg = 0.0;
                ConsumptionPerLapRecent = 0.0;
                BestLapTime = TimeSpan.Zero;
                _lastCarTrackCombo = carTrackCombo;
            }

            UpdateSetupFuelLevel(ref data);
            UpdateConsumptionPerLap(pluginManager, ref data);

            RawDataHelper.TryGetSessionData<double>(ref data, out double driverCarFuelMaxLtr, "DriverInfo", "DriverCarFuelMaxLtr");
            MaxFuel = driverCarFuelMaxLtr * ConvertFromLiters;
            MaxFuelAllowed = MaxFuel * _sessionModule.MaxFuelPct;

            UpdateBestLapTime(ref data);

            bool beforeRaceStart = _sessionModule.Race && !_sessionModule.RaceStarted;
            if (StartFuelCalculatorVisible || beforeRaceStart)
            {
                UpdateAllSessionFuel(ref data);
            }

            UpdateFuelCalculations(ref data);
            UpdateAutoFuel(ref data);
        }

        public override void End(PluginManager pluginManager, benofficial2 plugin)
        {
            plugin.SaveCommonSettings("FuelCalcSettings", Settings);
        }

        private void UpdateBestLapTime(ref GameData data)
        {
            dynamic raw = data.NewData.GetRawDataObject();
            if (raw == null)
                return;

            int driverCount = 0;
            try { driverCount = (int)raw.AllSessionData["DriverInfo"]["Drivers"].Count; } catch { Debug.Assert(false); }

            int playerCarIdx = -1;
            try { playerCarIdx = int.Parse(raw.AllSessionData["DriverInfo"]["DriverCarIdx"]); } catch { Debug.Assert(false); }

            if (playerCarIdx < 0 || playerCarIdx >= driverCount)
                return;

            string playerClassId = data.NewData.CarClass;
            try { playerClassId = raw.AllSessionData["DriverInfo"]["Drivers"][playerCarIdx]["CarClassID"]; } catch { Debug.Assert(false); }

            // Try to find the fastest time of any session
            int sessionCount = 0;
            try { sessionCount = (int)raw.AllSessionData["SessionInfo"]["Sessions"].Count; } catch { Debug.Assert(false); }

            double fastestTime = 0;
            for (int sessionIdx = 0; sessionIdx < sessionCount; sessionIdx++)
            {
                List<object> positions = null;
                try { positions = raw.AllSessionData["SessionInfo"]["Sessions"][sessionIdx]["ResultsPositions"]; } catch { Debug.Assert(false); }
                if (positions == null)
                    continue;

                for (int posIdx = 0; posIdx < positions.Count; posIdx++)
                {
                    int carIdx = -1;
                    try { carIdx = int.Parse(raw.AllSessionData["SessionInfo"]["Sessions"][sessionIdx]["ResultsPositions"][posIdx]["CarIdx"]); } catch { Debug.Assert(false); }

                    if (!_driverModule.DriversByCarIdx.TryGetValue(carIdx, out Driver driver))
                    {
                        continue;
                    }

                    string classId = string.Empty;
                    try { classId = raw.AllSessionData["DriverInfo"]["Drivers"][driver.DriverInfoIdx]["CarClassID"]; } catch { Debug.Assert(false); }

                    // Must be in same class as player
                    if (playerClassId == null || playerClassId == classId)
                    {
                        double timeSecs = 0;
                        try { timeSecs = double.Parse(raw.AllSessionData["SessionInfo"]["Sessions"][sessionIdx]["ResultsPositions"][posIdx]["FastestTime"]); } catch { Debug.Assert(false); }

                        if (timeSecs > 0 && (timeSecs < fastestTime || fastestTime == 0))
                        {
                            fastestTime = timeSecs;
                        }
                    }
                }
            }

            if (fastestTime > 0)
            {
                BestLapTime = TimeSpan.FromSeconds(fastestTime);
            }
        }

        private void UpdateSetupFuelLevel(ref GameData data)
        {
            string fuelLevelStr = GetSetupFuelLevel(ref data);

            RawDataHelper.TryGetSessionData<double>(ref data, out double driverCarFuelKgPerLtr, "DriverInfo", "DriverCarFuelKgPerLtr");
            if (driverCarFuelKgPerLtr <= 0)
                driverCarFuelKgPerLtr = Constants.FuelKgPerLiter;

            if (fuelLevelStr.IndexOf("L") != -1)
            {
                double fuelLevelLiters = double.Parse(fuelLevelStr.Substring(0, fuelLevelStr.Length - 2), CultureInfo.InvariantCulture);
                if (data.NewData.FuelUnit == "Liters")
                {
                    Units = "L";
                    ConvertFromLiters = 1.0;
                    ConvertFromSimHubUnits = 1.0;
                    SetupFuelLevel = fuelLevelLiters;
                }
                else
                {
                    Units = "gal";
                    ConvertFromLiters = Constants.GallonPerLiter;
                    ConvertFromSimHubUnits = 1.0;
                    SetupFuelLevel = fuelLevelLiters * Constants.GallonPerLiter;
                }
            }
            else if (fuelLevelStr.IndexOf("Kg") != -1)
            {
                double fuelLevelKg = double.Parse(fuelLevelStr.Substring(0, fuelLevelStr.Length - 3), CultureInfo.InvariantCulture);

                if (data.NewData.FuelUnit == "Liters")
                {
                    Units = "Kg";
                    ConvertFromLiters = driverCarFuelKgPerLtr;
                    ConvertFromSimHubUnits = driverCarFuelKgPerLtr;
                    SetupFuelLevel = fuelLevelKg;
                }
                else
                {
                    Units = "lb";
                    ConvertFromLiters = Constants.PoundPerKg * driverCarFuelKgPerLtr;
                    ConvertFromSimHubUnits = (1 / Constants.GallonPerLiter) * ConvertFromLiters;
                    SetupFuelLevel = fuelLevelKg * Constants.PoundPerKg;
                }
            }
            else
            {
                SetupFuelLevel = 0.0;

                if (data.NewData.FuelUnit == "Liters")
                {
                    Units = "L";
                    ConvertFromLiters = 1.0;
                }
                else
                {
                    Units = "gal";
                    ConvertFromLiters = Constants.GallonPerLiter;
                }
            }
        }

        private string GetSetupFuelLevel(ref GameData data)
        {
            // Search for fuel level in various places in the car setup.
            string fuelLevel;
            if (RawDataHelper.TryGetSessionData<string>(ref data, out fuelLevel, "CarSetup", "Suspension", "Rear", "FuelLevel"))
                return fuelLevel;

            if (RawDataHelper.TryGetSessionData<string>(ref data, out fuelLevel, "CarSetup", "Chassis", "Rear", "FuelLevel"))
                return fuelLevel;

            if (RawDataHelper.TryGetSessionData<string>(ref data, out fuelLevel, "CarSetup", "Chassis", "Rear", "FuelFillTo"))
                return fuelLevel;

            if (RawDataHelper.TryGetSessionData<string>(ref data, out fuelLevel, "CarSetup", "Chassis", "Front", "FuelLevel"))
                return fuelLevel;

            if (RawDataHelper.TryGetSessionData<string>(ref data, out fuelLevel, "CarSetup", "Chassis", "BrakesInCarMisc", "FuelLevel"))
                return fuelLevel;

            if (RawDataHelper.TryGetSessionData<string>(ref data, out fuelLevel, "CarSetup", "BrakesDriveUnit", "Fuel", "FuelLevel"))
                return fuelLevel;

            if (RawDataHelper.TryGetSessionData<string>(ref data, out fuelLevel, "CarSetup", "InCarSystems", "Fuel", "FuelLevel"))
                return fuelLevel;

            if (RawDataHelper.TryGetSessionData<string>(ref data, out fuelLevel, "CarSetup", "Systems", "Fuel", "FuelLevel"))
                return fuelLevel;

            if (RawDataHelper.TryGetSessionData<string>(ref data, out fuelLevel, "CarSetup", "VehicleSystems", "Fuel", "FuelLevel"))
                return fuelLevel;

            if (RawDataHelper.TryGetSessionData<string>(ref data, out fuelLevel, "CarSetup", "TiresFuel", "Fuel", "FuelLevel"))
                return fuelLevel;

            return string.Empty;
        }

        private void UpdateConsumptionPerLap(PluginManager pluginManager, ref GameData data)
        {
            var dataCorePlugin = pluginManager.GetPlugin<DataCorePlugin>();
            double fuelPerLapSimHub = dataCorePlugin.properties.Computed_Fuel_LitersPerLap.Value;
            double fuelLastLapSimHub = dataCorePlugin.properties.Computed_Fuel_LastLapConsumption.Value;

            // Even though the property is called "LitersPerLap", consumption will be in gallons when SimHub is set to gallons.
            ConsumptionLastLap = fuelLastLapSimHub * ConvertFromSimHubUnits;

            TrackerMinConsumption = _consumptionTracker.GetConsumption(0) * ConvertFromLiters;
            TrackerMedianConsumption = _consumptionTracker.GetConsumption(Settings.ConsumptionPercentile) * ConvertFromLiters;
            TrackerMaxConsumption = _consumptionTracker.GetConsumption(100) * ConvertFromLiters;
            TrackerRecentConsumption = _consumptionTracker.GetRecentConsumption(Settings.ConsumptionRecentLapCount) * ConvertFromLiters;

            if (_consumptionTracker.GetValidLapCount() > 0)
            {
                ConsumptionPerLapAvg = TrackerMedianConsumption;                
                ConsumptionPerLapRecent = TrackerRecentConsumption;
            }
            else if (fuelPerLapSimHub > 0)
            {
                // Fallback to SimHub's computed fuel per lap if no valid laps are recorded.
                ConsumptionPerLapAvg = fuelPerLapSimHub * ConvertFromSimHubUnits;
                ConsumptionPerLapRecent = ConsumptionPerLapAvg;
            }

            ConsumptionPerLapSafe = ConsumptionPerLapAvg * (1 + Settings.ExtraConsumption / 100.0);
        }

        private void UpdateAllSessionFuel(ref GameData data)
        {
            for (int sessionIdx = 0; sessionIdx < MaxSessions; sessionIdx++)
            {
                UpdateSessionFuel(ref data, sessionIdx);
            }
        }

        private void UpdateSessionFuel(ref GameData data, int sessionIdx)
        {
            if (sessionIdx < 0 || sessionIdx >= MaxSessions)
                return;

            FuelCalcSession session = Sessions[sessionIdx];

            RawDataHelper.TryGetSessionData<List<object>>(ref data, out List<object> sessions, "SessionInfo", "Sessions");
            if (sessions == null || sessionIdx >= sessions.Count)
            {
                BlankSession(session);
                return;
            }

            RawDataHelper.TryGetSessionData<string>(ref data, out string sessionType, "SessionInfo", "Sessions", sessionIdx, "SessionType");
            session.TypeName = sessionType;

            RawDataHelper.TryGetSessionData<string>(ref data, out string sessionSubType, "SessionInfo", "Sessions", sessionIdx, "SessionSubType");
            session.SubTypeName = sessionSubType;

            RawDataHelper.TryGetSessionData<string>(ref data, out string sessionLaps, "SessionInfo", "Sessions", sessionIdx, "SessionLaps");
            if (sessionLaps.IndexOf("unlimited") != -1)
            {
                session.Laps = -1;
            }
            else
            {
                session.Laps = int.Parse(sessionLaps);
            }

            RawDataHelper.TryGetSessionData<string>(ref data, out string sessionTime, "SessionInfo", "Sessions", sessionIdx, "SessionTime");
            if (sessionTime.IndexOf("unlimited") != -1 || sessionTime.Length <= 4)
            {
                session.Time = TimeSpan.Zero;
            }
            else
            {
                double sessionTimeSecs = double.Parse(sessionTime.Substring(0, sessionTime.Length - 4));
                session.Time = TimeSpan.FromSeconds(sessionTimeSecs);
            }

            if (session.Laps < 0 && session.Time == TimeSpan.MinValue)
            {
                session.FuelNeeded = 0.0;
                session.StopsNeeded = 0;
                return;
            }

            TimeSpan minTimeForLaps = TimeSpan.Zero;
            if (session.Laps > 0 && BestLapTime > TimeSpan.Zero)
            {
                minTimeForLaps = TimeSpan.FromSeconds(session.Laps * BestLapTime.TotalSeconds);
            }

            if (session.Laps > 0 && (session.Time <= TimeSpan.Zero || minTimeForLaps < session.Time))
            {
                session.LimitedByTime = false;
                session.FuelNeeded = ConsumptionPerLapSafe * session.Laps;
            }
            else
            {
                session.LimitedByTime = true;
                if (BestLapTime <= TimeSpan.Zero)
                {
                    // Can't compute fuel without a best lap time.
                    session.FuelNeeded = 0.0;
                    session.StopsNeeded = 0;
                    return;
                }

                double lapsEstimate = Math.Ceiling(session.Time.TotalSeconds / BestLapTime.TotalSeconds);

                // Add an extra lap if we would cross the line with more than X% of a lap remaining
                // It is unknown what are the exact white flag rule constants used by iRacing and seem to change per track.
                double remainingTimeSecs = session.Time.TotalSeconds % BestLapTime.TotalSeconds;
                if (remainingTimeSecs > Constants.WhiteFlagRuleLapPct * BestLapTime.TotalSeconds)
                {
                    lapsEstimate++;
                }

                session.FuelNeeded = ConsumptionPerLapSafe * lapsEstimate;
            }

            double outLaps = 0;
            if (session.TypeName.IndexOf("Qual") != -1)
            {
                // Add an outlap for qualifying.
                // At some tracks (e.g. Nurburgring) the outlap is shorter.
                outLaps = 1 - _trackModule.QualStartTrackPct;
            }
            else if (session.TypeName.IndexOf("Race") != -1)
            {
                if (!_sessionModule.StandingStart)
                {
                    if (_sessionModule.Oval)
                    {
                        if (_trackModule.RaceStartTrackPct > 0.0f)
                        {
                            outLaps = 1 - _trackModule.RaceStartTrackPct;
                        }
                        else if (_sessionModule.ShortParadeLap)
                        {
                            outLaps = Constants.ShortParadeLapPct;
                        }
                        else if (_trackModule.TrackType == "super speedway")
                        {
                            outLaps = 1.0;
                        }
                        else
                        {
                            outLaps = 2.0;
                        }
                    }
                    else
                    {
                        // Add an outlap for the formation lap on road courses.
                        // At some tracks (e.g. Nurburgring) the formation lap is shorter.
                        outLaps = 1 - _trackModule.RaceStartTrackPct;
                    }
                }
            }

            session.FuelNeeded += ConsumptionPerLapSafe * outLaps;

            if (session.FuelNeeded > 0 && ConsumptionPerLapSafe > 0)
            {
                session.FuelNeeded += Settings.FuelReserveLiters * ConvertFromLiters;

                if (session.TypeName.IndexOf("Race") != -1)
                {
                    if (_sessionModule.Oval)
                    {
                        session.FuelNeeded += ConsumptionPerLapSafe * Settings.ExtraRaceLapsOval;
                    }
                    else
                    {
                        session.FuelNeeded += ConsumptionPerLapSafe * Settings.ExtraRaceLaps;
                    }
                }

                double extraDistanceLaps = (_trackModule.TrackLength > Constants.DistanceEpsilon) ? (Settings.ExtraDistance.Value / _trackModule.TrackLength) : 0.0;
                session.FuelNeeded += (extraDistanceLaps * ConsumptionPerLapSafe);
            }

            session.StopsNeeded = (int)Math.Floor(session.FuelNeeded / MaxFuelAllowed);
            if (session.StopsNeeded >= 1)
            {
                if (Settings.EvenFuelStints)
                {
                    // Don't fill up the tank to the maximum so that each stint is evenly fueled.
                    int stints = Math.Max(1, session.StopsNeeded + 1);
                    session.FuelNeeded = Math.Min(session.FuelNeeded / stints, MaxFuelAllowed);
                }
                else
                {
                    session.FuelNeeded = MaxFuelAllowed;
                }
            }
        }

        private void UpdateFuelCalculations(ref GameData data)
        {
            if (_standingsModule.HighlightedCarClassIdx >= 0 && _standingsModule.HighlightedCarClassIdx < StandingsModule.MaxCarClasses)
                EstimatedTotalLaps = _standingsModule.CarClasses[_standingsModule.HighlightedCarClassIdx].EstimatedTotalLaps;
            else
                EstimatedTotalLaps = 0;

            CalculateFuel(/*fuelLevel*/ FuelLevel,
                    /*consumptionPerLapAvg*/ ConsumptionPerLapAvg,
                    /*consumptionPerLapRecent*/ ConsumptionPerLapRecent,
                    /*currentLapHighPrecision*/ _driverModule.HighlightedDriver.CurrentLapHighPrecision,
                    /*estimatedTotalLaps*/ EstimatedTotalLaps,
                    /*isRace*/ _sessionModule.Race,
                    /*isOval*/ _sessionModule.Oval,
                    /*trackLength*/ _trackModule.TrackLength,
                    /*maxFuelAllowed*/ MaxFuelAllowed,
                    /*fuelReserve*/ Settings.FuelReserveLiters.Value * ConvertFromLiters,
                    /*extraConsumptionPct*/ Settings.ExtraConsumption,
                    /*extraRaceLaps*/ Settings.ExtraRaceLaps,
                    /*extraRaceLapsOval*/ Settings.ExtraRaceLapsOval,
                    /*extraFuelPerStop*/ Settings.ExtraFuelPerStopLiters.Value * ConvertFromLiters,
                    /*extraDistance*/ Settings.ExtraDistance.Value,
                    /*evenFuelStints*/ Settings.EvenFuelStints,
                    out double remainingLaps,
                    out int pitLap,
                    out int pitWindowLap,
                    out int pitStopsNeeded,
                    out double refuelNeeded,
                    out bool pitIndicatorOn,
                    out bool pitWindowIndicatorOn,
                    out double extraFuelAtFinish,
                    out double consumptionTargetForExtraLap);

            RemainingLaps = remainingLaps;
            PitLap = pitLap;
            PitWindowLap = pitWindowLap;
            PitStopsNeeded = pitStopsNeeded;
            RefuelNeeded = refuelNeeded;
            PitIndicatorOn = pitIndicatorOn;
            PitWindowIndicatorOn = pitWindowIndicatorOn;
            ExtraFuelAtFinish = extraFuelAtFinish;
            ConsumptionTargetForExtraLap = consumptionTargetForExtraLap;
        }

        static public void CalculateFuel(double fuelLevel,
            double consumptionPerLapAvg,
            double consumptionPerLapRecent,
            double currentLapHighPrecision,
            int estimatedTotalLaps,
            bool isRace,
            bool isOval,
            double trackLength,
            double maxFuelAllowed,
            double fuelReserve,
            double extraConsumptionPct,
            double extraRaceLaps,
            double extraRaceLapsOval,
            double extraFuelPerStop,
            double extraDistance,
            bool evenFuelStints,
            out double remainingLaps,
            out int pitLap,
            out int pitWindowLap,
            out int pitStopsNeeded,
            out double refuelNeeded,
            out bool pitIndicatorOn,
            out bool pitWindowIndicatorOn,
            out double extraFuelAtFinish,
            out double consumptionTargetForExtraLap)
        {
            if (consumptionPerLapAvg < Constants.FuelEpsilon || consumptionPerLapRecent < Constants.FuelEpsilon)
            {
                remainingLaps = 0.0;
                pitLap = 0;
                pitStopsNeeded = 0;
                refuelNeeded = 0.0;
                pitWindowLap = 0;
                pitIndicatorOn = false;
                pitWindowIndicatorOn = false;
                extraFuelAtFinish = 0.0;
                consumptionTargetForExtraLap = 0.0;
                return;
            }

            double consumptionPerLapSafe = consumptionPerLapAvg * (1 + extraConsumptionPct / 100.0);
            double fuelLeftSafe = Math.Max(0.0, fuelLevel - fuelReserve);
            remainingLaps = fuelLeftSafe / consumptionPerLapRecent;

            // Determine the lap when we run out of fuel and pit the lap before that. Or on the current lap if we went beyond.
            int currentLap = Math.Max(0, (int)Math.Ceiling(currentLapHighPrecision));
            pitLap = Math.Max(currentLap, (int)Math.Floor(currentLapHighPrecision + remainingLaps));

            int lapsToFinishAfterNextStop = Math.Max(0, estimatedTotalLaps - pitLap);
            if (lapsToFinishAfterNextStop > 0)
            {
                pitIndicatorOn = currentLapHighPrecision >= (pitLap - 1);
                extraFuelAtFinish = 0.0;

                double lapsToNextStop = Math.Max(0.0, pitLap - currentLapHighPrecision);
                double fuelToNextStop = lapsToNextStop * consumptionPerLapRecent;
                double fuelLeftAtStop = Math.Max(0.0, fuelLevel - fuelToNextStop);
                double fuelToFinishSafe = (lapsToFinishAfterNextStop * consumptionPerLapSafe) + fuelReserve;

                if (isRace)
                {
                    if (isOval)
                        fuelToFinishSafe += (extraRaceLapsOval * consumptionPerLapSafe);
                    else
                        fuelToFinishSafe += (extraRaceLaps * consumptionPerLapSafe);
                }

                double extraDistanceLaps = (trackLength > Constants.DistanceEpsilon) ? (extraDistance / trackLength) : 0.0;
                fuelToFinishSafe += (extraDistanceLaps * consumptionPerLapSafe);

                double pitStopsNeededProvisional = (int)Math.Floor(fuelToFinishSafe / maxFuelAllowed) + 1;
                fuelToFinishSafe += (pitStopsNeededProvisional * extraFuelPerStop);
                pitStopsNeeded = (int)Math.Floor(fuelToFinishSafe / maxFuelAllowed) + 1;

                if (pitStopsNeeded > pitStopsNeededProvisional)
                {
                    fuelToFinishSafe += extraFuelPerStop;
                }

                if (evenFuelStints)
                {
                    // Don't fill up the tank to the maximum so that each stint is evenly fueled.
                    refuelNeeded = Math.Max(0.0, Math.Min(maxFuelAllowed, (fuelToFinishSafe / pitStopsNeeded)) - fuelLeftAtStop);
                }
                else
                {
                    refuelNeeded = Math.Max(0.0, Math.Min(maxFuelAllowed, fuelToFinishSafe) - fuelLeftAtStop);
                }

                double maxRefuel = maxFuelAllowed * pitStopsNeeded;
                double maxRefuelSafe = Math.Max(0.0, maxRefuel - fuelReserve);
                double maxLapsAfterRefuel = Math.Max(0.0, maxRefuelSafe / consumptionPerLapSafe);

                if (isRace)
                {
                    if (isOval)
                        maxLapsAfterRefuel = Math.Max(0.0, maxLapsAfterRefuel - extraRaceLapsOval);
                    else
                        maxLapsAfterRefuel = Math.Max(0.0, maxLapsAfterRefuel - extraRaceLaps);
                }

                pitWindowLap = Math.Max(0, estimatedTotalLaps - (int)Math.Floor(maxLapsAfterRefuel));
                pitWindowIndicatorOn = Math.Max(0.0, currentLapHighPrecision) >= (pitWindowLap - 1);
            }
            else
            {
                if (estimatedTotalLaps > 0)
                {
                    double lapsToFinish = Math.Max(0.0, estimatedTotalLaps - currentLapHighPrecision);
                    double fuelToFinish = (lapsToFinish * consumptionPerLapRecent) + fuelReserve;
                    extraFuelAtFinish = Math.Max(0.0, fuelLevel - fuelToFinish);

                    if (extraFuelAtFinish < Constants.FuelEpsilon)
                    {
                        // Went beyond the Pit Lap and will probably run out of fuel.
                        double fuelToFinishSafe = (lapsToFinish * consumptionPerLapSafe) + fuelReserve;

                        if (isRace)
                        {
                            if (isOval)
                                fuelToFinishSafe += (extraRaceLapsOval * consumptionPerLapSafe);
                            else
                                fuelToFinishSafe += (extraRaceLaps * consumptionPerLapSafe);
                        }

                        pitIndicatorOn = true;
                        pitStopsNeeded = (int)Math.Floor(fuelToFinishSafe / maxFuelAllowed) + 1;
                        refuelNeeded = Math.Max(0.0, Math.Min(maxFuelAllowed, fuelToFinishSafe) - fuelLevel);
                        pitWindowLap = currentLap;
                        pitWindowIndicatorOn = true;
                    }
                    else
                    {
                        pitIndicatorOn = false;
                        pitStopsNeeded = 0;
                        refuelNeeded = 0.0;
                        pitWindowLap = 0;
                        pitWindowIndicatorOn = false;
                    }
                }
                else
                {
                    extraFuelAtFinish = 0.0;
                    pitIndicatorOn = pitLap == currentLap;
                    pitStopsNeeded = 0;
                    refuelNeeded = 0.0;
                    pitWindowLap = 0;
                    pitWindowIndicatorOn = false;
                }
            }

            // Calculate the consumption target for an extra lap.
            double lapsToNextStopExtra = Math.Max(0.0, (pitLap + 1) - currentLapHighPrecision);
            consumptionTargetForExtraLap = (lapsToNextStopExtra > Constants.LapEpsilon) ? (fuelLeftSafe / lapsToNextStopExtra) : 0.0;
        }

        private void BlankSession(FuelCalcSession session)
        {
            session.TypeName = string.Empty;
            session.SubTypeName = string.Empty;
            session.Laps = 0;
            session.Time = TimeSpan.Zero;
            session.LimitedByTime = false;
            session.FuelNeeded = 0.0;
            session.StopsNeeded = 0;
        }

        private void UpdateWarningVisible(ref GameData data)
        {
            if (!_sessionModule.Race || !Settings.EnablePreRaceWarning || StartFuelCalculatorVisible || _sessionModule.RaceStarted)
            {
                WarningVisible = false;
                return;
            }

            RawDataHelper.TryGetTelemetryData<int>(ref data, out int sessionNum, "SessionNum");

            if (sessionNum < 0 || sessionNum >= MaxSessions)
            {
                WarningVisible = false;
                return;
            }

            FuelCalcSession session = Sessions[sessionNum];
            if (session.FuelNeeded <= 0 || SetupFuelLevel <= 0)
            {
                WarningVisible = false;
                return;
            }

            double errorMarginLiters = 0.05;
            double errorMargin = errorMarginLiters * ConvertFromLiters;
            if (SetupFuelLevel + errorMargin >= session.FuelNeeded)
            {
                WarningVisible = false;
                return;
            }

            WarningVisible = true;
        }

        private void UpdateAutoFuel(ref GameData data)
        {
            if (!Settings.AutoFuelEnabled)
                return;

            Driver playerDriver = _driverModule.GetPlayerDriver();
            if (playerDriver == null)
                return;

            bool enteringPitLane = data.NewData.IsInPitLane > 0 && !_lastIsInPitLane && data.NewData.SpeedKmh > 1;
            bool startedTowing = playerDriver.Towing && !_lastIsTowing;

            if (enteringPitLane || startedTowing)
            {
                int amountLiters = (int)Math.Ceiling(RefuelNeeded / ConvertFromLiters);
                if (amountLiters > 0)
                {
                    SendAddFuel(amountLiters);
                }
                else
                {
                    // Don't send a clear fuel command when total laps is unknown so that we get the default fuel level.
                    if (EstimatedTotalLaps > 0)
                    {
                        SendClearFuel();
                    }                    
                }
            }

            _lastIsInPitLane = data.NewData.IsInPitLane > 0;
            _lastIsTowing = playerDriver.Towing;
        }

        public void SendAddFuel(int amountLiters)
        {
            BroadcastMessage.Broadcast(BroadcastMessageTypes.PitCommand, (int)PitCommandModeTypes.Fuel, amountLiters, 0);
        }

        public void SendClearFuel()
        {
            BroadcastMessage.Broadcast(BroadcastMessageTypes.PitCommand, (int)PitCommandModeTypes.ClearFuel, 0, 0);
        }

        private void UpdateConsumptionTracker(ref GameData data)
        {
            // Only track when in the car.
            RawDataHelper.TryGetTelemetryData<int>(ref data, out int enterExitReset, "EnterExitReset");
            if (enterExitReset <= 0)
                return;
            
            RawDataHelper.TryGetTelemetryData<double>(ref data, out double fuelLevel, "FuelLevel");
            RawDataHelper.TryGetTelemetryData<int>(ref data, out int incidentCount, "PlayerCarMyIncidentCount");
            RawDataHelper.TryGetTelemetryData<double>(ref data, out double lapDistPct, "LapDistPct");

            // Only track full-course cautions on ovals.
            bool caution = _sessionModule.Oval && data.NewData.Flag_Yellow == 1;

            bool blackFlag = data.NewData.Flag_Black == 1;
            bool stopped = data.NewData.SpeedKmh < 1;
            bool onPitRoad = data.NewData.IsInPitLane > 0;
            bool paceLap = _sessionModule.Race && !_sessionModule.RaceStarted;
            bool invalidate = caution || onPitRoad || paceLap || stopped || blackFlag;

            _consumptionTracker.Update(lapDistPct, fuelLevel, invalidate, incidentCount);
        }
    }
}
