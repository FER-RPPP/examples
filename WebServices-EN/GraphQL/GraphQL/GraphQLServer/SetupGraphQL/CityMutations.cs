using EFModel;
using HotChocolate;
using System.Threading.Tasks;

namespace GraphQLServer.SetupGraphQL
{
  public partial class Mutations
  {
    public async Task<City> AddCity([Service] FirmContext ctx, CityInput input)
    {
      var item = new City
      {
        CityName = input.CityName,
        PostalCode = input.PostalCode,
        PostalName = input.PostalName,
        CountryCode = input.CountryCode
      };
      ctx.Add(item);
      await ctx.SaveChangesAsync();
      return item;
    }

    public async Task<City> UpdateCity([Service] FirmContext ctx, int id, CityInput input)
    {
      var item = await ctx.Cities.FindAsync(id);
      if (item != null)
      {
        item.CityName = input.CityName;
        item.PostalCode = input.PostalCode;
        item.PostalName = input.PostalName;
        item.CountryCode = input.CountryCode;
        await ctx.SaveChangesAsync();
        return item;
      }
      else
      {
        return null;
      }      
    }

    public async Task<bool> DeleteCity([Service] FirmContext ctx, int id)
    {
      var item = await ctx.Cities.FindAsync(id);
      if (item != null)
      {
        ctx.Remove(item);
        await ctx.SaveChangesAsync();
        return true;
      }
      else
      {
        return false;
      }      
    }
  }
}
