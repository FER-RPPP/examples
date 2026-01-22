using AutoMapper;
using BenchmarkDotNet.Attributes;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReflectionBenchmark.Model;

#nullable disable

namespace ReflectionBenchmark;

[MemoryDiagnoser]
public class DBAccess
{
  IMapper mapper;
  ServiceProvider serviceProvider;
  string connectionString;

  [Params(1, 10, 1000, 10000)]
  public int Top { get; set; }

  [GlobalSetup]
  public void PrepareMappings()
  {
    serviceProvider = DISetup.BuildDI();
    var config = new MapperConfiguration(cfg => cfg.CreateMap<Model.Person, PersonDest>()
                                                   .ForMember(d => d.Address,
                             opt => opt.MapFrom(e => e.PersonNavigation.ResidenceAddress)));
    
    mapper = new Mapper(config);
    connectionString = serviceProvider.GetRequiredService<IConfiguration>().GetConnectionString("Firm");
  }

  /// <summary>
  /// Load top Top people ordered by last name, and then first name
  /// storing them in PersonDest manually
  /// </summary>
  /// <returns></returns>
  [Benchmark]
  public List<PersonDest> MapManually()
  {
    using var ctx = serviceProvider.GetRequiredService<FirmContext>();
    var query = ctx.People
                   .OrderBy(p => p.LastName)
                   .ThenBy(p => p.FirstName)
                   .Take(Top)
                   .Select(p => new PersonDest
                   {
                     FirstName = p.FirstName,
                     LastName = p.LastName,
                     Address = p.PersonNavigation.ResidenceAddress
                   });                   
    
    var list = query.ToList();
    return list;
  }

  [Benchmark(Baseline = true)]
  public List<PersonDest> AdoNet()
  {
    List<PersonDest> list = new();
    using var connection = new SqlConnection(connectionString);
    using var command = connection.CreateCommand();
    command.CommandText = $"SELECT TOP {Top} FirstName, LastName, ResidenceAddress FROM Person INNER JOIN Partner ON Partner.PartnerId = Person.PersonId ORDER BY LastName, FirstName";
    command.CommandType = System.Data.CommandType.Text;
    connection.Open();
    using var reader = command.ExecuteReader();
    while(reader.Read())
    {
      list.Add(new PersonDest
      {
        FirstName = reader.GetString(0),
        LastName = reader.GetString(1),
        Address = reader.GetString(2)
      });
    }
          
    return list;
  }

  /// <summary>
  /// Load top Top people ordered by last name, and then first name
  /// storing them in PersonDest using AutoMapper
  /// </summary>
  /// <returns></returns>
  [Benchmark]
  public List<PersonDest> MapAutoMapper()
  {
    using var ctx = serviceProvider.GetRequiredService<FirmContext>();
    var query = ctx.People
                   .OrderBy(p => p.LastName)
                   .ThenBy(p => p.FirstName)
                   .Take(Top);

    IQueryable<PersonDest> projectedQuery = mapper.ProjectTo<PersonDest>(query);

    var list = projectedQuery.ToList();
    return list;
  }
}
