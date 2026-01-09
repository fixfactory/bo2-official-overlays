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
from typing import List, Dict, Any, cast
from iracing_oauth_client import IRacingOAuthClient, Config
from data_api import IRacingDataAPI


def fetch_flairs_list(data_api: IRacingDataAPI) -> Any:
    """Fetch and display flairs list."""
    print("\n" + "="*50)
    print("FETCHING FLAIRS LIST")
    print("="*50)

    flairs_list = data_api.get_flairs_list()
    if flairs_list:
        # Handle both direct list response and wrapped response
        flairs_data: List[Dict[str, Any]]
        if isinstance(flairs_list, list):
            flairs_data = flairs_list
        else:
            # Extract data from dict response, ensure it's a list
            extracted_data = flairs_list.get('data', flairs_list)
            flairs_data = extracted_data if isinstance(extracted_data, list) else []

        print(f"✅ Successfully retrieved {len(flairs_data)} flairs")

        # Show first few flairs as examples
        for i, flair in enumerate(cast(List[Dict[str, Any]], flairs_data)[:3]):
            print(f"   {i+1}. {flair.get('name', 'Unknown')} - {flair.get('shortname', 'N/A')}")

        if len(flairs_data) > 3:
            print(f"   ... and {len(flairs_data) - 3} more flairs")
    else:
        print("❌ Failed to retrieve flairs list")
    return flairs_list


def dump_to_json(data: Any, filename: str) -> None:
    """Dump data to a JSON file.

    Args:
        data: Data to dump to JSON
        filename: Name of the file to write to
    """
    try:
        with open(filename, 'w') as f:
            json.dump(data, f, indent=2)
        print(f"   📄 Dumped to {filename}")
    except Exception as e:
        print(f"   ❌ Failed to dump to {filename}: {e}")


def main():
    # Load configuration
    try:
        config = Config()
    except ValueError as e:
        print(f"❌ Configuration error: {e}")
        print("\nPlease ensure you have:")
        print("1. Copied .env.example to .env")
        print("2. Updated .env with your actual iRacing credentials")
        print("3. Registered your client with iRacing for Password Limited Grant")
        return

    # Create and authenticate OAuth client
    try:
        oauth_client = IRacingOAuthClient(
            client_id=config.client_id,
            client_secret=config.client_secret,
            username=config.username,
            password=config.password,
            request_timeout=config.request_timeout,
            token_refresh_buffer_seconds=config.token_refresh_buffer_seconds,
            log_level=config.log_level,
            log_format=config.log_format,
            ir_env=config.ir_env
        )

        print("Authenticating with iRacing...")
        if not oauth_client.authenticate(scope=config.scope):
            print("❌ Authentication failed!")
            return

        print("✅ Authentication successful!")

        # Display authentication status
        print("\n" + "="*50)
        print("AUTHENTICATION STATUS")
        print("="*50)
        print(f"✅ Client is authenticated: {oauth_client.is_authenticated()}")
        print(f"   Access token expires at: {oauth_client.token_expires_at}")
        if oauth_client.refresh_token_expires_at:
            print(f"   Refresh token expires at: {oauth_client.refresh_token_expires_at}")
        print(f"   Granted scope: {oauth_client.scope}")

    except Exception as e:
        print(f"❌ Authentication error: {e}")
        return

    # Create Data API client
    try:
        data_api = IRacingDataAPI(
            oauth_client,
            log_level=config.log_level,
            log_format=config.log_format
        )
    except Exception as e:
        print(f"❌ Failed to create Data API client: {e}")
        return
    
    # Fetch flairs list
    try:
        flairs_list = fetch_flairs_list(data_api)
        dump_to_json(flairs_list, '../Data/Flairs.json')
    except Exception as e:
        print(f"❌ Error during API calls: {e}")


if __name__ == "__main__":
    main()