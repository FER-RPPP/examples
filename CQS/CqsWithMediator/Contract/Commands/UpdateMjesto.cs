﻿using MediatR;

namespace Contract.Commands
{
  public class UpdateMjesto : IRequest
  {
    public int IdMjesta { get; set; }
    public string NazivMjesta { get; set; }

    public int PostBrojMjesta { get; set; }

    public string OznDrzave { get; set; }
    public string PostNazivMjesta { get; set; }
  }
}
