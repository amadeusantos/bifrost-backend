using Bifrost.Infrastructure.Persistence.Entity;
using Microsoft.EntityFrameworkCore;

namespace Bifrost.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<CourseEntity> Courses { get; set; }
    public DbSet<AssessmentSeasonEntity> AssessmentSeasons { get; set; }
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<CoordinationEntity> Coordinations { get; set; }
    public DbSet<CoordinationMemberEntity> CoordinationMembers { get; set; }
    public DbSet<AcademicCenterEntity> AcademicCenters { get; set; }
    public DbSet<AcademicCenterMemberEntity> AcademicCenterMembers { get; set; }
    public DbSet<DisciplineEntity> Disciplines { get; set; }
    public DbSet<DisciplineStudentEntity> DisciplineStudents { get; set; }

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

        modelBuilder.Entity<UserEntity>()
            .Navigation(p => p.Course)
            .AutoInclude();

        modelBuilder.Entity<CoordinationMemberEntity>()
            .Navigation(p => p.User)
            .AutoInclude();

        modelBuilder.Entity<CoordinationEntity>()
            .Navigation(p => p.AssessmentSeason)
            .AutoInclude();

        modelBuilder.Entity<CoordinationEntity>()
            .Navigation(p => p.CoordinationMembers)
            .AutoInclude();

        modelBuilder.Entity<AcademicCenterMemberEntity>()
            .Navigation(p => p.User)
            .AutoInclude();

        modelBuilder.Entity<AcademicCenterEntity>()
            .Navigation(p => p.AssessmentSeason)
            .AutoInclude();

        modelBuilder.Entity<AcademicCenterEntity>()
            .Navigation(p => p.AcademicCenterMembers)
            .AutoInclude();

        modelBuilder.Entity<DisciplineStudentEntity>()
            .Navigation(p => p.User)
            .AutoInclude();

        modelBuilder.Entity<DisciplineEntity>()
            .Navigation(p => p.AssessmentSeason)
            .AutoInclude();

        modelBuilder.Entity<DisciplineEntity>()
            .Navigation(p => p.Professor)
            .AutoInclude();

        modelBuilder.Entity<DisciplineEntity>()
            .Navigation(p => p.DisciplineStudents)
            .AutoInclude();
    }
}