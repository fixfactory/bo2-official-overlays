"""
iRacing Data API Client

This module provides data retrieval capabilities for the iRacing Data API.
It relies on the IRacingOAuthClient for authentication and token management.
"""

import json
from typing import Dict, Optional, Any, Literal

import requests

from iracing_oauth_client import IRacingOAuthClient
from iracing_oauth_client.logging_utils import create_logger


class IRacingDataAPI:
    """
    Client for accessing iRacing's /data API endpoints.

    This class handles all interactions with iRacing's data endpoints,
    including the special signed URL handling that many endpoints use.
    It relies on an IRacingOAuthClient instance for authentication.
    """

    def __init__(self, oauth_client: IRacingOAuthClient,
                 log_level: str = "INFO", log_format: Literal["human", "json"] = "human"):
        """
        Initialize the Data API client.

        Args:
            oauth_client: An authenticated IRacingOAuthClient instance
            log_level: Logging level (DEBUG, INFO, WARNING, ERROR, CRITICAL) (default: "INFO")
            log_format: Logging format - "human" for readable format, "json" for JSON format (default: "human")
        """
        self.oauth_client = oauth_client
        self.data_api_base = f"https://{oauth_client.ir_env}-ng.iracing.com/data"

        # Set up logging using shared utility
        self.logger = create_logger(
            f"{__name__}.{self.__class__.__name__}",
            log_level,
            log_format
        )

    def _fetch_from_signed_url(self, signed_url: str) -> Optional[Dict[str, Any]]:
        """
        Fetch data from a signed URL returned by iRacing's API.

        Args:
            signed_url: The signed URL to fetch data from

        Returns:
            JSON response data, or None if request failed
        """
        try:
            response = requests.get(signed_url, timeout=self.oauth_client.request_timeout)
            response.raise_for_status()

            return response.json()

        except requests.RequestException as e:
            self.logger.error("Signed URL request failed: %s", e)
            return None
        except json.JSONDecodeError as e:
            self.logger.error("Invalid JSON response from signed URL: %s", e)
            return None

    def make_api_request(self, endpoint: str, params: Optional[Dict[str, Any]] = None,
                        auto_fetch_signed_url: bool = True) -> Optional[Dict[str, Any]]:
        """
        Make an authenticated request to the iRacing Data API.

        This method handles both direct API responses and the two-step signed URL process
        that iRacing uses for most endpoints.

        Args:
            endpoint: API endpoint (e.g., '/member/get', '/series/get')
            params: Query parameters for the request
            auto_fetch_signed_url: If True, automatically fetch data from signed URLs

        Returns:
            JSON response data, or None if request failed
        """
        if not self.oauth_client._ensure_valid_token():
            return None

        url = f"{self.data_api_base}{endpoint}"
        headers = {
            'Authorization': f'Bearer {self.oauth_client.access_token}',
            'Accept': 'application/json'
        }

        # Log the GET request details. The Headers log contains sensitive data.
        self.logger.info(f"GET {url}")
        self.logger.debug(f"Headers: {headers}")
        self.logger.debug(f"Params: {params}")

        try:
            response = requests.get(url, headers=headers, params=params,
                                   timeout=self.oauth_client.request_timeout)

            # Log the response
            self.logger.info(f"Response Status: {response.status_code}")
            self.logger.debug(f"Response Headers: {dict(response.headers)}")

            response.raise_for_status()

            response_data = response.json()
            self.logger.debug(f"Response JSON: {json.dumps(response_data, indent=2)}")

            # Check if this is a signed URL response
            if (auto_fetch_signed_url and
                isinstance(response_data, dict) and
                'link' in response_data and
                'expires' in response_data):

                self.logger.info("Received signed URL for %s, fetching data...", endpoint)
                signed_url = response_data['link']

                # Fetch the actual data from the signed URL
                actual_data = self._fetch_from_signed_url(signed_url)
                return actual_data

            # Return the response as-is (for direct data endpoints)
            return response_data

        except requests.RequestException as e:
            self.logger.error("API request failed: %s", e)
            return None
        except json.JSONDecodeError as e:
            self.logger.error("Invalid JSON response: %s", e)
            return None

    def make_api_request_raw(self, endpoint: str, params: Optional[Dict[str, Any]] = None) -> Optional[Dict[str, Any]]:
        """
        Make a raw API request without automatic signed URL fetching.

        Use this when you want to get the raw response (including signed URLs)
        without automatic fetching.

        Args:
            endpoint: API endpoint (e.g., '/member/get', '/series/get')
            params: Query parameters for the request

        Returns:
            Raw JSON response data, or None if request failed
        """
        return self.make_api_request(endpoint, params, auto_fetch_signed_url=False)

    def get_member_info(self, cust_ids: Optional[list] = None) -> Optional[Dict[str, Any]]:
        """
        Get member information.

        Args:
            cust_ids: List of customer IDs to get info for (optional)

        Returns:
            Member information data (automatically fetched from signed URL)
        """
        params = {}
        if cust_ids:
            params['cust_ids'] = ','.join(map(str, cust_ids))

        return self.make_api_request('/member/get', params)

    def get_series_list(self) -> Optional[Dict[str, Any]]:
        """
        Get list of all series.

        Returns:
            Series list data (automatically fetched from signed URL if needed)
        """
        return self.make_api_request('/series/get')

    def get_cars_list(self) -> Optional[Dict[str, Any]]:
        """
        Get list of all cars.

        Returns:
            Cars list data (automatically fetched from signed URL if needed)
        """
        return self.make_api_request('/car/get')

    def get_tracks_list(self) -> Optional[Dict[str, Any]]:
        """
        Get list of all tracks.

        Returns:
            Tracks list data (automatically fetched from signed URL if needed)
        """
        return self.make_api_request('/track/get')

    def get_flairs_list(self) -> Optional[Dict[str, Any]]:
        """
        Get list of all flairs.

        Returns:
            Flairs list data (automatically fetched from signed URL if needed)
        """
        return self.make_api_request('/lookup/flairs')