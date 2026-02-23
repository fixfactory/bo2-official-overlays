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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace benofficial2.Plugin
{
    public class StandingsSettings : ModuleSettings
    {
        public int HeaderWidth { get; set; } = 10;
        public int WidthPixels { get; set; } = 600;
        public int WidthPixelsInRace { get; set; } = 640;
        public bool HideInReplay { get; set; } = true;
        public bool HeaderVisible { get; set; } = true;
        public bool CarClassHeaderVisible { get; set; } = true;
        public int LeadFocusedRows { get; set; } = 3;
        public int MaxRowsOtherClasses { get; set; } = 3;
        public int MaxRowsPlayerClass { get; set; } = 15;
        public bool PositionChangeVisible { get; set; } = true;
        public bool CountryFlagVisible { get; set; } = true;
        public bool CarLogoVisible { get; set; } = true;
        public bool SafetyRatingVisible { get; set; } = true;
        public bool IRatingVisible { get; set; } = true;
        public bool IRatingChangeVisible { get; set; } = true;
        public bool CarLogoVisibleInRace { get; set; } = true;
        public bool GapVisibleInRace { get; set; } = true;
        public bool IntervalVisibleInRace { get; set; } = true;
        public bool BestVisibleInRace { get; set; } = true;
        public bool LastVisibleInRace { get; set; } = true;
        public bool DeltaVisibleInRace { get; set; } = true;
        public bool BestVisible { get; set; } = true;
        public bool LastVisible { get; set; } = true;
        public bool DeltaVisible { get; set; } = true;
        public bool UseDeltaToPlayer { get; set; } = true;
        public bool InvertDeltaToPlayer { get; set; } = false;
        public bool ShowStintLapInRace { get; set; } = true;
        public bool PitDurationVisible { get; set; } = false;
        public bool PitDurationVisibleInRace { get; set; } = false;
        public bool UsePitStopDuration { get; set; } = false;
        public int AlternateRowBackgroundColor { get; set; } = 15;
        public bool HighlightPlayerRow { get; set; } = true;
        public int HeaderOpacity { get; set; } = 90;
        public int BackgroundOpacity { get; set; } = 60;

        // Deprecated pre-4.0
        public int Width { get; set; } = 80;
    }

    public class StandingRow
    {
        public bool RowVisible { get; set; } = false;
        public bool IsPlayer { get; set; } = false;
        public bool IsHighlighted { get; set; } = false;
        public bool Connected { get; set; } = false;
        public int LivePositionInClass { get; set; } = 0;
        public int PositionChange { get; set; } = 0;
        public string Number { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string CarId { get; set; } = string.Empty;
        public string CarBrand { get; set; } = string.Empty;
        public string CarClassColor { get; set; } = string.Empty;
        public string CarClassTextColor { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
        public bool InPitLane { get; set; } = false;
        public bool Towing { get; set; } = false;
        public bool OutLap { get; set; } = false;
        public int EnterPitLap { get; set; } = 0;
        public TimeSpan LastPitStopDuration { get; set; } = TimeSpan.Zero;
        public TimeSpan LastPitDuration { get; set; } = TimeSpan.Zero;
        public int iRating { get; set; } = 0;
        public float iRatingChange { get; set; } = 0;
        public string License {  get; set; } = string.Empty;
        public double SafetyRating { get; set; } = 0;
        public int CurrentLap {  get; set; } = 0;
        public int StintLap { get; set; } = 0;
        public int LapsToClassLeader { get; set; } = 0;
        public double GapToClassLeader { get; set; } = 0;
        public int LapsToClassOpponentAhead { get; set; } = 0;
        public double GapToClassOpponentAhead { get; set; } = 0.0;
        public TimeSpan? DeltaToClassLeader { get; set; } = null;
        public TimeSpan? DeltaToPlayer { get; set; } = null;
        public string TireCompound {  get; set; } = string.Empty;
        public bool TireCompoundVisible { get; set; } = false;
        public TimeSpan BestLapTime { get; set; } = TimeSpan.Zero;
        public TimeSpan LastLapTime { get; set; } = TimeSpan.Zero;
        public int JokerLapsComplete { get; set; } = 0;
    }

    public class StandingCarClass
    {
        public const int MaxRows = 25;
        public string Name { get; set; } = string.Empty;
        public string LastMeasuredName { get; set; } = string.Empty;
        public float NameSize { get; set; } = 0;
        public int VisibleRowCount { get; set; } = 0;
        public List<StandingRow> Rows { get; internal set; }
        public bool LeadFocusedDividerVisible { get; set; } = false;
        public string Color { get; set; } = string.Empty;
        public string TextColor { get; set; } = string.Empty;
        public int Sof { get; set; } = 0;
        public int DriverCount { get; set; } = 0;
        public TimeSpan BestLapTime { get; set; } = TimeSpan.Zero;
        public TimeSpan BestQualLapTime { get; set; } = TimeSpan.Zero;
        public TimeSpan LeaderLastLapTime { get; set; } = TimeSpan.Zero;
        public TimeSpan LeaderAvgLapTime { get; set; } = TimeSpan.Zero;
        public int EstimatedTotalLaps { get; set; } = 0;
        public bool EstimatedTotalLapsConfirmed { get; set; } = false;
        public bool EstimatedTotalLapsLogged { get; set; } = false;
        public bool IsLeadingClass { get; set; } = false;

        public StandingCarClass()
        {
            Rows = new List<StandingRow>(Enumerable.Range(0, MaxRows).Select(x => new StandingRow()));
        }
    }

    public class ClassLeaderboard
    {
        public int CarClassId { get; set; } = 0;
        public string CarClassName { get; set; } = string.Empty;
        public string CarClassColor { get; set; } = string.Empty;
        public List<Driver> Drivers { get; set; } = new List<Driver>();
        public HashSet<string> CarNames { get; } = new HashSet<string>();
        public int LeaderPosition { get; set; } = 0;
        public double EstLapTime { get; set; } = 0.0;
    }

    public class StandingsModule : PluginModuleBase
    {
        private DriverModule _driverModule = null;
        private CarModule _carModule = null;
        private SessionModule _sessionModule = null;
        private FlairModule _flairModule = null;

        private DateTime _lastUpdateTime = DateTime.MinValue;
        private TimeSpan _updateInterval = TimeSpan.FromMilliseconds(500);

        private Font _font = null;
        private Bitmap _fontBitmap = null;
        private Graphics _fontGraphics = null;

        public const int MaxCarClasses = 4;

        public StandingsSettings Settings { get; set; }
        public List<StandingCarClass> CarClasses { get; internal set; }
        public int VisibleClassCount { get; internal set; } = 0;
        public int TotalDriverCount { get; internal set; } = 0;
        public int TotalSoF { get; internal set; } = 0;
        public int HighlightedCarClassIdx { get; internal set; } = 0;
        public int WidthPixels { get; set; } = 600;
        public bool CarLogoVisible { get; internal set; } = true;
        public bool GapVisible { get; internal set; } = true;
        public bool IntervalVisible { get; internal set; } = true;
        public bool BestVisible { get; internal set; } = true;
        public bool LastVisible { get; internal set; } = true;
        public bool DeltaVisible { get; internal set; } = true;
        public bool PitDurationVisible { get; internal set; } = false;
        public bool LeadingClassDoingExtraLap { get; internal set; } = false;

        public List<ClassLeaderboard> LiveClassLeaderboards { get; private set; } = new List<ClassLeaderboard>();

        // Reusable buffers to avoid per-frame allocations
        private readonly Dictionary<int, ClassLeaderboard> _classMapBuffer = new Dictionary<int, ClassLeaderboard>(MaxCarClasses);
        private readonly List<Driver> _scoredDriversBuffer = new List<Driver>(64);
        private readonly List<ClassLeaderboard> _leaderboardPool = new List<ClassLeaderboard>();

        public override int UpdatePriority => 80;

        public StandingsModule()
        {
            CarClasses = new List<StandingCarClass>(Enumerable.Range(0, MaxCarClasses).Select(x => new StandingCarClass()));
        }

        public override void Init(PluginManager pluginManager, benofficial2 plugin)
        {
            _driverModule = plugin.GetModule<DriverModule>();
            _carModule = plugin.GetModule<CarModule>();
            _sessionModule = plugin.GetModule<SessionModule>();
            _flairModule = plugin.GetModule<FlairModule>();

            _font = new Font("Roboto", 16);
            _fontBitmap = new Bitmap(1, 1);
            _fontGraphics = Graphics.FromImage(_fontBitmap);

            Settings = plugin.ReadCommonSettings<StandingsSettings>("StandingsSettings", () => new StandingsSettings());
            plugin.AttachDelegate(name: $"Standings.HeaderVisible", valueProvider: () => Settings.HeaderVisible);
            plugin.AttachDelegate(name: $"Standings.HeaderWidth", valueProvider: () => Settings.HeaderWidth);
            plugin.AttachDelegate(name: $"Standings.Width", valueProvider: () => WidthPixels);
            plugin.AttachDelegate(name: $"Standings.CarClassHeaderVisible", valueProvider: () => Settings.CarClassHeaderVisible);
            plugin.AttachDelegate(name: $"Standings.VisibleClassCount", valueProvider: () => VisibleClassCount);
            plugin.AttachDelegate(name: $"Standings.TotalDriverCount", valueProvider: () => TotalDriverCount);
            plugin.AttachDelegate(name: $"Standings.TotalSoF", valueProvider: () => TotalSoF);
            plugin.AttachDelegate(name: $"Standings.PlayerCarClassIdx", valueProvider: () => HighlightedCarClassIdx);
            plugin.AttachDelegate(name: $"Standings.HideInReplay", valueProvider: () => Settings.HideInReplay);
            plugin.AttachDelegate(name: $"Standings.LeadFocusedRows", valueProvider: () => Settings.LeadFocusedRows);
            plugin.AttachDelegate(name: $"Standings.MaxRowsPlayerClass", valueProvider: () => Settings.MaxRowsPlayerClass);
            plugin.AttachDelegate(name: $"Standings.MaxRowsOtherClasses", valueProvider: () => Settings.MaxRowsOtherClasses);
            plugin.AttachDelegate(name: $"Standings.PositionChangeVisible", valueProvider: () => Settings.PositionChangeVisible);
            plugin.AttachDelegate(name: $"Standings.CountryFlagVisible", valueProvider: () => Settings.CountryFlagVisible);
            plugin.AttachDelegate(name: $"Standings.CarLogoVisible", valueProvider: () => CarLogoVisible);
            plugin.AttachDelegate(name: $"Standings.SafetyRatingVisible", valueProvider: () => Settings.SafetyRatingVisible);
            plugin.AttachDelegate(name: $"Standings.iRatingVisible", valueProvider: () => Settings.IRatingVisible);
            plugin.AttachDelegate(name: $"Standings.iRatingChangeVisible", valueProvider: () => Settings.IRatingChangeVisible);
            plugin.AttachDelegate(name: $"Standings.GapVisible", valueProvider: () => GapVisible);
            plugin.AttachDelegate(name: $"Standings.IntervalVisible", valueProvider: () => IntervalVisible);
            plugin.AttachDelegate(name: $"Standings.BestVisible", valueProvider: () => BestVisible);
            plugin.AttachDelegate(name: $"Standings.LastVisible", valueProvider: () => LastVisible);
            plugin.AttachDelegate(name: $"Standings.DeltaVisible", valueProvider: () => DeltaVisible);
            plugin.AttachDelegate(name: $"Standings.UseDeltaToPlayer", valueProvider: () => Settings.UseDeltaToPlayer);
            plugin.AttachDelegate(name: $"Standings.ShowStintLapInRace", valueProvider: () => Settings.ShowStintLapInRace);
            plugin.AttachDelegate(name: $"Standings.PitDurationVisible", valueProvider: () => PitDurationVisible);
            plugin.AttachDelegate(name: $"Standings.UsePitStopDuration", valueProvider: () => Settings.UsePitStopDuration);
            plugin.AttachDelegate(name: "Standings.AlternateRowBackgroundColor", valueProvider: () => Settings.AlternateRowBackgroundColor);
            plugin.AttachDelegate(name: "Standings.HighlightPlayerRow", valueProvider: () => Settings.HighlightPlayerRow);
            plugin.AttachDelegate(name: "Standings.HeaderOpacity", valueProvider: () => Settings.HeaderOpacity);
            plugin.AttachDelegate(name: "Standings.BackgroundOpacity", valueProvider: () => Settings.BackgroundOpacity);

            for (int carClassIdx = 0; carClassIdx < MaxCarClasses; carClassIdx++)
            {
                StandingCarClass carClass = CarClasses[carClassIdx];
                plugin.AttachDelegate(name: $"Standings.Class{carClassIdx:00}.Name", valueProvider: () => carClass.Name);
                plugin.AttachDelegate(name: $"Standings.Class{carClassIdx:00}.NameSize", valueProvider: () => carClass.NameSize);
                plugin.AttachDelegate(name: $"Standings.Class{carClassIdx:00}.VisibleRowCount", valueProvider: () => carClass.VisibleRowCount);
                plugin.AttachDelegate(name: $"Standings.Class{carClassIdx:00}.LeadFocusedDividerVisible", valueProvider: () => carClass.LeadFocusedDividerVisible);
                plugin.AttachDelegate(name: $"Standings.Class{carClassIdx:00}.Color", valueProvider: () => carClass.Color);
                plugin.AttachDelegate(name: $"Standings.Class{carClassIdx:00}.TextColor", valueProvider: () => carClass.TextColor);
                plugin.AttachDelegate(name: $"Standings.Class{carClassIdx:00}.Sof", valueProvider: () => carClass.Sof);
                plugin.AttachDelegate(name: $"Standings.Class{carClassIdx:00}.DriverCount", valueProvider: () => carClass.DriverCount);
                plugin.AttachDelegate(name: $"Standings.Class{carClassIdx:00}.BestLapTime", valueProvider: () => carClass.BestLapTime);
                plugin.AttachDelegate(name: $"Standings.Class{carClassIdx:00}.EstimatedTotalLaps", valueProvider: () => carClass.EstimatedTotalLaps);

                for (int rowIdx = 0; rowIdx < StandingCarClass.MaxRows; rowIdx++)
                {
                    StandingRow row = carClass.Rows[rowIdx];
                    plugin.AttachDelegate(name: $"Standings.Class{carClassIdx:00}.Row{rowIdx:00}.RowVisible", valueProvider: () => row.RowVisible);
                    plugin.AttachDelegate(name: $"Standings.Class{carClassIdx:00}.Row{rowIdx:00}.IsPlayer", valueProvider: () => row.IsPlayer);
                    plugin.AttachDelegate(name: $"Standings.Class{carClassIdx:00}.Row{rowIdx:00}.IsHighlighted", valueProvider: () => row.IsHighlighted);
                    plugin.AttachDelegate(name: $"Standings.Class{carClassIdx:00}.Row{rowIdx:00}.Connected", valueProvider: () => row.Connected);
                    plugin.AttachDelegate(name: $"Standings.Class{carClassIdx:00}.Row{rowIdx:00}.LivePositionInClass", valueProvider: () => row.LivePositionInClass);
                    plugin.AttachDelegate(name: $"Standings.Class{carClassIdx:00}.Row{rowIdx:00}.PositionChange", valueProvider: () => row.PositionChange);
                    plugin.AttachDelegate(name: $"Standings.Class{carClassIdx:00}.Row{rowIdx:00}.Number", valueProvider: () => row.Number);
                    plugin.AttachDelegate(name: $"Standings.Class{carClassIdx:00}.Row{rowIdx:00}.Name", valueProvider: () => row.Name);
                    plugin.AttachDelegate(name: $"Standings.Class{carClassIdx:00}.Row{rowIdx:00}.CarId", valueProvider: () => row.CarId);
                    plugin.AttachDelegate(name: $"Standings.Class{carClassIdx:00}.Row{rowIdx:00}.CarBrand", valueProvider: () => row.CarBrand);
                    plugin.AttachDelegate(name: $"Standings.Class{carClassIdx:00}.Row{rowIdx:00}.CarClassColor", valueProvider: () => row.CarClassColor);
                    plugin.AttachDelegate(name: $"Standings.Class{carClassIdx:00}.Row{rowIdx:00}.CarClassTextColor", valueProvider: () => row.CarClassTextColor);
                    plugin.AttachDelegate(name: $"Standings.Class{carClassIdx:00}.Row{rowIdx:00}.CountryCode", valueProvider: () => row.CountryCode);
                    plugin.AttachDelegate(name: $"Standings.Class{carClassIdx:00}.Row{rowIdx:00}.InPitLane", valueProvider: () => row.InPitLane);
                    plugin.AttachDelegate(name: $"Standings.Class{carClassIdx:00}.Row{rowIdx:00}.Towing", valueProvider: () => row.Towing);
                    plugin.AttachDelegate(name: $"Standings.Class{carClassIdx:00}.Row{rowIdx:00}.OutLap", valueProvider: () => row.OutLap);
                    plugin.AttachDelegate(name: $"Standings.Class{carClassIdx:00}.Row{rowIdx:00}.EnterPitLap", valueProvider: () => row.EnterPitLap);
                    plugin.AttachDelegate(name: $"Standings.Class{carClassIdx:00}.Row{rowIdx:00}.LastPitStopDuration", valueProvider: () => row.LastPitStopDuration);
                    plugin.AttachDelegate(name: $"Standings.Class{carClassIdx:00}.Row{rowIdx:00}.LastPitDuration", valueProvider: () => row.LastPitDuration);
                    plugin.AttachDelegate(name: $"Standings.Class{carClassIdx:00}.Row{rowIdx:00}.iRating", valueProvider: () => row.iRating);
                    plugin.AttachDelegate(name: $"Standings.Class{carClassIdx:00}.Row{rowIdx:00}.iRatingChange", valueProvider: () => row.iRatingChange);
                    plugin.AttachDelegate(name: $"Standings.Class{carClassIdx:00}.Row{rowIdx:00}.License", valueProvider: () => row.License);
                    plugin.AttachDelegate(name: $"Standings.Class{carClassIdx:00}.Row{rowIdx:00}.SafetyRating", valueProvider: () => row.SafetyRating);
                    plugin.AttachDelegate(name: $"Standings.Class{carClassIdx:00}.Row{rowIdx:00}.CurrentLap", valueProvider: () => row.CurrentLap);
                    plugin.AttachDelegate(name: $"Standings.Class{carClassIdx:00}.Row{rowIdx:00}.StintLap", valueProvider: () => row.StintLap);
                    plugin.AttachDelegate(name: $"Standings.Class{carClassIdx:00}.Row{rowIdx:00}.LapsToClassLeader", valueProvider: () => row.LapsToClassLeader);
                    plugin.AttachDelegate(name: $"Standings.Class{carClassIdx:00}.Row{rowIdx:00}.GapToClassLeader", valueProvider: () => row.GapToClassLeader);
                    plugin.AttachDelegate(name: $"Standings.Class{carClassIdx:00}.Row{rowIdx:00}.LapsToClassOpponentAhead", valueProvider: () => row.LapsToClassOpponentAhead);
                    plugin.AttachDelegate(name: $"Standings.Class{carClassIdx:00}.Row{rowIdx:00}.GapToClassOpponentAhead", valueProvider: () => row.GapToClassOpponentAhead);
                    plugin.AttachDelegate(name: $"Standings.Class{carClassIdx:00}.Row{rowIdx:00}.DeltaToClassLeader", valueProvider: () => row.DeltaToClassLeader);
                    plugin.AttachDelegate(name: $"Standings.Class{carClassIdx:00}.Row{rowIdx:00}.DeltaToPlayer", valueProvider: () => row.DeltaToPlayer);
                    plugin.AttachDelegate(name: $"Standings.Class{carClassIdx:00}.Row{rowIdx:00}.TireCompound", valueProvider: () => row.TireCompound);
                    plugin.AttachDelegate(name: $"Standings.Class{carClassIdx:00}.Row{rowIdx:00}.TireCompoundVisible", valueProvider: () => row.TireCompoundVisible);
                    plugin.AttachDelegate(name: $"Standings.Class{carClassIdx:00}.Row{rowIdx:00}.BestLapTime", valueProvider: () => row.BestLapTime);
                    plugin.AttachDelegate(name: $"Standings.Class{carClassIdx:00}.Row{rowIdx:00}.LastLapTime", valueProvider: () => row.LastLapTime);
                    plugin.AttachDelegate(name: $"Standings.Class{carClassIdx:00}.Row{rowIdx:00}.JokerLapsComplete", valueProvider: () => row.JokerLapsComplete);
                }
            }
        }

        public override void DataUpdate(PluginManager pluginManager, benofficial2 plugin, ref GameData data)
        {
            if (data.FrameTime - _lastUpdateTime < _updateInterval) 
                return;

            _lastUpdateTime = data.FrameTime;

            UpdateLeaderboards(ref data);
            UpdateLeaderboardGaps(ref data);
            _driverModule.UpdateIRatingChange(ref data);

            Driver highlightedDriver = null;
            (highlightedDriver, HighlightedCarClassIdx) = FindHighlightedDriver(ref data);

            if (_sessionModule.Race)
            {
                WidthPixels = Settings.WidthPixelsInRace;
                CarLogoVisible = Settings.CarLogoVisibleInRace;
                GapVisible = Settings.GapVisibleInRace;
                IntervalVisible = Settings.IntervalVisibleInRace;
                BestVisible = Settings.BestVisibleInRace;
                LastVisible = Settings.LastVisibleInRace;
                DeltaVisible = Settings.DeltaVisibleInRace;
                PitDurationVisible = Settings.PitDurationVisibleInRace;
            }
            else
            {
                WidthPixels = Settings.WidthPixels;
                CarLogoVisible = Settings.CarLogoVisible;
                GapVisible = false;
                IntervalVisible = false;
                BestVisible = Settings.BestVisible;
                LastVisible = Settings.LastVisible;
                DeltaVisible = Settings.DeltaVisible;
                PitDurationVisible = Settings.PitDurationVisible;
            }

            int visibleClassCount = 0;

            for (int carClassIdx = 0; carClassIdx < MaxCarClasses; carClassIdx++)
            {
                StandingCarClass carClass = CarClasses[carClassIdx];
                if (carClassIdx < LiveClassLeaderboards.Count)
                {
                    ClassLeaderboard leaderboard = LiveClassLeaderboards[carClassIdx];
                    carClass.IsLeadingClass = (carClassIdx == 0);
                    carClass.Color = leaderboard.CarClassColor;
                    carClass.TextColor = "#000000";

                    if ((leaderboard.CarClassName == null || leaderboard.CarClassName.Length == 0 || leaderboard.CarClassName == "Hosted All Cars"))
                    {
                        // iRacing does not provide a class name in AI races.
                        if (leaderboard.CarNames.Count == 1)
                        {
                            // Fallback to the car model name when there's only 1 car model.
                            carClass.Name = leaderboard.CarNames.First();
                        }
                        else
                        {
                            // Fallback to a generic name when there are multiple car models.
                            carClass.Name = "Class " + leaderboard.CarClassId;
                        }
                    }
                    else
                    {
                        carClass.Name = _carModule.GetCarClassName(leaderboard.CarClassName);
                    }

                    if (carClass.LastMeasuredName != carClass.Name)
                    {
                        carClass.NameSize = MeasureTextInPixels(carClass.Name);

                        // Cache the last name used to compute NameSize to avoid re-measuring every update
                        carClass.LastMeasuredName = carClass.Name;
                    }

                    carClass.Sof = CalculateSof(leaderboard.Drivers);
                    carClass.DriverCount = leaderboard.Drivers.Count;
                    carClass.BestLapTime = FindBestLapTime(leaderboard.Drivers);
                    carClass.BestQualLapTime = FindBestQualLapTime(leaderboard.Drivers);

                    if (leaderboard.Drivers.Count > 0)
                    {
                        carClass.LeaderLastLapTime = leaderboard.Drivers[0].LastLapTime;
                        carClass.LeaderAvgLapTime = leaderboard.Drivers[0].AvgLapTime.GetAverageLapTime();
                    }
                    else
                    {
                        carClass.LeaderLastLapTime = TimeSpan.Zero;
                        carClass.LeaderAvgLapTime = TimeSpan.Zero;
                    }

                    UpdateEstimatedTotalLaps(ref data, carClass, leaderboard.Drivers);

                    int skipRowCount = 0;
                    int maxRowCount;
                    if (highlightedDriver != null && HighlightedCarClassIdx == carClassIdx)
                    {
                        maxRowCount = Settings.MaxRowsPlayerClass;

                        // How many rows to skip to have a lead-focused leaderboard
                        skipRowCount = GetLeadFocusedSkipRowCount(highlightedDriver.CarIdx, leaderboard.Drivers);
                    }
                    else
                    {
                        maxRowCount = Settings.MaxRowsOtherClasses;
                    }

                    carClass.LeadFocusedDividerVisible = skipRowCount > 0;

                    int visibleRowCount = 0;
                    for (int rowIdx = 0; rowIdx < StandingCarClass.MaxRows; rowIdx++)
                    {
                        StandingRow row = carClass.Rows[rowIdx];
                        if (visibleRowCount >= maxRowCount)
                        {
                            // Reached row count limit
                            BlankRow(row);
                            continue;
                        }

                        int actualDriverIdx = rowIdx;
                        if (rowIdx > Settings.LeadFocusedRows - 1)
                        {
                            actualDriverIdx += skipRowCount;
                        }

                        if (actualDriverIdx >= leaderboard.Drivers.Count)
                        {
                            BlankRow(row);
                            continue;
                        }

                        Driver driver = leaderboard.Drivers[actualDriverIdx];
                        if (!IsValidRow(driver))
                        {
                            BlankRow(row);
                            continue;
                        }

                        row.RowVisible = true;
                        row.IsPlayer = driver.IsPlayer;
                        row.IsHighlighted = ((highlightedDriver?.CarIdx ?? -1) == driver.CarIdx);
                        row.Connected = driver.IsConnected;
                        row.LivePositionInClass = driver.LivePositionInClass;

                        if (_sessionModule.Race && driver.QualPositionInClass > 0 && driver.LivePositionInClass > 0)
                        {
                            row.PositionChange = driver.QualPositionInClass - driver.LivePositionInClass;
                        }

                        row.Number = driver.CarNumber;
                        if (_sessionModule.TeamRacing)
                        {
                            row.Name = driver.TeamName;
                        }
                        else
                        {
                            row.Name = driver.Name;
                        }
                        row.CarId = driver.CarId;
                        row.CarBrand = driver.CarBrand;
                        row.CarClassColor = driver.CarClassColor;
                        row.CarClassTextColor = "#000000";
                        row.CountryCode = driver.CountryCode;
                        row.InPitLane = driver.InPit;
                        row.Towing = driver.Towing;
                        row.OutLap = driver.OutLap;
                        row.EnterPitLap = driver.EnterPitLap;
                        row.LastPitStopDuration = driver.LastPitStopDuration;
                        row.LastPitDuration = driver.LastPitDuration;
                        row.iRating = driver.IRating;
                        row.iRatingChange = driver.IRatingChange;
                        row.License = driver.License;
                        row.SafetyRating = driver.SafetyRating;
                        row.CurrentLap = Math.Max(0, driver.Lap > 0 ? driver.Lap : (int)Math.Ceiling(driver.CurrentLapHighPrecision));
                        row.StintLap = driver.StintLap;
                        row.TireCompound = driver.TireCompound;
                        row.TireCompoundVisible = row.TireCompound.Length > 0;

                        row.BestLapTime = GetBestLapTime(driver);

                        if (_sessionModule.Race && !_sessionModule.RaceStarted)
                        {
                            row.LastLapTime = TimeSpan.Zero;
                            row.LapsToClassLeader = 0;
                            row.GapToClassLeader = 0.0;
                            row.LapsToClassOpponentAhead = 0;
                            row.GapToClassOpponentAhead = 0.0;
                        }
                        else
                        {
                            row.LastLapTime = driver.LastLapTime;
                            row.LapsToClassLeader = driver.LapsToClassLeader;
                            row.GapToClassLeader = driver.GapToClassLeader;
                            row.LapsToClassOpponentAhead = driver.LapsToClassOpponentAhead;
                            row.GapToClassOpponentAhead = driver.GapToClassOpponentAhead;
                        }

                        row.JokerLapsComplete = driver.JokerLapsCompleted;

                        if (_sessionModule.Race)
                        {
                            if (row.LastLapTime > TimeSpan.Zero && carClass.LeaderLastLapTime > TimeSpan.Zero)
                            {
                                row.DeltaToClassLeader = row.LastLapTime - carClass.LeaderLastLapTime;
                            }
                            else
                            {
                                row.DeltaToClassLeader = null;
                            }

                            if (highlightedDriver != null && row.LastLapTime > TimeSpan.Zero && highlightedDriver.LastLapTime > TimeSpan.Zero)
                            {
                                if (Settings.InvertDeltaToPlayer)
                                {
                                    row.DeltaToPlayer = highlightedDriver.LastLapTime - row.LastLapTime;
                                }
                                else
                                {
                                    row.DeltaToPlayer = row.LastLapTime - highlightedDriver.LastLapTime;
                                }
                            }
                            else
                            {
                                row.DeltaToPlayer = null;
                            }
                        }
                        else
                        {
                            if (row.BestLapTime > TimeSpan.Zero && carClass.BestLapTime > TimeSpan.Zero)
                            {
                                row.DeltaToClassLeader = row.BestLapTime - carClass.BestLapTime;
                            }
                            else
                            {
                                row.DeltaToClassLeader = null;
                            }

                            if (highlightedDriver != null && row.BestLapTime > TimeSpan.Zero && highlightedDriver.BestLapTime > TimeSpan.Zero)
                            {
                                if (Settings.InvertDeltaToPlayer)
                                {
                                    row.DeltaToPlayer = highlightedDriver.BestLapTime - row.BestLapTime;
                                }
                                else
                                {
                                    row.DeltaToPlayer = row.BestLapTime - highlightedDriver.BestLapTime;
                                }
                            }
                            else
                            {
                                row.DeltaToPlayer = null;
                            }
                        }

                        visibleRowCount++;
                    }

                    carClass.VisibleRowCount = visibleRowCount;
                    visibleClassCount++;
                }
                else
                {
                    BlankCarClass(carClass);
                }
            }

            VisibleClassCount = visibleClassCount;
        }

        public override void End(PluginManager pluginManager, benofficial2 plugin)
        {
            plugin.SaveCommonSettings("StandingsSettings", Settings);
        }

        public void BlankCarClass(StandingCarClass carClass)
        {
            carClass.Name = string.Empty;
            carClass.LastMeasuredName = string.Empty;
            carClass.NameSize = 0;
            carClass.VisibleRowCount = 0;
            carClass.LeadFocusedDividerVisible = false;
            carClass.Color = string.Empty;
            carClass.TextColor = string.Empty;
            carClass.Sof = 0;
            carClass.DriverCount = 0;
            carClass.BestLapTime = TimeSpan.Zero;
            carClass.BestQualLapTime = TimeSpan.Zero;
            carClass.LeaderLastLapTime = TimeSpan.Zero;
            carClass.LeaderAvgLapTime = TimeSpan.Zero;
            carClass.EstimatedTotalLaps = 0;
            carClass.EstimatedTotalLapsConfirmed = false;
            carClass.EstimatedTotalLapsLogged = false;
            carClass.IsLeadingClass = false;

            for (int driverIdx = 0; driverIdx < StandingCarClass.MaxRows; driverIdx++)
            {
                StandingRow driver = carClass.Rows[driverIdx];
                BlankRow(driver);
            }
        }

        public void BlankRow(StandingRow row)
        {
            row.RowVisible = false;
            row.IsPlayer = false;
            row.IsHighlighted = false;
            row.Connected = false;
            row.LivePositionInClass = 0;
            row.PositionChange = 0;
            row.Number = string.Empty;
            row.Name = string.Empty;
            row.CarId = string.Empty;
            row.CarBrand = string.Empty;
            row.CarClassColor = string.Empty;
            row.CarClassTextColor = string.Empty;
            row.CountryCode = string.Empty;
            row.InPitLane = false;
            row.Towing = false;
            row.OutLap = false;
            row.EnterPitLap = 0;
            row.LastPitStopDuration = TimeSpan.Zero;
            row.LastPitDuration = TimeSpan.Zero;
            row.iRating = 0;
            row.iRatingChange = 0;
            row.License = string.Empty;
            row.SafetyRating = 0;
            row.CurrentLap = 0;
            row.StintLap = 0;
            row.LapsToClassLeader = 0;
            row.GapToClassLeader = 0;
            row.LapsToClassOpponentAhead = 0;
            row.GapToClassOpponentAhead = 0;
            row.DeltaToClassLeader = null;
            row.DeltaToPlayer = null;
            row.TireCompound = string.Empty;
            row.TireCompoundVisible = false;
            row.BestLapTime = TimeSpan.Zero;
            row.LastLapTime = TimeSpan.Zero;
            row.JokerLapsComplete = 0;
        }

        private ClassLeaderboard GetLeaderboardFromPool(int poolIndex)
        {
            if (poolIndex < _leaderboardPool.Count)
            {
                var leaderboard = _leaderboardPool[poolIndex];
                leaderboard.CarClassId = 0;
                leaderboard.CarClassName = string.Empty;
                leaderboard.CarClassColor = string.Empty;
                leaderboard.Drivers.Clear();
                leaderboard.CarNames.Clear();
                leaderboard.LeaderPosition = 0;
                leaderboard.EstLapTime = 0.0;
                return leaderboard;
            }
            else
            {
                var newLeaderboard = new ClassLeaderboard();
                _leaderboardPool.Add(newLeaderboard);
                return newLeaderboard;
            }
        }

        private void UpdateLeaderboards(ref GameData data)
        {
            // Build leaderboards grouped by car class without LINQ allocations
            // Reuse buffers to avoid per-frame allocations
            var classMap = _classMapBuffer;
            classMap.Clear();
            var scoredDriversAllClasses = _scoredDriversBuffer;
            scoredDriversAllClasses.Clear();

            int poolIndex = 0;
            foreach (var driver in _driverModule.Drivers.Values)
            {
                int carClassId = driver.CarClassId;
                bool newLeaderboard = false;
                if (!classMap.TryGetValue(carClassId, out var leaderboard))
                {
                    leaderboard = GetLeaderboardFromPool(poolIndex++);
                    leaderboard.CarClassId = carClassId;
                    leaderboard.CarClassColor = driver.CarClassColor;
                    leaderboard.CarClassName = driver.CarClassName;
                    newLeaderboard = true;
                }

                bool scored = true;
                if (_sessionModule.Race)
                {
                    // In a loaded replay, sometimes the qual results are missing.
                    // Consider all drivers as scored in that case.
                    if (_driverModule.QualResultsUpdated)
                    {
                        // Only consider drivers that have an official qual position.
                        // In heat races, this ignores drivers not in the current heat.
                        scored = driver.QualPositionInClass > 0;
                    }
                }
                else
                {
                    scored = driver.Position > 0 || driver.IsConnected;
                }

                if (driver.IsPaceCar)
                    scored = false;

                if (scored)
                {
                    leaderboard.Drivers.Add(driver);
                    leaderboard.CarNames.Add(driver.CarName);

                    // Don't add empty leaderboards
                    if (newLeaderboard)
                        classMap[carClassId] = leaderboard;
                }
            }

            // Reuse existing list to avoid allocation; clear and repopulate
            LiveClassLeaderboards.Clear();
            foreach (var leaderboard in classMap.Values)
            {
                LiveClassLeaderboards.Add(leaderboard);
            }

            // Sort drivers inside each leaderboard using in-place Sort to avoid LINQ
            foreach (var leaderboard in LiveClassLeaderboards)
            {
                bool sorted = false;
                if (_sessionModule.Race)
                {
                    if (!_sessionModule.RaceStarted)
                    {
                        // Before the start keep the leaderboard sorted by qual position
                        leaderboard.Drivers.Sort((a, b) => a.QualPositionInClass.CompareTo(b.QualPositionInClass));
                        sorted = true;
                    }
                    else if (!_sessionModule.RaceFinished)
                    {
                        // During the race sort on position on track for a live leaderboard.
                        // Except for ovals under caution, show the official position.
                        if (!(_sessionModule.Oval && data.NewData.Flag_Yellow == 1))
                        {
                            leaderboard.Drivers.Sort((a, b) => b.CurrentLapHighPrecision.CompareTo(a.CurrentLapHighPrecision));
                            sorted = true;
                        }
                    }
                }

                if (!sorted)
                {
                    // Place drivers with PositionInClass > 0 first, then by PositionInClass ascending
                    leaderboard.Drivers.Sort((a, b) =>
                    {
                        int aMissing = a.PositionInClass <= 0 ? 1 : 0;
                        int bMissing = b.PositionInClass <= 0 ? 1 : 0;
                        if (aMissing != bMissing)
                            return aMissing.CompareTo(bMissing);
                        return a.PositionInClass.CompareTo(b.PositionInClass);
                    });
                }

                scoredDriversAllClasses.AddRange(leaderboard.Drivers);

                int posInClass = 1;
                for (int i = 0; i < leaderboard.Drivers.Count; i++)
                {
                    var driver = leaderboard.Drivers[i];
                    if (_sessionModule.Race)
                    {
                        if (!_sessionModule.RaceStarted)
                        {
                            driver.LivePositionInClass = driver.QualPositionInClass;
                        }
                        else
                        {
                            driver.LivePositionInClass = posInClass++;
                        }
                    }
                    else
                    {
                        driver.LivePositionInClass = driver.Position > 0 ? posInClass++ : 0;
                    }

                    if (driver.IsPlayer)
                    {
                        _driverModule.PlayerDriver.LivePositionInClass = driver.LivePositionInClass;
                    }

                    if (driver.CarIdx == _driverModule.HighlightedDriver.CarIdx)
                    {
                        _driverModule.HighlightedDriver.LivePositionInClass = driver.LivePositionInClass;
                        _driverModule.HighlightedDriver.CarClassColor = driver.CarClassColor;
                        _driverModule.HighlightedDriver.CarClassTextColor = "#000000";
                    }

                    if (driver.Position > 0 && (leaderboard.LeaderPosition == 0 || driver.Position < leaderboard.LeaderPosition))
                    {
                        leaderboard.LeaderPosition = driver.Position;
                    }

                    if (driver.CarClassEstLapTime > 0 && (leaderboard.EstLapTime == 0.0 || driver.CarClassEstLapTime < leaderboard.EstLapTime))
                    {
                        leaderboard.EstLapTime = driver.CarClassEstLapTime;
                    }
                }
            }

            // Sort the class leaderboards in-place on the estimated lap time (fastest first).
            LiveClassLeaderboards.Sort((a, b) => a.EstLapTime.CompareTo(b.EstLapTime));

            TotalDriverCount = scoredDriversAllClasses.Count;
            TotalSoF = CalculateSof(scoredDriversAllClasses);
        }

        private void UpdateLeaderboardGaps(ref GameData data)
        {
            foreach (var leaderboard in LiveClassLeaderboards)
            {
                if (leaderboard.Drivers.Count == 0)
                    continue;

                Driver classLeader = leaderboard.Drivers[0];
                Driver driverAhead = null;
                foreach (var driver in leaderboard.Drivers)
                {
                    if (driver.CarIdx == classLeader.CarIdx)
                    {
                        driver.LapsToClassLeader = 0;
                        driver.GapToClassLeader = 0.0;
                        driver.LapsToClassOpponentAhead = 0;
                        driver.GapToClassOpponentAhead = 0.0;
                    }
                    else
                    {
                        driver.LapsToClassLeader = Math.Max(0, (int)Math.Floor(classLeader.CurrentLapHighPrecision - driver.CurrentLapHighPrecision));

                        if (driver.LapsToClassLeader > 0)
                        {
                            driver.GapToClassLeader = 0.0;
                        }
                        else
                        {
                            driver.GapToClassLeader = GapToCarAhead(driver, classLeader);
                        }

                        if (driverAhead != null)
                        {
                            driver.LapsToClassOpponentAhead = Math.Max(0, (int)Math.Floor(driverAhead.CurrentLapHighPrecision - driver.CurrentLapHighPrecision));
                            if (driver.LapsToClassOpponentAhead > 0)
                            {
                                driver.GapToClassOpponentAhead = 0.0;
                            }
                            else
                            {
                                driver.GapToClassOpponentAhead = GapToCarAhead(driver, driverAhead);
                            }
                        }
                        else
                        {
                            driver.LapsToClassOpponentAhead = 0;
                            driver.GapToClassOpponentAhead = 0.0;
                        }
                    }

                    driverAhead = driver;
                }
            }
        }

        static private double GapToCarAhead(Driver current, Driver ahead)
        {
            double timeDiff = RelativeModule.GetEstTimeDiff(current.CarClassEstLapTime, ahead.EstTime, current.EstTime);

            // Make sure timeDiff is positive
            while (timeDiff < 0.0)
                timeDiff += current.CarClassEstLapTime;

            return timeDiff;
        }

        public (Driver, int) FindHighlightedDriver(ref GameData data)
        {
            Driver highlightedDriver = _driverModule.GetHighlightedDriver(true);
            if (highlightedDriver != null)
            {
                for (int carClassIdx = 0; carClassIdx < LiveClassLeaderboards.Count; carClassIdx++)
                {
                    var leaderboard = LiveClassLeaderboards[carClassIdx];

                    // Looping instead of only checking the first driver, because in AI races
                    // all classes are grouped together.
                    foreach (Driver driver in leaderboard.Drivers)
                    {
                        if (driver.CarClassId == highlightedDriver.CarClassId)
                        {
                            return (highlightedDriver, carClassIdx);
                        }
                    }
                }
            }

            return (null, -1);
        }

        public bool IsValidRow(Driver driver)
        {
            return true;
        }

        public int GetValidRowCount(List<Driver> drivers)
        {
            int validRowCount = 0;
            foreach (Driver driver in drivers)
            {
                if (IsValidRow(driver)) 
                    validRowCount++;
            }
            return validRowCount;
        }

        public int GetLeadFocusedSkipRowCount(int highlightedCarIdx, List<Driver> drivers)
        {
            // Find the highlighted car in the driver list
            int highlightedDriverIdx = -1;
            for (int driverIdx = 0; driverIdx < drivers.Count; driverIdx++)
            {
                if (drivers[driverIdx].CarIdx == highlightedCarIdx)
                {
                    highlightedDriverIdx = driverIdx;
                    break;
                }
            }

            if (highlightedDriverIdx < 0 || !IsValidRow(drivers[highlightedDriverIdx]))
            {
                // Highlighted car not in driver list
                return 0;
            }

            int validRowCount = GetValidRowCount(drivers);
            if (validRowCount <= Settings.MaxRowsPlayerClass)
            {
                // They all fit in
                return 0;
            }

            int nonLeadFocusedRowCount = Math.Max(0, Settings.MaxRowsPlayerClass - Settings.LeadFocusedRows);
            if (highlightedDriverIdx <= Settings.LeadFocusedRows + nonLeadFocusedRowCount / 2)
            {
                // Player already centered in top rows
                return 0;
            }

            // Center the player in the view by trying to keep an equal amount of rows before as after 
            int shown = Math.Max(0, validRowCount - Settings.LeadFocusedRows);
            int before = Math.Max(0, highlightedDriverIdx - Settings.LeadFocusedRows);
            int after = Math.Max(0, validRowCount - highlightedDriverIdx - 1);
            int skipRowCount = 0;

            // TODO make this O(1)
            while (shown > nonLeadFocusedRowCount)
            {
                if (before > after)
                {
                    before--;
                    shown--;
                    skipRowCount++;
                }
                else
                {
                    after--;
                    shown--;
                }
            }

            return skipRowCount;
        }

        public TimeSpan GetBestLapTime(Driver driver)
        {
            if (_sessionModule.Race)
            {
                // Show qualification times before the start of the race.
                if (!_sessionModule.RaceStarted)
                    return driver.QualLapTime;

                // Make sure we have a best lap time for the first lap of a race.
                // iRacing often doesn't provide a valid best lap time for lap 1.
                if (driver.BestLapTime <= TimeSpan.Zero)
                    return driver.LastLapTime;
            }

            return driver.BestLapTime;
        }

        public TimeSpan FindBestLapTime(List<Driver> drivers)
        {
            TimeSpan bestLapTime = TimeSpan.MaxValue;
            foreach (Driver driver in drivers)
            {
                TimeSpan driverBestLapTime = GetBestLapTime(driver);
                if (driverBestLapTime > TimeSpan.Zero && driverBestLapTime < bestLapTime)
                {
                    bestLapTime = driverBestLapTime;
                }
            }
            return bestLapTime < TimeSpan.MaxValue ? bestLapTime : TimeSpan.Zero;
        }

        public TimeSpan FindBestQualLapTime(List<Driver> drivers)
        {
            TimeSpan bestQualLapTime = TimeSpan.MaxValue;
            foreach (Driver driver in drivers)
            {
                if (driver.QualLapTime > TimeSpan.Zero && driver.QualLapTime < bestQualLapTime)
                {
                    bestQualLapTime = driver.QualLapTime;
                }
            }
            return bestQualLapTime < TimeSpan.MaxValue ? bestQualLapTime : TimeSpan.Zero;
        }

        public int CalculateSof(List<Driver> drivers)
        {
            if (drivers.Count <= 0) 
                return 0;

            double sum = 0.0;
            foreach (var driver in drivers)
            {
                sum += Math.Pow(2.0, -(driver.IRating) / 1600.0);
            }

            return (int)((1600.0 / Math.Log(2.0)) * Math.Log(drivers.Count / sum));
        }

        public float MeasureTextInPixels(string text)
        {
            if (_fontBitmap == null || _fontGraphics == null || _font == null)
                return 0.0f;

            SizeF textSize;
            textSize = _fontGraphics.MeasureString(text, _font);
            return textSize.Width;
        }

        public void UpdateEstimatedTotalLaps(ref GameData data, StandingCarClass carClass, List<Driver> drivers)
        {
            // Use a slightly faster avg lap time as a safety margin in case the leader will pace up.
            // This will overestimate the laps slightly in the beginning, but for fuel decisions it's better than underestimating.
            const double lapTimeSafePct = 0.99;
            bool extraLap = false;

            if (_sessionModule.Race)
            {
                if (!_sessionModule.RaceStarted)
                {
                    carClass.EstimatedTotalLapsConfirmed = false;
                    carClass.EstimatedTotalLapsLogged = false;
                }

                if (carClass.EstimatedTotalLapsConfirmed)
                    return;

                double leaderCurrentLapHighPrecision = drivers.Count > 0 ? drivers[0].CurrentLapHighPrecisionRaw : 0.0;

                // Confirm the total laps when the player gets the white flag.
                // Because we don't have the data for other cars in the iRacing SDK.
                // TODO: Missing edge cases such as when the player gets lapped on the leader's last lap.
                // Or when the player tows before starting the last lap. Etc.
                bool playerStartingLastLap = _driverModule.PlayerDriver.HadWhiteFlag && _driverModule.PlayerDriver.CurrentLapHighPrecision % 1.0 < 0.50;
                bool playerHadCheckered = data.NewData.Flag_Checkered > 0;
                if (playerStartingLastLap || playerHadCheckered)
                {
                    carClass.EstimatedTotalLaps = (int)Math.Max(1, Math.Ceiling(leaderCurrentLapHighPrecision));
                    carClass.EstimatedTotalLapsConfirmed = true;

                    SimHub.Logging.Current.Info($"Estimated total laps confirmed: " +
                        $"CarClassName={carClass.Name}, " +
                        $"EstimatedTotalLaps={carClass.EstimatedTotalLaps}, " +
                        $"LeaderCurrentLapHighPrecision={leaderCurrentLapHighPrecision}, ");

                    return;
                }

                double sessionTimeRemain = Math.Max(0.0, _sessionModule.SessionTimeTotal.TotalSeconds - _sessionModule.RaceTimer);
                TimeSpan avgLapTime = carClass.LeaderAvgLapTime;
                if (avgLapTime <= TimeSpan.Zero)
                    avgLapTime = carClass.BestLapTime;

                if (avgLapTime <= TimeSpan.Zero)
                    avgLapTime = carClass.BestQualLapTime;

                carClass.EstimatedTotalLaps = EstimateTotalLaps(leaderCurrentLapHighPrecision, 
                    _sessionModule.SessionLapsTotal, 
                    sessionTimeRemain, 
                    avgLapTime.TotalSeconds * lapTimeSafePct,
                    out extraLap);

                if (carClass.IsLeadingClass)
                {
                    if (sessionTimeRemain > Constants.SecondsEpsilon)
                        LeadingClassDoingExtraLap = extraLap;
                }
                else
                {
                    if (LeadingClassDoingExtraLap && !extraLap && sessionTimeRemain > Constants.SecondsEpsilon)
                    {
                        // Other classes must do the extra lap as well.
                        carClass.EstimatedTotalLaps += 1;
                    }
                }

                if (_driverModule.PlayerDriver.HadWhiteFlag && !carClass.EstimatedTotalLapsLogged)
                {
                    SimHub.Logging.Current.Info($"Estimated total laps at player's white flag: " +
                        $"CarClassName={carClass.Name}, " +
                        $"EstimatedTotalLaps={carClass.EstimatedTotalLaps}, " +
                        $"SessionTotalLaps={_sessionModule.SessionLapsTotal}, " +
                        $"LeaderCurrentLapHighPrecision={leaderCurrentLapHighPrecision}, " +
                        $"SessionTimeRemain={sessionTimeRemain}, " +
                        $"AvgLapTime={avgLapTime.TotalSeconds}");

                    carClass.EstimatedTotalLapsLogged = true;
                }

                return;
            }

            carClass.EstimatedTotalLaps = EstimateTotalLaps(_driverModule.PlayerDriver.CurrentLapHighPrecision, 
                _sessionModule.SessionLapsTotal,
                data.NewData.SessionTimeLeft.TotalSeconds, 
                _driverModule.PlayerDriver.BestLapTime.TotalSeconds * lapTimeSafePct,
                out extraLap);
        }

        static public int EstimateTotalLaps(double currentLapHighPrecision, int sessionTotalLaps, double sessionTimeRemain, double avgLapTime, out bool extraLap)
        {
            currentLapHighPrecision = Math.Max(0.0, currentLapHighPrecision);
            extraLap = false;

            if (sessionTimeRemain >= Constants.UnlimitedTimeSeconds)
                return Math.Max(0, sessionTotalLaps);

            // Even if it is a lap-limited race, check if there's enough time to complete the remaining laps.
            if (sessionTotalLaps > 0)
            {
                double lapsRemaining = Math.Max(0.0, sessionTotalLaps - currentLapHighPrecision);
                TimeSpan minTimeForLaps = TimeSpan.FromSeconds(lapsRemaining * avgLapTime);
                if (minTimeForLaps <= TimeSpan.FromSeconds(sessionTimeRemain))
                {
                    // We have enough time left to complete the remaining laps.
                    return sessionTotalLaps;
                }
            }

            // Can't estimate laps in a timed race without a valid average lap time.
            if (avgLapTime < Constants.SecondsEpsilon)
                return 0;

            double estimatedTotalLaps = Math.Max(0.0, sessionTimeRemain / avgLapTime);
            estimatedTotalLaps += currentLapHighPrecision;

            // Add an extra lap if we would cross the line with more than X% of a lap time remaining on the timer.
            // It is unknown what are the exact white flag rule constants used by iRacing and seem to change per track.
            double lapCompletedPctWhenTimerHitsZero = estimatedTotalLaps % 1.0;
            if (lapCompletedPctWhenTimerHitsZero < Constants.LapEpsilon || lapCompletedPctWhenTimerHitsZero > Constants.WhiteFlagRuleLapPct)
            {
                estimatedTotalLaps++;
                extraLap = true;
            }

            return (int)Math.Max(0, Math.Ceiling(estimatedTotalLaps));
        }
    }
}
