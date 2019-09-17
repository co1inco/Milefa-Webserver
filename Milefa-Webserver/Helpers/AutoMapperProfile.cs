using AutoMapper;
using Milefa_WebServer.Models;
using Milefa_WebServer.Dtos;
using Milefa_WebServer.Entities;

namespace Milefa_WebServer.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
        }
    }
}