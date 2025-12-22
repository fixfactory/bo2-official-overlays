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

using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace benofficial2.Plugin
{
    public class VersionChecker
    {
        private const string _versionUrl = "https://raw.githubusercontent.com/fixfactory/bo2-official-overlays/main/Versions.json";
        private const string _downloadPageUrl = "https://github.com/fixfactory/bo2-official-overlays/releases";

        public const string CurrentVersion = "5.0";
        public const string BetaVersion = "1";
        public bool FailedToCheck { get; private set; } = false;

        static public bool IsBetaVersion
        {
            get
            {
                return !string.IsNullOrEmpty(BetaVersion);
            }
        }

        static public string FullVersion
        {
            get
            {
                if (IsBetaVersion)
                {
                    return $"{CurrentVersion} (beta{BetaVersion})";
                }
                return CurrentVersion;
            }
        }

        public async Task CheckForUpdateAsync()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string json = await client.GetStringAsync(_versionUrl);
                    JObject jsonObject = JObject.Parse(json);

                    string latestVersion = jsonObject["plugin"]?.ToString();

                    if (!string.IsNullOrEmpty(latestVersion) && IsNewerVersion(latestVersion, CurrentVersion))
                    {
                        DialogResult result = MessageBox.Show(
                            $"A new version {latestVersion} for benofficial2 plugin is available.\nDo you want to visit the download page?",
                            "Update Available",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Information
                        );

                        if (result == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(_downloadPageUrl);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FailedToCheck = true;
                SimHub.Logging.Current.Error($"An error occurred while checking for benofficial2 plugin updates:\n{ex.Message}");
            }
        }

        private bool IsNewerVersion(string latestVersion, string currentVersion)
        {
            Version latest = new Version(latestVersion);
            Version current = new Version(currentVersion);
            if (IsBetaVersion)
            {
                return latest >= current;
            }
            return latest > current;
        }
    }
}