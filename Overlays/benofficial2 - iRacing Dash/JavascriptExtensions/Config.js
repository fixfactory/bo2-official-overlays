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

function carIsGT3()
{
    return isnull($prop('benofficial2.Car.IsGT3'), false);
}

function carIsGT4()
{
    return isnull($prop('benofficial2.Car.IsGT4'), false);
}

function carIsGTE()
{
    return isnull($prop('benofficial2.Car.IsGTE'), false);
}

function carIsLMDh()
{
    return isnull($prop('benofficial2.Car.IsGTP'), false);
}

// Used for the small top green box.
function carHasPushToPass()
{
    return isnull($prop('benofficial2.Car.HasPushToPass'), false);
}

// Used for the upper green box.
function carHasPushToPassCount()
{
    return isnull($prop('benofficial2.Car.HasPushToPassCount'), false);
}

// Used for the upper green box.
function carHasPushToPassTimer()
{
    return isnull($prop('benofficial2.Car.HasPushToPassTimer'), false);
}

// Used for the small top green box.
function carHasPushToPassCooldown()
{
    return isnull($prop('benofficial2.Car.HasPushToPassCooldown'), false);
}

// Used for the small top green box.
function carHasDrsDetection()
{
    return isnull($prop('benofficial2.Car.HasDrsDetection'), false);
}

// Used for the small top green box.
function carHasDrsCount()
{
    return isnull($prop('benofficial2.Car.HasDrsCount'), false);
}

// Used for the upper green box.
function carHasErs()
{
    return isnull($prop('benofficial2.Car.HasErs'), false);
}

// Used for the upper green box.
function carHasBoost()
{
    return isnull($prop('benofficial2.Car.HasBoost'), false);
}

// Used for the upper green box.
function carHasEnginePowerMode()
{
    return isnull($prop('benofficial2.Car.HasEnginePowerMode'), false);
}

// Used for the lower green box.
function carHasDeployMode()
{
    return isnull($prop('benofficial2.Car.HasDeployMode'), false);
}

// Used for cars that show their ARB as 'P2' instead of '1'.
function carHasARBModeP()
{
    return isnull($prop('benofficial2.Car.HasARBModeP'), false);
}

// Used for the upper orange box.
function carHasFrontARB()
{
    return isnull($prop('benofficial2.Car.HasFrontARB'), false);
}

// Used for the upper orange box.
function carHasExitDiff()
{
    return isnull($prop('benofficial2.Car.HasExitDiff'), false);
}

// Used for the upper orange box.
function carHasEntryDiffPreload()
{
    return isnull($prop('benofficial2.Car.HasEntryDiffPreload'), false);
}

// Used for the upper orange box.
function carHasTC2()
{
    return isnull($prop('benofficial2.Car.HasTC2'), false);
}

// Used for the lower orange box.
function carHasRearARB()
{
    return isnull($prop('benofficial2.Car.HasRearARB'), false);
}

// Used for the lower orange box.
function carHasEntryDiff()
{
    return isnull($prop('benofficial2.Car.HasEntryDiff'), false);
}

// Used for the lower orange box.
function carHasTC()
{
    return isnull($prop('benofficial2.Car.HasTC'), false);
}

// Used for the upper red box.
function carHasTwoPartBrakeBias()
{
    return isnull($prop('benofficial2.Car.HasTwoPartBrakeBias'), false);
}

// Used for the lower red box.
function carHasWeightJacker()
{
    return isnull($prop('benofficial2.Car.HasWeightJacker'), false);
}

// Used for the lower red box.
function carHasTwoPartPeakBrakeBias()
{
    return isnull($prop('benofficial2.Car.HasTwoPartPeakBrakeBias'), false);
}

// Used for the lower red box.
function carHasABS()
{
    return isnull($prop('benofficial2.Car.HasABS'), false);
}

// Used for the lower red box.
function carHasFineBrakeBias()
{
    return isnull($prop('benofficial2.Car.HasFineBrakeBias'), false);
}

// Used for the lower red box.
function carHasBrakeBiasMigration()
{
    return isnull($prop('benofficial2.Car.HasBrakeBiasMigration'), false);
}