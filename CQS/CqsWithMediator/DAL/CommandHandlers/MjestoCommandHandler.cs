﻿using Contract.Commands;
using DAL.Models;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DAL.CommandHandlers
{
  public class MjestoCommandHandler : IRequestHandler<DeleteMjesto>,
                                      IRequestHandler<AddMjesto, int>,
                                      IRequestHandler<UpdateMjesto>
  {
    private readonly FirmaContext ctx;    

    public MjestoCommandHandler(FirmaContext ctx)
    {
      this.ctx = ctx;
    }   

    public async Task<Unit> Handle(DeleteMjesto command, CancellationToken cancellationToken)
    {
      var drzava = await ctx.Mjesto.FindAsync(command.Id);
      if (drzava != null)
      {       
        ctx.Remove(drzava);
        await ctx.SaveChangesAsync(cancellationToken);
        return default;
      }
      else
      {
        throw new ArgumentException($"Nepostojeća oznaka mjesta: {command.Id}");
      }
    }

    public async Task<int> Handle(AddMjesto command, CancellationToken cancellationToken)
    {
      var mjesto = new Mjesto
      {
        NazMjesta = command.NazivMjesta,
        PostBrMjesta = command.PostBrojMjesta,
        PostNazMjesta = command.PostNazivMjesta,
        OznDrzave = command.OznDrzave
      };
      ctx.Add(mjesto);
      await ctx.SaveChangesAsync(cancellationToken);
      return mjesto.IdMjesta;
    }

    public async Task<Unit> Handle(UpdateMjesto command, CancellationToken cancellationToken)
    {
      var mjesto = await ctx.Mjesto.FindAsync(new object[] { command.IdMjesta },cancellationToken:cancellationToken);
      if (mjesto != null)
      {
        mjesto.NazMjesta = command.NazivMjesta;
        mjesto.PostBrMjesta = command.PostBrojMjesta;
        mjesto.PostNazMjesta = command.PostNazivMjesta;
        mjesto.OznDrzave = command.OznDrzave;
        await ctx.SaveChangesAsync(cancellationToken);
        return default;
      }
      else 
        throw new ArgumentException($"Nepostojeća oznaka mjesta: {command.IdMjesta}");
    }
  }
}
