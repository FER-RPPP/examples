using CommandQueryCore;
using Contract.DTOs;
using Contract.Queries;

namespace Contract.QueryHandlers;

public interface ICityQueryHandler : IQueryHandler<CityQuery, City?>
{
}
