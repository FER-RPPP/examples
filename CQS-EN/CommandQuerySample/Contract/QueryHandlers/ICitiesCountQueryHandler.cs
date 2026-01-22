using CommandQueryCore;
using Contract.Queries;

namespace Contract.QueryHandlers;

public interface ICitiesCountQueryHandler : IQueryHandler<CitiesCountQuery, int>
{
}
