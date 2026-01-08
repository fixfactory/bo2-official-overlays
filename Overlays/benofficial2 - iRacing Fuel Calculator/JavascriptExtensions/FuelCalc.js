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

function getFuelCalcSessionProp(sessionIdx, prop)
{
    return $prop('benofficial2.FuelCalc.Session' + format(sessionIdx, '00') + '.' + prop)
}

function getSessionDetails(sessionIdx)
{
    let sessionTypeName = getFuelCalcSessionProp(sessionIdx, 'TypeName')
    if (sessionTypeName == "")
    {
        return ''
    }

    const sessionSubTypeName = getFuelCalcSessionProp(sessionIdx, 'SubTypeName')
    if (sessionSubTypeName != null && sessionSubTypeName != "")
    {
        sessionTypeName = sessionSubTypeName;
    }

    const limitedByTime = getFuelCalcSessionProp(sessionIdx, 'LimitedByTime')
    if (!limitedByTime)
    {
        let laps = " laps:"
        const sessionLaps = getFuelCalcSessionProp(sessionIdx, 'Laps')
        if (sessionLaps == 1)
        {
            laps = " lap:"
        }
        return sessionTypeName + ' ' + sessionLaps + laps
    }
    
    const sessionTime = getFuelCalcSessionProp(sessionIdx, 'Time')
    const sessionTimeSecs = timespantoseconds(sessionTime)
    if (sessionTimeSecs <= 0)
    {
        return sessionTypeName + ' ∞:'
    }

    return sessionTypeName + ' ' + (sessionTimeSecs / 60).toFixed(0) + ' min:'
}

function getFuelNeeded(sessionIdx)
{
    const sessionTypeName = getFuelCalcSessionProp(sessionIdx, 'TypeName')
    if (sessionTypeName == "")
    {
        return ''
    }

    const fuelNeeded = getFuelCalcSessionProp(sessionIdx, 'FuelNeeded')
    if (fuelNeeded <= 0)
    {
        return '-.-'
    }

    let stops = ''
    const stopsNeeded = getFuelCalcSessionProp(sessionIdx, 'StopsNeeded')
    if (stopsNeeded == 1)
    {
        stops = '(' + stopsNeeded + ' stop)'
    }
    else if (stopsNeeded > 1)
    {
        stops = '(' + stopsNeeded + ' stops)'
    }

    const units = $prop('benofficial2.FuelCalc.Units')
    return fuelNeeded.toFixed(1) + ' ' + units + ' ' + stops
}
