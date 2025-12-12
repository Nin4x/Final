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
        var loans = await _loanService.GetAllAsync(cancellationToken);
        return Ok(loans);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LoanResponse>> GetLoan(Guid id, CancellationToken cancellationToken)
    {
        var loan = await _loanService.GetAsync(id, cancellationToken);
        return loan is null ? NotFound() : Ok(loan);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LoanResponse>> CreateLoan([FromBody] CreateLoanRequest request, CancellationToken cancellationToken)
    {
        var created = await _loanService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetLoan), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LoanResponse>> UpdateLoan(Guid id, [FromBody] UpdateLoanRequest request, CancellationToken cancellationToken)
    {
        var updated = await _loanService.UpdateAsync(id, request, cancellationToken);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpPut("{id:guid}/status")]
    [Authorize(Roles = "Accountant")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LoanResponse>> UpdateStatus(Guid id, [FromBody] UpdateLoanStatusRequest request, CancellationToken cancellationToken)
    {
        var updated = await _loanService.UpdateStatusAsync(id, request, cancellationToken);
        return updated is null ? NotFound() : Ok(updated);
    }
}
