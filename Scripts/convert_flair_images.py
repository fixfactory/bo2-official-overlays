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

# Requires ImageMagick:
# > winget install ImageMagick.Q16-HDRI

import os
import subprocess

# Set your source and destination folders
source_folder = '../../flag-icons/flags/4x3'
destination_folder = '../../flag-icons/flags/4x3-png'

# Create destination folder if it doesn't exist
os.makedirs(destination_folder, exist_ok=True)

# Loop through all SVG files
for filename in os.listdir(source_folder):
    if filename.lower().endswith('.svg'):
        svg_path = os.path.join(source_folder, filename)
        png_filename = os.path.splitext(filename)[0] + '.png'
        png_path = os.path.join(destination_folder, png_filename)

        try:
            subprocess.run([
                'magick', svg_path, png_path
            ], check=True)
            print(f"Converted: {filename} -> {png_filename}")
        except subprocess.CalledProcessError as e:
            print(f"Failed to convert {filename}: {e}")
