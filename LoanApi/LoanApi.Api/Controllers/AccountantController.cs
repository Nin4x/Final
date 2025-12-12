using LoanApi.Api.Extensions;
using LoanApi.Application.DTOs;
using LoanApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoanApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Accountant")]
public class AccountantController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILoanService _loanService;

    public AccountantController(IUserService userService, ILoanService loanService)
    {
        _userService = userService;
        _loanService = loanService;
    }

    [HttpPatch("users/{id:guid}/block")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> BlockUser(Guid id, [FromBody] BlockUserRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = User.GetUserId();
        var role = User.GetUserRole();

        var user = await _userService.SetBlockStatusAsync(id, request.IsBlocked, currentUserId, role, cancellationToken);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPatch("loans/{id:guid}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LoanResponse>> UpdateLoanStatus(Guid id, [FromBody] UpdateLoanStatusRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = User.GetUserId();
        var role = User.GetUserRole();

        var loan = await _loanService.UpdateStatusAsync(id, currentUserId, role, request, cancellationToken);
        return loan is null ? NotFound() : Ok(loan);
    }
}
