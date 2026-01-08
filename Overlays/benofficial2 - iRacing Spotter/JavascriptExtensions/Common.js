/*
    benofficial2's Official Overlays
    Copyright (C) 2023-2026 benofficial2

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

function isGameRunning()
{
    return isnull($prop('benofficial2.iRacingRunning'), false);
}

function isReplayPlaying()
{
    return isnull($prop('benofficial2.Session.ReplayPlaying'), false);
}

function isDriving()
{
    return isGameRunning() && !isReplayPlaying();
}

function isInPitLane()
{
    return $prop('DataCorePlugin.GameData.IsInPitLane');
}

function isRace()
{
    return isnull($prop('benofficial2.Session.Race'), false);
}

function isRaceStarted()
{
    return isnull($prop('benofficial2.Session.RaceStarted'), false);
}

function isRaceFinished()
{
    return isnull($prop('benofficial2.Session.RaceFinished'), false);
}

function isRaceInProgress()
{
    return isRaceStarted() && !isRaceFinished();
}

function isQual()
{
    return isnull($prop('benofficial2.Session.Qual'), false);
}

function isLoneQual()
{
    const sessionTypeName = $prop('DataCorePlugin.GameData.SessionTypeName');
    return String(sessionTypeName).indexOf('Lone Qualify') != -1;
}

function isPractice()
{
    return isnull($prop('benofficial2.Session.Practice'), false);
}

function isOffline()
{
    return isnull($prop('benofficial2.Session.Offline'), false);
}
