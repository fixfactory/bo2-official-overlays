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

import os
import hashlib
import base64
import requests
import json
from dotenv import load_dotenv

# Load environment variables
load_dotenv()
email = os.getenv("IRACING_EMAIL")
password = os.getenv("IRACING_PASSWORD")

def encode_pw(email, password):
    initial_hash = hashlib.sha256((password + email.lower()).encode('utf-8')).digest()
    return base64.b64encode(initial_hash).decode('utf-8')

session = requests.Session()

headers = {'Content-Type': 'application/json'}
encoded_password = encode_pw(email, password)
data = {"email": email, "password": encoded_password}

# Authenticate
response = session.post('https://members-ng.iracing.com/auth', headers=headers, json=data, timeout=5.0)
print("Auth status:", response.status_code)

# Access flair endpoint
params = {}
response = session.get('https://members-ng.iracing.com/data/lookup/flairs', params=params, timeout=5.0)
print("Flair GET status:", response.status_code)

# If initial request is successful and contains a 'link'
if response.status_code == 200:
    flair_meta = response.json()
    flair_url = flair_meta.get("link")

    if flair_url:
        # Now download the actual flair data from the link
        flair_data_response = session.get(flair_url, timeout=5.0)
        if flair_data_response.status_code == 200:
            with open("../Data/Flairs.json", "w", encoding="utf-8") as f:
                json.dump(flair_data_response.json(), f, indent=2)
            print("Flair data saved to Flairs.json")
        else:
            print(f"Failed to fetch data from link: {flair_data_response.status_code}")
    else:
        print("No 'link' found in response.")
else:
    print(f"Initial flair lookup failed: {response.status_code}")
    print(response.text)