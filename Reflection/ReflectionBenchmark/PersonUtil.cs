using AutoMapper;
using BenchmarkDotNet.Attributes;
using Bogus;
using System.Reflection;

#nullable disable

namespace ReflectionBenchmark;

[MemoryDiagnoser]
public class PersonUtil
{
  Mapper mapper;
  Faker<PersonSource> faker;
  List<PersonSource> source;
  public record Pair(PropertyInfo DestProp, PropertyInfo SourceProp);
  List<Pair> mapping = new();

  [Params(10)]
  public int ListSize { get; set; }

  [GlobalSetup]
  public void PrepareMappingsAndData()
  {
    var config = new MapperConfiguration(cfg => cfg.CreateMap<PersonSource, PersonDest>());
    mapper = new Mapper(config);

    faker = new Faker<PersonSource>()
      .RuleFor(p => p.FirstName, f => f.Name.FirstName())
      .RuleFor(p => p.LastName, f => f.Name.LastName())
      .RuleFor(p => p.Address, f => f.Address.FullAddress())
      .RuleFor(p => p.Birthday, f => f.Date.Between(new DateTime(1900), DateTime.Now));

    source = faker.Generate(ListSize);

    Type sourceType = typeof(PersonSource);
    Type destType = typeof(PersonDest);

    PropertyInfo[] sourceProperties = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
    PropertyInfo[] destProperties = destType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
    foreach (PropertyInfo destProperty in destProperties)
    {
      PropertyInfo sourceProperty = sourceProperties.FirstOrDefault(d => d.Name == destProperty.Name);
      if (sourceProperty != null)
      {
        mapping.Add(new Pair(destProperty, sourceProperty));
      }
    }
  }

  /// <summary>
  /// Copy source list to destination list converting each object to an object of another type
  /// "Conversion" is done line by line in foreach loop
  /// </summary>
  /// <returns></returns>
  [Benchmark(Baseline = true)]
  public List<PersonDest> MapForeach()
  {
    List<PersonDest> dest = new List<PersonDest>(source.Count);
    foreach (var item in source)
    {
      dest.Add(new PersonDest
      {
        FirstName = item.FirstName,
        LastName = item.LastName,
        Address = item.Address,
        Birthday = item.Birthday
      });
    }
    return dest;
  }

  /// <summary>
  /// Copy source list to destination list converting each object to an object of another type
  /// It is done using Linq instead of foreach loop
  /// </summary>
  /// <returns></returns>
  [Benchmark]
  public List<PersonDest> MapLinq()
  {
    return source.Select(s => new PersonDest
    {
      FirstName = s.FirstName,
      LastName = s.LastName,
      Address = s.Address,
      Birthday = s.Birthday
    }).ToList();
  }

  /// <summary>
  /// Copy source list to destination list converting each object to an object of another type
  /// "Conversion" is done using reflection, where mapping is established in each test iteration
  /// </summary>
  /// <returns></returns>
  [Benchmark]
  public List<PersonDest> MapWithReflection()
  {
    List<PersonDest> dest = new List<PersonDest>(source.Count);
    
    foreach (var sourceItem in source)
    {
      PersonDest personDest = new ();
      foreach (var pair in mapping)
      {
        pair.DestProp.SetValue(personDest, pair.SourceProp.GetValue(sourceItem));
      }
      dest.Add(personDest);
    }
    return dest;
  }

  /// <summary>
  /// Copy source list to destination list converting each object to an object of another type
  /// "Conversion" is done with AutoMapper (library made for this purpose)
  /// </summary>
  /// <returns></returns>
  [Benchmark]
  public List<PersonDest> MapWithAutoMapper()
  {
    List<PersonDest> dest = new List<PersonDest>(source.Count);
    foreach (var item in source)
    {
      dest.Add(mapper.Map<PersonDest>(item));
    }
    return dest;
  }
}
