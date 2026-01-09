"""
Shared logging utilities for the iRacing OAuth application.

This module provides common logging formatters and utilities that can be
used across different components of the application.
"""

import json
import logging


class JSONFormatter(logging.Formatter):
    """
    Custom JSON formatter for log records.
    
    This formatter converts log records into structured JSON format,
    making them suitable for log aggregation systems and structured
    log analysis tools.
    
    The JSON output includes:
    - timestamp: Formatted timestamp of the log record
    - level: Log level (DEBUG, INFO, WARNING, ERROR, CRITICAL)
    - message: The formatted log message
    - module: Name of the module where the log was generated
    - function: Name of the function where the log was generated
    - line: Line number where the log was generated
    - exception: Exception traceback if present
    """
    
    def format(self, record):
        """
        Format the log record as JSON.
        
        Args:
            record: LogRecord instance containing log information
            
        Returns:
            JSON string representation of the log record
        """
        log_entry = {
            'timestamp': self.formatTime(record, self.datefmt),
            'level': record.levelname,
            'message': record.getMessage(),
            'module': record.module,
            'function': record.funcName,
            'line': record.lineno
        }
        
        # Add exception info if present
        if record.exc_info:
            log_entry['exception'] = self.formatException(record.exc_info)
            
        return json.dumps(log_entry)


def create_logger(name: str, log_level: str = "INFO", log_format: str = "human") -> logging.Logger:
    """
    Create a configured logger instance.
    
    This is a utility function to create consistently configured loggers
    across the application.
    
    Args:
        name: Name for the logger (typically module.class)
        log_level: Logging level (DEBUG, INFO, WARNING, ERROR, CRITICAL)
        log_format: Format type - "human" for readable, "json" for structured
        
    Returns:
        Configured Logger instance
    """
    logger = logging.getLogger(name)
    logger.setLevel(getattr(logging, log_level.upper()))
    
    # Remove existing handlers to avoid duplicates
    logger.handlers.clear()
    
    # Create console handler
    console_handler = logging.StreamHandler()
    console_handler.setLevel(getattr(logging, log_level.upper()))
    
    # Set formatter based on format preference
    if log_format == "json":
        formatter = JSONFormatter(datefmt='%Y-%m-%d %H:%M:%S')
    else:
        formatter = logging.Formatter(
            '%(asctime)s - %(name)s - %(levelname)s - %(message)s',
            datefmt='%Y-%m-%d %H:%M:%S'
        )
    
    console_handler.setFormatter(formatter)
    logger.addHandler(console_handler)
    
    return logger