
using ConsoleClient;

_ = await MjestoClient.DodajMjesto(7, "Neko mjesto s lošim poštanskim brojem", "HR");
int? id = await MjestoClient.DodajMjesto(1234, "Neko mjesto s valjanim poštanskim brojem", "HR");
if (id.HasValue)
{
  await MjestoClient.AzurirajMjesto(id.Value, 1235, "Novi naziv", "BA");
  await MjestoClient.DohvatiMjesto(id.Value);
  Console.WriteLine("ENTER za nastavak");
  Console.ReadLine();
  await MjestoClient.ObrisiMjesto(id.Value);
}