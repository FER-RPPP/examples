using AutoMapper;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using ReflectionBenchmark.Model;

#nullable disable

namespace ReflectionBenchmark
{
  public class DBAccess
  {
    IMapper mapper;
    ServiceProvider serviceProvider;

    [Params(10)]
    public int Top { get; set; }

    [GlobalSetup]
    public void PrepareMappings()
    {
      serviceProvider = DISetup.BuildDI();
      var config = new MapperConfiguration(cfg => cfg.CreateMap<Model.Person, PersonDest>()
                                                     .ForMember(d => d.Address,
                               opt => opt.MapFrom(e => e.PersonNavigation.ResidenceAddress)));
      
      mapper = new Mapper(config);
    }

    /// <summary>
    /// Load top Top people ordered by last name, and then first name
    /// storing them in PersonDest manually
    /// </summary>
    /// <returns></returns>
    [Benchmark(Baseline = true)]
    public List<PersonDest> MapManually()
    {
      using var ctx = serviceProvider.GetRequiredService<FirmContext>();
      var query = ctx.People
                     .OrderBy(p => p.LastName)
                     .ThenBy(p => p.FirstName)
                     .Select(p => new PersonDest
                     {
                       FirstName = p.FirstName,
                       LastName = p.LastName,
                       Address = p.PersonNavigation.ResidenceAddress
                     })
                     .Take(Top);

      var list = query.ToList();
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
}
