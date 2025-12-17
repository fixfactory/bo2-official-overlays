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

namespace benofficial2.Plugin
{
    using OpponentsWithDrivers = List<(Opponent, Driver)>;

    public class DeltaSettings : ModuleSettings
    {
        public int BackgroundOpacity { get; set; } = 60;
        public int ColoredBackgroundOpacity { get; set; } = 90;
    }

    public class HeadToHeadRow
    {
        public bool Visible { get; set; } = false;
        public int LivePositionInClass { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
        public double GapToPlayer { get; set; } = 0;
        public double LapsToPlayer { get; set; } = 0;
        public TimeSpan LastLapTime { get; set; } = TimeSpan.Zero;
    }

    public class DeltaModule : PluginModuleBase
    {
        private DateTime _lastUpdateTime = DateTime.MinValue;
        private TimeSpan _updateInterval = TimeSpan.FromMilliseconds(500);
        private SessionModule _sessionModule = null;
        private StandingsModule _standingsModule = null;
        private DriverModule _driverModule = null;

        public float Speed { get; internal set; } = 0.0f;

        public DeltaSettings Settings { get; set; }

        public HeadToHeadRow HeadToHeadRowAhead { get; internal set; }
        public HeadToHeadRow HeadToHeadRowBehind { get; internal set; }

        public override void Init(PluginManager pluginManager, benofficial2 plugin)
        {
            _sessionModule = plugin.GetModule<SessionModule>();
            _standingsModule = plugin.GetModule<StandingsModule>();
            _driverModule = plugin.GetModule<DriverModule>();

            HeadToHeadRowAhead = new HeadToHeadRow();
            HeadToHeadRowBehind = new HeadToHeadRow();

            Settings = plugin.ReadCommonSettings<DeltaSettings>("DeltaSettings", () => new DeltaSettings());
            plugin.AttachDelegate(name: "Delta.BackgroundOpacity", valueProvider: () => Settings.BackgroundOpacity);
            plugin.AttachDelegate(name: "Delta.ColoredBackgroundOpacity", valueProvider: () => Settings.ColoredBackgroundOpacity);
            plugin.AttachDelegate(name: "Delta.Speed", valueProvider: () => Speed);

            InitHeadToHead(plugin, "Ahead", HeadToHeadRowAhead);
            InitHeadToHead(plugin, "Behind", HeadToHeadRowBehind);
        }

        public void InitHeadToHead(benofficial2 plugin, string aheadBehind, HeadToHeadRow row)
        {
            plugin.AttachDelegate(name: $"Delta.{aheadBehind}.Visible", valueProvider: () => row.Visible);
            plugin.AttachDelegate(name: $"Delta.{aheadBehind}.LivePositionInClass", valueProvider: () => row.LivePositionInClass);
            plugin.AttachDelegate(name: $"Delta.{aheadBehind}.Name", valueProvider: () => row.Name);
            plugin.AttachDelegate(name: $"Delta.{aheadBehind}.GapToPlayer", valueProvider: () => row.GapToPlayer);
            plugin.AttachDelegate(name: $"Delta.{aheadBehind}.LapsToPlayer", valueProvider: () => row.LapsToPlayer);
            plugin.AttachDelegate(name: $"Delta.{aheadBehind}.LastLapTime", valueProvider: () => row.LastLapTime);
        }

        public override void DataUpdate(PluginManager pluginManager, benofficial2 plugin, ref GameData data)
        {
            dynamic raw = data.NewData.GetRawDataObject();
            if (raw == null) return;
            if (_sessionModule == null) return;

            float delta = 0.0f;
            if (_sessionModule.Practice)
            {
                try { delta = (float)raw.Telemetry["LapDeltaToSessionBestLap_DD"]; } catch { }
            }
            else if (_sessionModule.Race)
            {
                try { delta = (float)raw.Telemetry["LapDeltaToSessionBestLap_DD"]; } catch { }
            }
            else if (_sessionModule.Qual)
            {
                try { delta = (float)raw.Telemetry["LapDeltaToBestLap_DD"]; } catch { }
            }

            Speed = Math.Min((float)data.NewData.SpeedLocal, (float)data.NewData.SpeedLocal * -delta);

            if (data.FrameTime - _lastUpdateTime < _updateInterval) return;
            _lastUpdateTime = data.FrameTime;

            UpdateHeadToHead(ref data, HeadToHeadRowAhead, -1);
            UpdateHeadToHead(ref data, HeadToHeadRowBehind, 1);
        }

        public void UpdateHeadToHead(ref GameData data, HeadToHeadRow row, int relativeIdx)
        {
            if (_standingsModule.HighlightedCarClassIdx < 0 || _standingsModule.HighlightedCarClassIdx >= _standingsModule.LiveClassLeaderboards.Count)
            {
                BlankRow(row);
                return;
            }

            List<Driver> drivers = _standingsModule.LiveClassLeaderboards[_standingsModule.HighlightedCarClassIdx].Drivers;

            int livePositionInClass = _driverModule.PlayerDriver.LivePositionInClass + relativeIdx;
            int opponentIdx = livePositionInClass - 1;
            if (opponentIdx < 0 || opponentIdx >= drivers.Count)
            {
                BlankRow(row);
                return;
            }

            Driver driver = drivers[opponentIdx];

            if (!driver.IsConnected)
            {
                BlankRow(row);
                return;
            }

            row.Visible = driver.Position > 0;
            row.LivePositionInClass = livePositionInClass;
            row.Name = driver.Name;
            row.LastLapTime = driver.LastLapTime;

            if (relativeIdx < 0)
            {
                Driver playerDriver = _driverModule.GetPlayerDriver();
                if (playerDriver != null)
                {
                    row.GapToPlayer = playerDriver.GapToClassOpponentAhead;
                    row.LapsToPlayer = playerDriver.LapsToClassOpponentAhead;
                }
            }
            else
            {
                row.GapToPlayer = driver.GapToClassOpponentAhead;
                row.LapsToPlayer = driver.LapsToClassOpponentAhead;
            }
        }

        public override void End(PluginManager pluginManager, benofficial2 plugin)
        {
            plugin.SaveCommonSettings("DeltaSettings", Settings);
        }

        public void BlankRow(HeadToHeadRow row)
        {
            row.Visible = false;
            row.LivePositionInClass = 0;
            row.Name = string.Empty;
            row.GapToPlayer = 0;
            row.LapsToPlayer = 0;
            row.LastLapTime = TimeSpan.Zero;
        }
    }
}
