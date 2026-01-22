using Contract.DTOs;
using MediatR;

namespace Contract.Queries;

public record CityQuery(int Id) : IRequest<City?>;