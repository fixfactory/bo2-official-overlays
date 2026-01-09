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

# This script copies every image library we want to publish from the SimHub folder.
# We use robocopy so it only copies changed files, and removes deleted files.

from subprocess import call

simhub_folder = "C:\\Program Files (x86)\\SimHub"
images_folder = "..\\Images"
libraries_to_copy = ["CarLogos", "Flairs", "Icons"]

def copy_libraries_from_simhub():
    for library_name in libraries_to_copy:
        call(["robocopy", simhub_folder + "\\ImageLibrary\\benofficial2\\" + library_name, images_folder + "\\" + library_name, "/MIR"])

copy_libraries_from_simhub()

