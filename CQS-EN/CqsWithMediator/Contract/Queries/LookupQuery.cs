using Contract.DTOs;
using MediatR;
using System.Collections.Generic;

namespace Contract.Queries;

public abstract class LookupQuery<V> : IRequest<IEnumerable<TextValue<V>>>
{
    
}  
