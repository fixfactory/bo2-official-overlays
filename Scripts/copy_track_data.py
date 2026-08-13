'''
    benofficial2's Official Overlays
    Copyright (C) 2026 benofficial2

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
'''

# This script updates the TrackInfo.json file with any missing trackId entries from TrackData.json.

import json
from pathlib import Path


# Paths
track_data_path = Path(r"C:\Program Files (x86)\SimHub\PluginsData\IRacing\TrackData.json")
track_info_path = Path(__file__).resolve().parent / ".." / "Data" / "TrackInfo.json"


def main():
    # Load TrackData.json
    with track_data_path.open("r", encoding="utf-8-sig") as f:
        track_data = json.load(f)

    # Load TrackInfo.json
    with track_info_path.open("r", encoding="utf-8-sig") as f:
        track_info = json.load(f)

    additions = 0

    for track_id, data in track_data.items():
        entry_key = "pitlaneEntryTrackPct"
        exit_key = "pitlaneExitTrackPct"

        source_entry_key = "PitlaneEntryTrackPct"
        source_exit_key = "PitlaneExitTrackPct"

        # If the track doesn't exist, create it.
        if track_id not in track_info:
            track_info[track_id] = {}

        # Add entry only if it exists in TrackData and is missing in TrackInfo.
        if (
            source_entry_key in data
            and entry_key not in track_info[track_id]
        ):
            track_info[track_id][entry_key] = data[source_entry_key]
            additions += 1

        # Add exit only if it exists in TrackData and is missing in TrackInfo.
        if (
            source_exit_key in data
            and exit_key not in track_info[track_id]
        ):
            track_info[track_id][exit_key] = data[source_exit_key]
            additions += 1

    # Write the updated TrackInfo.json
    with track_info_path.open("w", encoding="utf-8") as f:
        json.dump(track_info, f, indent=4, ensure_ascii=False)
        f.write("\n")

    print(f"Added {additions} missing pitlane properties.")


if __name__ == "__main__":
    main()