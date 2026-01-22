namespace Contract.Commands;

public class UpdateCity(int cityId) : AddCity
{
  public int CityId { get; set; } = cityId;
}
