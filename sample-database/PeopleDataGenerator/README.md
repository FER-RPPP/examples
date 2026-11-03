# PeopleDataGenerator

`PeopleDataGenerator` contains the .NET helper for generating person seed SQL for both PostgreSQL and Microsoft SQL Server.

It is used as supporting material for the teaching examples in this repository and helps produce realistic multilingual seed data for the sports-events domain.

The commands below assume you are running them from inside the `PeopleDataGenerator` folder.

## What It Does

- generates fake participant data with locale-specific names and transliterations
- asks whether the target should be PostgreSQL or Microsoft SQL Server
- writes SQL insert statements to the matching `init` folder

By default, the people generator writes to:

```text
postgres-eventsdb/init/06-people.sql
mssql-eventsdb/init/06-people.sql
```

## Prerequisites

- .NET SDK 10.0

## Running

```powershell
dotnet run --project PeopleDataGenerator.csproj
```

You can also pass the target as the first argument:

```powershell
dotnet run --project PeopleDataGenerator.csproj -- postgres
dotnet run --project PeopleDataGenerator.csproj -- mssql
```

The tool asks for the target database when no argument is provided, then asks for the output path. Press `Enter` to accept the default path derived from the `PeopleDataGenerator` folder.

## Files

- [Program.cs](Program.cs): .NET generator for `06-people.sql` targeting PostgreSQL or Microsoft SQL Server
