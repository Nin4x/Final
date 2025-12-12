using LoanApi.Application.DTOs;
using LoanApi.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LoanApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoansController : ControllerBase
{
    private readonly ILoanService _loanService;

    public LoansController(ILoanService loanService)
    {
        _loanService = loanService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<LoanDto>>> GetLoans(CancellationToken cancellationToken)
    {
        var loans = await _loanService.GetAllAsync(cancellationToken);
        return Ok(loans);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LoanDto>> GetLoan(Guid id, CancellationToken cancellationToken)
    {
        var loan = await _loanService.GetAsync(id, cancellationToken);
        return loan is null ? NotFound() : Ok(loan);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LoanDto>> CreateLoan([FromBody] CreateLoanRequest request, CancellationToken cancellationToken)
    {
        var created = await _loanService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetLoan), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LoanDto>> UpdateStatus(Guid id, [FromBody] UpdateLoanStatusRequest request, CancellationToken cancellationToken)
    {
        var updated = await _loanService.UpdateStatusAsync(id, request, cancellationToken);
        return updated is null ? NotFound() : Ok(updated);
    }
}
