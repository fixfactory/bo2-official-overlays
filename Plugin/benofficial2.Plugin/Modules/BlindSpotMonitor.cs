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
using System.ComponentModel;

namespace benofficial2.Plugin
{
    public class BlindSpotMonitorSettings : ModuleSettings
    {
        public bool Enabled { get; set; } = false;
    }

    public class BlindSpotMonitorModule : PluginModuleBase
    {
        private SpotterModule _spotterModule = null;

        public BlindSpotMonitorSettings Settings { get; set; }
        public bool Visible { get; set; } = false;

        public override void Init(PluginManager pluginManager, benofficial2 plugin)
        {
            _spotterModule = plugin.GetModule<SpotterModule>();

            Settings = plugin.ReadCommonSettings<BlindSpotMonitorSettings>("BlindSpotMonitorSettings", () => new BlindSpotMonitorSettings());
            plugin.AttachDelegate(name: "BlindSpotMonitor.Enabled", valueProvider: () => Settings.Enabled);
            plugin.AttachDelegate(name: "BlindSpotMonitor.Visible", valueProvider: () => Visible);
        }

        public override void DataUpdate(PluginManager pluginManager, benofficial2 plugin, ref GameData data)
        {
            if (!Settings.Enabled)
            {
                Visible = false;
            }
            else
            {
                Visible = _spotterModule.OverlapAhead < 0 || _spotterModule.OverlapBehind > 0;
            }
        }

        public override void End(PluginManager pluginManager, benofficial2 plugin)
        {
            plugin.SaveCommonSettings("BlindSpotMonitorSettings", Settings);
        }
    }
}
