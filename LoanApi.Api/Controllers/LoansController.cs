using LoanApi.Application.DTOs.Loans;
using LoanApi.Application.Interfaces.Services;
using LoanApi.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(LoanResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(CreateLoanRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());
        var role = User.FindFirstValue(ClaimTypes.Role) ?? Roles.User;
        var isBlocked = bool.TryParse(User.FindFirstValue("isBlocked"), out var blocked) && blocked;
        var loan = await _loanService.CreateAsync(request, userId, role, isBlocked);
        return Created($"/api/loans/{loan.Id}", loan);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());
        var role = User.FindFirstValue(ClaimTypes.Role) ?? Roles.User;
        var loans = await _loanService.GetAsync(userId, role);
        return Ok(loans);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());
        var role = User.FindFirstValue(ClaimTypes.Role) ?? Roles.User;
        var loan = await _loanService.GetByIdAsync(id, userId, role);
        return Ok(loan);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateLoanRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());
        var role = User.FindFirstValue(ClaimTypes.Role) ?? Roles.User;
        var loan = await _loanService.UpdateAsync(id, request, userId, role);
        return Ok(loan);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());
        var role = User.FindFirstValue(ClaimTypes.Role) ?? Roles.User;
        await _loanService.DeleteAsync(id, userId, role);
        return NoContent();
    }
}
