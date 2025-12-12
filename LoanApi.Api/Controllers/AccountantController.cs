using LoanApi.Application.Interfaces.Repositories;
using LoanApi.Application.Interfaces.Services;
using LoanApi.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoanApi.Api.Controllers;

[ApiController]
[Route("api/accountant")]
[Authorize(Roles = Roles.Accountant)]
public class AccountantController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly ILoanService _loanService;

    public AccountantController(IUserRepository userRepository, ILoanService loanService)
    {
        _userRepository = userRepository;
        _loanService = loanService;
    }

    [HttpPatch("users/{id}/block")]
    public async Task<IActionResult> BlockUser(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) return NotFound();
        user.IsBlocked = true;
        await _userRepository.UpdateAsync(user);
        return Ok(new { message = "User blocked" });
    }

    [HttpPatch("loans/{id}/status")]
    public async Task<IActionResult> ChangeStatus(Guid id, [FromQuery] string status)
    {
        var loan = await _loanService.ChangeStatusAsync(id, status, Roles.Accountant);
        return Ok(loan);
    }
}
