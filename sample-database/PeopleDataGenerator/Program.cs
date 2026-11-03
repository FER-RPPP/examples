using System.Text;
using Bogus;
using Bogus.Extensions;

const int peoplePerCountry = 50;
var generatorDirectory = GetGeneratorDirectory();

Console.OutputEncoding = Encoding.UTF8;

var targetDatabase = PromptForTargetDatabase(args);
var defaultOutputPath = GetDefaultPeopleOutputPath(generatorDirectory, targetDatabase);
var outputPath = PromptForPath("Enter people SQL output", defaultOutputPath);
var fullOutputPath = Path.GetFullPath(outputPath, Environment.CurrentDirectory);

var insertStatements = BuildPersonInsertStatements(targetDatabase);
var fileContent = BuildOutputContent(targetDatabase, insertStatements);

Directory.CreateDirectory(Path.GetDirectoryName(fullOutputPath)!);
await File.WriteAllTextAsync(fullOutputPath, fileContent, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

Console.WriteLine($"Target database: {targetDatabase.DisplayName()}");
Console.WriteLine($"SQL script saved to: {fullOutputPath}");

static DatabaseTarget PromptForTargetDatabase(string[] args)
{
    if (args.Length > 0)
    {
        return ParseTargetDatabase(args[0]);
    }

    Console.Write("Generate schema for [postgres/mssql] (default: mssql): ");
    var input = Console.ReadLine();

    return string.IsNullOrWhiteSpace(input) ? DatabaseTarget.MsSql : ParseTargetDatabase(input.Trim());
}

static DatabaseTarget ParseTargetDatabase(string value)
{
    return value.Trim().ToLowerInvariant() switch
    {
        "postgres" or "postgresql" or "pg" => DatabaseTarget.Postgres,
        "mssql" or "sqlserver" or "sql-server" or "sql" => DatabaseTarget.MsSql,
        _ => throw new ArgumentException($"Unsupported target database '{value}'. Use 'postgres' or 'mssql'.")
    };
}

static string GetDefaultPeopleOutputPath(string generatorDirectory, DatabaseTarget targetDatabase) =>
    targetDatabase == DatabaseTarget.Postgres
        ? Path.GetFullPath(@"..\postgres-eventsdb\init\06-people.sql", generatorDirectory)
        : Path.GetFullPath(@"..\mssql-eventsdb\init\06-people.sql", generatorDirectory);

static string GetGeneratorDirectory()
{
    var currentDirectory = new DirectoryInfo(AppContext.BaseDirectory);

    while (currentDirectory is not null)
    {
        if (File.Exists(Path.Combine(currentDirectory.FullName, "PeopleDataGenerator.csproj")))
        {
            return currentDirectory.FullName;
        }

        currentDirectory = currentDirectory.Parent;
    }

    throw new InvalidOperationException("Could not locate the PeopleDataGenerator project directory.");
}

static string PromptForPath(string prompt, string defaultPath)
{
    Console.Write($"{prompt} [{defaultPath}]: ");
    var input = Console.ReadLine();

    return string.IsNullOrWhiteSpace(input) ? defaultPath : input.Trim();
}

static string BuildOutputContent(DatabaseTarget targetDatabase, IReadOnlyList<string> insertStatements)
{
    var content = string.Join("\n", insertStatements) + "\n";

    if (targetDatabase == DatabaseTarget.MsSql)
    {
        return $"USE [$(MSSQL_DB)];\nGO\n\n{content}";
    }

    return content;
}

static List<string> BuildPersonInsertStatements(DatabaseTarget targetDatabase)
{
    var statements = new List<string>();
    var nonLatinLocales = new HashSet<string>
    {
        "ar",
        "el",
        "fa",
        "ge",
        "ne",
        "ru",
        "uk"
    };

    var localeToCountryCode = new (string Locale, string CountryCode)[]
    {
        ("af_ZA", "ZA"),
        ("ar", "SA"),
        ("az", "AZ"),
        ("cz", "CZ"),
        ("de", "DE"),
        ("de_AT", "AT"),
        ("de_CH", "CH"),
        ("el", "GR"),
        ("en_AU", "AU"),
        ("en_CA", "CA"),
        ("en_GB", "GB"),
        ("en_IE", "IE"),
        ("en_IND", "IN"),
        ("en_NG", "NG"),
        ("en_US", "US"),
        ("es", "ES"),
        ("es_MX", "MX"),
        ("fa", "IR"),
        ("fi", "FI"),
        ("fr", "FR"),
        ("ge", "GE"),
        ("hr", "HR"),
        ("id_ID", "ID"),
        ("it", "IT"),
        ("lv", "LV"),
        ("nb_NO", "NO"),
        ("ne", "NP"),
        ("nl", "NL"),
        ("nl_BE", "BE"),
        ("pl", "PL"),
        ("pt_BR", "BR"),
        ("pt_PT", "PT"),
        ("ro", "RO"),
        ("ru", "RU"),
        ("sk", "SK"),
        ("sv", "SE"),
        ("tr", "TR"),
        ("uk", "UA"),
        ("vi", "VN")
    };

    foreach (var (locale, countryCode) in localeToCountryCode)
    {
        var faker = new Faker(locale);
        var transliterationLanguage = locale.Split('_')[0];
        var useTranscriptionForAddress = nonLatinLocales.Contains(transliterationLanguage);
        var addressCountry = GetEnglishCountryName(countryCode);

        for (var i = 0; i < peoplePerCountry; i++)
        {
            var firstName = faker.Name.FirstName();
            var lastName = faker.Name.LastName();
            var firstNameTranscription = firstName.Transliterate(transliterationLanguage);
            var lastNameTranscription = lastName.Transliterate(transliterationLanguage);
            var city = NormalizeForStorage(faker.Address.City(), transliterationLanguage, useTranscriptionForAddress);
            var addressLine = NormalizeForStorage(faker.Address.StreetAddress(), transliterationLanguage, useTranscriptionForAddress);
            var postalCode = NormalizeForStorage(faker.Address.ZipCode(), transliterationLanguage, useTranscriptionForAddress);
            var email = faker.Internet.Email(firstNameTranscription, lastNameTranscription);
            var contactPhone = NormalizePhoneNumber(faker.Phone.PhoneNumber());
            var birthDate = faker.Date.BetweenDateOnly(new DateOnly(1950, 1, 1), new DateOnly(2010, 12, 31));
            var documentNumber = $"{countryCode}-{faker.Random.Replace("########")}";

            statements.Add(CreateInsertStatement(
                targetDatabase,
                firstName,
                lastName,
                firstNameTranscription,
                lastNameTranscription,
                addressLine,
                postalCode,
                city,
                addressCountry,
                email,
                contactPhone,
                birthDate,
                documentNumber,
                countryCode));
        }
    }

    return statements;
}

static string CreateInsertStatement(
    DatabaseTarget targetDatabase,
    string firstName,
    string lastName,
    string firstNameTranscription,
    string lastNameTranscription,
    string addressLine,
    string postalCode,
    string city,
    string addressCountry,
    string email,
    string contactPhone,
    DateOnly birthDate,
    string documentNumber,
    string countryCode)
{
    var tableName = targetDatabase == DatabaseTarget.Postgres ? "person" : "dbo.Person";
    var columnList = targetDatabase == DatabaseTarget.Postgres
        ? "(first_name, last_name, first_name_transcription, last_name_transcription, address_line, postal_code, city, address_country, email, contact_phone, birth_date, document_number, country_code)"
        : "(FirstName, LastName, FirstNameTranscription, LastNameTranscription, AddressLine, PostalCode, City, AddressCountry, Email, ContactPhone, BirthDate, DocumentNumber, CountryCode)";

    return $"INSERT INTO {tableName} {columnList} VALUES ('{EscapeSql(firstName)}', '{EscapeSql(lastName)}', '{EscapeSql(firstNameTranscription)}', '{EscapeSql(lastNameTranscription)}', '{EscapeSql(addressLine)}', '{EscapeSql(postalCode)}', '{EscapeSql(city)}', '{EscapeSql(addressCountry)}', '{EscapeSql(email)}', '{EscapeSql(contactPhone)}', '{birthDate:yyyy-MM-dd}', '{EscapeSql(documentNumber)}', '{countryCode}');";
}

static string EscapeSql(string value) => value.Replace("'", "''");

static string GetEnglishCountryName(string countryCode) =>
    new System.Globalization.RegionInfo(countryCode).EnglishName;

static string NormalizeForStorage(string value, string transliterationLanguage, bool useTranscriptionForAddress) =>
    useTranscriptionForAddress ? value.Transliterate(transliterationLanguage) : value;

static string NormalizePhoneNumber(string value) =>
    value.Replace("\r", string.Empty).Replace("\n", " ").Trim();

enum DatabaseTarget
{
    Postgres,
    MsSql
}

static class DatabaseTargetExtensions
{
    public static string DisplayName(this DatabaseTarget targetDatabase) =>
        targetDatabase == DatabaseTarget.Postgres ? "PostgreSQL" : "Microsoft SQL Server";
}
