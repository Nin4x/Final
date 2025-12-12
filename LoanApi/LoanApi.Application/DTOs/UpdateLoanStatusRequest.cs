using LoanApi.Domain.Enums;

namespace LoanApi.Application.DTOs;

public record UpdateLoanStatusRequest(LoanStatus Status);
