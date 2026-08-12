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

import json
from pathlib import Path


def update_track_info(track_folder, track_info_path):
    track_folder = Path(track_folder)
    track_info_path = Path(track_info_path)

    # Load existing TrackInfo.json
    with track_info_path.open("r", encoding="utf-8") as f:
        track_info = json.load(f)

    added = 0
    updated = 0
    skipped = 0

    # Process all JSON files in the track folder
    for track_file in track_folder.glob("*.json"):

        # Don't process TrackInfo.json itself
        if track_file.resolve() == track_info_path.resolve():
            continue

        try:
            with track_file.open("r", encoding="utf-8") as f:
                track_data = json.load(f)
        except (json.JSONDecodeError, OSError) as e:
            print(f"Skipping {track_file.name}: {e}")
            skipped += 1
            continue

        track_id = track_data.get("trackId")

        if not track_id:
            print(f"Skipping {track_file.name}: no trackId")
            skipped += 1
            continue

        # Only use values that actually exist in the track file
        pit_entry = track_data.get("pitentry")
        pit_exit = track_data.get("pitexit")

        if pit_entry is None and pit_exit is None:
            print(f"Skipping {track_file.name}: no pitentry/pitexit")
            skipped += 1
            continue

        # Get existing TrackInfo entry, or create a new one
        if track_id not in track_info:
            track_info[track_id] = {}
            added += 1
            print(f"Adding new track: {track_id}")
        else:
            updated += 1
            print(f"Updating track: {track_id}")

        # Add/update pit entry and exit values
        if pit_entry is not None:
            track_info[track_id]["pitEntryTrackPct"] = pit_entry

        if pit_exit is not None:
            track_info[track_id]["pitExitTrackPct"] = pit_exit

    # Write the updated TrackInfo.json
    with track_info_path.open("w", encoding="utf-8") as f:
        json.dump(track_info, f, indent=4)
        f.write("\n")

    print()
    print("Done.")
    print(f"Tracks added:   {added}")
    print(f"Tracks updated: {updated}")
    print(f"Tracks skipped: {skipped}")


if __name__ == "__main__":
    TRACK_FOLDER = r"..\..\lovely-track-data\data\iracing"
    TRACK_INFO_FILE = r"..\Data\TrackInfo.json"

    update_track_info(TRACK_FOLDER, TRACK_INFO_FILE)