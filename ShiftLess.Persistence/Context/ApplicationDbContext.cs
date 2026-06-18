using Microsoft.EntityFrameworkCore;
using ShiftLess.Domain.Entities;

namespace ShiftLess.Persistence.Context;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TaskRequest>()
            .Property(x => x.Budget)
            .HasPrecision(18, 2);

        modelBuilder.Entity<User>()
            .HasIndex(x => x.Email)
            .IsUnique();


    }

    public DbSet<User> Users => Set<User>();

    public DbSet<TaskRequest> TaskRequests => Set<TaskRequest>();

    public DbSet<TaskApplication> TaskApplications => Set<TaskApplication>();

    public DbSet<Assignment> Assignments => Set<Assignment>();
}