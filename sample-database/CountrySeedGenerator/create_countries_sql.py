from __future__ import annotations

from pathlib import Path
import json
import sys

import pycountry
from babel import Locale


SCRIPT_DIR = Path(__file__).resolve().parent
DOCKER_DEFINITIONS_DIR = SCRIPT_DIR.parent
MANUAL_COUNTRIES = [
    {
        "alpha_2": "XK",
        "alpha_3": "XKX",
        "name": "Kosovo",
        "translations": {
            "hr": "Kosovo",
            "mk": "Косово",
        },
    }
]


def prompt_target_database(argv: list[str]) -> str:
    if len(argv) > 1:
        return parse_target_database(argv[1])

    raw_value = input("Generate schema for [postgres/mssql] (default: mssql): ").strip()
    return "mssql" if not raw_value else parse_target_database(raw_value)


def parse_target_database(value: str) -> str:
    normalized = value.strip().lower()

    if normalized in {"postgres", "postgresql", "pg"}:
        return "postgres"
    if normalized in {"mssql", "sqlserver", "sql-server", "sql"}:
        return "mssql"

    raise ValueError(f"Unsupported target database '{value}'. Use 'postgres' or 'mssql'.")


def get_output_path(target_database: str) -> Path:
    target_folder = "postgres-eventsdb" if target_database == "postgres" else "mssql-eventsdb"
    return DOCKER_DEFINITIONS_DIR / target_folder / "init" / "04-countries.sql"


def get_insert_target(target_database: str) -> tuple[str, str]:
    if target_database == "postgres":
        return "country", "(code, alpha3, name, translations)"

    return "dbo.Country", "(Code, Alpha3, Name, Translations)"


def get_script_prefix(target_database: str) -> str:
    if target_database == "mssql":
        return "USE [$(MSSQL_DB)];\nGO\n\n"

    return ""


def confirm_output_path(output_path: Path) -> Path:
    print(f"Output path: {output_path}")
    raw_value = input("Press Enter to confirm or type a different path: ").strip()

    if not raw_value:
        return output_path

    custom_path = Path(raw_value)
    return custom_path if custom_path.is_absolute() else (SCRIPT_DIR / custom_path).resolve()


def generate_minimal_sql(target_database: str) -> None:
    default_output_path = get_output_path(target_database)
    output_path = confirm_output_path(default_output_path)
    table_name, column_list = get_insert_target(target_database)
    script_prefix = get_script_prefix(target_database)
    sql_header = f"INSERT INTO {table_name} {column_list} VALUES\n"

    print(f"Generating SQL for {target_database} countries script...")

    try:
        lang_hr = Locale("hr")
        lang_mk = Locale("mk")
    except Exception as exc:
        print(f"Babel error: {exc}. Check whether you installed the 'Babel' package.")
        return

    rows: list[str] = []
    countries = sorted(pycountry.countries, key=lambda country: country.alpha_2)

    for country in countries:
        code = country.alpha_2
        alpha3 = country.alpha_3
        english_name = getattr(country, "common_name", country.name)

        croatian_name = lang_hr.territories.get(code, english_name)
        macedonian_name = lang_mk.territories.get(code, english_name)

        s_en = english_name.replace("'", "''")
        s_hr = croatian_name.replace("'", "''")
        s_mk = macedonian_name.replace("'", "''")

        trans_dict = {
            "hr": s_hr,
            "mk": s_mk,
        }

        trans_json = json.dumps(trans_dict, ensure_ascii=False).replace("'", "''")
        rows.append(f"('{code}', '{alpha3}', '{s_en}', '{trans_json}')")

    for country in MANUAL_COUNTRIES:
        code = country["alpha_2"]
        alpha3 = country["alpha_3"]
        english_name = country["name"]
        croatian_name = country["translations"]["hr"]
        macedonian_name = country["translations"]["mk"]

        s_en = english_name.replace("'", "''")
        s_hr = croatian_name.replace("'", "''")
        s_mk = macedonian_name.replace("'", "''")

        trans_dict = {
            "hr": s_hr,
            "mk": s_mk,
        }

        trans_json = json.dumps(trans_dict, ensure_ascii=False).replace("'", "''")
        rows.append(f"('{code}', '{alpha3}', '{s_en}', '{trans_json}')")

    rows.sort()

    final_sql = script_prefix + sql_header + ",\n".join(rows) + ";"

    output_path.parent.mkdir(parents=True, exist_ok=True)
    output_path.write_text(final_sql, encoding="utf-8")
    print(f"Success! Generated {len(rows)} countries in '{output_path}'.")


if __name__ == "__main__":
    generate_minimal_sql(prompt_target_database(sys.argv))
