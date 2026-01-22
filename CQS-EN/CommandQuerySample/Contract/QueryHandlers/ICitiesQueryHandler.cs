using CommandQueryCore;
using Contract.Queries;
using System.Collections.Generic;

namespace Contract.QueryHandlers;

public interface ICitiesQueryHandler : IQueryHandler<CitiesQuery, List<DTOs.City>>
{
}
