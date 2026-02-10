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

using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace benofficial2.Plugin
{
    public class RemoteJsonFile
    {
        private readonly string _url = string.Empty;

        public JObject Json { get; private set; } = null;

        public RemoteJsonFile(string url)
        {
            _url = url;
        }

        public void LoadAsync()
        {
            Task.Run(() =>
            {
                Load().Wait();
            });
        }

        private async Task Load()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string json = await client.GetStringAsync(_url);
                    Json = JObject.Parse(json);
                }
            }
            catch (Exception ex)
            {
                SimHub.Logging.Current.Error($"An error occurred while downloading {_url}\n{ex.Message}");
            }
        }
    }
}