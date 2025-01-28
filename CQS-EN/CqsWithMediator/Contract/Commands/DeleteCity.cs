using MediatR;

namespace Contract.Commands
{
  public class DeleteCity : IRequest
  {
    public DeleteCity(int id)
    {
      Id = id;
    }
   
    public int Id { get; set; }
  }
}
