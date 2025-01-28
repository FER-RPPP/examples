namespace Contract.Commands
{
  public class DeleteCity
  {
    public DeleteCity(int id)
    {
      Id = id;
    }
   
    public int Id { get; set; }
  }
}
