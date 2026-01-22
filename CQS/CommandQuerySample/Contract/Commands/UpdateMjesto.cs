namespace Contract.Commands
{
  public class UpdateMjesto(int idMjesta) : AddMjesto
  {
    public int IdMjesta { get; set; } = idMjesta;    
  }
}
