using AutoMapper;
using LoanApi.Application.DTOs.Auth;
using LoanApi.Application.DTOs.Loans;
using LoanApi.Application.DTOs.Users;
using LoanApi.Domain.Entities;

namespace LoanApi.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserResponse>();
        CreateMap<RegisterRequest, User>();
        CreateMap<Loan, LoanResponse>().ReverseMap();
        CreateMap<CreateLoanRequest, Loan>();
        CreateMap<UpdateLoanRequest, Loan>();
    }
}
