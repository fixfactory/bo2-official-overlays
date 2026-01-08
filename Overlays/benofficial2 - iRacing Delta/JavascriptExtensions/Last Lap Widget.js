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

// Used at the end of a lap to display the delta with the reference time
// that was used on the previous lap.
function getDeltaToPreviousReferenceTime()
{
    // Fix for the first laps of the race.
    var lap = $prop('DataCorePlugin.GameData.CurrentLap');
    if (isRace() && lap <= 2)
    {
        return '';
    }

    var lastLapTime = $prop('DataCorePlugin.GameData.LastLapTime');
    var previousReferenceTime = $prop('variable.previousReferenceTime');
    return computeDeltaTime(lastLapTime, previousReferenceTime);
}

// This is for the delta shown on the big lap timer.
function showTimerDelta()
{
    var lap = $prop('DataCorePlugin.GameData.CurrentLap');
    if (lap <= 1)
    {
        return false;
    }
    
    var lastTime = $prop('DataCorePlugin.GameData.LastLapTime');
    if (isInvalidTime(lastTime))
    {
        // Happens when getting out of the pit after a reset.
        return false;
    }

    var time = $prop('DataCorePlugin.GameData.CurrentLapTime');
    var d = new Date(time);
    var min = d.getMinutes();
    var sec = d.getSeconds();
    var hideAfterSecs = 10

    if (min > 0 || sec > hideAfterSecs)
    {
        return false;
    }

    return true;
}