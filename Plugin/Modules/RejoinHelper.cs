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
using System.ComponentModel;
using System.Globalization;

namespace benofficial2.Plugin
{
    public class RejoinHelperSettings : ModuleSettings
    {
        public bool Enabled { get; set; } = true;
        public ModuleSettingFloat MinimumClearGap { get; set; } = new ModuleSettingFloat(3.5f);
        public ModuleSettingFloat MinimumCareGap { get; set; } = new ModuleSettingFloat(1.5f);
        public int MinSpeed { get; set; } = 35;

        // Legacy properties for backwards compatibility (saved pre 3.0)
        public string MinClearGapString { get => MinimumClearGap.ValueString; set => MinimumClearGap.ValueString = value; }
        public string MinCareGapString { get => MinimumCareGap.ValueString; set => MinimumCareGap.ValueString = value; }
    }

    public class RejoinHelperModule : PluginModuleBase
    {
        private DriverModule _driverModule = null;
        private SessionModule _sessionModule = null;

        public RejoinHelperSettings Settings { get; set; }
        public bool Visible { get; set; } = false;
        public double Gap { get; set; } = 0;
        public string State { get; set; } = string.Empty;
        public double ColorPct { get; set; } = 0;

        public const string StateClear = "Clear";
        public const string StateCare = "Care";
        public const string StateYield = "Yield";

        public override void Init(PluginManager pluginManager, benofficial2 plugin)
        {
            _driverModule = plugin.GetModule<DriverModule>();
            _sessionModule = plugin.GetModule<SessionModule>();

            Settings = plugin.ReadCommonSettings<RejoinHelperSettings>("RejoinHelperSettings", () => new RejoinHelperSettings());
            plugin.AttachDelegate(name: "RejoinHelper.Enabled", valueProvider: () => Settings.Enabled);
            plugin.AttachDelegate(name: "RejoinHelper.MinClearGap", valueProvider: () => Settings.MinimumClearGap.Value);
            plugin.AttachDelegate(name: "RejoinHelper.MinCareGap", valueProvider: () => Settings.MinimumCareGap.Value);
            plugin.AttachDelegate(name: "RejoinHelper.MinSpeed", valueProvider: () => Settings.MinSpeed);
            plugin.AttachDelegate(name: "RejoinHelper.Visible", valueProvider: () => Visible);
            plugin.AttachDelegate(name: "RejoinHelper.Gap", valueProvider: () => Gap);
            plugin.AttachDelegate(name: "RejoinHelper.State", valueProvider: () => State);
            plugin.AttachDelegate(name: "RejoinHelper.ColorPct", valueProvider: () => ColorPct);
        }

        public override void DataUpdate(PluginManager pluginManager, benofficial2 plugin, ref GameData data)
        {
            // Wait for race to be started for a few seconds not to trigger on a standing start
            if (!Settings.Enabled || (_sessionModule.Race && (_sessionModule.RaceFinished || _sessionModule.RaceTimer < 3)))
            {
                Visible = false;
                Gap = 0;
                State = StateClear;
                ColorPct = 100;
            }
            else
            {
                RawDataHelper.TryGetTelemetryData<int>(ref data, out int trackSurface, "PlayerTrackSurface");
                bool isSlow = data.NewData.SpeedKmh < Settings.MinSpeed;
                Visible = isSlow || trackSurface == 0;

                List<Opponent> opponents = data.NewData.OpponentsBehindOnTrack;
                if (opponents.Count > 0)
                {
                    _driverModule.Drivers.TryGetValue(opponents[0].CarNumber, out Driver driver);
                    if (driver != null)
                    {
                        Gap = Math.Abs(driver.RelativeGapToPlayer);
                    }
                    else
                    {
                        Gap = 0;
                    }
                }
                else
                {
                    Gap = 0;
                }

                if (Gap <= 0)
                {
                    State = StateClear;
                    ColorPct = 100;
                }
                else
                {
                    if (Gap >= Settings.MinimumClearGap.Value)
                    {
                        State = StateClear;
                        ColorPct = 100;
                    }
                    else if (Gap >= Settings.MinimumCareGap.Value)
                    {
                        State = StateCare;
                        double ratio = (Gap - Settings.MinimumCareGap.Value) / (Settings.MinimumClearGap.Value - Settings.MinimumCareGap.Value);
                        ColorPct = ((100 - 50) * ratio) + 50;
                    }
                    else
                    {
                        State = StateYield;
                        double ratio = Gap / Settings.MinimumClearGap.Value;
                        ColorPct = 50 * ratio;
                    }
                }
            }
        }

        public override void End(PluginManager pluginManager, benofficial2 plugin)
        {
            plugin.SaveCommonSettings("RejoinHelperSettings", Settings);
        }
    }
}
