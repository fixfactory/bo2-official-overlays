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
using System.ComponentModel;

namespace benofficial2.Plugin
{
    public class TelemetrySettings : ModuleSettings
    {
        public bool HideInReplay { get; set; } = true;
        public bool TracesVisible { get; set; } = true;
        public int TracesWidth { get; set; } = 500;
        public int TracesSpeed { get; set; } = 75;
        public bool HandbrakeTraceVisible { get; set; } = true;
        public bool SteeringTraceVisible { get; set; } = true;
        public bool GuideLinesVisible { get; set; } = true;
        public bool PedalsVisible { get; set; } = true;
        public bool PedalsValuesVisible { get; set; } = true;
        public bool SteeringVisible { get; set; } = true;
        public bool GearAndSpeedVisible { get; set; } = true;
        public bool ShiftLightsVisible { get; set; } = true;
        public bool BlinkWhenAssistActive { get; set; } = true;
        public int BackgroundOpacity { get; set; } = 60;
    }

    public class TelemetryModule : PluginModuleBase
    {
        public TelemetrySettings Settings { get; set; }

        public override void Init(PluginManager pluginManager, benofficial2 plugin)
        {
            Settings = plugin.ReadCommonSettings<TelemetrySettings>("TelemetrySettings", () => new TelemetrySettings());
            plugin.AttachDelegate(name: "Telemetry.HideInReplay", valueProvider: () => Settings.HideInReplay);
            plugin.AttachDelegate(name: "Telemetry.TracesVisible", valueProvider: () => Settings.TracesVisible);
            plugin.AttachDelegate(name: "Telemetry.TracesWidth", valueProvider: () => Settings.TracesWidth);
            plugin.AttachDelegate(name: "Telemetry.TracesSpeed", valueProvider: () => Settings.TracesSpeed);
            plugin.AttachDelegate(name: "Telemetry.HandbrakeTraceVisible", valueProvider: () => Settings.HandbrakeTraceVisible);
            plugin.AttachDelegate(name: "Telemetry.SteeringTraceVisible", valueProvider: () => Settings.SteeringTraceVisible);
            plugin.AttachDelegate(name: "Telemetry.GuideLinesVisible", valueProvider: () => Settings.GuideLinesVisible);
            plugin.AttachDelegate(name: "Telemetry.PedalsVisible", valueProvider: () => Settings.PedalsVisible);
            plugin.AttachDelegate(name: "Telemetry.PedalsValuesVisible", valueProvider: () => Settings.PedalsValuesVisible);
            plugin.AttachDelegate(name: "Telemetry.SteeringVisible", valueProvider: () => Settings.SteeringVisible);
            plugin.AttachDelegate(name: "Telemetry.GearAndSpeedVisible", valueProvider: () => Settings.GearAndSpeedVisible);
            plugin.AttachDelegate(name: "Telemetry.ShiftLightsVisible", valueProvider: () => Settings.ShiftLightsVisible);
            plugin.AttachDelegate(name: "Telemetry.BlinkWhenAssistActive", valueProvider: () => Settings.BlinkWhenAssistActive);
            plugin.AttachDelegate(name: "Telemetry.BackgroundOpacity", valueProvider: () => Settings.BackgroundOpacity);
        }

        public override void DataUpdate(PluginManager pluginManager, benofficial2 plugin, ref GameData data)
        {
        }

        public override void End(PluginManager pluginManager, benofficial2 plugin)
        {
            plugin.SaveCommonSettings("TelemetrySettings", Settings);
        }
    }
}
