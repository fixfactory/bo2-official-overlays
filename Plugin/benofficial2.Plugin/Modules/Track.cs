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
using Newtonsoft.Json.Linq;
using SimHub.Plugins;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime;
using System.Threading.Tasks;

namespace benofficial2.Plugin
{
    public class TrackModule : PluginModuleBase
    {
        private RemoteJsonFile _trackInfo = new RemoteJsonFile("https://raw.githubusercontent.com/fixfactory/bo2-official-overlays/main/Data/TrackInfo.json");
        private string _lastTrackId = string.Empty;

        // Persistence
        private TrackDataPersistence _trackDataPersistence = new TrackDataPersistence();
        private PersistentTrackData _trackData = new PersistentTrackData();
        private bool _trackDataChanged = false;

        public int PushToPassCooldown { get; set; } = 0;
        public float PitlaneExitTrackPct { get; set; } = -1.0f;
        public float PitlaneEntryTrackPct { get; set; } = -1.0f;
        public float PitExitTrackPct { get; set; } = -1.0f;
        public float PitEntryTrackPct { get; set; } = -1.0f;
        public float QualStartTrackPct { get; set; } = 0.0f;
        public float RaceStartTrackPct { get; set; } = 0.0f;
        public string TrackType { get; set; } = string.Empty;
        public double TrackLength { get; set; } = 0.0;
        public override int UpdatePriority => 20;

        public override void Init(PluginManager pluginManager, benofficial2 plugin)
        {
            _trackInfo.LoadAsync();
            _ = _trackDataPersistence.LoadAsync();

            plugin.AttachDelegate(name: "Track.PitlaneExitTrackPct", valueProvider: () => PitlaneExitTrackPct);
            plugin.AttachDelegate(name: "Track.PitlaneEntryTrackPct", valueProvider: () => PitlaneEntryTrackPct);
            plugin.AttachDelegate(name: "Track.PitExitTrackPct", valueProvider: () => PitExitTrackPct);
            plugin.AttachDelegate(name: "Track.PitEntryTrackPct", valueProvider: () => PitEntryTrackPct);
            plugin.AttachDelegate(name: "Track.QualStartTrackPct", valueProvider: () => QualStartTrackPct);
            plugin.AttachDelegate(name: "Track.RaceStartTrackPct", valueProvider: () => RaceStartTrackPct);            
        }

        public override void DataUpdate(PluginManager pluginManager, benofficial2 plugin, ref GameData data)
        {
            if (_trackInfo.Json == null || !_trackDataPersistence.IsLoaded()) 
                return;

            if (_trackDataChanged && !string.IsNullOrEmpty(_lastTrackId))
            {
                _ = _trackDataPersistence.SaveAsync(_lastTrackId, _trackData);
                _trackDataChanged = false;
            }

            if (data.NewData.TrackId == _lastTrackId) 
                return;

            _lastTrackId = data.NewData.TrackId;

            if (data.NewData.TrackId.Length == 0)
            {
                _trackData = new PersistentTrackData();
                PushToPassCooldown = 0;
                PitEntryTrackPct = -1.0f;
                PitExitTrackPct = -1.0f;
                QualStartTrackPct = 0.0f;
                RaceStartTrackPct = 0.0f;
                TrackType = string.Empty;
                TrackLength = 0.0;
                return;
            }

            JToken track = _trackInfo.Json[data.NewData.TrackId];
            _trackData = _trackDataPersistence.GetData(data.NewData.TrackId);

            if (data.NewData.CarId == "superformulasf23 toyota" || data.NewData.CarId == "superformulasf23 honda")
            {
                PushToPassCooldown = track?["pushToPassCooldown_SF23"]?.Value<int>() ?? 100;
            }
            else
            {
                PushToPassCooldown = 0;
            }

            QualStartTrackPct = track?["qualStartTrackPct"]?.Value<float>() ?? 0.0f;
            RaceStartTrackPct = track?["raceStartTrackPct"]?.Value<float>() ?? 0.0f;

            PitlaneEntryTrackPct = track?["pitlaneEntryTrackPct"]?.Value<float>() ?? _trackData.PitlaneEntryTrackPct;
            PitlaneExitTrackPct = track?["pitlaneExitTrackPct"]?.Value<float>() ?? _trackData.PitlaneExitTrackPct;
            PitEntryTrackPct = track?["pitEntryTrackPct"]?.Value<float>() ?? _trackData.PitEntryTrackPct;
            PitExitTrackPct = track?["pitExitTrackPct"]?.Value<float>() ?? _trackData.PitExitTrackPct;

            RawDataHelper.TryGetSessionData<string>(ref data, out string trackType, "WeekendInfo", "TrackType");
            TrackType = trackType;

            RawDataHelper.TryGetSessionData<string>(ref data, out string trackLengthStr, "WeekendInfo", "TrackLength");
            string[] parts = trackLengthStr.Split(' '); // e.g. "3.426 km"
            if (double.TryParse(parts[0], out double value))
                TrackLength = value;
            else
                TrackLength = 0.0;
        }

        public override void End(PluginManager pluginManager, benofficial2 plugin)
        {
            // Wait for any pending save to complete to avoid losing data on shutdown
            _trackDataPersistence?.WaitForPendingSave();
        }

        public void SetMeasuredPitlaneExitTrackPct(float value)
        {
            if (value < 0.0f || value > 1.0f || _trackData.PitlaneExitTrackPct >= 0.0f)
                return;

            _trackData.PitlaneExitTrackPct = value;
            _trackDataChanged = true;

            if (PitlaneExitTrackPct < 0.0f)
                PitlaneExitTrackPct = value;
        }

        public void SetMeasuredPitlaneEntryTrackPct(float value)
        {
            if (value < 0.0f || value > 1.0f || _trackData.PitlaneEntryTrackPct >= 0.0f)
                return; 

            _trackData.PitlaneEntryTrackPct = value;
            _trackDataChanged = true;

            if (PitlaneEntryTrackPct < 0.0f)
                PitlaneEntryTrackPct = value;
        }
    }
}
