using CommandQueryCore;
using Contract.DTOs;
using Contract.Queries;
using System.Collections.Generic;

namespace Contract.QueryHandlers
{
  public interface ICountriesLookupQueryHandler : IQueryHandler<CountriesLookupQuery, IEnumerable<TextValue<string>>>
  {
  }
}
