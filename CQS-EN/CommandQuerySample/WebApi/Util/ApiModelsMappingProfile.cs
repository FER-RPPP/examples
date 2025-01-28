using AutoMapper;
using Contract.Commands;
using Contract.DTOs;

namespace WebApi.Util
{
  public class ApiModelsMappingProfile : Profile
  {
    public ApiModelsMappingProfile()
    {     
      CreateMap<City, AddCity>(); //DTO for clients -> Command class
      CreateMap<City, UpdateCity>();
    }
  }
}
