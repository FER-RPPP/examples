using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

#region Prepare IConfiguration object and prepare connString
IConfiguration configuration = new ConfigurationBuilder()
                                   .AddJsonFile("appsettings.json")
                                   .AddUserSecrets<Program>() //foldername defined in .csproj of the project containing class Project
                                   .Build();

string? connString = configuration.GetConnectionString("Firma");
if (connString == null)
{
  throw new Exception("Missing connection string Firma");
}
Console.WriteLine(connString);
#endregion

try
{
  using (var conn = new SqlConnection(connString))
  {
    using (var command = conn.CreateCommand())
    {
      command.CommandText = "SELECT TOP 3 * FROM Artikl ORDER BY CijArtikla DESC";
      command.Connection = conn;

      conn.Open();

      using (var reader = command.ExecuteReader())
      {
        while (reader.Read())
        {
          object productCode = reader["SifArtikla"];
          string productName = reader.GetString("NazArtikla");
          decimal price = reader.GetDecimal("CijArtikla");
          Console.WriteLine($"{productCode} - {productName} {price:C2}");
        }
      }
    }
  }
}
catch (Exception exc)
{
  Console.WriteLine(exc.Message);
  Console.WriteLine(exc.StackTrace);
}