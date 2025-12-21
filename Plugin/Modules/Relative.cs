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
using System.ComponentModel;
using System.Linq;
using System.Security.Policy;

namespace benofficial2.Plugin
{
    public class RelativeSettings : ModuleSettings
    {
        public bool HideInReplay { get; set; } = true;
        public int WidthPixels { get; set; } = 430;
        public int MaxRows { get; set; } = 4;
        public bool HeaderVisible { get; set; } = true;
        public int HeaderOpacity { get; set; } = 90;
        public bool FooterVisible { get; set; } = false;
        public bool CarLogoVisible { get; set; } = true;
        public bool CountryFlagVisible { get; set; } = true;
        public bool SafetyRatingVisible { get; set; } = true;
        public bool IRatingVisible { get; set; } = true;
        public bool IRatingChangeVisible { get; set; } = false;
        public bool LastLapTimeVisible { get; set; } = true;
        public bool TireCompoundVisible { get; set; } = true;
        public int AlternateRowBackgroundColor { get; set; } = 15;
        public bool HighlightPlayerRow { get; set; } = true;
        public int BackgroundOpacity { get; set; } = 60;

        // Deprecated pre-4.0
        public int Width { get; set; } = 80;
    }

    public class RelativeRow
    {
        public bool RowVisible { get; set; } = false;
        public int LivePositionInClass { get; set; } = 0;
        public string CarClassColor { get; set; } = string.Empty;
        public string CarClassTextColor { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string CarBrand { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
        public bool OutLap { get; set; } = false;
        public int iRating { get; set; } = 0;
        public float iRatingChange { get; set; } = 0;
        public string License { get; set; } = string.Empty;
        public double SafetyRating { get; set; } = 0;
        public double GapToPlayer { get; set; } = 0;
        public double CurrentLapHighPrecision { get; set; } = 0;
        public TimeSpan LastLapTime { get; set; } = TimeSpan.Zero;
        public int SessionFlags { get; set; } = 0;
        public string TireCompound { get; set; } = string.Empty;
        public int PushToPassCount { get; set; } = 0;
        public bool PushToPassActivated { get; set; } = false;
    }

    public class RelativeAhead
    {
        public const int MaxRows = 5;
        public List<RelativeRow> Rows { get; internal set; }

        public RelativeAhead()
        {
            Rows = new List<RelativeRow>(Enumerable.Range(0, MaxRows).Select(x => new RelativeRow()));
        }
    }
    public class RelativeBehind
    {
        public const int MaxRows = 5;
        public List<RelativeRow> Rows { get; internal set; }

        public RelativeBehind()
        {
            Rows = new List<RelativeRow>(Enumerable.Range(0, MaxRows).Select(x => new RelativeRow()));
        }
    }

    public class RelativeModule : PluginModuleBase
    {
        private DateTime _lastUpdateTime = DateTime.MinValue;
        private TimeSpan _updateInterval = TimeSpan.FromMilliseconds(500);

        private DriverModule _driverModule = null;
        private CarModule _carModule = null;
        private FlairModule _flairModule = null;

        public RelativeSettings Settings { get; set; }

        public RelativeAhead Ahead = new RelativeAhead();
        public RelativeBehind Behind = new RelativeBehind();

        public List<Driver> DriversAheadOnTrack { get; internal set; } = new List<Driver>();
        public List<Driver> DriversBehindOnTrack { get; internal set; } = new List<Driver>();

        public override void Init(PluginManager pluginManager, benofficial2 plugin)
        {
            _driverModule = plugin.GetModule<DriverModule>();
            _carModule = plugin.GetModule<CarModule>();
            _flairModule = plugin.GetModule<FlairModule>();

            Settings = plugin.ReadCommonSettings<RelativeSettings>("RelativeSettings", () => new RelativeSettings());
            plugin.AttachDelegate(name: "Relative.HideInReplay", valueProvider: () => Settings.HideInReplay);
            plugin.AttachDelegate(name: "Relative.Width", valueProvider: () => Settings.WidthPixels);
            plugin.AttachDelegate(name: "Relative.MaxRows", valueProvider: () => Settings.MaxRows);
            plugin.AttachDelegate(name: "Relative.HeaderVisible", valueProvider: () => Settings.HeaderVisible);
            plugin.AttachDelegate(name: "Relative.HeaderOpacity", valueProvider: () => Settings.HeaderOpacity);
            plugin.AttachDelegate(name: "Relative.FooterVisible", valueProvider: () => Settings.FooterVisible);
            plugin.AttachDelegate(name: "Relative.CarLogoVisible", valueProvider: () => Settings.CarLogoVisible);
            plugin.AttachDelegate(name: "Relative.CountryFlagVisible", valueProvider: () => Settings.CountryFlagVisible);
            plugin.AttachDelegate(name: "Relative.SafetyRatingVisible", valueProvider: () => Settings.SafetyRatingVisible);
            plugin.AttachDelegate(name: "Relative.iRatingVisible", valueProvider: () => Settings.IRatingVisible);
            plugin.AttachDelegate(name: "Relative.iRatingChangeVisible", valueProvider: () => Settings.IRatingChangeVisible);
            plugin.AttachDelegate(name: "Relative.LastLapTimeVisible", valueProvider: () => Settings.LastLapTimeVisible);
            plugin.AttachDelegate(name: "Relative.TireCompoundVisible", valueProvider: () => Settings.TireCompoundVisible);
            plugin.AttachDelegate(name: "Relative.AlternateRowBackgroundColor", valueProvider: () => Settings.AlternateRowBackgroundColor);
            plugin.AttachDelegate(name: "Relative.HighlightPlayerRow", valueProvider: () => Settings.HighlightPlayerRow);
            plugin.AttachDelegate(name: "Relative.BackgroundOpacity", valueProvider: () => Settings.BackgroundOpacity);

            InitRelative(plugin, "Ahead", Ahead.Rows, RelativeAhead.MaxRows);
            InitRelative(plugin, "Behind", Behind.Rows, RelativeBehind.MaxRows);
        }

        private void InitRelative(benofficial2 plugin, string aheadBehind, List<RelativeRow> rows, int maxRows)
        {
            for (int rowIdx = 0; rowIdx < maxRows; rowIdx++)
            {
                RelativeRow row = rows[rowIdx];
                plugin.AttachDelegate(name: $"Relative.{aheadBehind}{rowIdx:00}.RowVisible", valueProvider: () => row.RowVisible);
                plugin.AttachDelegate(name: $"Relative.{aheadBehind}{rowIdx:00}.LivePositionInClass", valueProvider: () => row.LivePositionInClass);
                plugin.AttachDelegate(name: $"Relative.{aheadBehind}{rowIdx:00}.CarClassColor", valueProvider: () => row.CarClassColor);
                plugin.AttachDelegate(name: $"Relative.{aheadBehind}{rowIdx:00}.CarClassTextColor", valueProvider: () => row.CarClassTextColor);
                plugin.AttachDelegate(name: $"Relative.{aheadBehind}{rowIdx:00}.Number", valueProvider: () => row.Number);
                plugin.AttachDelegate(name: $"Relative.{aheadBehind}{rowIdx:00}.Name", valueProvider: () => row.Name);
                plugin.AttachDelegate(name: $"Relative.{aheadBehind}{rowIdx:00}.CarBrand", valueProvider: () => row.CarBrand);
                plugin.AttachDelegate(name: $"Relative.{aheadBehind}{rowIdx:00}.CountryCode", valueProvider: () => row.CountryCode);
                plugin.AttachDelegate(name: $"Relative.{aheadBehind}{rowIdx:00}.OutLap", valueProvider: () => row.OutLap);
                plugin.AttachDelegate(name: $"Relative.{aheadBehind}{rowIdx:00}.iRating", valueProvider: () => row.iRating);
                plugin.AttachDelegate(name: $"Relative.{aheadBehind}{rowIdx:00}.iRatingChange", valueProvider: () => row.iRatingChange);
                plugin.AttachDelegate(name: $"Relative.{aheadBehind}{rowIdx:00}.License", valueProvider: () => row.License);
                plugin.AttachDelegate(name: $"Relative.{aheadBehind}{rowIdx:00}.SafetyRating", valueProvider: () => row.SafetyRating);
                plugin.AttachDelegate(name: $"Relative.{aheadBehind}{rowIdx:00}.GapToPlayer", valueProvider: () => row.GapToPlayer);
                plugin.AttachDelegate(name: $"Relative.{aheadBehind}{rowIdx:00}.CurrentLapHighPrecision", valueProvider: () => row.CurrentLapHighPrecision);
                plugin.AttachDelegate(name: $"Relative.{aheadBehind}{rowIdx:00}.LastLapTime", valueProvider: () => row.LastLapTime);
                plugin.AttachDelegate(name: $"Relative.{aheadBehind}{rowIdx:00}.SessionFlags", valueProvider: () => row.SessionFlags);
                plugin.AttachDelegate(name: $"Relative.{aheadBehind}{rowIdx:00}.TireCompound", valueProvider: () => row.TireCompound);
                plugin.AttachDelegate(name: $"Relative.{aheadBehind}{rowIdx:00}.PushToPassCount", valueProvider: () => row.PushToPassCount);
                plugin.AttachDelegate(name: $"Relative.{aheadBehind}{rowIdx:00}.PushToPassActivated", valueProvider: () => row.PushToPassActivated);
            }
        }

        public override void DataUpdate(PluginManager pluginManager, benofficial2 plugin, ref GameData data)
        {
            // Update gaps every frame to keep them smooth in the Rejoin Helper.
            UpdateGaps(ref data);

            if (data.FrameTime - _lastUpdateTime < _updateInterval) 
                return;

            _lastUpdateTime = data.FrameTime;

            UpdateRelativeDistances(ref data);
            UpdateDriversAheadBehindOnTrack(ref data);

            UpdateRelative(ref data, Ahead.Rows, RelativeAhead.MaxRows, DriversAheadOnTrack);
            UpdateRelative(ref data, Behind.Rows, RelativeBehind.MaxRows, DriversBehindOnTrack);
        }

        public void UpdateRelative(ref GameData data, List<RelativeRow> rows, int maxRows, List<Driver> drivers)
        {
            for (int rowIdx = 0; rowIdx < maxRows; rowIdx++)
            {
                RelativeRow row = rows[rowIdx];

                if (rowIdx >= drivers.Count)
                {
                    BlankRow(row);
                    continue;
                }

                Driver driver = drivers[rowIdx];
                if (driver == null || !IsValidRow(driver))
                {
                    BlankRow(row);
                    continue;
                }

                row.RowVisible = true;
                row.LivePositionInClass = driver.LivePositionInClass;
                row.CarClassColor = driver.CarClassColor;
                row.CarClassTextColor = "#000000";
                row.Number = driver.CarNumber;
                row.Name = driver.Name;
                row.CarBrand = _carModule.GetCarBrand(driver.CarId, driver.CarName); ;
                row.CountryCode = _flairModule.GetCountryCode(driver.FlairId);
                row.OutLap = driver.OutLap;
                row.iRating = driver.IRating;
                row.iRatingChange = driver.IRatingChange;
                row.License = driver.License;
                row.SafetyRating = driver.SafetyRating;
                row.GapToPlayer = driver.RelativeGapToPlayer;
                row.CurrentLapHighPrecision = driver.CurrentLapHighPrecisionRaw;
                row.LastLapTime = driver.LastLapTime;
                row.SessionFlags = driver.SessionFlags;
                row.TireCompound = _carModule.GetTireCompoundLetter(driver.TireCompoundIdx);
                row.PushToPassCount = driver.PushToPassCount;
                row.PushToPassActivated = driver.PushToPassActivated;
            }
        }

        public override void End(PluginManager pluginManager, benofficial2 plugin)
        {
            plugin.SaveCommonSettings("RelativeSettings", Settings);
        }

        public void UpdateGaps(ref GameData data)
        {
            Driver highlightedDriver = _driverModule.GetHighlightedDriver(false);
            if (highlightedDriver == null)
                return;

            foreach (Driver opponentDriver in DriversAheadOnTrack)
            {
                // Scale opponent estimated time to player's car class
                double opponentEstTimeScaled = GetEstTimeScaled(opponentDriver, highlightedDriver);

                double timeDiff = GetEstTimeDiff(highlightedDriver.CarClassEstLapTime, opponentEstTimeScaled, highlightedDriver.EstTime);

                // Make sure timeDiff is positive
                while (timeDiff < 0.0)
                    timeDiff += highlightedDriver.CarClassEstLapTime;

                opponentDriver.RelativeGapToPlayer = timeDiff;
            }

            foreach (Driver opponentDriver in DriversBehindOnTrack)
            {
                // Scale opponent estimated time to player's car class
                double opponentEstTimeScaled = GetEstTimeScaled(opponentDriver, highlightedDriver);

                double timeDiff = GetEstTimeDiff(highlightedDriver.CarClassEstLapTime, opponentEstTimeScaled, highlightedDriver.EstTime);

                // Make sure timeDiff is negative
                while (timeDiff > 0.0)
                    timeDiff -= highlightedDriver.CarClassEstLapTime;

                opponentDriver.RelativeGapToPlayer = timeDiff;
            }
        }

        public void UpdateRelativeDistances(ref GameData data)
        {
            Driver highlightedDriver = _driverModule.GetHighlightedDriver(false);
            if (highlightedDriver == null)
                return;

            bool isLoneQual = data.NewData.SessionTypeName.IndexOf("Lone Qual") != -1;

            foreach (Driver driver in _driverModule.Drivers.Values)
            {
                if (driver.CarIdx == highlightedDriver.CarIdx || driver.IsPaceCar || !driver.IsConnected || driver.InPit || isLoneQual)
                {
                    driver.RelativeDistanceToPlayer = 0.0;
                    continue;
                }
                    
                driver.RelativeDistanceToPlayer = GetRelativeTrackDistance(highlightedDriver.TrackPositionPercent, driver.TrackPositionPercent);
            }
        }

        public void UpdateDriversAheadBehindOnTrack(ref GameData data)
        {
            Driver highlightedDriver = _driverModule.GetHighlightedDriver(false);
            if (highlightedDriver == null)
            {
                DriversBehindOnTrack = new List<Driver>();
                DriversAheadOnTrack = new List<Driver>();
                return;
            }

            DriversBehindOnTrack = _driverModule.Drivers.Values
                .Where(d => d.RelativeDistanceToPlayer > 0)
                .OrderBy(d => d.RelativeDistanceToPlayer)
                .ToList();

            DriversAheadOnTrack = _driverModule.Drivers.Values
                .Where(d => d.RelativeDistanceToPlayer < 0)
                .OrderBy(d => Math.Abs(d.RelativeDistanceToPlayer))
                .ToList();
        }

        public static double GetRelativeTrackDistance(double currentTrackPosPct, double otherTrackPosPct)
        {
            if (currentTrackPosPct < otherTrackPosPct)
            {
                if (otherTrackPosPct - currentTrackPosPct < 0.5)
                    return (otherTrackPosPct - currentTrackPosPct) * -1;
                else
                    return 1.0 - otherTrackPosPct + currentTrackPosPct;
            }
            else
            {
                if (currentTrackPosPct - otherTrackPosPct <= 0.5)
                    return currentTrackPosPct - otherTrackPosPct;
                else
                    return (1.0 - currentTrackPosPct + otherTrackPosPct) * -1;

            }
        }

        private double GetEstTimeScaled(Driver opponentDriver, Driver playerDriver)
        {
            // Scale opponent estimated time to player's car class
            double opponentEstTimeScaled = opponentDriver.EstTime * playerDriver.CarClassEstLapTime / opponentDriver.CarClassEstLapTime;

            // Make sure opponent time is not ahead when behind on track, and not behind when ahead on track.
            if (opponentDriver.TrackPositionPercent < playerDriver.TrackPositionPercent)
            {
                opponentEstTimeScaled = Math.Min(playerDriver.EstTime, opponentEstTimeScaled);
            }
            else if (opponentDriver.TrackPositionPercent > playerDriver.TrackPositionPercent)
            {
                opponentEstTimeScaled = Math.Max(playerDriver.EstTime, opponentEstTimeScaled);
            }

            return opponentEstTimeScaled;
        }

        static public double GetEstTimeDiff(double estLapTime, double opponentEstTime, double playerEstTime)
        {
            if (estLapTime < Constants.SecondsEpsilon || playerEstTime < Constants.SecondsEpsilon || opponentEstTime < Constants.SecondsEpsilon)
                return 0.0;

            double timeDiff = opponentEstTime - playerEstTime;

            while (timeDiff < -0.5 * estLapTime)
                timeDiff += estLapTime;

            while (timeDiff > 0.5 * estLapTime)
                timeDiff -= estLapTime;

            return timeDiff;
        }

        public void BlankRow(RelativeRow row)
        {
            row.RowVisible = false;
            row.LivePositionInClass = 0;
            row.CarClassColor = string.Empty;
            row.CarClassTextColor = string.Empty;
            row.Number = string.Empty;
            row.Name = string.Empty;
            row.CarBrand = string.Empty;
            row.CountryCode = string.Empty;
            row.OutLap = false;
            row.iRating = 0;
            row.iRatingChange = 0;
            row.License = string.Empty;
            row.SafetyRating = 0;
            row.GapToPlayer = 0;
            row.CurrentLapHighPrecision = 0;
            row.LastLapTime = TimeSpan.Zero;
            row.SessionFlags = 0;
        }
        public bool IsValidRow(Driver driver)
        {
            return true;
        }
    }
}
