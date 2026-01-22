using Contract.Queries;
using DAL.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DAL.QueryHandlers;

public class CitiesQueryHandler : IRequestHandler<CitiesQuery, List<Contract.DTOs.City>>
{
  private static Dictionary<string, Expression<Func<City, object?>>> orderSelectors = new()
  {
    [nameof(Contract.DTOs.City.CityId).ToLower()] = c => c.CityId,
    [nameof(Contract.DTOs.City.CountryName).ToLower()] = c => c.CountryCodeNavigation.CountryName,
    [nameof(Contract.DTOs.City.CityName).ToLower()] = c => c.CityName,
    [nameof(Contract.DTOs.City.CountryCode).ToLower()] = c => c.CountryCode,
    [nameof(Contract.DTOs.City.PostalCode).ToLower()] = c => c.PostalCode,
    [nameof(Contract.DTOs.City.PostalName).ToLower()] = c => c.PostalName
  };

  private readonly FirmContext ctx;


  public CitiesQueryHandler(FirmContext ctx)
  {
    this.ctx = ctx;
  }

  public async Task<List<Contract.DTOs.City>> Handle(CitiesQuery query, CancellationToken cancellationToken)
  {
    var dbQuery = ctx.Cities.AsNoTracking();

    if (!string.IsNullOrWhiteSpace(query.SearchText))
    {
      dbQuery = dbQuery.Where(m => m.CityName.Contains(query.SearchText));
    }

    if (query.Sort?.ColumnOrder != null)
    {
      foreach(var sortInfo in query.Sort.ColumnOrder)
      {
        if (orderSelectors.TryGetValue(sortInfo.Key.ToLower(), out var expr))
        {
          dbQuery = sortInfo.Value == SortInfo.Order.ASCENDING ? dbQuery.OrderBy(expr) : dbQuery.OrderByDescending(expr);
        }
      }       
    }

    if (query.From.HasValue)
    {
      dbQuery = dbQuery.Skip(query.From.Value);
    }

    if (query.Count.HasValue)
    {
      dbQuery = dbQuery.Take(query.Count.Value);
    }

    var list = await dbQuery.Select(m => new Contract.DTOs.City
                            {
                              CityId = m.CityId,
                              CountryName = m.CountryCodeNavigation.CountryName,
                              CityName = m.CityName,
                              CountryCode = m.CountryCode,
                              PostalCode = m.PostalCode,
                              PostalName = m.PostalName
                            })                              
                            .ToListAsync(cancellationToken);

    return list;      
  }
}
