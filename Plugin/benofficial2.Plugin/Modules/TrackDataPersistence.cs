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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace benofficial2.Plugin
{
    public class TrackDataPersistence
    {
        private readonly string _filePath;
        private Dictionary<string, PersistentTrackData> _data = null;
        private readonly object _dataLock = new object();
        private Task _lastSaveTask = Task.CompletedTask;

        public TrackDataPersistence()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory ?? Environment.CurrentDirectory;
            var dir = Path.Combine(baseDir, "PluginsData", "IRacing");
            Directory.CreateDirectory(dir);
            _filePath = Path.Combine(dir, "TrackData.json");
        }

        public async Task LoadAsync()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    Clear();
                    return;
                }

                string json = File.ReadAllText(_filePath);
                if (string.IsNullOrWhiteSpace(json))
                {
                    Clear();
                    return;
                }

                var dict = JsonConvert.DeserializeObject<Dictionary<string, PersistentTrackData>>(json);
                lock (_dataLock)
                {
                    _data = dict ?? new Dictionary<string, PersistentTrackData>();
                }
            }
            catch (Exception ex)
            {
                SimHub.Logging.Current.Error($"Failed to load track data from {_filePath}: {ex.Message}");
                Clear();
                return;
            }
        }

        public bool IsLoaded()
        {
            lock (_dataLock)
            {
                return _data != null;
            }
        }

        public void Clear()
        {
            lock (_dataLock)
            {
                if (_data != null)
                    _data.Clear();
                else
                    _data = new Dictionary<string, PersistentTrackData>();
            }
        }

        public PersistentTrackData GetData(string trackId)
        {
            if (string.IsNullOrEmpty(trackId))
                return new PersistentTrackData();

            lock (_dataLock)
            {
                if (!_data.TryGetValue(trackId, out var data))
                {
                    data = new PersistentTrackData();
                    _data[trackId] = data;
                }
                return data;
            }
        }

        public async Task SaveAsync(string trackId, PersistentTrackData toSave)
        {
            if (string.IsNullOrEmpty(trackId))
                return;

            try
            {
                lock (_dataLock)
                {
                    _data[trackId] = toSave;
                }

                // Create a shallow copy to pass to persistence to avoid locking during IO
                Dictionary<string, PersistentTrackData> copy;
                lock (_dataLock)
                {
                    copy = new Dictionary<string, PersistentTrackData>(_data);
                }

                _lastSaveTask = Task.Run(async () =>
                {
                    if (copy == null) return;

                    try
                    {
                        var json = JsonConvert.SerializeObject(copy, Formatting.Indented);
                        await Task.Run(() => File.WriteAllText(_filePath, json)).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        SimHub.Logging.Current.Error($"Failed to save track data to {_filePath}: {ex.Message}");
                    }
                });
                await _lastSaveTask.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                SimHub.Logging.Current.Error($"Error saving track data: {ex.Message}");
            }
        }

        public void WaitForPendingSave()
        {
            try
            {
                _lastSaveTask?.Wait(2000);
            }
            catch (Exception)
            {
                // Swallow any exceptions during shutdown
            }
        }
    }

    public class PersistentTrackData
    {
        public float PitlaneEntryTrackPct { get; set; } = -1.0f;
        public float PitlaneExitTrackPct { get; set; } = -1.0f;
    }
}
