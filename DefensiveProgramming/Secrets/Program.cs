using Microsoft.Extensions.Configuration;
using Secrets;

var enumerator = Environment.GetEnvironmentVariables().GetEnumerator();
while (enumerator.MoveNext())
{
  Console.WriteLine($"{enumerator.Key} = {enumerator.Value}");
}
Console.WriteLine("----------------------------");
IConfiguration configuration = new ConfigurationBuilder()
                                   .AddJsonFile("appsettings.json")   //order is important!
                                   .AddJsonFile("missing_one.json", optional: true)   //
                                   .AddEnvironmentVariables()    // include Package Microsoft.Extensions.Configuration.EnvironmentVariables                                     
                                   .AddUserSecrets("RPPP-Secrets") //package Microsoft.Extensions.Configuration.UserSecrets
                                   .Build();

Console.WriteLine($"Custom env variable using configuration = " + configuration["CustomEnvValue"]);
Console.WriteLine($"Name = " + configuration["Name"]);
Console.WriteLine($"Key0 = " + configuration["Demo:Key0"]);

Demo? demo = configuration.GetSection("Demo").Get<Demo>(); // package Microsoft.Extensions.Configuration.Binder
Console.WriteLine($"Key1 = " + demo?.Key1);
Console.WriteLine($"Key2 = " + demo?.Key2);