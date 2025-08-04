using AutoMapper;
using backend.Dto.Auth;
using backend.Dto.Budget;
using backend.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Helpers
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<ApplicationUser,  UserInfo>().ReverseMap();
            CreateMap<Account, UserInfo>().ReverseMap();
            CreateMap<Budget, GetBudgetDto>().ReverseMap();
        }
    }
}
