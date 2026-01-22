using CommandQueryCore;
using Contract.Queries;
using System.Collections.Generic;

namespace Contract.QueryHandlers;

public interface IMjestaQueryHandler : IQueryHandler<MjestaQuery, List<DTOs.Mjesto>>
{
}
