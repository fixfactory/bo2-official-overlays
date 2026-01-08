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

// Returns true for the specified delay in miliseconds if the value changed
// since the last time the function was called. Must specificy a unique name
// so the function can be called multiple times for different variables.
// Ex: 
// return changed(500, $prop('DataCorePlugin.GameData.CurrentLap'), 'lap')
function changed(delay, value, name) 
{
    const previousValueName = 'previousValue' + name;
    const changedTimeName = 'changedTime' + name;

    if (root[previousValueName] != null && root[previousValueName] != value)
    {
        root[changedTimeName] = Date.now();
    }

    root[previousValueName] = value;

    if (root[changedTimeName] != null)
    {
        const timeSinceChanged = Date.now() - root[changedTimeName];
        if (timeSinceChanged <= delay)
        {
            return true;
        }
    }

    return false;
}

// Returns true for the specified delay in miliseconds if the value increased
// since the last time the function was called. Must specificy a unique name
// so the function can be called multiple times for different variables.
// Ex: 
// return isincreasing(500, $prop('DataCorePlugin.GameData.CurrentLap'), 'lap')
function isincreasing(delay, value, name) 
{
    const previousValueName = 'previousValue' + name;
    const changedTimeName = 'changedTime' + name;

    if (root[previousValueName] != null && root[previousValueName] < value)
    {
        root[changedTimeName] = Date.now();
    }

    root[previousValueName] = value;

    if (root[changedTimeName] != null)
    {
        const timeSinceChanged = Date.now() - root[changedTimeName];
        if (timeSinceChanged <= delay)
        {
            return true;
        }
    }

    return false;
}

// Returns true for the specified delay in miliseconds if the value decreased
// since the last time the function was called. Must specificy a unique name
// so the function can be called multiple times for different variables.
// Ex: 
// return isdecreasing(500, $prop('DataCorePlugin.GameData.CurrentLap'), 'lap')
function isdecreasing(delay, value, name) 
{
    const previousValueName = 'previousValue' + name;
    const changedTimeName = 'changedTime' + name;

    if (root[previousValueName] != null && root[previousValueName] > value)
    {
        root[changedTimeName] = Date.now();
    }

    root[previousValueName] = value;

    if (root[changedTimeName] != null)
    {
        const timeSinceChanged = Date.now() - root[changedTimeName];
        if (timeSinceChanged <= delay)
        {
            return true;
        }
    }

    return false;
}