using CommandQueryCore;

namespace Contract.Queries;

public record CitiesCountQuery(string? SearchText) : IQuery<int>;
