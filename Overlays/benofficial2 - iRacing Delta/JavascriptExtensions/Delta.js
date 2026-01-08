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

function isInvalidTime(time)
{
    return time == null || time == '' || time == '00:00:00' || time == '00:00.000' || time == '00:00.0000000';
}

function getSessionBestTime()
{
    // The live delta seems to always be against the best clean lap.
    // TODO: Track the session best clean lap time in the plugin
    let best = $prop('benofficial2.Player.SessionBestCleanLapTime');
    if (isInvalidTime(best))
    {
        // When there's no clean lap, fallback to SessionBest
        best = $prop('PersistantTrackerPlugin.SessionBest');
        if (isInvalidTime(best))
        {
            best = $prop('DataCorePlugin.GameData.BestLapTime');
        }
    }
    return best;    
}

function getReferenceLapTime()
{
    let best = null;
    if (isQual())
    {
        best = $prop('PersistantTrackerPlugin.AllTimeBest');
    }
    else if (isRace())
    {
        best = getSessionBestTime();
    }
    else if (isPractice())
    {
        best = getSessionBestTime();
    }

    if (isInvalidTime(best))
    {
        // Fallback to last lap time.
        // Happens on the 2nd lap of the race.
        best = $prop('DataCorePlugin.GameData.LastLapTime');
    }
    return best;
}

function isReferenceLapTimeOk()
{
    if (isQual())
    {
        return $prop('DataCorePlugin.GameRawData.Telemetry.LapDeltaToBestLap_OK');
    }
    else if (isRace())
    {
        return $prop('DataCorePlugin.GameRawData.Telemetry.LapDeltaToSessionBestLap_OK');
    }
    else if (isPractice())
    {
        return $prop('DataCorePlugin.GameRawData.Telemetry.LapDeltaToSessionBestLap_OK');   
    }
    else
    {
        return $prop('DataCorePlugin.GameRawData.Telemetry.LapDeltaToSessionLastlLap_OK');
    }
}

function getReferenceLapTimeDelta()
{
    if (isQual())
    {
        return $prop('DataCorePlugin.GameRawData.Telemetry.LapDeltaToBestLap_DD');
    }
    else if (isRace())
    {
        return $prop('DataCorePlugin.GameRawData.Telemetry.LapDeltaToSessionBestLap_DD');
    }
    else if (isPractice())
    {
        return $prop('DataCorePlugin.GameRawData.Telemetry.LapDeltaToSessionBestLap_DD');   
    }
    else
    {
        return $prop('DataCorePlugin.GameRawData.Telemetry.LapDeltaToSessionLastlLap_DD');
    }
}

function getBestLiveDeltaTimeSecs()
{
    var delta = null;
    if (isQual())
    {
        delta = $prop('DataCorePlugin.GameRawData.Telemetry.LapDeltaToBestLap');
        //delta = $prop('PersistantTrackerPlugin.AllTimeBestLiveDeltaSeconds');
    }
    else if (isRace())
    {   
        var lap = $prop('DataCorePlugin.GameData.CurrentLap');
        if (lap <= 2)
        {
            delta = $prop('DataCorePlugin.GameRawData.Telemetry.LapDeltaToSessionLastlLap');
            //delta = $prop('PersistantTrackerPlugin.SessionBestLiveDeltaSeconds');
        }
        else
        {
            delta = $prop('DataCorePlugin.GameRawData.Telemetry.LapDeltaToSessionBestLap');
            //delta = $prop('PersistantTrackerPlugin.SessionBestLiveDeltaSeconds');
        }
    }
    else if (isPractice())
    {
        delta = $prop('DataCorePlugin.GameRawData.Telemetry.LapDeltaToSessionBestLap');
        //delta = $prop('PersistantTrackerPlugin.SessionBestLiveDeltaSeconds');
    }
    return delta;
}

function computeDeltaTime(ourTime, theirTime)
{
    if (isInvalidTime(ourTime) || isInvalidTime(theirTime))
    {
        return '';
    }

    var our = new Date(ourTime)
    var their = new Date(theirTime);
    var delta = new Date(Math.abs(their - our));

    var sign = '-';
    if (our > their)
    {
        sign = '+';
    }

    if (delta.getSeconds() > 9 || delta.getMinutes() > 0)
    {
        return sign + "9.99";
    }

    var sec = String(delta.getSeconds())
    var mil = String(Math.floor(delta.getMilliseconds() / 10)).padStart(2, '0');
    return sign + sec + '.' + mil;
}

function getDeltaTimeColor(deltaTimeWithSign)
{
    var sign = String(deltaTimeWithSign).substring(0, 1);
    if (sign == '+')
    {
        // Red
        return -1
    }
    else if (sign == '-')
    {
        // Green
        return 1
    }
    // White
    return 0;
}

function formatLapTime(time, decimalCount)
{
    decimalCount = Math.max(1, Math.min(3, decimalCount));
    if (isInvalidTime(time))
    {
        var value = '--:--.';
        while (decimalCount-- > 0)
        {
            value += '-'
        }
        return value;
    }

    var value = String(time).substring(4, 8) + '.';
    var decimals = String(time).substring(9, 9 + decimalCount);
    var missingDecimals = decimalCount - decimals.length;
    while (missingDecimals-- > 0)
    {
        decimals += '0';
    }
    return value + decimals;
}
