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
using System.Diagnostics;

namespace benofficial2.Plugin
{
    public class SessionState
    {
        private int _lastSessionIdx = -1;
        private double _lastSessionTime = double.MaxValue;
        private Guid _lastSessionId = Guid.Empty;
        private string _lastSessionTypeName = string.Empty;
        private string _lastSubSessionID = string.Empty;

        public int CurrentSessionIdx { get; private set; } = -1;
        public TimeSpan SessionTime { get; private set; } = TimeSpan.Zero;
        public TimeSpan DeltaTime { get; private set; } = TimeSpan.Zero;
        public bool SessionChanged { get; private set; } = true;
        
        public void Update(ref GameData data)
        {
            if (RawDataHelper.TryGetSessionData<int>(ref data, out int currentSessionIdx, "SessionInfo", "CurrentSessionNum"))
                CurrentSessionIdx = currentSessionIdx;
            else
                CurrentSessionIdx = -1;

            RawDataHelper.TryGetSessionData<string>(ref data, out string subSessionID, "WeekendInfo", "SubSessionID");
            RawDataHelper.TryGetTelemetryData<double>(ref data, out double sessionTime, "SessionTime");
            SessionTime = TimeSpan.FromSeconds(sessionTime);
            DeltaTime = TimeSpan.FromSeconds(Math.Max(sessionTime - _lastSessionTime, 0));

            // Also consider the session changed when time flows backward.
            // Because many checks are based on time flowing forward and would break otherwise.
            // Only happens when manually moving the time backwards in a SimHub replay.
            SessionChanged = (CurrentSessionIdx != _lastSessionIdx || 
                subSessionID != _lastSubSessionID ||
                sessionTime < _lastSessionTime || 
                data.SessionId != _lastSessionId || 
                data.NewData.SessionTypeName != _lastSessionTypeName);

            _lastSessionIdx = CurrentSessionIdx;
            _lastSessionTime = sessionTime;
            _lastSessionId = data.SessionId;
            _lastSessionTypeName = data.NewData.SessionTypeName;
            _lastSubSessionID = subSessionID;
        }
    }

    public class SessionModule : PluginModuleBase
    {
        private bool _raceFinishedForPlayer = false;
        private double? _lastTrackPct = null;
        private TimeSpan _raceStartedTime = TimeSpan.Zero;

        public SessionState State { get; internal set; } = new SessionState();
        public bool Race { get; internal set; } = false;
        public bool Qual { get; internal set; } = false;
        public bool Practice { get; internal set; } = false;
        public bool Offline { get; internal set; } = false;
        public bool ReplayPlaying { get; internal set; } = false;
        public bool SessionScreen { get; internal set; } = false;
        public bool UIHidden { get; internal set; } = false;
        public bool RaceStarted { get; internal set; } = false;
        public bool RaceFinished { get; internal set; } = false;
        public TimeSpan SessionTimeTotal { get; internal set; } = TimeSpan.Zero;
        public int SessionLapsTotal { get; internal set; } = 0;
        public double RaceTimer { get; internal set; } = 0;
        public bool JoinedRaceInProgress { get; internal set; } = false;
        public bool Oval { get; internal set; } = false;
        public bool StandingStart { get; internal set; } = false;
        public bool ShortParadeLap { get; internal set; } = false;
        public double MaxFuelPct { get; internal set; } = 1.0;
        public bool TeamRacing { get; internal set; } = false;
        public string SubType { get; internal set; } = string.Empty;
        public double TrackRubberPct { get; internal set; } = 0.0;

        public override int UpdatePriority => 10;

        public override void Init(PluginManager pluginManager, benofficial2 plugin)
        {
            plugin.AttachDelegate(name: "Session.Race", valueProvider: () => Race);
            plugin.AttachDelegate(name: "Session.Qual", valueProvider: () => Qual);
            plugin.AttachDelegate(name: "Session.Practice", valueProvider: () => Practice);
            plugin.AttachDelegate(name: "Session.Offline", valueProvider: () => Offline);
            plugin.AttachDelegate(name: "Session.ReplayPlaying", valueProvider: () => ReplayPlaying);
            plugin.AttachDelegate(name: "Session.SessionScreen", valueProvider: () => SessionScreen);
            plugin.AttachDelegate(name: "Session.UIHidden", valueProvider: () => UIHidden);
            plugin.AttachDelegate(name: "Session.RaceStarted", valueProvider: () => RaceStarted);
            plugin.AttachDelegate(name: "Session.RaceFinished", valueProvider: () => RaceFinished);
            plugin.AttachDelegate(name: "Session.RaceTimer", valueProvider: () => RaceTimer);
            plugin.AttachDelegate(name: "Session.Oval", valueProvider: () => Oval);
            plugin.AttachDelegate(name: "Session.StandingStart", valueProvider: () => StandingStart);
            plugin.AttachDelegate(name: "Session.TeamRacing", valueProvider: () => TeamRacing);
            plugin.AttachDelegate(name: "Session.LapsTotal", valueProvider: () => SessionLapsTotal);
            plugin.AttachDelegate(name: "Session.SubType", valueProvider: () => SubType);
            plugin.AttachDelegate(name: "Session.TrackRubberPct", valueProvider: () => TrackRubberPct);
        }

        public override void DataUpdate(PluginManager pluginManager, benofficial2 plugin, ref GameData data)
        {
            State.Update(ref data);

            if (State.SessionChanged)
            {
                Race = data.NewData.SessionTypeName.IndexOf("Race") != -1;
                Qual = data.NewData.SessionTypeName.IndexOf("Qual") != -1;

                Practice = data.NewData.SessionTypeName.IndexOf("Practice") != -1 ||
                    data.NewData.SessionTypeName.IndexOf("Warmup") != -1 ||
                    data.NewData.SessionTypeName.IndexOf("Testing") != -1;

                Offline = data.NewData.SessionTypeName.IndexOf("Offline") != -1;

                RawDataHelper.TryGetSessionData<string>(ref data, out string category, "WeekendInfo", "Category");
                Oval = category == "Oval" || category == "DirtOval";

                RawDataHelper.TryGetSessionData<int>(ref data, out int standingStart, "WeekendInfo", "WeekendOptions", "StandingStart");
                StandingStart = standingStart == 1;

                RawDataHelper.TryGetSessionData<int>(ref data, out int shortParadeLap, "WeekendInfo", "WeekendOptions", "ShortParadeLap");
                ShortParadeLap = shortParadeLap == 1;

                RawDataHelper.TryGetSessionData<float>(ref data, out float driverCarMaxFuelPct, "DriverInfo", "DriverCarMaxFuelPct");
                MaxFuelPct = driverCarMaxFuelPct;

                RawDataHelper.TryGetSessionData<string>(ref data, out string teamRacing, "WeekendInfo", "TeamRacing");
                TeamRacing = teamRacing == "1";

                if (State.CurrentSessionIdx >= 0)
                {
                    RawDataHelper.TryGetSessionData<string>(ref data, out string sessionSubType, "SessionInfo", "Sessions", State.CurrentSessionIdx, "SessionSubType");
                    SubType = sessionSubType;

                    RawDataHelper.TryGetSessionData<string>(ref data, out string rubberState, "SessionInfo", "Sessions", State.CurrentSessionIdx, "SessionTrackRubberState");
                    TrackRubberPct = ParseTrackRubberPct(rubberState);
                }
                else
                {
                    SubType = string.Empty;
                    TrackRubberPct = 0.0;
                }

                RawDataHelper.TryGetTelemetryData<int>(ref data, out int sessionTimeTotal, "SessionTimeTotal");
                SessionTimeTotal = TimeSpan.FromSeconds(sessionTimeTotal);

                RawDataHelper.TryGetTelemetryData<int>(ref data, out int totalLaps, "SessionLapsTotal");
                SessionLapsTotal = (totalLaps > 0) && (totalLaps < 20000) ? totalLaps : 0;               
            }

            // Determine if replay is playing.
            // There's a short moment when loading into a session when isReplayPlaying is false
            // but position or trackSurface is -1.
            // IsReplayPlaying is false when spotting.
            RawDataHelper.TryGetTelemetryData<bool>(ref data, out bool isReplayPLaying, "IsReplayPlaying");
            RawDataHelper.TryGetTelemetryData<int>(ref data, out int position, "PlayerCarPosition");
            RawDataHelper.TryGetTelemetryData<int>(ref data, out int trackSurface, "PlayerTrackSurface");
            ReplayPlaying = isReplayPLaying || position < 0 || trackSurface < 0;

            // Determine if Session Screen is active (out of car)
            // Remains true when spotting.
            RawDataHelper.TryGetTelemetryData<int>(ref data, out int camCameraState, "CamCameraState");
            SessionScreen = (camCameraState & 0x0001) != 0; // irsdk_IsSessionScreen
            UIHidden = (camCameraState & 0x0008) != 0; // irsdk_UIHidden

            // Determine if race started
            RawDataHelper.TryGetTelemetryData<int>(ref data, out int sessionState, "SessionState");
            RaceStarted = Race && sessionState >= 4;

            // Determine if we joined a race session in progress.
            // This will also be true when stepping backwards in a SimHub replay.
            if (RaceStarted)
            {
                if (State.SessionChanged)
                {
                    JoinedRaceInProgress = true;
                }
            }
            else
            {
                JoinedRaceInProgress = false;
            }

            // Determine if race finished for the player
            if (!Race || State.SessionChanged)
            {
                // Reset when changing/restarting session
                _lastTrackPct = null;
                _raceFinishedForPlayer = false;
                RaceFinished = false;
            }
            else
            {
                if (_raceFinishedForPlayer)
                {
                    // Race finished
                    RaceFinished = true;
                }
                else if (data.NewData.Flag_Checkered != 1)
                {
                    // Checkered flag is not shown
                    RaceFinished = false;
                }
                else if (!_lastTrackPct.HasValue || _lastTrackPct.Value <= data.NewData.TrackPositionPercent)
                {
                    // Heading toward the checkered flag
                    _lastTrackPct = data.NewData.TrackPositionPercent;
                    RaceFinished = false;
                }
                else
                {
                    // Crossed the line with the checkered flag
                    _raceFinishedForPlayer = true;
                    RaceFinished = true;
                }
            }

            // Update race timer
            if (RaceStarted)
            {
                // Freeze timer when race is finished
                if (!RaceFinished)
                {
                    if (_raceStartedTime <= TimeSpan.Zero)
                    {
                        _raceStartedTime = State.SessionTime;
                    }

                    RaceTimer = (State.SessionTime - _raceStartedTime).TotalSeconds;
                }
            }
            else
            {
                RaceTimer = 0;
                _raceStartedTime = TimeSpan.Zero;
            }
        }

        public override void End(PluginManager pluginManager, benofficial2 plugin)
        {
        }

        static public double ParseTrackRubberPct(string rubberState)
        {
            // Return Track Rubber Percentage (aka baseRubberPct) based on values shared by iRacing developers.
            switch (rubberState)
            {
                case "clean":
                    return 0.04;
                case "slight usage":
                    return 0.15;
                case "low usage":
                    return 0.28;
                case "moderately low usage":
                    return 0.42;
                case "moderate usage":
                    return 0.57;
                case "moderately high usage":
                    return 0.71;
                case "high usage":
                    return 0.84;
                case "extensive usage":
                    return 0.95;
                case "maximum usage":
                    return 1.00;
                default:
                    return 0.0;
            }
        }
    }
}
