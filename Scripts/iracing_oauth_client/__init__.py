"""
iRacing OAuth Client Package

A Python client for authenticating with iRacing's API using the Password Limited Grant OAuth flow.
This package handles authentication, token management, and automatic token refresh.

Example:
    >>> from iracing_oauth_client import IRacingOAuthClient, Config
    >>> config = Config()
    >>> client = IRacingOAuthClient(
    ...     client_id=config.client_id,
    ...     client_secret=config.client_secret,
    ...     username=config.username,
    ...     password=config.password
    ... )
    >>> if client.authenticate():
    ...     print("Authenticated!")
"""

from .oauth import IRacingOAuthClient
from .config import Config
from .logging_utils import JSONFormatter, create_logger

__version__ = "0.1.0"
__all__ = ["IRacingOAuthClient", "Config", "JSONFormatter", "create_logger"]
