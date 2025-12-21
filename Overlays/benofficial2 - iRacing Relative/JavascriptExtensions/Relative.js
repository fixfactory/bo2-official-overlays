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

function getHighlightedDriverProp(prop)
{
    const carIdx = $prop('benofficial2.Highlighted.CarIdx')
    if (carIdx >= 0)
        return $prop('benofficial2.Highlighted.' + prop)

    return $prop('benofficial2.Player.' + prop)
}

function getRelativeProp(index, prop)
{
    if (index == 0)
        return getHighlightedDriverProp(prop)
            
    if (index > 0)
        return $prop('benofficial2.Relative.Behind' + format(index - 1, '00') + '.' + prop);
    
    return $prop('benofficial2.Relative.Ahead' + format(Math.abs(index + 1), '00') + '.' + prop);
}

function getStandingsClassProp(classIdx, prop)
{
    return $prop('benofficial2.Standings.Class' + format(classIdx, '00') + '.' + prop);
}

function getRelativeRowVisible(index)
{
    if (index == 0)
        return isnull($prop('benofficial2.Highlighted.CarIdx'), -1) >= 0
        //return isnull(getHighlightedDriverProp('CarIdx'), -1) >= 0
    
    return getRelativeProp(index, 'RowVisible');
}

function getRelativeTextColor(index)
{
    if (!isRace()) 
        return 'White';

    if (index == 0)
    {
        const highlight = isnull($prop('benofficial2.Relative.HighlightPlayerRow'), false)
        if (highlight) 
            return 'White'

        return '#FFEBAE00'
    }

    const lap = getRelativeProp(index, 'CurrentLapHighPrecision');
    const playerLap = getRelativeProp(0, 'CurrentLapHighPrecision');

    if (playerLap <= 0 || lap <= 0) 
        return 'White';

    const red = '#FFFF6345';
    const blue = '#43B7EA';
    const white = 'White';

    let deltaLap = playerLap - lap;

    if (index < 0)
    {
        if (deltaLap < -0.75) 
            return red;

        if (deltaLap > 0.25) 
            return blue;
    }
    else if (index > 0)
    {
        if (deltaLap < -0.25) 
            return red;

        if (deltaLap > 0.75) 
            return blue;
    }
    return white;
}

function getClassSof(classIdx)
{
    return isnull(getStandingsClassProp(classIdx, 'Sof'), 0);
}

function truncateToDecimal(num, decimals) 
{
  const factor = Math.pow(10, decimals)
  return (Math.floor(num * factor) / factor).toFixed(decimals)
}

function formatSafetyRating(license, sr)
{
    let rating = String(license) + ' '

    if (Number(sr) <= 0.01)
        return rating + '--'
    
    return rating + truncateToDecimal(sr, 1)
}

function formatIRating(iRating)
{
    if (Number(iRating) / 1000 <= 0.01)
        return '--'
    
    return truncateToDecimal(Number(iRating) / 1000, 1) + 'k';
}
