using CommandQueryCore;
using Contract.Commands;
using DAL.Models;
using System;
using System.Threading.Tasks;

namespace DAL.CommandHandlers
{
  public class CityCommandHandlers : ICommandHandler<DeleteCity>, 
                                     ICommandHandler<AddCity, int>, 
                                     ICommandHandler<UpdateCity>
  {
    private readonly FirmContext ctx;    

    public CityCommandHandlers(FirmContext ctx)
    {
      this.ctx = ctx;
    }   

    public async Task Handle(DeleteCity command)
    {
      var city = await ctx.Cities.FindAsync(command.Id);
      if (city != null)
      {       
        ctx.Remove(city);
        await ctx.SaveChangesAsync();        
      }
      else
      {
        throw new ArgumentException($"Invalid city id: {command.Id}");
      }
    }

    public async Task<int> Handle(AddCity command)
    {
      var city = new City
      {
        CityName = command.CityName,
        PostalCode = command.PostalCode,
        PostalName = command.PostalName,
        CountryCode = command.CountryCode
      };
      ctx.Add(city);
      await ctx.SaveChangesAsync();
      return city.CityId;
    }

    public async Task Handle(UpdateCity command)
    {
      var city = await ctx.Cities.FindAsync(command.CityId);
      if (city != null)
      {
        city.CityName = command.CityName;
        city.PostalCode = command.PostalCode;
        city.PostalName = command.PostalName;
        city.CountryCode = command.CountryCode;
        await ctx.SaveChangesAsync();
      }
      else 
        throw new ArgumentException($"Invalid city id: {command.CityId}");
    }
  }
}
