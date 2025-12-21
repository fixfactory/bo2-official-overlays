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

using GameReaderCommon;
using SimHub.Plugins;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime;

namespace benofficial2.Plugin
{
    public class HighlightedDriverSettings : ModuleSettings
    {
        public bool HideWhenUIVisible { get; set; } = false;
        public bool HideWhenInCar { get; set; } = true;
        public bool CarBrandVisible { get; set; } = true;
        public int Width { get; set; } = 30;
        public int BackgroundOpacity { get; set; } = 60;
    }

    public class AverageLapTime
    {
        private readonly Queue<TimeSpan> _lapTimes = new Queue<TimeSpan>();
        private readonly int _maxLapCount;
        private int _previousLap = -1;
        private int _invalidLap = -1;
        private TimeSpan _previousLapTime = TimeSpan.Zero;

        public AverageLapTime(int lapCount)
        {
            if (lapCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lapCount), "Lap count must be greater than zero.");
            }
            _maxLapCount = lapCount;
        }

        public void AddLapTime(int currentLap, TimeSpan lapTime)
        {
            if (lapTime <= TimeSpan.Zero)
                return;

            // Both must change to consider it a new valid lap.
            // This handles the case when the lap increments but the lap time hasn't been updated yet.
            if (currentLap == _previousLap || lapTime == _previousLapTime)
                return;

            _previousLap = currentLap;
            _previousLapTime = lapTime;            

            if (currentLap <= _invalidLap)
                return;

            if (_lapTimes.Count == _maxLapCount)
            {
                _lapTimes.Dequeue();
            }

            _lapTimes.Enqueue(lapTime);
            //SimHub.Logging.Current.Info($"AverageLapTime: Added lapTime={lapTime}, currentLap={currentLap}");
        }

        public void InvalidateLap(int currentLap)
        {
            if (currentLap != _invalidLap)
            {
                _invalidLap = currentLap;
                //SimHub.Logging.Current.Info($"AverageLapTime: Invalidated currentLap={currentLap}");
            }         
        }

        public TimeSpan GetAverageLapTime()
        {
            if (_lapTimes.Count == 0)
            {
                return TimeSpan.Zero;
            }

            long averageTicks = (long)_lapTimes.Average(ts => ts.Ticks);
            return new TimeSpan(averageTicks);
        }
    }

    public class Driver
    {
        // Index in the array AllSessionData["DriverInfo"]["Drivers"]
        public int DriverInfoIdx { get; set; } = -1;
        public int CarIdx { get; set; } = -1;
        public string CarId { get; set; } = string.Empty;
        public string CarName { get; set; } = string.Empty;
        public string CarNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string TeamName { get; set; } = string.Empty;
        public int FlairId { get; set; } = 0;
        public int CarClassId { get; set; } = 0;
        public string CarClassName { get; set; } = string.Empty;
        public string CarClassColor { get; set; } = string.Empty;
        public double CarClassEstLapTime { get; set; } = 0.0;
        public bool IsPlayer { get; set; } = false;
        public bool IsConnected { get; set; } = false;
        public bool IsPaceCar { get; set; } = false;
        public int Lap { get; set; } = 0;
        public int EnterPitLapUnconfirmed { get; set; } = -1;
        public int EnterPitLap { get; set; } = -1;
        public int ExitPitLap { get; set; } = -1;
        public bool OutLap { get; set; } = false;
        public bool InPit { get; set; } = false;
        public DateTime InPitSince { get; set; } = DateTime.MinValue;
        public bool InPitBox { get; set; } = false;
        public DateTime InPitBoxSince { get; set; } = DateTime.MinValue;
        public TimeSpan LastPitStopDuration { get; set; } = TimeSpan.Zero;
        public int StintLap { get; set; } = 0;
        public int Position { get; set; } = 0;
        public int PositionInClass { get; set; } = 0;
        public int QualPositionInClass { get; set; } = 0;
        public int LivePositionInClass { get; set; } = 0;
        public double LastCurrentLapHighPrecision { get; set; } = -1;
        public double CurrentLapHighPrecision { get; set; } = -1;
        public double CurrentLapHighPrecisionRaw { get; set; } = -1;
        public double TrackPositionPercent { get; set; } = -1;
        public bool Towing { get; set; } = false;
        public DateTime TowingEndTime { get; set; } = DateTime.MinValue;
        public TimeSpan LastLapTime { get; set; } = TimeSpan.Zero;
        public TimeSpan BestLapTime { get; set; } = TimeSpan.Zero;
        public TimeSpan QualLapTime { get; set; } = TimeSpan.Zero;
        public AverageLapTime AvgLapTime { get; set; } = new AverageLapTime(3);
        public int LapsCompleted { get; set; } = 0;
        public int JokerLapsCompleted { get; set; } = 0;
        public int SessionFlags { get; set; } = 0;
        public int TeamIncidentCount { get; set; } = 0;
        public int IRating { get; set; } = 0;
        public int IRatingChange { get; set; } = 0;
        public string License { get; set; } = string.Empty;
        public double SafetyRating { get; set; } = 0.0;
        public int LapsToClassLeader { get; set; } = 0;
        public double GapToClassLeader { get; set; } = 0.0;
        public int LapsToClassOpponentAhead { get; set; } = 0;
        public double GapToClassOpponentAhead { get; set; } = 0.0;
        public double RelativeGapToPlayer { get; set; } = 0.0;
        public double RelativeDistanceToPlayer { get; set; } = 0.0;
        public double EstTime { get; set; } = 0.0;
        public int TireCompoundIdx { get; set; } = -1;
        public int PushToPassCount { get; set; } = 0;
        public bool PushToPassActivated { get; set; } = false;
    }

    public class PlayerDriver
    {
        public int CarIdx { get; set; } = -1;
        public bool OutLap { get; internal set; } = false;
        public int StintLap { get; internal set; } = 0;
        public string Number { get; internal set; } = "";
        public string Name { get; set; } = string.Empty;
        public string CarBrand { get; internal set; } = "";
        public string CountryCode { get; internal set; } = "";
        public int SessionFlags { get; set; } = 0;
        public int IRating { get; set; } = 0;
        public int IRatingChange { get; set; } = 0;
        public string License { get; set; } = string.Empty;
        public double SafetyRating { get; set; } = 0.0;
        public int Position { get; set; } = 0;
        public int LivePositionInClass { get; internal set; } = 0;
        public bool HadWhiteFlag { get; internal set; } = false;
        public bool HadCheckeredFlag { get; internal set; } = false;
        public TimeSpan LastLapTime { get; internal set; } = TimeSpan.Zero;
        public TimeSpan BestLapTime { get; internal set; } = TimeSpan.Zero;
        public TimeSpan AvgLapTime { get; internal set; } = TimeSpan.Zero;
        public double CurrentLapHighPrecision { get; set; } = -1;
        public int CurrentLap { get; set; } = 0;
        public int TeamIncidentCount { get; set; } = 0;
        public string CarClassColor { get; set; } = string.Empty;
        public string TireCompound { get; set; } = string.Empty;
        public int PushToPassCount { get; set; } = 0;
        public bool PushToPassActivated { get; set; } = false;
    }

    public class HighlightedDriver
    {
        public int CarIdx { get; set; } = -1;
        public bool OutLap { get; internal set; } = false;
        public string Number { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string CarBrand { get; set; } = string.Empty;
        public string CarName { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
        public int SessionFlags { get; set; } = 0;
        public int IRating { get; set; } = 0;
        public int IRatingChange { get; set; } = 0;
        public string License { get; set; } = string.Empty;
        public double SafetyRating { get; set; } = 0.0;
        public int Position { get; set; } = 0;
        public int LivePositionInClass { get; set; } = 0;
        public int CurrentLap { get; set; } = 0;
        public double CurrentLapHighPrecision { get; set; } = 0.0;
        public int TeamIncidentCount { get; set; } = 0;
        public string CarClassColor { get; set; } = string.Empty;
        public string CarClassTextColor { get; set; } = string.Empty;
        public TimeSpan LastLapTime { get; internal set; } = TimeSpan.Zero;
        public TimeSpan BestLapTime { get; internal set; } = TimeSpan.Zero;
        public string TireCompound { get; set; } = string.Empty;
        public int PushToPassCount { get; set; } = 0;
        public bool PushToPassActivated { get; set; } = false;
    }

    public class DriverModule : PluginModuleBase
    {
        private DateTime _lastUpdateTime = DateTime.MinValue;
        private TimeSpan _updateInterval = TimeSpan.FromMilliseconds(500);
        private TimeSpan _minTimeInPit = TimeSpan.FromMilliseconds(2500);

        private SessionModule _sessionModule = null;
        private CarModule _carModule = null;
        private FlairModule _flairModule = null;
        private StandingsModule _standingsModule = null;
        private RelativeModule _relativeModule = null;

        private SessionState _sessionState = new SessionState();

        public const int MaxDrivers = 64;

        public bool QualResultsUpdated { get; private set; } = false;       

        // Key is car number
        public Dictionary<string, Driver> Drivers { get; private set; } = new Dictionary<string, Driver>();

        // Key is CarIdx
        public Dictionary<int, Driver> DriversByCarIdx { get; private set; } = new Dictionary<int, Driver>();

        public HighlightedDriverSettings HighlightedDriverSettings { get; set; }

        public PlayerDriver PlayerDriver { get; private set; } = new PlayerDriver();

        public HighlightedDriver HighlightedDriver { get; private set; } = new HighlightedDriver();

        public override int UpdatePriority => 30;

        public override void Init(PluginManager pluginManager, benofficial2 plugin)
        {
            _sessionModule = plugin.GetModule<SessionModule>();
            _carModule = plugin.GetModule<CarModule>();
            _flairModule = plugin.GetModule<FlairModule>();
            _standingsModule = plugin.GetModule<StandingsModule>();
            _relativeModule = plugin.GetModule<RelativeModule>();

            HighlightedDriverSettings = plugin.ReadCommonSettings<HighlightedDriverSettings>("HighlightedDriverSettings", () => new HighlightedDriverSettings());

            plugin.AttachDelegate(name: "Player.CarIdx", valueProvider: () => PlayerDriver.CarIdx);
            plugin.AttachDelegate(name: "Player.OutLap", valueProvider: () => PlayerDriver.OutLap);
            plugin.AttachDelegate(name: "Player.StintLap", valueProvider: () => PlayerDriver.StintLap);
            plugin.AttachDelegate(name: "Player.Number", valueProvider: () => PlayerDriver.Number);
            plugin.AttachDelegate(name: "Player.Name", valueProvider: () => PlayerDriver.Name);
            plugin.AttachDelegate(name: "Player.CarBrand", valueProvider: () => PlayerDriver.CarBrand);
            plugin.AttachDelegate(name: "Player.CountryCode", valueProvider: () => PlayerDriver.CountryCode);
            plugin.AttachDelegate(name: "Player.SessionFlags", valueProvider: () => PlayerDriver.SessionFlags);
            plugin.AttachDelegate(name: "Player.Position", valueProvider: () => PlayerDriver.Position);
            plugin.AttachDelegate(name: "Player.LivePositionInClass", valueProvider: () => PlayerDriver.LivePositionInClass);
            plugin.AttachDelegate(name: "Player.CurrentLap", valueProvider: () => PlayerDriver.CurrentLap);
            plugin.AttachDelegate(name: "Player.CurrentLapHighPrecision", valueProvider: () => PlayerDriver.CurrentLapHighPrecision);
            plugin.AttachDelegate(name: "Player.LastLapTime", valueProvider: () => PlayerDriver.LastLapTime);
            plugin.AttachDelegate(name: "Player.BestLapTime", valueProvider: () => PlayerDriver.BestLapTime);
            plugin.AttachDelegate(name: "Player.AvgLapTime", valueProvider: () => PlayerDriver.AvgLapTime);
            plugin.AttachDelegate(name: "Player.CurrentLap", valueProvider: () => PlayerDriver.CurrentLap);
            plugin.AttachDelegate(name: "Player.TeamIncidentCount", valueProvider: () => PlayerDriver.TeamIncidentCount);
            plugin.AttachDelegate(name: "Player.CarClassColor", valueProvider: () => PlayerDriver.CarClassColor);
            plugin.AttachDelegate(name: "Player.IRating", valueProvider: () => PlayerDriver.IRating);
            plugin.AttachDelegate(name: "Player.iRatingChange", valueProvider: () => PlayerDriver.IRatingChange);
            plugin.AttachDelegate(name: "Player.License", valueProvider: () => PlayerDriver.License);
            plugin.AttachDelegate(name: "Player.SafetyRating", valueProvider: () => PlayerDriver.SafetyRating);
            plugin.AttachDelegate(name: "Player.TireCompound", valueProvider: () => PlayerDriver.TireCompound);
            plugin.AttachDelegate(name: "Player.PushToPassCount", valueProvider: () => PlayerDriver.PushToPassCount);
            plugin.AttachDelegate(name: "Player.PushToPassActivated", valueProvider: () => PlayerDriver.PushToPassActivated);
            plugin.AttachDelegate(name: "Highlighted.HideWhenInCar", valueProvider: () => HighlightedDriverSettings.HideWhenInCar);
            plugin.AttachDelegate(name: "Highlighted.HideWhenUIVisible", valueProvider: () => HighlightedDriverSettings.HideWhenUIVisible);
            plugin.AttachDelegate(name: "Highlighted.CarBrandVisible", valueProvider: () => HighlightedDriverSettings.CarBrandVisible);
            plugin.AttachDelegate(name: "Highlighted.Width", valueProvider: () => HighlightedDriverSettings.Width);
            plugin.AttachDelegate(name: "Highlighted.BackgroundOpacity", valueProvider: () => HighlightedDriverSettings.BackgroundOpacity);
            plugin.AttachDelegate(name: "Highlighted.CarIdx", valueProvider: () => HighlightedDriver.CarIdx);
            plugin.AttachDelegate(name: "Highlighted.OutLap", valueProvider: () => HighlightedDriver.OutLap);
            plugin.AttachDelegate(name: "Highlighted.Number", valueProvider: () => HighlightedDriver.Number);
            plugin.AttachDelegate(name: "Highlighted.Name", valueProvider: () => HighlightedDriver.Name);
            plugin.AttachDelegate(name: "Highlighted.CarBrand", valueProvider: () => HighlightedDriver.CarBrand);
            plugin.AttachDelegate(name: "Highlighted.CarName", valueProvider: () => HighlightedDriver.CarName);
            plugin.AttachDelegate(name: "Highlighted.SessionFlags", valueProvider: () => HighlightedDriver.SessionFlags);
            plugin.AttachDelegate(name: "Highlighted.CountryCode", valueProvider: () => HighlightedDriver.CountryCode);
            plugin.AttachDelegate(name: "Highlighted.IRating", valueProvider: () => HighlightedDriver.IRating);
            plugin.AttachDelegate(name: "Highlighted.iRatingChange", valueProvider: () => HighlightedDriver.IRatingChange);
            plugin.AttachDelegate(name: "Highlighted.License", valueProvider: () => HighlightedDriver.License);
            plugin.AttachDelegate(name: "Highlighted.SafetyRating", valueProvider: () => HighlightedDriver.SafetyRating);
            plugin.AttachDelegate(name: "Highlighted.Position", valueProvider: () => HighlightedDriver.Position);
            plugin.AttachDelegate(name: "Highlighted.LivePositionInClass", valueProvider: () => HighlightedDriver.LivePositionInClass);
            plugin.AttachDelegate(name: "Highlighted.CurrentLap", valueProvider: () => HighlightedDriver.CurrentLap);
            plugin.AttachDelegate(name: "Highlighted.CurrentLapHighPrecision", valueProvider: () => HighlightedDriver.CurrentLapHighPrecision);
            plugin.AttachDelegate(name: "Highlighted.TeamIncidentCount", valueProvider: () => HighlightedDriver.TeamIncidentCount);
            plugin.AttachDelegate(name: "Highlighted.CarClassColor", valueProvider: () => HighlightedDriver.CarClassColor);
            plugin.AttachDelegate(name: "Highlighted.CarClassTextColor", valueProvider: () => HighlightedDriver.CarClassTextColor);
            plugin.AttachDelegate(name: "Highlighted.LastLapTime", valueProvider: () => HighlightedDriver.LastLapTime);
            plugin.AttachDelegate(name: "Highlighted.BestLapTime", valueProvider: () => HighlightedDriver.BestLapTime);
            plugin.AttachDelegate(name: "Highlighted.TireCompound", valueProvider: () => HighlightedDriver.TireCompound);
            plugin.AttachDelegate(name: "Highlighted.PushToPassCount", valueProvider: () => HighlightedDriver.PushToPassCount);
            plugin.AttachDelegate(name: "Highlighted.PushToPassActivated", valueProvider: () => HighlightedDriver.PushToPassActivated);
        }

        public override void DataUpdate(PluginManager pluginManager, benofficial2 plugin, ref GameData data)
        {
            UpdateDriversHighFreq(ref data);

            if (data.FrameTime - _lastUpdateTime < _updateInterval) 
                return;

            _lastUpdateTime = data.FrameTime;
            _sessionState.Update(ref data);

            // Reset when changing/restarting session
            if (_sessionState.SessionChanged)
            {
                Drivers = new Dictionary<string, Driver>();
                DriversByCarIdx = new Dictionary<int, Driver>();
                BlankPlayerDriver();
                BlankHighlightedDriver();
                QualResultsUpdated = false;
            }

            InvalidatePositions();
            UpdateDrivers(ref data);

            // Update lap times for all drivers based on the session results.
            // Do this after first trying to get the times from telemetry. 
            // Because lap times will be invalid in telemetry after the driver disconnected or exited the car.
            UpdateSessionResults(ref data);

            // Update the highlighted car index
            RawDataHelper.TryGetTelemetryData<int>(ref data, out int highlightedCarIdx, "CamCarIdx");
            RawDataHelper.TryGetTelemetryData<int>(ref data, out int camCameraState, "CamCameraState");
            bool camCameraIsScenic = (camCameraState & 0x0002) != 0;
            if (highlightedCarIdx >= 0 && !camCameraIsScenic)
            {
                HighlightedDriver.CarIdx = highlightedCarIdx;
            }
            else
            {
                BlankHighlightedDriver();
            }

            foreach (Driver driver in Drivers.Values)
            {
                // Update the average lap time for the driver
                int currentLap = driver.Lap;
                driver.AvgLapTime.AddLapTime(currentLap - 1, driver.LastLapTime);

                // Evaluate the lap when they entered the pit lane
                if (driver.InPit)
                {
                    // Ignore in-laps and out-laps for the average lap time.
                    driver.AvgLapTime.InvalidateLap(currentLap);

                    // Remember when they entered the pit.
                    if (driver.InPitSince == DateTime.MinValue)
                    {
                        driver.InPitSince = DateTime.Now;
                        driver.EnterPitLapUnconfirmed = currentLap;
                    }

                    // If they are in the pit for a very short time then we consider that a glitch in telemetry and ignore it.
                    if (driver.InPitSince > DateTime.MinValue &&
                        driver.InPitSince + _minTimeInPit < DateTime.Now)
                    {
                        driver.EnterPitLap = driver.EnterPitLapUnconfirmed;
                        driver.OutLap = false;
                        driver.ExitPitLap = -1;
                        driver.StintLap = 0;

                        if (driver.InPitBox)
                        {
                            if (driver.InPitBoxSince == DateTime.MinValue)
                            {
                                driver.InPitBoxSince = DateTime.Now;
                            }
                            driver.LastPitStopDuration = DateTime.Now - driver.InPitBoxSince;
                        }
                        else
                        {
                            driver.InPitBoxSince = DateTime.MinValue;
                        }
                    }                    
                }
                else
                {
                    // If they are in the pit for a very short time then we consider that a glitch in telemetry and ignore it.
                    // Ignore pit exit before the race start.
                    if (driver.IsConnected &&
                        driver.InPitSince > DateTime.MinValue &&
                        !(_sessionModule.Race && !_sessionModule.RaceStarted) &&
                        driver.InPitSince + _minTimeInPit < DateTime.Now)
                    {
                        driver.ExitPitLap = currentLap;

                        // Edge case when the pit exit is before the finish line.
                        // The currentLap will increment, so consider the next lap an out lap too.
                        if (driver.TrackPositionPercent > 0.5)
                        {
                            driver.ExitPitLap++;
                        }
                    }

                    driver.OutLap = driver.IsConnected && driver.ExitPitLap >= currentLap;
                    driver.InPitSince = DateTime.MinValue;
                    driver.InPitBoxSince = DateTime.MinValue;

                    if (driver.ExitPitLap >= 0)
                    {
                        driver.StintLap = currentLap - driver.ExitPitLap + 1;
                    }
                    else if (_sessionModule.Race && !_sessionModule.JoinedRaceInProgress)
                    {
                        // When we join a race session in progress, we cannot know when the driver exited the pit, so StintLap should stay 0.
                        driver.StintLap = currentLap;
                    }
                }

                if (_sessionModule.Race)
                {
                    RawDataHelper.TryGetTelemetryData<double>(ref data, out double playerCarTowTime, "PlayerCarTowTime");

                    if (!driver.Towing)
                    {
                        // Check for a jump in continuity, this means the driver teleported (towed) back to the pit.
                        if (driver.CurrentLapHighPrecision > -1 && driver.CurrentLapHighPrecisionRaw > -1)
                        {
                            // Use avg speed because in SimHub we can step forward in time in a recorded replay.
                            double avgSpeedKph = ComputeAvgSpeedKph(data.NewData.TrackLength, driver.CurrentLapHighPrecision, driver.CurrentLapHighPrecisionRaw, _sessionState.DeltaTime);
                            bool teleportingToPit = avgSpeedKph > 500 && driver.InPitBox;
                            bool playerTowing = driver.IsPlayer && playerCarTowTime > 0;

                            if (playerTowing || teleportingToPit)
                            {
                                driver.Towing = true;

                                if (driver.IsPlayer)
                                {
                                    driver.TowingEndTime = DateTime.Now + TimeSpan.FromSeconds(playerCarTowTime);
                                }
                                else
                                {
                                    (double towLength, TimeSpan towTime) = ComputeTowLengthAndTime(data.NewData.TrackLength, driver.CurrentLapHighPrecision, driver.CurrentLapHighPrecisionRaw);
                                    driver.TowingEndTime = DateTime.Now + towTime;
                                }
                            }
                        }
                    }
                    else
                    {
                        // iRacing doesn't provide a tow time for other drivers, so we have to estimate it.
                        // Consider towing done if the car starts moving forward from a valid position
                        double smallDistancePct = 0.05 / data.NewData.TrackLength; // 0.05m is roughly the distance you cover at 10km/h in 16ms.

                        bool movingForward = driver.CurrentLapHighPrecisionRaw > -1 &&
                            driver.LastCurrentLapHighPrecision > -1 &&
                            driver.CurrentLapHighPrecisionRaw > driver.LastCurrentLapHighPrecision + smallDistancePct;

                        bool done = driver.CurrentLapHighPrecisionRaw == -1;
                        bool towEnded = !driver.IsPlayer && DateTime.Now > driver.TowingEndTime;
                        bool playerNotTowing = driver.IsPlayer && playerCarTowTime <= 0;
                        if (playerNotTowing || towEnded || movingForward || done)
                        {
                            driver.Towing = false;
                            driver.TowingEndTime = DateTime.MinValue;
                        }
                    }

                    // Pause updating the current lap if the driver is towing, so they stay at their last "on-track" position in the live standings.
                    // Otherwide they would leapfrog the leaders as they teleport in the pit.
                    if (!driver.Towing)
                    {
                        // Stop updating the current lap if the driver is done (-1), so they stay at their last known position in the live standings.
                        // Happens at the end of the race when they get out of the car.
                        if (driver.CurrentLapHighPrecisionRaw > -1)
                        {
                            driver.CurrentLapHighPrecision = driver.CurrentLapHighPrecisionRaw;
                        }
                    }

                    // Edge case when joining in progress while the driver is out of the car.
                    // We can't know for sure where they got out, so we just set their current lap to the last completed lap.
                    if (driver.CurrentLapHighPrecision < Constants.DistanceEpsilon && driver.LapsCompleted > 0)
                    {
                        driver.CurrentLapHighPrecision = driver.LapsCompleted + Constants.LapEpsilon;
                    }

                    driver.LastCurrentLapHighPrecision = driver.CurrentLapHighPrecisionRaw;
                }
                else
                {
                    driver.CurrentLapHighPrecision = driver.CurrentLapHighPrecisionRaw;
                }

                if (driver.IsPlayer)
                {
                    PlayerDriver.OutLap = driver.OutLap;
                    PlayerDriver.StintLap = driver.StintLap;
                    PlayerDriver.Number = driver.CarNumber;
                    PlayerDriver.Name = driver.Name;
                    PlayerDriver.CarBrand = _carModule.GetCarBrand(driver.CarId, driver.CarName);
                    PlayerDriver.CarClassColor = driver.CarClassColor;
                    PlayerDriver.CountryCode = _flairModule.GetCountryCode(driver.FlairId);
                    PlayerDriver.SessionFlags = driver.SessionFlags;
                    PlayerDriver.Position = driver.Position;
                    PlayerDriver.LastLapTime = driver.LastLapTime;
                    PlayerDriver.BestLapTime = driver.BestLapTime;
                    PlayerDriver.AvgLapTime = driver.AvgLapTime.GetAverageLapTime();
                    PlayerDriver.CurrentLapHighPrecision = driver.CurrentLapHighPrecision;
                    PlayerDriver.CurrentLap = Math.Max(0, driver.Lap > 0 ? driver.Lap : (int)Math.Ceiling(driver.CurrentLapHighPrecision));
                    PlayerDriver.TeamIncidentCount = driver.TeamIncidentCount;
                    PlayerDriver.TireCompound = _carModule.GetTireCompoundLetter(driver.TireCompoundIdx);
                    PlayerDriver.PushToPassCount = driver.PushToPassCount;
                    PlayerDriver.PushToPassActivated = driver.PushToPassActivated;
                    PlayerDriver.IRating = driver.IRating;
                    PlayerDriver.IRatingChange = driver.IRatingChange;
                    PlayerDriver.License = driver.License;
                    PlayerDriver.SafetyRating = driver.SafetyRating;

                    if (_sessionModule.Race)
                    {
                        PlayerDriver.HadWhiteFlag = PlayerDriver.HadWhiteFlag || data.NewData.Flag_White == 1;
                        PlayerDriver.HadCheckeredFlag = PlayerDriver.HadCheckeredFlag || data.NewData.Flag_Checkered == 1;
                    }
                }

                if (driver.CarIdx == HighlightedDriver.CarIdx)
                {
                    if (driver.IsPaceCar)
                    {
                        BlankHighlightedDriver();
                    }
                    else
                    {
                        HighlightedDriver.OutLap = driver.OutLap;
                        HighlightedDriver.Name = driver.Name;
                        HighlightedDriver.Number = driver.CarNumber;
                        HighlightedDriver.CarBrand = _carModule.GetCarBrand(driver.CarId, driver.CarName);
                        HighlightedDriver.CarName = driver.CarName;
                        HighlightedDriver.CarClassColor = driver.CarClassColor;
                        HighlightedDriver.CountryCode = _flairModule.GetCountryCode(driver.FlairId);
                        HighlightedDriver.SessionFlags = driver.SessionFlags;
                        HighlightedDriver.Position = driver.Position;
                        HighlightedDriver.IRating = driver.IRating;
                        HighlightedDriver.IRatingChange = driver.IRatingChange;
                        HighlightedDriver.License = driver.License;
                        HighlightedDriver.SafetyRating = driver.SafetyRating;
                        HighlightedDriver.CurrentLap = Math.Max(0, driver.Lap > 0 ? driver.Lap : (int)Math.Ceiling(driver.CurrentLapHighPrecision));
                        HighlightedDriver.CurrentLapHighPrecision = driver.CurrentLapHighPrecision;
                        HighlightedDriver.TeamIncidentCount = driver.TeamIncidentCount;
                        HighlightedDriver.LastLapTime = driver.LastLapTime;
                        HighlightedDriver.BestLapTime = driver.BestLapTime;
                        HighlightedDriver.TireCompound = _carModule.GetTireCompoundLetter(driver.TireCompoundIdx);
                        HighlightedDriver.PushToPassCount = driver.PushToPassCount;
                        HighlightedDriver.PushToPassActivated = driver.PushToPassActivated;
                    }                        
                }
            }

            UpdateQualResult(ref data);
        }

        public override void End(PluginManager pluginManager, benofficial2 plugin)
        {
            plugin.SaveCommonSettings("HighlightedDriverSettings", HighlightedDriverSettings);
        }

        private double ComputeAvgSpeedKph(double trackLength, double fromPos, double toPos, TimeSpan deltaTime)
        {
            if (deltaTime <= TimeSpan.Zero) return 0;
            double deltaPos = Math.Abs(toPos - fromPos);
            double length = deltaPos * trackLength;
            return (length / 1000) / (deltaTime.TotalSeconds / 3600);
        }

        private (double, TimeSpan) ComputeTowLengthAndTime(double trackLength, double fromPos, double toPos)
        {
            double deltaPos;
            if (toPos < fromPos)
            {
                // Must drive around the track
                deltaPos = 1.0 - fromPos + toPos;
            }
            else
            {
                deltaPos = toPos - fromPos;
            }
                
            double length = deltaPos * trackLength;
            const double towSpeedMs = 30;
            const double towTimeFixed = 50;
            return (length, TimeSpan.FromSeconds(length / towSpeedMs + towTimeFixed));
        }

        public static (string license, double rating) ParseLicenseString(string licenseString)
        {
            var parts = licenseString.Split(' ');
            string license = parts[0].Substring(0, 1); // take only the first letter, Pro is 'PWC'
            double rating = double.Parse(parts[1]);
            return (license, rating);
        }

        private void UpdateQualResult(ref GameData data)
        {
            // Optimization: Only update the qualifying results once before the race starts.
            if (QualResultsUpdated || !_sessionModule.Race)
                return;

            // First try to get from the current session's qualifying results.
            // In Heat races, only the drivers particpating in the heat will be present in the QualifyPositions array.
            if (RawDataHelper.TryGetSessionData<int>(ref data, out int currentSessionIdx, "SessionInfo", "CurrentSessionNum") && currentSessionIdx >= 0)
            {
                RawDataHelper.TryGetSessionData<List<object>>(ref data, out List<object> qualPositions, "SessionInfo", "Sessions", currentSessionIdx, "QualifyPositions");
                if (qualPositions != null)
                {
                    for (int i = 0; i < qualPositions.Count; i++)
                    {
                        RawDataHelper.TryGetValue<int>(qualPositions, out int carIdx, i, "CarIdx");
                        if (!DriversByCarIdx.TryGetValue(carIdx, out Driver driver))
                        {
                            Debug.Assert(false);
                            continue;
                        }

                        RawDataHelper.TryGetValue<int>(qualPositions, out int positionInClass, i, "ClassPosition");
                        RawDataHelper.TryGetValue<double>(qualPositions, out double fastestTime, i, "FastestTime");

                        driver.QualPositionInClass = positionInClass + 1;
                        driver.QualLapTime = fastestTime > 0 ? TimeSpan.FromSeconds(fastestTime) : TimeSpan.Zero;
                    }

                    QualResultsUpdated = true;
                    return;
                }
            }

            // For normal races, get from the overall QualifyResultsInfo.
            RawDataHelper.TryGetSessionData<List<object>>(ref data, out List<object> qualResults, "QualifyResultsInfo", "Results");
            if (qualResults != null)
            {
                for (int i = 0; i < qualResults.Count; i++)
                {
                    RawDataHelper.TryGetValue<int>(qualResults, out int carIdx, i, "CarIdx");
                    if (!DriversByCarIdx.TryGetValue(carIdx, out Driver driver))
                    {
                        Debug.Assert(false);
                        continue;
                    }

                    RawDataHelper.TryGetValue<int>(qualResults, out int positionInClass, i, "ClassPosition");
                    RawDataHelper.TryGetValue<double>(qualResults, out double fastestTime, i, "FastestTime");

                    driver.QualPositionInClass = positionInClass + 1;
                    driver.QualLapTime = fastestTime > 0 ? TimeSpan.FromSeconds(fastestTime) : TimeSpan.Zero;
                }

                QualResultsUpdated = true;
                return;
            }
        }

        private void InvalidatePositions()
        {
            foreach (var driver in Drivers.Values)
            {
                driver.Position = 0;
                driver.PositionInClass = 0;
                driver.LivePositionInClass = 0;

                // Patch for bug observed in a hosted session where a driver with a qualifying time but did not grid
                // was shown as P1 in the race's live standings.
                // Should normally not be needed after a SessionChanged.
                if (_sessionModule.Race && !_sessionModule.RaceStarted)
                {
                    driver.CurrentLapHighPrecision = -1;
                    driver.CurrentLapHighPrecisionRaw = -1;
                }
            }
        }

        private void UpdateDrivers(ref GameData data)
        {
            RawDataHelper.TryGetSessionData<int>(ref data, out int playerCarIdx, "DriverInfo", "DriverCarIdx");
            PlayerDriver.CarIdx = playerCarIdx;

            if (!RawDataHelper.TryGetSessionData<List<object>>(ref data, out List<object> drivers, "DriverInfo", "Drivers"))
                return;

            if (drivers == null)
                return;

            for (int i = 0; i < drivers.Count; i++)
            {
                RawDataHelper.TryGetValue<int>(drivers, out int carIdx, i, "CarIdx");
                RawDataHelper.TryGetValue<string>(drivers, out string carNumber, i, "CarNumber");

                if (carIdx < 0 || carIdx >= MaxDrivers || carNumber.Length == 0)
                {
                    Debug.Assert(false);
                    continue;
                }

                RawDataHelper.TryGetValue<string>(drivers, out string carPath, i, "CarPath");
                RawDataHelper.TryGetValue<int>(drivers, out int flairId, i, "FlairID");
                RawDataHelper.TryGetValue<int>(drivers, out int carClassId, i, "CarClassID");
                RawDataHelper.TryGetValue<int>(drivers, out int teamIncidentCount, i, "TeamIncidentCount");
                RawDataHelper.TryGetValue<int>(drivers, out int iRating, i, "IRating");
                RawDataHelper.TryGetValue<string>(drivers, out string carScreenNameShort, i, "CarScreenNameShort");
                RawDataHelper.TryGetValue<string>(drivers, out string carClassShortName, i, "CarClassShortName");
                RawDataHelper.TryGetValue<string>(drivers, out string carClassColor, i, "CarClassColor");
                RawDataHelper.TryGetValue<string>(drivers, out string licString, i, "LicString");
                RawDataHelper.TryGetValue<string>(drivers, out string userName, i, "UserName");
                RawDataHelper.TryGetValue<string>(drivers, out string teamName, i, "TeamName");
                RawDataHelper.TryGetValue<int>(drivers, out int carIsPaceCar, i, "CarIsPaceCar");
                RawDataHelper.TryGetValue<float>(drivers, out float carClassEstLapTime, i, "CarClassEstLapTime");

                RawDataHelper.TryGetTelemetryData<float>(ref data, out float lastLapTime, "CarIdxLastLapTime", carIdx);
                RawDataHelper.TryGetTelemetryData<float>(ref data, out float bestLapTime, "CarIdxBestLapTime", carIdx);
                RawDataHelper.TryGetTelemetryData<int>(ref data, out int sessionFlags, "CarIdxSessionFlags", carIdx);
                RawDataHelper.TryGetTelemetryData<int>(ref data, out int position, "CarIdxPosition", carIdx);
                RawDataHelper.TryGetTelemetryData<int>(ref data, out int classPosition, "CarIdxClassPosition", carIdx);
                RawDataHelper.TryGetTelemetryData<bool>(ref data, out bool onPitRoad, "CarIdxOnPitRoad", carIdx);
                RawDataHelper.TryGetTelemetryData<int>(ref data, out int lap, "CarIdxLap", carIdx);
                RawDataHelper.TryGetTelemetryData<int>(ref data, out int trackSurface, "CarIdxTrackSurface", carIdx);
                RawDataHelper.TryGetTelemetryData<int>(ref data, out int lapCompleted, "CarIdxLapCompleted", carIdx);
                RawDataHelper.TryGetTelemetryData<float>(ref data, out float lapDistPct, "CarIdxLapDistPct", carIdx);
                RawDataHelper.TryGetTelemetryData<float>(ref data, out float estTime, "CarIdxEstTime", carIdx);
                RawDataHelper.TryGetTelemetryData<int>(ref data, out int tireCompoundIdx, "CarIdxTireCompound", carIdx);
                RawDataHelper.TryGetTelemetryData<int>(ref data, out int p2pCount, "CarIdxP2P_Count", carIdx);
                RawDataHelper.TryGetTelemetryData<int>(ref data, out int p2pStatus, "CarIdxP2P_Status", carIdx);

                if (!Drivers.TryGetValue(carNumber, out Driver driver))
                {
                    driver = new Driver();
                    Drivers[carNumber] = driver;
                    DriversByCarIdx[carIdx] = driver;
                }

                driver.DriverInfoIdx = i;
                driver.CarIdx = carIdx;
                driver.CarId = carPath;
                driver.CarName = carScreenNameShort;
                driver.CarNumber = carNumber;
                driver.Name = userName;
                driver.TeamName = teamName;
                driver.FlairId = flairId;
                driver.IsPlayer = carIdx == playerCarIdx;
                driver.IsConnected = trackSurface > (int)TrackLoc.NotInWorld;
                driver.IsPaceCar = carIsPaceCar == 1;
                driver.CarClassId = carClassId;
                driver.CarClassName = carClassShortName;
                driver.CarClassColor = ConvertColorString(carClassColor);
                driver.CarClassEstLapTime = carClassEstLapTime;
                driver.TeamIncidentCount = teamIncidentCount;
                driver.IRating = iRating;
                (driver.License, driver.SafetyRating) = ParseLicenseString(licString);
                driver.LastLapTime = lastLapTime > 0 ? TimeSpan.FromSeconds(lastLapTime) : TimeSpan.Zero;
                driver.BestLapTime = bestLapTime > 0 ? TimeSpan.FromSeconds(bestLapTime) : TimeSpan.Zero;
                driver.SessionFlags = sessionFlags;
                driver.Position = position;
                driver.PositionInClass = classPosition;
                driver.InPit = onPitRoad;
                driver.InPitBox = trackSurface == (int)TrackLoc.InPitStall;
                driver.Lap = lap;
                driver.LapsCompleted = lapCompleted;
                driver.TrackPositionPercent = lapDistPct;
                driver.EstTime = estTime;
                driver.TireCompoundIdx = tireCompoundIdx;
                driver.PushToPassCount = p2pCount;
                driver.PushToPassActivated = p2pStatus > 0;

                if (driver.Lap > -1 && driver.TrackPositionPercent > -Constants.DistanceEpsilon)
                {
                    driver.CurrentLapHighPrecisionRaw = driver.Lap - 1 + driver.TrackPositionPercent;
                }
                else
                {
                    driver.CurrentLapHighPrecisionRaw = -1;
                }
            }
        }

        private void UpdateDriversHighFreq(ref GameData data)
        {
            foreach (var driver in Drivers.Values)
            {
                if (driver.CarIdx < 0 || driver.CarIdx >= MaxDrivers)
                    continue;

                RawDataHelper.TryGetTelemetryData<float>(ref data, out float estTime, "CarIdxEstTime", driver.CarIdx);

                driver.EstTime = estTime;
            }
        }

        private void UpdateSessionResults(ref GameData data)
        {
            if (!RawDataHelper.TryGetSessionData<List<object>>(ref data, out List<object> sessions, "SessionInfo", "Sessions"))
                return;

            // It can happen that CurrentSessionNum is missing on SessionInfo. We can't tell which session to use in that case.
            if (!RawDataHelper.TryGetSessionData<int>(ref data, out int sessionIdx, "SessionInfo", "CurrentSessionNum"))
                return;

            if (sessions == null || sessionIdx < 0 || sessionIdx >= sessions.Count)
                return;

            if (!RawDataHelper.TryGetValue<List<object>>(sessions, out List<object> positions, sessionIdx, "ResultsPositions"))
                return;

            if (positions == null)
                return;

            for (int posIdx = 0; posIdx < positions.Count; posIdx++)
            {
                RawDataHelper.TryGetValue<int>(positions, out int carIdx, posIdx, "CarIdx");

                if (carIdx < 0) 
                    continue;

                if (!DriversByCarIdx.TryGetValue(carIdx, out Driver driver))
                {
                    Debug.Assert(false);
                    continue;
                }

                RawDataHelper.TryGetValue<int>(positions, out int position, posIdx, "Position");
                if (driver.Position <= 0 && position > 0)
                {
                    driver.Position = position;
                }

                RawDataHelper.TryGetValue<int>(positions, out int classPosition, posIdx, "ClassPosition");
                if (driver.PositionInClass <= 0 && classPosition > -1)
                {
                    driver.PositionInClass = classPosition + 1;
                }

                RawDataHelper.TryGetValue<double>(positions, out double fastestTime, posIdx, "FastestTime");
                if (driver.BestLapTime == TimeSpan.Zero && fastestTime > 0)
                {
                    driver.BestLapTime = TimeSpan.FromSeconds(fastestTime);
                }

                RawDataHelper.TryGetValue<double>(positions, out double lastTime, posIdx, "LastTime");
                if (driver.LastLapTime == TimeSpan.Zero && lastTime > 0)
                {
                    driver.LastLapTime = TimeSpan.FromSeconds(lastTime);
                }

                RawDataHelper.TryGetValue<int>(positions, out int lapsComplete, posIdx, "LapsComplete");
                if (driver.LapsCompleted <= 0 && lapsComplete >= 0)
                {
                    driver.LapsCompleted = lapsComplete;
                }

                RawDataHelper.TryGetValue<int>(positions, out int lap, posIdx, "Lap");
                if (driver.Lap <= 0 && lap >= 0)
                {
                    // Fix a bug in iRacing's session results where the lap number is inconsistent for lapped cars.
                    if (driver.Lap < driver.LapsCompleted)
                        driver.Lap = 0;
                    else
                        driver.Lap = lap;
                }

                RawDataHelper.TryGetValue<int>(positions, out int jokerLapsComplete, posIdx, "JokerLapsComplete");
                driver.JokerLapsCompleted = jokerLapsComplete;
            }
        }

        public void UpdateIRatingChange(ref GameData data)
        {
            if (!(_sessionModule.Race))
                return;

            // Optim: Don't calculate if the iRating change is not visible.
            if (!_standingsModule.Settings.IRatingChangeVisible && !_relativeModule.Settings.IRatingChangeVisible)
                return;

            foreach (var group in Drivers.Values.GroupBy(d => d.CarClassId))
            {
                int carClassId = group.Key;
                int countInClass = group.Count();
                var raceResults = new List<RaceResult<Driver>>();

                if (!_sessionModule.RaceStarted)
                {
                    // Consider all drivers as if they finished in their qualifying position.
                    foreach (var driver in group)
                    {
                        if (driver.IsPaceCar)
                            continue;

                        raceResults.Add(new RaceResult<Driver>(
                         driver,
                         (uint)driver.QualPositionInClass,
                         (uint)driver.IRating,
                         true));
                    }
                }
                else
                {
                    // Consider drivers with an official position first. They are considered as started.
                    // TODO: Should DQ drivers be considered as not started?
                    // TODO: How much of the first lap should be completed to be considered started?
                    var withPosition = group.Where(d => d.PositionInClass != 0).ToList();
                    foreach (var driver in withPosition)
                    {
                        if (driver.IsPaceCar)
                            continue;

                        int positionInClass = driver.LivePositionInClass;
                        if (positionInClass <= 0)
                        {
                            // Fallback to the official position if the live position is not available.
                            positionInClass = driver.PositionInClass;
                        }

                        raceResults.Add(new RaceResult<Driver>(
                         driver,
                         (uint)positionInClass,
                         (uint)driver.IRating,
                         true));
                    }

                    // Then consider drivers without an official position. They are considered as not started.
                    // Assign them a position by sorting them by IRating.
                    var noPosition = group.Where(d => d.PositionInClass == 0).OrderByDescending(d => d.IRating).ToList();
                    int nextPosition = withPosition.Count + 1;
                    foreach (var driver in noPosition)
                    {
                        raceResults.Add(new RaceResult<Driver>(
                         driver,
                         (uint)nextPosition++,
                         (uint)driver.IRating,
                         false));
                    }
                }

                var results = IRatingCalculator.Calculate(raceResults);
                for (int i = 0; i < results.Count; i++)
                {
                    var result = results[i];
                    int change = (int)result.NewIRating - (int)result.RaceResult.StartIRating;
                    result.RaceResult.Driver.IRatingChange = change;

                    if (result.RaceResult.Driver.CarIdx == PlayerDriver.CarIdx)
                    {
                        PlayerDriver.IRatingChange = change;
                    }
                }
            }
        }

        private void BlankPlayerDriver()
        {
            PlayerDriver.CarIdx = -1;
            PlayerDriver.OutLap = false;
            PlayerDriver.StintLap = 0;
            PlayerDriver.Number = "";
            PlayerDriver.Name = "";
            PlayerDriver.CarBrand = "";
            PlayerDriver.CountryCode = "";
            PlayerDriver.SessionFlags = 0;
            PlayerDriver.IRating = 0;
            PlayerDriver.IRatingChange = 0;
            PlayerDriver.License = string.Empty;
            PlayerDriver.SafetyRating = 0.0;
            PlayerDriver.Position = 0;
            PlayerDriver.LivePositionInClass = 0;
            PlayerDriver.HadWhiteFlag = false;
            PlayerDriver.HadCheckeredFlag = false;
            PlayerDriver.LastLapTime = TimeSpan.Zero;
            PlayerDriver.BestLapTime = TimeSpan.Zero;
            PlayerDriver.AvgLapTime = TimeSpan.Zero;
            PlayerDriver.CurrentLapHighPrecision = -1;
            PlayerDriver.CurrentLap = 0;
            PlayerDriver.TeamIncidentCount = 0;
            PlayerDriver.CarClassColor = string.Empty;
            PlayerDriver.TireCompound = string.Empty;
            PlayerDriver.PushToPassCount = 0;
            PlayerDriver.PushToPassActivated = false;
        }
        private void BlankHighlightedDriver()
        {
            HighlightedDriver.CarIdx = -1;
            HighlightedDriver.OutLap = false;
            HighlightedDriver.Number = string.Empty;
            HighlightedDriver.Name = string.Empty;
            HighlightedDriver.CarBrand = string.Empty;
            HighlightedDriver.CarName = string.Empty;
            HighlightedDriver.CountryCode = string.Empty;
            HighlightedDriver.SessionFlags = 0;
            HighlightedDriver.IRating = 0;
            HighlightedDriver.IRatingChange = 0;
            HighlightedDriver.License = string.Empty;
            HighlightedDriver.SafetyRating = 0.0;
            HighlightedDriver.Position = 0;
            HighlightedDriver.LivePositionInClass = 0;
            HighlightedDriver.CurrentLap = 0;
            HighlightedDriver.CurrentLapHighPrecision = -1;
            HighlightedDriver.TeamIncidentCount = 0;
            HighlightedDriver.CarClassColor = string.Empty;
            HighlightedDriver.CarClassTextColor = string.Empty;
            HighlightedDriver.LastLapTime = TimeSpan.Zero;
            HighlightedDriver.BestLapTime = TimeSpan.Zero;
            HighlightedDriver.TireCompound = string.Empty;
            HighlightedDriver.PushToPassCount = 0;
            HighlightedDriver.PushToPassActivated = false;
        }

        public Driver GetPlayerDriver()
        {
            DriversByCarIdx.TryGetValue(PlayerDriver.CarIdx, out Driver driver);
            return driver;
        }

        public Driver GetHighlightedDriver(bool fallbackToPlayer = true)
        {
            int highlightedCarIdx = -1;
            if (HighlightedDriver.CarIdx >= 0)
            {
                highlightedCarIdx = HighlightedDriver.CarIdx;
            }
            else
            {
                if (!fallbackToPlayer)
                    return null;

                highlightedCarIdx = PlayerDriver.CarIdx;
            }

            DriversByCarIdx.TryGetValue(highlightedCarIdx, out Driver highlightedDriver);
            return highlightedDriver;
        }

        public static string ConvertColorString(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            // Remove the "0x" prefix if present
            if (input.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                input = input.Substring(2);
            }

            // Return in #RRGGBB format, uppercase
            return "#" + input.ToUpper();
        }

        public static bool IsAheadOnTrack(double currentTrackPosPct, double otherTrackPosPct)
        {
            // Return true if 'other' is ahead of 'current' on track, considering the lap wrap-around.
            if (currentTrackPosPct < otherTrackPosPct)
                return otherTrackPosPct - currentTrackPosPct < 0.5;
            else
                return currentTrackPosPct - otherTrackPosPct > 0.5;
        }
    }
}
