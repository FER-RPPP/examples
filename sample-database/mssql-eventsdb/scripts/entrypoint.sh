#!/bin/bash
set -euo pipefail

/opt/mssql/bin/sqlservr &

/usr/local/bin/configure-db.sh

wait