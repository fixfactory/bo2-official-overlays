/*
    benofficial2's Official Overlays
    Copyright (C) 2026 benofficial2

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
using System.ComponentModel;

namespace benofficial2.Plugin
{
    public class TrackTemperatureSettings : ModuleSettings
    {
        public bool HideInReplay { get; set; } = true;

        public int BackgroundOpacity { get; set; } = 0;
    }

    public class TrackTemperatureModule : PluginModuleBase
    {
        public TrackTemperatureSettings Settings { get; set; }

        public override void Init(PluginManager pluginManager, benofficial2 plugin)
        {
            Settings = plugin.ReadCommonSettings<TrackTemperatureSettings>("TrackTemperatureSettings", () => new TrackTemperatureSettings());
            plugin.AttachDelegate(name: "TrackTemperature.HideInReplay", valueProvider: () => Settings.HideInReplay);
            plugin.AttachDelegate(name: "TrackTemperature.BackgroundOpacity", valueProvider: () => Settings.BackgroundOpacity);
        }

        public override void DataUpdate(PluginManager pluginManager, benofficial2 plugin, ref GameData data)
        {

        }

        public override void End(PluginManager pluginManager, benofficial2 plugin)
        {
            plugin.SaveCommonSettings("TrackTemperatureSettings", Settings);
        }
    }
}
