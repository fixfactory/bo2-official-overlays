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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace benofficial2.Plugin
{
    public class Constants
    {
        // Smallest amount of time in seconds that is considered not zero.
        public const double SecondsEpsilon = 1e-5;

        // Smallest amount of fuel in local units that is considered not zero.
        public const double FuelEpsilon = 1e-9;

        // Smallest fraction of a lap that is considered not zero.
        public const double LapEpsilon = 1e-9;

        // Smallest fraction of a distance unit that is considered not zero.
        public const double DistanceEpsilon = 1e-9;

        // Maximum amount of a lap left for iRacing to show the white flag.
        // It is unknown what are the exact white flag rule constants used by iRacing and seem to change per track.
        public const double WhiteFlagRuleLapPct = 0.97;

        // How long is the short parade lap at supported tracks (as a percentage of a lap).
        // This is the default values for tracks we don't have in the database.
        public const double ShortParadeLapPct = 0.50;

        public const double PoundPerKg = 2.204623;
        public const double GallonPerLiter = 0.264172;
        public const double FuelKgPerLiter = 0.7438;

        // IRSDK_UNLIMITED_TIME
        public const double UnlimitedTimeSeconds = 604800.0f;
    }

    public enum TrackLoc
    {
        NotInWorld = -1,
        OffTrack = 0,
        InPitStall,
        AproachingPits,
        OnTrack
    }
}
