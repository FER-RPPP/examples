# Docker Definitions

This folder contains local Docker-based infrastructure definitions used by the teaching materials in this repository.

## Available Setups

- [postgres-eventsdb](sample-database/postgres-eventsdb): the main PostgreSQL database for application runs
- [mssql-eventsdb](sample-database/mssql-eventsdb): the same teaching database adapted for Microsoft SQL Server with Full-Text Search

## What The Containers Provide

- initialized PostgreSQL instances
- initialized Microsoft SQL Server instances
- schema creation through startup scripts in each `init` directory
- seed data used by the examples

## Important Files

For each PostgreSQL setup:

- `.env.example`: committed template for local container settings such as ports, usernames, and passwords
- `.env`: local copy created from `.env.example`
- `docker-compose.yml`: service definitions
- `init`: startup scripts copied into `/docker-entrypoint-initdb.d`
- `backup`: optional location for backup files

Before starting a setup for the first time, copy the example file in that folder:

```powershell
Copy-Item postgres-eventsdb\.env.example docker-definitions\postgres-eventsdb\.env
Copy-Item mssql-eventsdb\.env.example docker-definitions\mssql-eventsdb\.env
```

On Linux and macOS, make sure the shell scripts in each `init` folder are executable before starting the containers:

```bash
chmod +x postgres-eventsdb/init/*.sh
```

## Running The Main Database

For normal day-to-day startup, use:

```powershell
docker compose -f docker-definitions\postgres-eventsdb\docker-compose.yml up -d
docker compose -f docker-definitions\mssql-eventsdb\docker-compose.yml up -d
```

For `mssql-eventsdb`, use `--build` when you changed the Docker image setup, for example:

- `Dockerfile`
- files in `scripts`
- initialization logic that depends on the image contents

```bash
docker compose -f mssql-eventsdb/docker-compose.yml up -d --build
```

To stop the containers without deleting named volumes:

```bash
docker compose -f postgres-eventsdb/docker-compose.yml down
docker compose -f mssql-eventsdb/docker-compose.yml down
```

To stop the containers and also remove named volumes:

```bash
docker compose -f postgres-eventsdb/docker-compose.yml down -v
docker compose -f mssql-eventsdb/docker-compose.yml down -v
```

`-v` removes the named Docker volumes created by the setup, which also deletes the persisted database files. Use it only when you want a clean re-initialization.

The named database volumes use the `pi_` prefix to reduce the chance of accidental reuse across unrelated Docker setups.

Default services:

- PostgreSQL on port `5432`
- local PostgreSQL backup folder mounted from `postgres-eventsdb/backup` to `/backup`
- Microsoft SQL Server on port `1433`
- local SQL Server backup folder mounted from `mssql-eventsdb/backup` to `/var/opt/mssql/backup`

## Configuration Notes

- review the `.env` files before sharing or reusing the setup outside local teaching and demo environments
- application projects connect with the `sport` user created by the initialization scripts
- if you change ports or passwords in `.env`, update the matching `dotnet user-secrets` connection strings as well
- the SQL Server setup uses `Croatian_100_CI_AS_SC_UTF8` collation and has the Full-Text Search package installed, but no runtime full-text checks, catalogs, or indexes are created by default

## Troubleshooting

- If the database is not initialized as expected, remove the Docker volume and recreate the container so the init scripts run again
