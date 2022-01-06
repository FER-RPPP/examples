using EFModel;
using HotChocolate;
using System.Threading.Tasks;

namespace GraphQLServer.SetupGraphQL
{
  public partial class Mutations
  {
    public async Task<Mjesto> AddCity([Service] FirmaContext ctx, MjestoInput input)
    {
      var item = new Mjesto
      {
        NazMjesta = input.NazMjesta,
        PostBrMjesta = input.PostBrMjesta,
        PostNazMjesta = input.PostNazMjesta,
        OznDrzave = input.OznDrzave
      };
      ctx.Add(item);
      await ctx.SaveChangesAsync();
      return item;
    }

    public async Task<Mjesto> UpdateCity([Service] FirmaContext ctx, int id, MjestoInput input)
    {
      var item = await ctx.Mjesto.FindAsync(id);
      if (item != null)
      {
        item.NazMjesta = input.NazMjesta;
        item.PostBrMjesta = input.PostBrMjesta;
        item.PostNazMjesta = input.PostNazMjesta;
        item.OznDrzave = input.OznDrzave;
        await ctx.SaveChangesAsync();
        return item;
      }
      else
      {
        return null;
      }      
    }

    public async Task<bool> DeleteCity([Service] FirmaContext ctx, int id)
    {
      var item = await ctx.Mjesto.FindAsync(id);
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
