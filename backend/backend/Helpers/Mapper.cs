using AutoMapper;
using backend.Dto.Auth;
using backend.Dto.Budget;
using backend.Dto.Expense;
using backend.Model;
using backend.Model.Dto.Income;
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
            CreateMap<decimal, GetTotalExpensesDto>().ForMember(dest => dest.TotalExpenses, opt => opt.MapFrom(src => src));
            CreateMap<decimal, GetTotalIncomeDto>().ForMember(dest => dest.TotalIncome, opt => opt.MapFrom(src => src));
            CreateMap<Income, GetIncomeDto>().ReverseMap();
        }
    }
}
