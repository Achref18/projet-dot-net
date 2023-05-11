using HelpDesk.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Data;

public class ApplicationDbContext : IdentityDbContext<User>
{
    public DbSet<Complaint> Complaints { get; set; }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.Entity<Complaint>()
            .HasOne(c => c.User)
            .WithMany(u => u.Complaints)
            .OnDelete(DeleteBehavior.Cascade);
    }

    
    
}