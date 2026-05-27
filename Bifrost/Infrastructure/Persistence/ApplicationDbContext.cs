using Bifrost.Infrastructure.Persistence.Entity;
using Microsoft.EntityFrameworkCore;

namespace Bifrost.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<CourseEntity> Courses { get; set; }
    public DbSet<AssessmentSeasonEntity> AssessmentSeasons { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AssessmentSeasonEntity>()
            .Navigation(p => p.Course)
            .AutoInclude();
    }
}