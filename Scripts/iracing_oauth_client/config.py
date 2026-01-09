"""
Configuration management for iRacing OAuth Client

This module provides a three-layer configuration system:
1. defaults.ini - Default operational settings (committed to repo)
2. iracing_oauth.ini - User overrides (optional, gitignored)
3. .env - Credentials only (never committed, gitignored)
"""

import os
import configparser
from pathlib import Path
from typing import Optional, Literal
from dotenv import load_dotenv


class Config:
    """
    Configuration class that loads settings from defaults.ini, iracing_oauth.ini, and .env files.

    Operational settings are read from defaults.ini (committed) with optional
    overrides in iracing_oauth.ini (gitignored). Credentials are in .env (gitignored).
    """

    def __init__(self, env_file: Optional[str] = None, config_file: Optional[str] = None):
        """
        Initialize configuration.

        Args:
            env_file: Path to .env file (optional, defaults to .env in current directory)
            config_file: Path to iracing_oauth.ini override file (optional, defaults to iracing_oauth.ini in current directory)
        """
        if env_file:
            load_dotenv(env_file)
        else:
            load_dotenv()  # Load from default .env file

        # Load configuration with override pattern
        # defaults.ini (bundled with package) provides base settings
        # iracing_oauth.ini (optional, gitignored) overrides specific settings
        self._config = configparser.ConfigParser()

        # Build list of config files to read
        # Bundled defaults.ini (always present, in package directory)
        defaults_path = Path(__file__).parent / 'defaults.ini'
        config_files = [str(defaults_path)]

        # User's local iracing_oauth.ini (optional, in working directory)
        if config_file:
            config_files.append(config_file)
        else:
            # Add iracing_oauth.ini if it exists (user overrides)
            config_path = Path('iracing_oauth.ini')
            if config_path.exists():
                config_files.append('iracing_oauth.ini')
        self._config.read(config_files)

    @property
    def client_id(self) -> str:
        """Get client ID from environment."""
        value = os.getenv('IR_CLIENT_ID')
        if not value:
            raise ValueError("IR_CLIENT_ID environment variable is required")
        return value

    @property
    def client_secret(self) -> str:
        """Get client secret from environment."""
        value = os.getenv('IR_CLIENT_SECRET')
        if not value:
            raise ValueError("IR_CLIENT_SECRET environment variable is required")
        return value

    @property
    def username(self) -> str:
        """Get username from environment."""
        value = os.getenv('IR_USERNAME')
        if not value:
            raise ValueError("IR_USERNAME environment variable is required")
        return value

    @property
    def password(self) -> str:
        """Get password from environment."""
        value = os.getenv('IR_PASSWORD')
        if not value:
            raise ValueError("IR_PASSWORD environment variable is required")
        return value

    @property
    def scope(self) -> str:
        """Get OAuth scope from defaults.ini/iracing_oauth.ini."""
        return self._config.get('oauth', 'scope', fallback='iracing.auth')

    @property
    def request_timeout(self) -> int:
        """Get request timeout from defaults.ini/iracing_oauth.ini."""
        return self._config.getint('oauth', 'request_timeout', fallback=30)

    @property
    def token_refresh_buffer_seconds(self) -> int:
        """Get token refresh buffer from defaults.ini/iracing_oauth.ini."""
        return self._config.getint('oauth', 'token_refresh_buffer_seconds', fallback=60)

    @property
    def log_level(self) -> str:
        """Get logging level from defaults.ini/iracing_oauth.ini."""
        return self._config.get('logging', 'level', fallback='INFO').upper()

    @property
    def log_format(self) -> Literal["human", "json"]:
        """Get logging format from defaults.ini/iracing_oauth.ini."""
        format_value = self._config.get('logging', 'format', fallback='human').lower()
        if format_value == 'json':
            return 'json'
        else:
            return 'human'

    @property
    def ir_env(self) -> str:
        """Get iRacing environment from defaults.ini/iracing_oauth.ini."""
        return self._config.get('oauth', 'environment', fallback='members')
