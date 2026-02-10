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
using System.Windows.Media;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace benofficial2.Plugin
{
    [PluginDescription("Adds extra properties needed to use benofficial2's Official Overlays for iRacing. Enable 'Show in left main menu' to access overlay configuration options.")]
    [PluginAuthor("benofficial2")]
    [PluginName("benofficial2 Plugin")]
    public class benofficial2 : IPlugin, IDataPlugin, IWPFSettingsV2
    {
        private List<PluginModuleBase> _sortedModules;
        public Dictionary<string, PluginModuleBase> Modules { get; private set; }

        public string PluginName { get; internal set; } = "";
        public bool PluginRunning { get; internal set; } = false;
        public bool iRacingRunning { get; internal set; } = false;

        /// <summary>
        /// Instance of the current plugin manager
        /// </summary>
        public PluginManager PluginManager { get; set; }

        /// <summary>
        /// Gets the left menu icon. Icon must be 24x24 and compatible with black and white display.
        /// </summary>
        public ImageSource PictureIcon => this.ToIcon(Properties.Resources.sdkmenuicon);

        /// <summary>
        /// Gets a short plugin title to show in left menu. Return null if you want to use the title as defined in PluginName attribute.
        /// </summary>
        public string LeftMenuTitle => "benofficial2";

        /// <summary>
        /// Returns the settings control, return null if no settings control is required
        /// </summary>
        /// <param name="pluginManager"></param>
        /// <returns></returns>
        public System.Windows.Controls.Control GetWPFSettingsControl(PluginManager pluginManager)
        {
            return new SettingsControl(this);
        }

        /// <summary>
        /// Called once after plugins startup
        /// Plugins are rebuilt at game change
        /// </summary>
        /// <param name="pluginManager"></param>
        public void Init(PluginManager pluginManager)
        {
            PluginName = $"BENOFFICIAL2 PLUGIN {VersionChecker.FullVersion}";
            SimHub.Logging.Current.Info($"Starting benofficial2 plugin version {VersionChecker.FullVersion}");

            this.AttachDelegate(name: "PluginRunning", valueProvider: () => PluginRunning);
            this.AttachDelegate(name: "iRacingRunning", valueProvider: () => iRacingRunning);

            // Create all the modules
            Modules = PluginModuleFactory.CreateAllPluginModules();

            // Sort modules by update priority
            _sortedModules = Modules.Values.OrderBy(m => m.UpdatePriority).ToList();

            // Init each module
            foreach (var module in _sortedModules)
            {
                module.Init(pluginManager, this);
            }

            // Check for latest plugin version
            if (GetModule<GeneralModule>().Settings.CheckForUpdates)
            {
                Task.Run(() =>
                {
                    VersionChecker versionChecker = new VersionChecker();
                    versionChecker.CheckForUpdateAsync().Wait();
                    if (versionChecker.FailedToCheck)
                    {
                        GetModule<GeneralModule>().ErrorMessage = "Server unreachable";
                    }
                });
            }

            PluginRunning = true;
        }

        /// <summary>
        /// Called one time per game data update, contains all normalized game data,
        /// raw data are intentionally "hidden" under a generic object type (A plugin SHOULD NOT USE IT)
        ///
        /// This method is on the critical path, it must execute as fast as possible and avoid throwing any error
        ///
        /// </summary>
        /// <param name="pluginManager"></param>
        /// <param name="data">Current game data, including current and previous data frame.</param>
        public void DataUpdate(PluginManager pluginManager, ref GameData data)
        {
            iRacingRunning = data.GameRunning && data.GameName == "IRacing";
            if (!iRacingRunning) return;
            if (data.OldData == null || data.NewData == null) return;

            // Update each module
            foreach (var module in _sortedModules)
            {
                module.DataUpdate(pluginManager, this, ref data);
            }
        }

        /// <summary>
        /// Called at plugin manager stop, close/dispose anything needed here !
        /// Plugins are rebuilt at game change
        /// </summary>
        /// <param name="pluginManager"></param>
        public void End(PluginManager pluginManager)
        {
            // End each module
            foreach (var module in _sortedModules)
            {
                module.End(pluginManager, this);
            }

            PluginRunning = false;
        }

        public T GetModule<T>() where T : PluginModuleBase
        {
            var key = typeof(T).Name;
            if (Modules.TryGetValue(key, out var module))
            {
                return module as T;
            }
            SimHub.Logging.Current.Error($"Missing Plugin Module {typeof(T).Name}");
            return null;
        }
    }
}