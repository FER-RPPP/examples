﻿using CommandQueryCore;
using System.Collections.Generic;

namespace Contract.Queries
{
  public class SearchCitiesQuery : IQuery<IEnumerable<DTOs.City>>
  {
    public int? PostalCode { get; set; }
    public string CountryCode { get; set; }
    public string CityName { get; set; }
  }
}
