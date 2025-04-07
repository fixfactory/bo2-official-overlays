/*
    benofficial2's Official Overlays
    Copyright (C) 2025 benofficial2

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

function checkVersion(versionName, versionNumber)
{
    if (root['hide'])
    {
        return false;
    }

    const checkForUpdates = isnull($prop('benofficial2.CheckForUpdates'), true);
    if (!checkForUpdates)
    {
        return false;
    }

    const url = 'https://raw.githubusercontent.com/fixfactory/bo2-official-overlays/main/Versions.json';

    const jsonStr = downloadstringasync(500, url);

    if (jsonStr) 
    {
        const json = JSON.parse(jsonStr);
        if (json[versionName] != versionNumber) 
        {
            if (!root['timeChecked'])
            {
                root['timeChecked'] = Date.now();
            }

            if (((Date.now() - root['timeChecked'])) < 5000)
            {
                return true;
            }
        }

        root['hide'] = true;
    }

    return false;
}