using Microsoft.EntityFrameworkCore;
using AuthService.Models;

namespace AuthService.Data;

// Database context for Auth service
public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
}
