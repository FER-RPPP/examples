#!/bin/bash
# Docker init environment variables
USER=${POSTGRES_USER:-postgres}
DB=${POSTGRES_DB:-postgres}

pg_isready -U "$USER" -d "$DB"