using Bifrost.Core.Domain;
using Bifrost.Core.Domain.AssessmentSeason;
using Bifrost.Core.Domain.Course;
using Bifrost.Core.Port.Repository;
using Bifrost.Infrastructure.Persistence.Entity;
using Microsoft.EntityFrameworkCore;

namespace Bifrost.Infrastructure.Persistence.Repository;

public class AssessmentSeasonRepository(ApplicationDbContext applicationDbContext) : RepositoryBase<AssessmentSeasonEntity, AssessmentSeason>(applicationDbContext), IAssessmentSeasonRepository
{
    protected override AssessmentSeason EntityToDomain(AssessmentSeasonEntity entity)
    {
        return new AssessmentSeason
        {
            Id = entity.Id, 
            Period = entity.Period, 
            StartDateTime = entity.StartDateTime, 
            CourseId = entity.CourseId, Course = new ()
            {
                Id = entity.Course.Id,  Name = entity.Course.Name, Code =  entity.Course.Code
            }
        };
    }

    protected override AssessmentSeasonEntity DomainToEntity(AssessmentSeason domain)
    {
        return new AssessmentSeasonEntity
        {
            Id = domain.Id, 
            Period = domain.Period, 
            StartDateTime = domain.StartDateTime,
            EndDateTime =  domain.EndDateTime,
            CourseId =  domain.CourseId
        };
    }

    public async Task<Pagination<AssessmentSeason>> GetAssessmentSeasons(int page, int size, Guid? courseId)
    {
        int skip = (page - 1) * size;
        IQueryable<AssessmentSeasonEntity> query = dbSet;
        if (courseId != null)
        {
            query = query.Where(a => a.CourseId == courseId);
        }
        int total = await query.CountAsync();
        var assessmentSeasons = await query.Skip(skip).Take(size).OrderByDescending(entity => entity.Period).ToArrayAsync();
        return new Pagination<AssessmentSeason>(page, size, total, assessmentSeasons.Select(EntityToDomain).ToArray());
    }

    public Task<bool> ExistsAssessmentSeasonByPeriod(string period)
    {
        return dbSet.AnyAsync(a => a.Period == period);
    }
}
