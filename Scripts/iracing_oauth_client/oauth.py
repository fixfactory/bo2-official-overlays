"""
iRacing OAuth Client - Password Limited Flow Implementation

This module provides authentication and token refresh capabilities
using the Password Limited Grant OAuth flow.
"""

import base64
import hashlib
import json
import logging
import urllib.parse
from datetime import datetime, timedelta
from typing import Dict, Optional, Any, Literal

import requests

from .logging_utils import create_logger


class IRacingOAuthClient:
    """
    Client for iRacing OAuth authentication using Password Limited Grant flow.

    This implementation handles:
    - Password Limited Grant authentication
    - Token refresh management
    - Automatic token expiration handling
    - Data API requests with proper authentication
    """

    def __init__(self, client_id: str, client_secret: str, username: str, password: str,
                 request_timeout: int = 30, token_refresh_buffer_seconds: int = 60,
                 log_level: str = "INFO", log_format: Literal["human", "json"] = "human",
                 ir_env: str = "members"):
        """
        Initialize the iRacing OAuth client.

        Args:
            client_id: Client identifier issued during registration
            client_secret: Client secret (will be masked automatically)
            username: iRacing username/email
            password: iRacing password (will be masked automatically)
            request_timeout: Timeout for HTTP requests in seconds (default: 30)
            token_refresh_buffer_seconds: Buffer time before token expiry to trigger refresh (default: 60)
            log_level: Logging level (DEBUG, INFO, WARNING, ERROR, CRITICAL) (default: "INFO")
            log_format: Logging format - "human" for readable format, "json" for JSON format (default: "human")
            ir_env: iRacing environment (default: "members")
        """
        self.client_id = client_id
        self.client_secret = client_secret
        self.username = username
        self.password = password
        self.request_timeout = request_timeout
        self.token_refresh_buffer_seconds = token_refresh_buffer_seconds
        self.ir_env = ir_env

        # Set up logging using shared utility
        self.logger = create_logger(
            f"{__name__}.{self.__class__.__name__}",
            log_level,
            log_format
        )

        # OAuth endpoints - construct based on environment
        if ir_env == "members":
            self.token_url = "https://oauth.iracing.com/oauth2/token"
        else:
            self.token_url = f"https://{ir_env}-oauth.iracing.com/oauth2/token"

        # Token storage
        self.access_token: Optional[str] = None
        self.refresh_token: Optional[str] = None
        self.token_expires_at: Optional[datetime] = None
        self.refresh_token_expires_at: Optional[datetime] = None
        self.scope: Optional[str] = None

    def _mask_secret(self, secret: str, identifier: str) -> str:
        """
        Mask a secret (client_secret or password) using iRacing's masking algorithm.

        Args:
            secret: The secret to mask
            identifier: client_id for client_secret, username for password

        Returns:
            Base64 encoded SHA-256 hash of secret + normalized_identifier
        """
        # Normalize the identifier (trim and lowercase)
        normalized_id = identifier.strip().lower()

        # Concatenate secret with normalized identifier
        combined = f"{secret}{normalized_id}"

        # Create SHA-256 hash
        hasher = hashlib.sha256()
        hasher.update(combined.encode('utf-8'))

        # Return base64 encoded hash
        return base64.b64encode(hasher.digest()).decode('utf-8')

    def _make_token_request(self, data: Dict[str, str]) -> Dict[str, Any]:
        """
        Make a request to the token endpoint.

        Args:
            data: Form data for the token request

        Returns:
            JSON response from the token endpoint

        Raises:
            requests.RequestException: If the request fails
            ValueError: If the response is invalid
        """
        headers = {
            'Content-Type': 'application/x-www-form-urlencoded'
        }

        # Some debug logs here contain sensitive info
        self.logger.debug(f"POST {self.token_url}")
        self.logger.debug(f"Headers: {headers}")
        self.logger.debug(f"Data: {data}")
        self.logger.debug(f"URL-encoded POST body: {urllib.parse.urlencode(data)}")

        try:
            response = requests.post(
                self.token_url,
                data=data,
                headers=headers,
                timeout=self.request_timeout
            )

            # Log the response
            self.logger.debug(f"Response Status: {response.status_code}")
            self.logger.debug(f"Response Headers: {dict(response.headers)}")

            # Log response body for 4xx and 5xx errors
            if response.status_code >= 400:
                try:
                    error_body = response.text
                    self.logger.error(f"HTTP {response.status_code} error response body: {error_body}")
                except Exception:
                    self.logger.error(f"HTTP {response.status_code} error (could not read response body)")

            response.raise_for_status()

            response_json = response.json()
            # This log message contains sensitive info
            self.logger.debug(f"Response JSON: {json.dumps(response_json, indent=2)}")

            return response_json

        except requests.RequestException as e:
            raise requests.RequestException(f"Token request failed: {e}")
        except json.JSONDecodeError as e:
            raise ValueError(f"Invalid JSON response: {e}")

    def authenticate(self, scope: str = "iracing.auth") -> bool:
        """
        Authenticate using Password Limited Grant flow.

        Args:
            scope: OAuth scope to request (default: "iracing.auth")

        Returns:
            True if authentication successful, False otherwise
        """
        # Warn user if DEBUG logging is enabled
        if self.logger.isEnabledFor(logging.DEBUG):
            print("\n" + "="*70)
            print("WARNING: DEBUG logging is enabled!")
            print("="*70)
            print("Sensitive data including tokens and masked credentials will be logged.")
            print("This should only be used in secure development environments.")
            print("="*70)
            response = input("Do you want to continue? (yes/no): ").strip().lower()
            if response not in ('yes', 'y'):
                self.logger.info("Authentication cancelled by user due to DEBUG logging warning")
                return False
            print()

        try:
            # Mask the client secret and password
            masked_client_secret = self._mask_secret(self.client_secret, self.client_id)
            masked_password = self._mask_secret(self.password, self.username)

            # Prepare the token request data
            data = {
                'grant_type': 'password_limited',
                'client_id': self.client_id,
                'client_secret': masked_client_secret,
                'username': self.username,
                'password': masked_password,
                'scope': scope
            }

            # Make the token request
            response_data = self._make_token_request(data)

            # Store the tokens and metadata
            self.access_token = response_data.get('access_token')
            self.refresh_token = response_data.get('refresh_token')
            self.scope = response_data.get('scope')

            # Check if scope was granted
            if self.scope is None:
                self.logger.error(
                    "Authentication succeeded but no scopes were returned. "
                    "Check that the requested scope '%s' is spelled correctly and that the account is active.",
                    scope
                )
                return False

            # Calculate token expiration times
            expires_in = response_data.get('expires_in')
            if expires_in is None:
                self.logger.error("Authentication failed: 'expires_in' missing from token response")
                return False

            self.token_expires_at = datetime.now() + timedelta(seconds=expires_in)

            refresh_expires_in = response_data.get('refresh_token_expires_in')
            if refresh_expires_in:
                self.refresh_token_expires_at = datetime.now() + timedelta(seconds=refresh_expires_in)

            self.logger.info("Authentication successful! Token expires at: %s", self.token_expires_at)
            return True

        except Exception as e:
            self.logger.error("Authentication failed: %s", e)
            return False

    def refresh_access_token(self) -> bool:
        """
        Refresh the access token using the refresh token.

        Returns:
            True if refresh successful, False otherwise
        """
        if not self.refresh_token:
            self.logger.warning("No refresh token available")
            return False

        if self.refresh_token_expires_at and datetime.now() >= self.refresh_token_expires_at:
            self.logger.warning("Refresh token has expired, need to re-authenticate")
            return False

        try:
            # Mask the client secret
            masked_client_secret = self._mask_secret(self.client_secret, self.client_id)

            # Prepare the refresh token request data
            data = {
                'grant_type': 'refresh_token',
                'client_id': self.client_id,
                'client_secret': masked_client_secret,
                'refresh_token': self.refresh_token
            }

            # Make the token request
            response_data = self._make_token_request(data)

            # Update tokens and metadata
            self.access_token = response_data.get('access_token')
            new_refresh_token = response_data.get('refresh_token')
            if new_refresh_token:
                self.refresh_token = new_refresh_token

            # Check if scope was granted
            refreshed_scope = response_data.get('scope')
            if refreshed_scope is None:
                self.logger.error(
                    "Token refresh succeeded but no scopes were returned. "
                    "Check that the account is active."
                )
                return False

            self.scope = refreshed_scope

            # Update token expiration times
            expires_in = response_data.get('expires_in')
            if expires_in is None:
                self.logger.error("Token refresh failed: 'expires_in' missing from token response")
                return False

            self.token_expires_at = datetime.now() + timedelta(seconds=expires_in)

            refresh_expires_in = response_data.get('refresh_token_expires_in')
            if refresh_expires_in:
                self.refresh_token_expires_at = datetime.now() + timedelta(seconds=refresh_expires_in)

            self.logger.info("Token refreshed successfully! New token expires at: %s", self.token_expires_at)
            return True

        except Exception as e:
            self.logger.error("Token refresh failed: %s", e)
            return False

    def _ensure_valid_token(self) -> bool:
        """
        Ensure we have a valid access token, refreshing if necessary.

        Returns:
            True if we have a valid token, False otherwise
        """
        if not self.access_token:
            self.logger.warning("No access token, need to authenticate first")
            return False

        # Check if token is expired or will expire soon
        if self.token_expires_at:
            time_until_expiry = self.token_expires_at - datetime.now()
            if time_until_expiry.total_seconds() < self.token_refresh_buffer_seconds:
                self.logger.info("Token expired or expiring soon, attempting refresh...")
                return self.refresh_access_token()

        return True



    def is_authenticated(self) -> bool:
        """
        Check if the client is currently authenticated.

        Returns:
            True if authenticated with valid token, False otherwise
        """
        return self.access_token is not None and self._ensure_valid_token()
