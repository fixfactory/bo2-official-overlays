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

using SimHub.Plugins.Styles;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;

namespace benofficial2.Plugin
{
    public class SettingsControlViewModel
    {
        public benofficial2 Plugin { get; }

        public SettingsControlViewModel()
        {
        }
        public SettingsControlViewModel(benofficial2 plugin) : this()
        {
            Plugin = plugin;
        }
    }

    /// <summary>
    /// Logique d'interaction pour SettingsControl.xaml
    /// </summary>
    public partial class SettingsControl : UserControl
    {
        public benofficial2 Plugin { get; }

        public SettingsControl()
        {
            InitializeComponent();
        }

        public SettingsControl(benofficial2 plugin) : this()
        {
            this.Plugin = plugin;
            this.DataContext = new SettingsControlViewModel(plugin);
        }

        private void GitHubLink_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/fixfactory/bo2-official-overlays",
                UseShellExecute = true // Ensures it opens in the default browser
            });
        }

        private void DiscordLink_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://discord.gg/s2834nmdYx",
                UseShellExecute = true // Ensures it opens in the default browser
            });
        }

        private void TwitchLink_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://www.twitch.tv/benofficial2",
                UseShellExecute = true // Ensures it opens in the default browser
            });
        }

        private void DonateLink_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://streamelements.com/benofficial2/tip",
                UseShellExecute = true // Ensures it opens in the default browser
            });
        }

        private void CheckForUpdates_Click(object sender, RoutedEventArgs e)
        {
            if (CheckForUpdates.IsChecked == true)
            {
                Task.Run(() =>
                {
                    VersionChecker versionChecker = new VersionChecker();
                    versionChecker.CheckForUpdateAsync().Wait();
                });
            }
        }

        private void ChatIs_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://chatis.is2511.com/",
                UseShellExecute = true // Ensures it opens in the default browser
            });
        }

        private void SteeringBrowseImage_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Select PNG Image",
                Filter = "PNG Images (*.png)|*.png",
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = false
            };

            if (dialog.ShowDialog() == true)
            {
                SteeringImagePathTextBox.Text = dialog.FileName;
            }
        }

        private void SetupCoverBrowseImage_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Select PNG Image",
                Filter = "PNG Images (*.png)|*.png",
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = false
            };

            if (dialog.ShowDialog() == true)
            {
                SetupCoverImagePathTextBox.Text = dialog.FileName;
            }
        }
    }
}