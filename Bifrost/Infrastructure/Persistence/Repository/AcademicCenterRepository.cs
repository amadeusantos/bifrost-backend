using Bifrost.Core.Domain;
using Bifrost.Core.Domain.AcademicCenter;
using Bifrost.Core.Port.Repository;
using Bifrost.Infrastructure.Persistence.Entity;
using Microsoft.EntityFrameworkCore;

namespace Bifrost.Infrastructure.Persistence.Repository;

public class AcademicCenterRepository(ApplicationDbContext applicationDbContext)
    : RepositoryBase<AcademicCenterEntity, AcademicCenter>(applicationDbContext), IAcademicCenterRepository
{
    private DbSet<AcademicCenterMemberEntity> AcademicCenterMembers { get; set; } =
        applicationDbContext.Set<AcademicCenterMemberEntity>();

    public async Task<Pagination<AcademicCenter>> GetAcademicCenters(int page, int size, Guid? assessmentSeasonId)
    {
        int skip = (page - 1) * size;
        var query = assessmentSeasonId.HasValue
            ? dbSet.Where(ac => ac.AssessmentSeasonId == assessmentSeasonId.Value)
            : dbSet;
        int total = await query.CountAsync();
        AcademicCenterEntity[] result = await query
            .OrderByDescending(ac => ac.AssessmentSeason.Period)
            .Skip(skip).Take(size)
            .ToArrayAsync();
        return new Pagination<AcademicCenter>(page, size, total, result.Select(EntityToDomain).ToArray());
    }

    public async Task<bool> ExistsAcademicCenterByAssessmentSeasonId(Guid assessmentSeasonId)
    {
        return await dbSet.AnyAsync(ac => ac.AssessmentSeasonId == assessmentSeasonId);
    }

    public async Task RemoveAcademicCenterMembers(Guid academicCenterId)
    {
        await AcademicCenterMembers.Where(m => m.AcademicCenterId == academicCenterId).ExecuteDeleteAsync();
    }

    protected override AcademicCenter EntityToDomain(AcademicCenterEntity entity)
    {
        return new AcademicCenter
        {
            Id = entity.Id,
            Name = entity.Name,
            AvgScore = entity.AvgScore,
            AssessmentSeasonId = entity.AssessmentSeasonId,
            AcademicCenterMembers = entity.AcademicCenterMembers.Select(m => new AcademicCenterMember
            {
                Id = m.Id,
                Role = m.Role,
                UserId = m.UserId,
                AcademicCenterId = m.AcademicCenterId,
                User = new()
                {
                    Id = m.User.Id,
                    Email = m.User.Email,
                    FullName = m.User.FullName,
                    CourseId = m.User.CourseId,
                    IsAdmin = m.User.IsAdmin,
                    GoogleOpenid = m.User.GoogleOpenid,
                    Profile = m.User.Profile,
                },
            }).ToList(),
            assessmentSeason = new()
            {
                Id = entity.AssessmentSeason.Id,
                Period = entity.AssessmentSeason.Period,
                CourseId = entity.AssessmentSeason.CourseId,
                StartDateTime = entity.AssessmentSeason.StartDateTime,
                EndDateTime = entity.AssessmentSeason.EndDateTime,
            },
        };
    }

    protected override AcademicCenterEntity DomainToEntity(AcademicCenter domain)
    {
        return new AcademicCenterEntity
        {
            Id = domain.Id,
            Name = domain.Name,
            AvgScore = domain.AvgScore,
            AssessmentSeasonId = domain.AssessmentSeasonId,
            AcademicCenterMembers = domain.AcademicCenterMembers.Select(m => new AcademicCenterMemberEntity
            {
                Id = m.Id,
                Role = m.Role,
                UserId = m.UserId,
                AcademicCenterId = m.AcademicCenterId,
            }).ToList()
        };
    }
}
