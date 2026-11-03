USE [$(MSSQL_DB)];
GO

CREATE VIEW dbo.vw_EventRegistrations
AS
SELECT
    r.Id AS RegistrationId,
    e.Id AS EventId,
    e.Name AS EventName,
    e.EventDate,
    p.Id AS PersonId,
    CONCAT(p.FirstName, ' ', p.LastName) AS PersonName,
    s.Id AS SportId,
    s.Name AS SportName,
    r.RegisteredAt
FROM dbo.Registration AS r
INNER JOIN dbo.Event AS e ON e.Id = r.EventId
INNER JOIN dbo.Person AS p ON p.Id = r.PersonId
INNER JOIN dbo.Sport AS s ON s.Id = r.SportId;
GO
