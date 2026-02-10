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
    public class GeneralSettings : ModuleSettings
    {
        public bool CheckForUpdates { get; set; } = true;
        public bool ClockFormat24h { get; set; } = true;
        public System.Windows.Media.Color BackgroundColor { get; set; } = System.Windows.Media.Color.FromRgb(0x1B, 0x1B, 0x24);
    }

    public class GeneralModule : PluginModuleBase
    {
        public GeneralSettings Settings { get; set; }
        public string ErrorMessage { get; set; } = "";

        public override void Init(PluginManager pluginManager, benofficial2 plugin)
        {
            // There was an issue with loading settings stored pre-3.0, so we need to specify a new settings key
            Settings = plugin.ReadCommonSettings<GeneralSettings>("GeneralSettings_3.1", () => new GeneralSettings());
            plugin.AttachDelegate(name: "CheckForUpdates", valueProvider: () => Settings.CheckForUpdates);
            plugin.AttachDelegate(name: "ClockFormat24h", valueProvider: () => Settings.ClockFormat24h);
            plugin.AttachDelegate(name: "BackgroundColor", valueProvider: () => Settings.BackgroundColor);
            plugin.AttachDelegate(name: "ErrorMessage", valueProvider: () => ErrorMessage);
        }

        public override void DataUpdate(PluginManager pluginManager, benofficial2 plugin, ref GameData data)
        {

        }

        public override void End(PluginManager pluginManager, benofficial2 plugin)
        {
            plugin.SaveCommonSettings("GeneralSettings_3.1", Settings);
        }
    }
}
