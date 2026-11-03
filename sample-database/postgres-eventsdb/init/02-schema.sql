-- COUNTRIES
CREATE TABLE country (
    code VARCHAR(3) PRIMARY KEY,
	alpha3 CHAR(3) NOT NULL,
    name VARCHAR(100) NOT NULL,	
	translations JSONB,
	
	UNIQUE (name)
);

-- PERSONS
CREATE TABLE person (
    id SERIAL PRIMARY KEY,
    first_name VARCHAR(100),
    last_name VARCHAR(100),
    first_name_transcription VARCHAR(100) NOT NULL,
    last_name_transcription VARCHAR(100) NOT NULL,
    address_line VARCHAR(200),
    postal_code VARCHAR(20),
    city VARCHAR(100),
    address_country VARCHAR(100),
    email VARCHAR(255),
    contact_phone VARCHAR(50),
    birth_date DATE NOT NULL,
    document_number VARCHAR(50) NOT NULL,
    country_code VARCHAR(3) NOT NULL,

    FOREIGN KEY (country_code) REFERENCES country(code),

    -- UNIQUE dokument po državi
    UNIQUE (document_number, country_code)
);

-- SPORTS
CREATE TABLE sport (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
	UNIQUE (name)
);

-- EVENTS
CREATE TABLE event (
    id SERIAL PRIMARY KEY,
    name VARCHAR(150) NOT NULL,
    event_date DATE NOT NULL
);

-- REGISTRATIONS
CREATE TABLE registration (
    id SERIAL PRIMARY KEY,
    person_id INT NOT NULL,
    sport_id INT NOT NULL,
    event_id INT NOT NULL,
    registered_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,

    FOREIGN KEY (person_id) REFERENCES person(id) ON DELETE CASCADE,
    FOREIGN KEY (sport_id) REFERENCES sport(id) ON DELETE CASCADE,
    FOREIGN KEY (event_id) REFERENCES event(id) ON DELETE CASCADE,

    UNIQUE (person_id, sport_id, event_id)
);