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

function isGameIRacing()
{
    return $prop('DataCorePlugin.CurrentGame') == 'IRacing';
}

function isGameRunning()
{
    return $prop('DataCorePlugin.GameRunning');
}

function isReplayPlaying()
{
    if (isGameIRacing())
    {
        // There's a short moment when loading into a session when isReplayPlaying is false but position is -1
        const isReplayPlaying = $prop('DataCorePlugin.GameRawData.Telemetry.IsReplayPlaying');
        const position = $prop('DataCorePlugin.GameRawData.Telemetry.PlayerCarPosition');
        const trackSurface = $prop('DataCorePlugin.GameRawData.Telemetry.PlayerTrackSurface');
        return isReplayPlaying || position < 0 || trackSurface < 0;
    }
    return false;
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
    var sessionTypeName = $prop('DataCorePlugin.GameData.SessionTypeName');
    return String(sessionTypeName).indexOf('Race') != -1;   
}

function isQual()
{
    var sessionTypeName = $prop('DataCorePlugin.GameData.SessionTypeName');
    return String(sessionTypeName).indexOf('Qual') != -1;
}

function isPractice()
{
    var sessionTypeName = $prop('DataCorePlugin.GameData.SessionTypeName');
    return (String(sessionTypeName).indexOf('Practice') != -1) ||
           (String(sessionTypeName).indexOf('Warmup') != -1) ||
           (String(sessionTypeName).indexOf('Testing') != -1);
}

function isOffline()
{
    var sessionTypeName = $prop('DataCorePlugin.GameData.SessionTypeName');
    var isPractice = String(sessionTypeName).indexOf('Offline') != -1;
}

function getIndexedProp(name, index)
{
	return $prop(name + '_' + format(index, '00'));
}

// Returns the value of a leaderboard property.
// Ex:
//   getLeaderboardProp('IsInPit', 1, 0)
//   returns value of IRacingExtraProperties.iRacing_Class_01_Leaderboard_Driver_00_IsInPit
function getLeaderboardProp(name, classIndex, driverIndex)
{
    return $prop('IRacingExtraProperties.iRacing_Class_' 
        + format(classIndex, '00') 
        + '_Leaderboard_Driver_' 
        + format(driverIndex, '00')
        + '_'
        + name);
}

function addZero(i) 
{
	if (i < 10) 
	{
		i = "0" + i;
	}
	return i;
}

function getTimeFromSeconds(seconds)
{
    if (seconds > 172800)
    {
        return '';
    }
	var	date = new Date(1000 * parseFloat(seconds));
	if (date.getUTCHours() > 0)
	{
		return date.getUTCHours() + ':' + addZero(date.getUTCMinutes()) + ':' + addZero(date.getUTCSeconds());
	}
	return date.getUTCMinutes() + ':' + addZero(date.getUTCSeconds());
}

function isInvalidTime(time)
{
    return time == null || time == '00:00:00' || time == '00:00.000';
}

function carHasTireCompounds()
{
    var tire = $prop('DataCorePlugin.GameRawData.SessionData.CarSetup.Tires.TireCompound.TireCompound');
    var tire2 = $prop('DataCorePlugin.GameRawData.SessionData.CarSetup.TiresAero.TireCompound.TireCompound');
    return tire != null || tire2 != null;
}

function isLeadFocusedRow(index)
{
    var dividerVisible = $prop('IRacingExtraProperties.SLB_Top15DividerVisible');
    var leaders = $prop('IRacingExtraProperties.iRacing_LeaderboardLayout_SLBVisibleLeaders')
    return dividerVisible && (index <= leaders);
}

function rowTop(index)
{
    var gap = 0;
    var dividerVisible = $prop('IRacingExtraProperties.SLB_Top15DividerVisible');
    if (dividerVisible && !isLeadFocusedRow(index))
    {
        gap = 3;
    }
    return 25.5 + gap;
}

function isConnected(index)
{
    var pos = getIndexedProp('IRacingExtraProperties.SLB_Position', index);
    var i = parseInt(pos) - 1;
    if (i >= 0)
    {
        return $prop('IRacingExtraProperties.iRacing_Leaderboard_Driver_' + addZero(i) + '_IsConnected');
    }
    return true;
}

function tireCompoundVisible(index)
{
    var tireString = getIndexedProp('IRacingExtraProperties.SLB_TireCompound', index);
    if (!(String(tireString).length > 0))
    {
        return false;
    }

    const tireType = getTireType(index);
    if (tireType == 'Hidden')
    {
        return false;
    }

    if (isRace())
    {
        return raceInProgress();
    }

    var inPit = getIndexedProp('IRacingExtraProperties.SLB_IsInPit', index);
    var connected = isConnected(index);
    return connected && !inPit;
}

// Returns True if the specified flag is out.
// Supported colors: 'Black', 'Blue', 'Checkered', 'Green', 'Orange', 'White', 'Yellow'.
function isFlagOut(color)
{
    var flagOut = $prop('DataCorePlugin.GameData.Flag_' + color);
    return flagOut != 0;
}

function getCarIdx(fromCarNumber)
{
    let drivers = NewRawData().AllSessionData["DriverInfo"]["Drivers"]
    if (drivers != null)
    {
        for (let i = 0; i < drivers.length; i++) 
        {
            if (drivers[i]["CarNumber"] == fromCarNumber)
            {
                return i;
            }
        }
    }   
    return null;    
}

function getTireType(fromSLBIndex)
{
    let number = getIndexedProp('IRacingExtraProperties.SLB_CarNumber', fromSLBIndex);
    const idx = getCarIdx(number);
    const tires = $prop('GameRawData.Telemetry.CarIdxTireCompound')
    if (tires == null || idx == null)
    {
        return 'Dry';
    }
    switch(tires[idx]) 
    {
        case -1:
        return 'Hidden'

        case 0:
        return 'Dry'
        
        case 1:
        return 'Wet'

        default:
        return 'Unknown'
    }
}

// Get the SessionState with a confirmation delay in ms.
// 0: Invalid
// 1: GetInCar
// 2: Warmup
// 3: ParadeLaps
// 4: Racing
// 5: Checkered
// 6: Cooldown
function getSessionState(delay)
{
    state = $prop('DataCorePlugin.GameRawData.Telemetry.SessionState');

    // Initialize
    if (root['confirmed'] == null)
    {
        root['confirmed'] = state;
    }

    // Reset timer every time state changes
    if (state != root['state'])
    {
        root['state'] = state;
        root['changed'] = Date.now() + delay;
    }
    
    // State change confirmed after a delay
    if (Date.now() >= root['changed'])
    {
        root['confirmed'] = state;
    }
    
    return root['confirmed'];
}

function raceInProgress()
{
    // Wait a few seconds to make sure all the data is available after a race start
    return isRace() && getSessionState(3000) >= 4;
}