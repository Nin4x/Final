using LoanApi.Api.Extensions;
using LoanApi.Application.DTOs;
using LoanApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoanApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LoansController : ControllerBase
{
    private readonly ILoanService _loanService;

    public LoansController(ILoanService loanService)
    {
        _loanService = loanService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<LoanResponse>>> GetLoans(CancellationToken cancellationToken)
    {
        var currentUserId = User.GetUserId();
        var role = User.GetUserRole();
        var loans = await _loanService.GetAllAsync(currentUserId, role, cancellationToken);
        return Ok(loans);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LoanResponse>> GetLoan(Guid id, CancellationToken cancellationToken)
    {
        var currentUserId = User.GetUserId();
        var role = User.GetUserRole();
        var loan = await _loanService.GetAsync(id, currentUserId, role, cancellationToken);
        return loan is null ? NotFound() : Ok(loan);
    }

[HttpPost]
[Authorize(Roles = "User")]
[ProducesResponseType(StatusCodes.Status201Created)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
public async Task<ActionResult<LoanResponse>> CreateLoan([FromBody] CreateLoanRequest request, CancellationToken cancellationToken)
{
    var currentUserId = User.GetUserId();
    var role = User.GetUserRole();
    var created = await _loanService.CreateAsync(currentUserId, role, request, cancellationToken);
    return CreatedAtAction(nameof(GetLoan), new { id = created.Id }, created);
}

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LoanResponse>> UpdateLoan(Guid id, [FromBody] UpdateLoanRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = User.GetUserId();
        var role = User.GetUserRole();
        var updated = await _loanService.UpdateAsync(id, currentUserId, role, request, cancellationToken);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteLoan(Guid id, CancellationToken cancellationToken)
    {
        var currentUserId = User.GetUserId();
        var role = User.GetUserRole();
        var deleted = await _loanService.DeleteAsync(id, currentUserId, role, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
