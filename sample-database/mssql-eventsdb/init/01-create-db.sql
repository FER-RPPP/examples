IF DB_ID(N'$(MSSQL_DB)') IS NULL
BEGIN
    DECLARE @createDatabase nvarchar(max) =
        N'CREATE DATABASE [' + REPLACE(N'$(MSSQL_DB)', N']', N']]') + N'] COLLATE $(MSSQL_COLLATION);';

    EXEC (@createDatabase);
END;
GO

USE [$(MSSQL_DB)];
GO

IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = N'$(APP_DB_USER)')
BEGIN
    DECLARE @createLogin nvarchar(max) =
        N'CREATE LOGIN [' + REPLACE(N'$(APP_DB_USER)', N']', N']]') + N'] WITH PASSWORD = N''$(APP_DB_PASSWORD)'', CHECK_POLICY = OFF, CHECK_EXPIRATION = OFF;';

    EXEC (@createLogin);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N'$(APP_DB_USER)')
BEGIN
    DECLARE @createUser nvarchar(max) =
        N'CREATE USER [' + REPLACE(N'$(APP_DB_USER)', N']', N']]') + N'] FOR LOGIN [' + REPLACE(N'$(APP_DB_USER)', N']', N']]') + N'];';

    EXEC (@createUser);
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.database_role_members drm
    INNER JOIN sys.database_principals role_principal ON role_principal.principal_id = drm.role_principal_id
    INNER JOIN sys.database_principals member_principal ON member_principal.principal_id = drm.member_principal_id
    WHERE role_principal.name = N'db_datareader'
      AND member_principal.name = N'$(APP_DB_USER)'
)
BEGIN
    ALTER ROLE db_datareader ADD MEMBER [$(APP_DB_USER)];
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.database_role_members drm
    INNER JOIN sys.database_principals role_principal ON role_principal.principal_id = drm.role_principal_id
    INNER JOIN sys.database_principals member_principal ON member_principal.principal_id = drm.member_principal_id
    WHERE role_principal.name = N'db_datawriter'
      AND member_principal.name = N'$(APP_DB_USER)'
)
BEGIN
    ALTER ROLE db_datawriter ADD MEMBER [$(APP_DB_USER)];
END;
GO