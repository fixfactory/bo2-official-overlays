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
    public class PitlaneHelperSettings : ModuleSettings
    {
        public bool DistanceVisible { get; set; } = true;
        public bool DialsVisible { get; set; } = true;
        public bool SpeedVisible { get; set; } = true;
        public int BackgroundOpacity { get; set; } = 0;
    }

    public class PitlaneHelperModule : PluginModuleBase
    {
        private DriverModule _driverModule = null;
        private TrackModule _trackModule = null;

        private DateTime _lastUpdateTime = DateTime.MinValue;
        private TimeSpan _updateInterval = TimeSpan.FromMilliseconds(100);

        public PitlaneHelperSettings Settings { get; set; }

        public float ApproachingPitsFromTrackPct { get; set; } = -1.0f;

        public string Target { get; set; } = string.Empty;

        public float DistanceToTarget { get; set; } = 0.0f;

        public float MaxDistanceToTarget { get; set; } = 0.0f;

        public override void Init(PluginManager pluginManager, benofficial2 plugin)
        {
            _driverModule = plugin.GetModule<DriverModule>();
            _trackModule = plugin.GetModule<TrackModule>();

            Settings = plugin.ReadCommonSettings<PitlaneHelperSettings>("PitlaneHelperSettings", () => new PitlaneHelperSettings());
            plugin.AttachDelegate(name: "PitlaneHelper.DistanceVisible", valueProvider: () => Settings.DistanceVisible);
            plugin.AttachDelegate(name: "PitlaneHelper.DialsVisible", valueProvider: () => Settings.DialsVisible);
            plugin.AttachDelegate(name: "PitlaneHelper.SpeedVisible", valueProvider: () => Settings.SpeedVisible);
            plugin.AttachDelegate(name: "PitlaneHelper.BackgroundOpacity", valueProvider: () => Settings.BackgroundOpacity);
            plugin.AttachDelegate(name: "PitlaneHelper.Target", valueProvider: () => Target);
            plugin.AttachDelegate(name: "PitlaneHelper.DistanceToTarget", valueProvider: () => DistanceToTarget);
            plugin.AttachDelegate(name: "PitlaneHelper.MaxDistanceToTarget", valueProvider: () => MaxDistanceToTarget);
        }

        public override void DataUpdate(PluginManager pluginManager, benofficial2 plugin, ref GameData data)
        {
            if (data.FrameTime - _lastUpdateTime < _updateInterval)
                return;

            _lastUpdateTime = data.FrameTime;

            UpdateDistanceToTarget(ref data);
        }

        public override void End(PluginManager pluginManager, benofficial2 plugin)
        {
            plugin.SaveCommonSettings("PitlaneHelperSettings", Settings);
        }

        private void UpdateDistanceToTarget(ref GameData data)
        {
            RawDataHelper.TryGetTelemetryData<float>(ref data, out float lapDistPct, "LapDistPct");
            RawDataHelper.TryGetSessionData<float>(ref data, out float driverPitTrkPct, "DriverInfo", "DriverPitTrkPct");

            if (_driverModule.PlayerDriver.AproachingPits)
            {
                Target = "PitEntry";

                if (_trackModule.PitEntryTrackPct >= 0.0f && ApproachingPitsFromTrackPct >= 0.0f)
                {
                    DistanceToTarget = Math.Min(1.0f, Math.Max(0.0f, (_trackModule.PitEntryTrackPct - lapDistPct) * (float)_trackModule.TrackLength));
                    MaxDistanceToTarget = Math.Min(1.0f, Math.Max(0.0f, (_trackModule.PitEntryTrackPct - ApproachingPitsFromTrackPct) * (float)_trackModule.TrackLength));
                }
                else
                {
                    DistanceToTarget = 0.0f;
                    MaxDistanceToTarget = 0.0f;
                }

                return;
            }

            if (_driverModule.PlayerDriver.InPit)
            {
                float distanceToPitBoxTrackPct = (float)RelativeModule.GetRelativeTrackDistance(lapDistPct, driverPitTrkPct);

                if (distanceToPitBoxTrackPct < 0.0f)
                {
                    Target = "PitBox";

                    if (_trackModule.PitEntryTrackPct >= 0.0f && driverPitTrkPct >= 0.0f)
                    {
                        DistanceToTarget = Math.Min(1.0f, Math.Max(0.0f, Math.Abs(distanceToPitBoxTrackPct) * (float)_trackModule.TrackLength));

                        float maxDistanceToPitBoxTrackPct = (float)RelativeModule.GetRelativeTrackDistance(_trackModule.PitEntryTrackPct, driverPitTrkPct);
                        MaxDistanceToTarget = Math.Min(1.0f, Math.Max(0.0f, Math.Abs(maxDistanceToPitBoxTrackPct) * (float)_trackModule.TrackLength));
                    }
                    else
                    {
                        DistanceToTarget = 0.0f;
                        MaxDistanceToTarget = 0.0f;
                    }                    
                }
                else
                {
                    Target = "PitExit";

                    if (_trackModule.PitExitTrackPct >= 0.0f && driverPitTrkPct >= 0.0f)
                    {
                        float distanceToPitExitTrackPct = (float)RelativeModule.GetRelativeTrackDistance(lapDistPct, _trackModule.PitExitTrackPct);
                        DistanceToTarget = Math.Min(1.0f, Math.Max(0.0f, Math.Abs(distanceToPitExitTrackPct) * (float)_trackModule.TrackLength));

                        float maxDistanceToPitBoxTrackPct = (float)RelativeModule.GetRelativeTrackDistance(driverPitTrkPct, _trackModule.PitExitTrackPct);
                        MaxDistanceToTarget = Math.Min(1.0f, Math.Max(0.0f, Math.Abs(maxDistanceToPitBoxTrackPct) * (float)_trackModule.TrackLength));
                    }
                    else
                    {
                        DistanceToTarget = 0.0f;
                        MaxDistanceToTarget = 0.0f;
                    }
                }

                return;
            }

            Target = string.Empty;
        }
    }
}
