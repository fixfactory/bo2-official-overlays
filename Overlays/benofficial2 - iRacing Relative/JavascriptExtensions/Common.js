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

function isPractice()
{
    return isnull($prop('benofficial2.Session.Practice'), false);
}

function isOffline()
{
    return isnull($prop('benofficial2.Session.Offline'), false);
}

function isInvalidTime(time)
{
    return time == null || time == '' || time == '00:00:00' || time == '00:00.000' || time == '00:00.0000000';
}

function formatSecondsToTimecode(totalSeconds) 
{
    if (totalSeconds < 0 || totalSeconds > 172800)
    {
        return '';
    }

    totalSeconds = Math.floor(totalSeconds); // Ensure it's an integer
    const hours = Math.floor(totalSeconds / 3600);
    const minutes = Math.floor((totalSeconds % 3600) / 60);
    const seconds = totalSeconds % 60;
    
    if (hours > 0) 
    {
        return `${hours}:${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`;
    } 
    else 
    {
        return `${minutes}:${seconds.toString().padStart(2, '0')}`;
    }
}

function blendColors(color1, color2, percentage) 
{
    // Force inputs to strings
    color1 = "" + color1;
    color2 = "" + color2;

    // Clamp percentage between 0 and 100
    if (percentage < 0) percentage = 0;
    if (percentage > 100) percentage = 100;
    var t = percentage / 100;

    function hexToRGBA(hex) 
    {
        if (hex.charAt(0) === "#") 
        {
            hex = hex.substring(1);
        }
        var a, r, g, b;
        if (hex.length === 6) 
        {
            a = 255;
            r = parseInt(hex.substring(0, 2), 16);
            g = parseInt(hex.substring(2, 4), 16);
            b = parseInt(hex.substring(4, 6), 16);
        } 
        else if (hex.length === 8) {
            a = parseInt(hex.substring(0, 2), 16);
            r = parseInt(hex.substring(2, 4), 16);
            g = parseInt(hex.substring(4, 6), 16);
            b = parseInt(hex.substring(6, 8), 16);
        } 
        else 
        {
            throw "Invalid color format: " + hex;
        }
        return { a: a, r: r, g: g, b: b };
    }

    function toHex(v) 
    {
        var h = v.toString(16).toUpperCase();
        return h.length === 1 ? "0" + h : h;
    }

    var c1 = hexToRGBA(color1);
    var c2 = hexToRGBA(color2);

    var r = Math.round(c1.r + (c2.r - c1.r) * t);
    var g = Math.round(c1.g + (c2.g - c1.g) * t);
    var b = Math.round(c1.b + (c2.b - c1.b) * t);
    var a = Math.round(c1.a + (c2.a - c1.a) * t);

    return "#" + toHex(a) + toHex(r) + toHex(g) + toHex(b);
}