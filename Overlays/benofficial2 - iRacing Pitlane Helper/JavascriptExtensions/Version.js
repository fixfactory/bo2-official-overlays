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

function checkVersion(versionName, versionNumber)
{
    if (root['hide'])
        return false;

    const checkForUpdates = isnull($prop('benofficial2.CheckForUpdates'), true);
    if (!checkForUpdates)
        return false;

    const url = 'https://raw.githubusercontent.com/fixfactory/bo2-official-overlays/main/Versions.json';
    const jsonStr = downloadstringasync(500, url);

    if (jsonStr) 
    {
        if (jsonStr.startsWith("ERROR")) 
        {
	    	root['hide'] = true
		    return false
        }
		
        const json = JSON.parse(jsonStr);
        if (isVersionNewer(json[versionName], versionNumber))
        {
            if (!root['timeChecked'])
                root['timeChecked'] = Date.now();

            if (((Date.now() - root['timeChecked'])) < 5000)
                return true;
        }

        root['hide'] = true;
    }

    return false;
}

function isVersionNewer(v1, v2) 
{
    // Coerce to string; null/undefined/number → string
    v1 = (v1 ?? "").toString();
    v2 = (v2 ?? "").toString();

    const a = v1.split('.').map(n => parseInt(n, 10) || 0);
    const b = v2.split('.').map(n => parseInt(n, 10) || 0);

    const len = Math.max(a.length, b.length);

    for (let i = 0; i < len; i++) 
    {
        const num1 = a[i] ?? 0;
        const num2 = b[i] ?? 0;

        if (num1 > num2) 
            return true;

        if (num1 < num2) 
            return false;
    }

    return false; // equal or invalid → not newer
}

function getErrorMessage()
{
    if (!root['timeChanged'])
        root['timeChanged'] = Date.now()

    if (!root['errormessage'])
        root['errormessage'] = ''

    const errorMessage = isnull($prop('benofficial2.ErrorMessage'), '')
    if (errorMessage != '' && root['errormessage'] != errorMessage)
    {
        root['errormessage'] = errorMessage
        root['timeChanged'] = Date.now()
        root['show'] = true
        return errorMessage
    }

    if (((Date.now() - root['timeChanged'])) > 5000)
        root['show'] = false

    if (root['show'] == true)
        return root['errormessage']

    return ''
}