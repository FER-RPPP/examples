CREATE VIEW vw_event_registrations AS
SELECT
    r.id AS registration_id,
    e.id AS event_id,
    e.name AS event_name,
    e.event_date,
    p.id AS person_id,
    concat_ws(' ', p.first_name, p.last_name) AS person_name,
    s.id AS sport_id,
    s.name AS sport_name,
    r.registered_at
FROM registration AS r
INNER JOIN event AS e ON e.id = r.event_id
INNER JOIN person AS p ON p.id = r.person_id
INNER JOIN sport AS s ON s.id = r.sport_id;