using MediatR;

namespace Contract.Commands
{
  public class DeleteMjesto : IRequest
  {
    public DeleteMjesto(int id)
    {
      Id = id;
    }
   
    public int Id { get; set; }
  }
}
