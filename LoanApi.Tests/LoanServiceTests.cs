using AutoMapper;
using FluentAssertions;
using LoanApi.Application.DTOs.Loans;
using LoanApi.Application.Exceptions;
using LoanApi.Application.Mapping;
using LoanApi.Application.Services;
using LoanApi.Domain.Entities;
using LoanApi.Domain.Enums;
using Moq;
using LoanApi.Application.Interfaces.Repositories;

namespace LoanApi.Tests;

public class LoanServiceTests
{
    private readonly IMapper _mapper;

    public LoanServiceTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task User_cannot_update_non_processing_loan()
    {
        var loan = new Loan { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Status = LoanStatus.Approved };
        var loanRepo = new Mock<ILoanRepository>();
        var userRepo = new Mock<IUserRepository>();
        loanRepo.Setup(r => r.GetByIdAsync(loan.Id)).ReturnsAsync(loan);
        var service = new LoanService(loanRepo.Object, userRepo.Object, _mapper);

        var act = async () => await service.UpdateAsync(loan.Id, new UpdateLoanRequest { Amount = 10, Currency = "USD", LoanType = LoanType.AutoLoan, Period = 12 }, loan.UserId, Roles.User);
        await act.Should().ThrowAsync<AppException>();
    }

    [Fact]
    public async Task Accountant_can_change_status()
    {
        var loan = new Loan { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Status = LoanStatus.Processing };
        var loanRepo = new Mock<ILoanRepository>();
        var userRepo = new Mock<IUserRepository>();
        loanRepo.Setup(r => r.GetByIdAsync(loan.Id)).ReturnsAsync(loan);
        loanRepo.Setup(r => r.UpdateAsync(loan)).Returns(Task.CompletedTask);
        var service = new LoanService(loanRepo.Object, userRepo.Object, _mapper);

        var result = await service.ChangeStatusAsync(loan.Id, "approved", Roles.Accountant);
        result.Status.Should().Be(LoanStatus.Approved);
    }
}
