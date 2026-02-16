'''
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
'''

# This script copies every overlay we want to publish from the SimHub folder.
# We use robocopy so it only copies changed files, and removes deleted files.

from subprocess import call
import os
import shutil

simhub_folder = "C:\\Program Files (x86)\\SimHub"
overlays_folder = "..\\Overlays"
overlays_to_copy = [
    "benofficial2 - iRacing Dash", 
    "benofficial2 - iRacing Delta", 
    "benofficial2 - iRacing Inputs", 
    "benofficial2 - iRacing Relative", 
    "benofficial2 - iRacing Standings", 
    "benofficial2 - iRacing Track Map",
    "benofficial2 - iRacing Launch Assist",
    "benofficial2 - iRacing Setup Cover",
    "benofficial2 - iRacing Spotter",
    "benofficial2 - iRacing Fuel Calculator",
    "benofficial2 - iRacing Fuel Calculator Live",
    "benofficial2 - Twitch Chat",
    "benofficial2 - iRacing Wind",
    "benofficial2 - iRacing Precipitation",
    "benofficial2 - iRacing Track Wetness",
    "benofficial2 - iRacing Track Rubber",
    "benofficial2 - iRacing Track Time",
    "benofficial2 - iRacing Track Temperature",
    "benofficial2 - iRacing Air Temperature",
    "benofficial2 - iRacing Relative Humidity",
    "benofficial2 - iRacing Multi-Class Standings",
    "benofficial2 - iRacing Highlighted Driver",
    "benofficial2 - iRacing ABS"]

def copy_overlays_from_simhub():
    for overlay_name in overlays_to_copy:
        call(["robocopy", simhub_folder + "\\DashTemplates\\" + overlay_name, overlays_folder + "\\" + overlay_name, "/MIR"])

def delete_backup_folders(start_path):
    for root, dirs, files in os.walk(start_path, topdown=False):
        for folder in dirs:
            if folder == "_Backups":
                folder_path = os.path.join(root, folder)
                try:
                    shutil.rmtree(folder_path)
                    print(f"Deleted: {folder_path}")
                except Exception as e:
                    print(f"Failed to delete {folder_path}: {e}")

copy_overlays_from_simhub()
delete_backup_folders(overlays_folder)
