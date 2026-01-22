using MediatR;

namespace Contract.Commands;
public record DeleteCity(int Id) : IRequest;