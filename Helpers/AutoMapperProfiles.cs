using aaaNew.Dtos;
using aaaNew.Models;
using AutoMapper;

namespace aaaNew.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForListDto>();
            CreateMap<User, UserForDetailedDto>();
        }
    }
}