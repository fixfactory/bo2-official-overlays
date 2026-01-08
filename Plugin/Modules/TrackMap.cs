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
using System.ComponentModel;

namespace benofficial2.Plugin
{
    public class TrackMapSettings : ModuleSettings
    {
        public bool HideInReplay { get; set; } = true;
        public int DotRadius { get; set; } = 20;
        public int FontSize { get; set; } = 20;
        public int LineThickness { get; set; } = 20;
        public int BackgroundOpacity { get; set; } = 0;
    }

    public class TrackMapModule : PluginModuleBase
    {
        public TrackMapSettings Settings { get; set; }

        public override void Init(PluginManager pluginManager, benofficial2 plugin)
        {
            Settings = plugin.ReadCommonSettings<TrackMapSettings>("TrackMapSettings", () => new TrackMapSettings());
            plugin.AttachDelegate(name: "TrackMap.HideInReplay", valueProvider: () => Settings.HideInReplay);
            plugin.AttachDelegate(name: "TrackMap.DotRadius", valueProvider: () => Settings.DotRadius);
            plugin.AttachDelegate(name: "TrackMap.FontSize", valueProvider: () => Settings.FontSize);
            plugin.AttachDelegate(name: "TrackMap.LineThickness", valueProvider: () => Settings.LineThickness);
            plugin.AttachDelegate(name: "TrackMap.BackgroundOpacity", valueProvider: () => Settings.BackgroundOpacity);
        }

        public override void DataUpdate(PluginManager pluginManager, benofficial2 plugin, ref GameData data)
        {

        }

        public override void End(PluginManager pluginManager, benofficial2 plugin)
        {
            plugin.SaveCommonSettings("TrackMapSettings", Settings);
        }
    }
}
