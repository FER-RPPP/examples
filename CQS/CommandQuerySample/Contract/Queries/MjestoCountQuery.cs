using CommandQueryCore;

namespace Contract.Queries;

public record MjestoCountQuery(string? SearchText) : IQuery<int>;