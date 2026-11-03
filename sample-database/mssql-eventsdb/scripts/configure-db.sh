#!/bin/bash
set -euo pipefail

sqlcmd_bin="/opt/mssql-tools18/bin/sqlcmd"
server_ready=false
db_exists=""

run_sqlcmd_query() {
    "$sqlcmd_bin" -S localhost -U sa -P "${MSSQL_SA_PASSWORD}" -C "$@"
}

for _ in $(seq 1 60); do
    if run_sqlcmd_query -Q "SELECT 1" >/dev/null 2>&1; then
        server_ready=true
        break
    fi

    sleep 2
done

if [ "$server_ready" != "true" ]; then
    echo "SQL Server did not become ready in time." >&2
    exit 1
fi

for _ in $(seq 1 30); do
    if db_exists="$(run_sqlcmd_query -h -1 -W -Q "SET NOCOUNT ON; SELECT CASE WHEN DB_ID(N'${MSSQL_DB}') IS NULL THEN 0 ELSE 1 END;" 2>/dev/null)"; then
        db_exists="$(echo "$db_exists" | tr -d '[:space:]')"
        break
    fi

    sleep 2
done

if [ -z "$db_exists" ]; then
    echo "Could not determine whether database ${MSSQL_DB} exists." >&2
    exit 1
fi

if [ "$db_exists" = "1" ]; then
    echo "Database ${MSSQL_DB} already exists. Skipping initialization."
    exit 0
fi

for sql_file in /docker-entrypoint-initdb.d/*.sql; do
    echo "Running ${sql_file}"
    run_sqlcmd_query \
        -b \
        -f 65001 \
        -i "${sql_file}" \
        -v MSSQL_DB="${MSSQL_DB}" APP_DB_USER="${APP_DB_USER}" APP_DB_PASSWORD="${APP_DB_PASSWORD}" MSSQL_COLLATION="${MSSQL_COLLATION}"
done
