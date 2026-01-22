using Contract.Commands;
using DAL.Models;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DAL.CommandHandlers;

public class CitiesCommandHandler : IRequestHandler<DeleteCity>,
                                    IRequestHandler<AddCity, int>,
                                    IRequestHandler<UpdateCity>
{
  private readonly FirmContext ctx;    

  public CitiesCommandHandler(FirmContext ctx)
  {
    this.ctx = ctx;
  }   

  public async Task Handle(DeleteCity command, CancellationToken cancellationToken)
  {
    var city = await ctx.Cities.FindAsync(command.Id);
    if (city != null)
    {       
      ctx.Remove(city);
      await ctx.SaveChangesAsync(cancellationToken);
    }
    else
    {
      throw new ArgumentException($"Invalid city id: {command.Id}");
    }
  }

  public async Task<int> Handle(AddCity command, CancellationToken cancellationToken)
  {
    var city = new City
    {
      CityName = command.CityName,
      PostalCode = command.PostalCode,
      PostalName = command.PostalName,
      CountryCode = command.CountryCode
    };
    ctx.Add(city);
    await ctx.SaveChangesAsync(cancellationToken);
    return city.CityId;
  }

  public async Task Handle(UpdateCity command, CancellationToken cancellationToken)
  {
    var city = await ctx.Cities.FindAsync(new object[] { command.CityId },cancellationToken:cancellationToken);
    if (city != null)
    {
      city.CityName = command.CityName;
      city.PostalCode = command.PostalCode;
      city.PostalName = command.PostalName;
      city.CountryCode = command.CountryCode;
      await ctx.SaveChangesAsync(cancellationToken);
    }
    else 
      throw new ArgumentException($"Invalid city id: {command.CityId}");
  }
}
