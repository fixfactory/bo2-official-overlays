/*
    benofficial2's Official Overlays
    Copyright (C) 2023-2025 benofficial2

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

function getStandingsProp(classIdx, rowIdx, prop)
{
    return $prop('benofficial2.Standings.Class' + format(classIdx, '00') 
        + '.Row' + format(rowIdx, '00')
        + '.' + prop);
}

function getStandingsClassProp(classIdx, prop)
{
    return $prop('benofficial2.Standings.Class' + format(classIdx, '00') 
        + '.' + prop);
}

function isDividerVisible(classIdx)
{
    return isnull($prop('benofficial2.Standings.Class' + format(classIdx, '00') + '.LeadFocusedDividerVisible'), false);
}

function isLeadFocusedRow(classIdx, rowIdx)
{
    const leadFocusedRows = $prop('benofficial2.Standings.LeadFocusedRows');
    return isDividerVisible(classIdx) && (rowIdx < leadFocusedRows);
}

function getTireCompound(classIdx, rowIdx)
{
    const compound = isnull(getStandingsProp(classIdx, rowIdx, 'TireCompound'), '');
    const carId = $prop('DataCorePlugin.GameData.CarId');

    // Special case for IndyCar, return M as S and S as H because that's matching the color.
    if (carId == 'dallarair18')
    {
        if (compound == 'M')
        {
            return 'S';
        }
        else if (compound == 'S')
        {
            return 'H';
        }
    }

    return compound;
}

function getTireCompoundColor(classIdx, rowIdx)
{
    const compound = getTireCompound(classIdx, rowIdx);
    
    if (compound == 'H')
    {
        return '#FEFEFC';
    }
    else if (compound == 'M')
    {
        return '#FFF94D'
    }
    else if (compound == 'S')
    {
        return '#FF584F';
    }
    else if (compound == 'W')
    {
        return '#48D3FA';
    }
    return '#FEFEFC'
}

function getTireCompoundVisible(classIdx, rowIdx)
{
    const visible = isnull(getStandingsProp(classIdx, rowIdx, 'TireCompoundVisible'), false);
    if (!visible) return false;

    // Hide before the race start
    if (isRace()) return isRaceStarted();

    const inPit = getStandingsProp(classIdx, rowIdx, 'InPitLane');
    const connected = getStandingsProp(classIdx, rowIdx, 'Connected');
    return connected && !inPit;
}

function getLicenseColor(classIdx, rowIdx)
{
    const license = isnull(getStandingsProp(classIdx, rowIdx, 'License'), 'R');
    if (license == 'A')
    {
        return '#006EFF'
    }
    else if (license == 'B')
    {
        return '#33CC00'
    }
    else if (license == 'C')
    {
        return '#FFCC00'
    }
    else if (license == 'D')
    {
        return '#FF6600'
    }
    else if (license == 'R')
    {
        return '#E1251B'
    }
    return 'Black'
}

function getLicenseTextColor(classIdx, rowIdx)
{
    const license = isnull(getStandingsProp(classIdx, rowIdx, 'License'), 'R');
    if (license == 'A')
    {
        return '#66A8FF'
    }
    else if (license == 'B')
    {
        return '#85E066'
    }
    else if (license == 'C')
    {
        return '#FFE066'
    }
    else if (license == 'D')
    {
        return '#FFA366'
    }
    else if (license == 'R')
    {
        return '#ED7C66'
    }
    return 'White'
}

function getLicenseBackColor(classIdx, rowIdx)
{
    const license = isnull(getStandingsProp(classIdx, rowIdx, 'License'), 'R');
    if (license == 'A')
    {
        return '#032F6F'
    }
    else if (license == 'B')
    {
        return '#175509'
    }
    else if (license == 'C')
    {
        return '#50410A'
    }
    else if (license == 'D')
    {
        return '#692C09'
    }
    else if (license == 'R')
    {
        return '#5D1214'
    }
    return 'Black'
}

function getClassColor(classIdx)
{
    return isnull(getStandingsClassProp(classIdx, 'Color'), 'White');
}

function getClassTextColor(classIdx)
{
    return isnull(getStandingsClassProp(classIdx, 'TextColor'), 'Black');
}

function getClassSof(classIdx)
{
    return isnull(getStandingsClassProp(classIdx, 'Sof'), 0);
}

function formatIRating(iRating)
{
    return (Number(iRating) / 1000).toFixed(1) + 'k';
}