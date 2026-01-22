using CommandQueryCore;
using Contract.DTOs;

namespace Contract.Queries;

public record MjestoQuery(int Id) : IQuery<Mjesto?>;