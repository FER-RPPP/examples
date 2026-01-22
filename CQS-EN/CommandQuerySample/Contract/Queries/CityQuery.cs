using CommandQueryCore;
using Contract.DTOs;

namespace Contract.Queries;

public record CityQuery(int Id) : IQuery<City?>;
