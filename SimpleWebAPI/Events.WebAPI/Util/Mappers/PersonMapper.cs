using Events.EF.Models;
using Events.WebAPI.Models;
using Microsoft.EntityFrameworkCore;
using Riok.Mapperly.Abstractions;

namespace Events.WebAPI.Util.Mappers;

[Mapper]
public static partial class PersonMapper
{   
  [MapPropertyFromSource(nameof(PersonDTO.FullNameTranscription), Use = nameof(MapFullNameTranscription))]
  [MapProperty(nameof(Person.CountryCodeNavigation.Name), nameof(PersonDTO.CountryName))]
  public static partial PersonDTO PersonToPersonDTO(Person person);

  private static string MapFullNameTranscription(Person person) => person.FirstNameTranscription + " " + person.LastNameTranscription;


  // DTO -> entity (used for both Create and Update)
  // Id is never copied from the model (identity on insert, from route on update),
  // computed DTO properties and navigation properties are ignored
  [MapperIgnoreSource(nameof(PersonDTO.Id))]
  [MapperIgnoreSource(nameof(PersonDTO.CountryName))]
  [MapperIgnoreSource(nameof(PersonDTO.FullNameTranscription))]
  [MapperIgnoreSource(nameof(PersonDTO.RegistrationsCount))]
  [MapperIgnoreTarget(nameof(Person.Id))]
  [MapperIgnoreTarget(nameof(Person.CountryCodeNavigation))]
  [MapperIgnoreTarget(nameof(Person.Registrations))]
  public static partial void MapToPerson(this PersonDTO model, Person person);

  //EF Mapping for Sieve filtering and sorting
  public static IQueryable<PersonDTO> ProjectToDTO(this DbSet<Person> query) => query.AsQueryable().ProjectToDTO();
  public static partial IQueryable<PersonDTO> ProjectToDTO(this IQueryable<Person> query);
}
