using Bifrost.Infrastructure.Persistence.Entity;
using Microsoft.EntityFrameworkCore;

namespace Bifrost.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<CourseEntity> Courses { get; set; }
    public DbSet<AssessmentSeasonEntity> AssessmentSeasons { get; set; }
    public DbSet<UserEntity> Users { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AssessmentSeasonEntity>()
            .Navigation(p => p.Course)
            .AutoInclude();
        
        modelBuilder.Entity<UserEntity>()
            .Navigation(p => p.Course)
            .AutoInclude();
        
        modelBuilder.Entity<UserEntity>()
            .Property(e => e.Profile)
            .HasConversion<string>();
    }
}