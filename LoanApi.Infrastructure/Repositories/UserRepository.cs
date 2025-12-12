using LoanApi.Application.Interfaces.Repositories;
using LoanApi.Domain.Entities;
using LoanApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LoanApi.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<User?> GetByIdAsync(Guid id) => _context.Users.Include(x => x.Loans).FirstOrDefaultAsync(x => x.Id == id);
    public Task<User?> GetByUsernameAsync(string username) => _context.Users.FirstOrDefaultAsync(x => x.Username == username);
    public Task<User?> GetByEmailAsync(string email) => _context.Users.FirstOrDefaultAsync(x => x.Email == email);

    public async Task AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }
}
