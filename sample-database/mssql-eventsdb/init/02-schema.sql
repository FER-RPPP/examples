USE [$(MSSQL_DB)];
GO

CREATE TABLE dbo.Country (
    Code varchar(3) NOT NULL,
    Alpha3 char(3) NOT NULL,
    Name varchar(100) NOT NULL,
    Translations nvarchar(max) NULL,
    CONSTRAINT PK_Country PRIMARY KEY CLUSTERED (Code),
    CONSTRAINT UQ_Country_Name UNIQUE (Name),
    CONSTRAINT CK_Country_Translations_Json CHECK (Translations IS NULL OR ISJSON(Translations) = 1)
);
GO

CREATE TABLE dbo.Person (
    Id int IDENTITY(1,1) NOT NULL,
    FirstName varchar(100) NULL,
    LastName varchar(100) NULL,
    FirstNameTranscription varchar(100) NOT NULL,
    LastNameTranscription varchar(100) NOT NULL,
    AddressLine varchar(200) NULL,
    PostalCode varchar(20) NULL,
    City varchar(100) NULL,
    AddressCountry varchar(100) NULL,
    Email varchar(255) NULL,
    ContactPhone varchar(50) NULL,
    BirthDate date NOT NULL,
    DocumentNumber varchar(50) NOT NULL,
    CountryCode varchar(3) NOT NULL,
    CONSTRAINT PK_Person PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT FK_Person_Country FOREIGN KEY (CountryCode) REFERENCES dbo.Country(Code),
    CONSTRAINT UQ_Person_DocumentNumber_CountryCode UNIQUE (DocumentNumber, CountryCode)
);
GO

CREATE TABLE dbo.Sport (
    Id int IDENTITY(1,1) NOT NULL,
    Name varchar(100) NOT NULL,
    CONSTRAINT PK_Sport PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UQ_Sport_Name UNIQUE (Name)
);
GO

CREATE TABLE dbo.Event (
    Id int IDENTITY(1,1) NOT NULL,
    Name varchar(150) NOT NULL,
    EventDate date NOT NULL,
    CONSTRAINT PK_Event PRIMARY KEY CLUSTERED (Id)
);
GO

CREATE TABLE dbo.Registration (
    Id int IDENTITY(1,1) NOT NULL,
    PersonId int NOT NULL,
    SportId int NOT NULL,
    EventId int NOT NULL,
    RegisteredAt datetime2 NOT NULL CONSTRAINT DF_Registration_RegisteredAt DEFAULT SYSUTCDATETIME(),
    CONSTRAINT PK_Registration PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT FK_Registration_Person FOREIGN KEY (PersonId) REFERENCES dbo.Person(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Registration_Sport FOREIGN KEY (SportId) REFERENCES dbo.Sport(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Registration_Event FOREIGN KEY (EventId) REFERENCES dbo.Event(Id) ON DELETE CASCADE,
    CONSTRAINT UQ_Registration_PersonId_SportId_EventId UNIQUE (PersonId, SportId, EventId)
);
GO
