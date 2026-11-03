# CountrySeedGenerator

This folder contains the Python helper for generating `04-countries.sql` seed scripts for PostgreSQL and Microsoft SQL Server 
(defined as Docker containers using docker-compose.yml)

The commands below assume you are running them from inside the `CountrySeedGenerator` folder.

Install dependencies:

```powershell
python -m pip install -r requirements.txt
```

Run the generator:

```powershell
python create_countries_sql.py postgres
python create_countries_sql.py mssql
```

The script shows the default output path based on the selected target and asks you to confirm it before writing the file.
