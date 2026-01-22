using MediatR;

namespace Contract.Queries;

public record CitiesCountQuery(string? SearchText) : IRequest<int>;