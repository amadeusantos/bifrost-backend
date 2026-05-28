using Bifrost.Core.Domain;
using Bifrost.Core.Domain.Discipline;
using Bifrost.Core.Domain.User;
using Bifrost.Core.Port.Repository;
using Bifrost.Infrastructure.Persistence.Entity;
using Microsoft.EntityFrameworkCore;

namespace Bifrost.Infrastructure.Persistence.Repository;

public class DisciplineRepository(ApplicationDbContext applicationDbContext)
    : RepositoryBase<DisciplineEntity, Discipline>(applicationDbContext), IDisciplineRepository
{
    private DbSet<DisciplineStudentEntity> DisciplineStudents { get; set; } =
        applicationDbContext.Set<DisciplineStudentEntity>();

    public async Task<Pagination<Discipline>> GetDisciplines(int page, int size, Guid? assessmentSeasonId)
    {
        int skip = (page - 1) * size;
        var query = assessmentSeasonId.HasValue
            ? dbSet.Where(d => d.AssessmentSeasonId == assessmentSeasonId.Value)
            : dbSet;
        int total = await query.CountAsync();
        DisciplineEntity[] result = await query
            .OrderBy(d => d.Code)
            .Skip(skip).Take(size)
            .ToArrayAsync();
        return new Pagination<Discipline>(page, size, total, result.Select(EntityToDomain).ToArray());
    }

    public async Task RemoveDisciplineStudents(Guid disciplineId)
    {
        await DisciplineStudents.Where(ds => ds.DisciplineId == disciplineId).ExecuteDeleteAsync();
    }

    protected override Discipline EntityToDomain(DisciplineEntity entity)
    {
        return new Discipline
        {
            Id = entity.Id,
            Name = entity.Name,
            Code = entity.Code,
            AvgScore = entity.AvgScore,
            AssessmentSeasonId = entity.AssessmentSeasonId,
            ProfessorId = entity.ProfessorId,
            assessmentSeason = new()
            {
                Id = entity.AssessmentSeason.Id,
                Period = entity.AssessmentSeason.Period,
                CourseId = entity.AssessmentSeason.CourseId,
                StartDateTime = entity.AssessmentSeason.StartDateTime,
                EndDateTime = entity.AssessmentSeason.EndDateTime,
            },
            Professor = new UserMinimal
            {
                Id = entity.Professor.Id,
                Email = entity.Professor.Email,
                FullName = entity.Professor.FullName,
                CourseId = entity.Professor.CourseId,
                IsAdmin = entity.Professor.IsAdmin,
                GoogleOpenid = entity.Professor.GoogleOpenid,
                Profile = entity.Professor.Profile,
            },
            Students = entity.DisciplineStudents.Select(ds => new UserMinimal
            {
                Id = ds.User.Id,
                Email = ds.User.Email,
                FullName = ds.User.FullName,
                CourseId = ds.User.CourseId,
                IsAdmin = ds.User.IsAdmin,
                GoogleOpenid = ds.User.GoogleOpenid,
                Profile = ds.User.Profile,
            }).ToList(),
        };
    }

    protected override DisciplineEntity DomainToEntity(Discipline domain)
    {
        return new DisciplineEntity
        {
            Id = domain.Id,
            Name = domain.Name,
            Code = domain.Code,
            AvgScore = domain.AvgScore,
            AssessmentSeasonId = domain.AssessmentSeasonId,
            ProfessorId = domain.ProfessorId,
            DisciplineStudents = domain.Students.Select(s => new DisciplineStudentEntity
            {
                DisciplineId = domain.Id,
                UserId = s.Id,
            }).ToList(),
        };
    }
}
