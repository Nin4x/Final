using AutoMapper;
using LoanApi.Application.DTOs;
using LoanApi.Domain.Entities;
using LoanApi.Domain.Enums;

namespace LoanApi.Application.Mapping;

public class ApplicationMappingProfile : Profile
{
    public ApplicationMappingProfile()
    {
        CreateMap<User, UserResponse>();

        CreateMap<Loan, LoanResponse>();

        CreateMap<CreateLoanRequest, Loan>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => LoanStatus.Processing))
            .ForMember(dest => dest.CreatedOnUtc, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedOnUtc, opt => opt.Ignore())
            .ForMember(dest => dest.BorrowerName, opt => opt.MapFrom(src => src.BorrowerName.Trim()));

        CreateMap<UpdateLoanRequest, Loan>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedOnUtc, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedOnUtc, opt => opt.Ignore())
            .ForMember(dest => dest.BorrowerName, opt => opt.MapFrom(src => src.BorrowerName.Trim()));
    }
}
