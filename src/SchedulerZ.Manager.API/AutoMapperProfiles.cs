using AutoMapper;
using SchedulerZ.Manager.API.Model.Dto;
using SchedulerZ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchedulerZ.Manager.API
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap();
        }



        public void CreateMap()
        {
            CreateMap<User, UserDto>().ReverseMap().ForMember(d => d.Password, s => s.Ignore());

            CreateMap<Role, RoleDto>().ReverseMap();

            CreateMap<Router, RouterDto>().ReverseMap();
        }
    }
}
