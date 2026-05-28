using Bifrost.Core.Domain;
using Bifrost.Core.Domain.Coordination;
using Bifrost.Core.Domain.CoordinationMember;
using Bifrost.Core.Port.Repository;
using Bifrost.Infrastructure.Persistence.Entity;
using Microsoft.EntityFrameworkCore;

namespace Bifrost.Infrastructure.Persistence.Repository;

public class CoordinationRepository(ApplicationDbContext applicationDbContext):RepositoryBase<CoordinationEntity, Coordination>(applicationDbContext), ICoordinationRepository
{
    private DbSet<CoordinationMemberEntity> CoordinationMembers { get; set; } = applicationDbContext.Set<CoordinationMemberEntity>();
    public async Task<Pagination<Coordination>> GetCoordinations(int page, int size)
    {
        int skip = (page - 1) * size;
        int total = await dbSet.CountAsync();
        CoordinationEntity[] result = await dbSet.Skip(skip).Take(size)
            .OrderByDescending(coordination =>  coordination.AssessmentSeason.Period)
            .ToArrayAsync();
        return new Pagination<Coordination>(page, size, total, result.Select(EntityToDomain).ToArray());
    }

    public async Task<Coordination?> GetCoordinationByAssessmentSeasonId(Guid assessmentSeasonId)
    {
        CoordinationEntity? coordination = await dbSet.FirstOrDefaultAsync(c =>c.AssessmentSeasonId == assessmentSeasonId);
        return  coordination is not null ? EntityToDomain(coordination) : null;
    }

    public async Task<bool> ExistsCoordinationByAssessmentSeasonId(Guid assessmentSeasonId)
    {
        return await dbSet.AnyAsync(coordination => coordination.AssessmentSeasonId == assessmentSeasonId);
    }

    public async Task RemoveCoordinationMembers(Guid coordinationId)
    {
        await CoordinationMembers.Where(cm => cm.CoordinationId == coordinationId).ExecuteDeleteAsync();
    }

    protected override Coordination EntityToDomain(CoordinationEntity entity)
    {
        return new Coordination
        {
            Id = entity.Id,
            Name =  entity.Name,
            AvgScore =  entity.AvgScore,
            AssessmentSeasonId =  entity.AssessmentSeasonId,
            CoordinationMembers = entity.CoordinationMembers.Select(cm => new CoordinationMember
            {
                Id = cm.Id,
                Role =  cm.Role,
                UserId =  cm.UserId,
                CoordinationId =  cm.CoordinationId,
                User = new () {
                    Id = cm.User.Id, 
                    Email = cm.User.Email, 
                    FullName = cm.User.FullName, 
                    CourseId = cm.User.CourseId, 
                    IsAdmin = cm.User.IsAdmin, 
                    GoogleOpenid = cm.User.GoogleOpenid,
                    Profile =  cm.User.Profile,
                },
            }).ToList(),
            assessmentSeason = new ()
            {
                Id =  entity.AssessmentSeason.Id, 
                Period = entity.AssessmentSeason.Period,
                CourseId =  entity.AssessmentSeason.CourseId,
                StartDateTime =  entity.AssessmentSeason.StartDateTime,
                EndDateTime =  entity.AssessmentSeason.EndDateTime,
            },
        };
    }

    protected override CoordinationEntity DomainToEntity(Coordination domain)
    {
        return new CoordinationEntity
        {
            Id = domain.Id,
            Name = domain.Name,
            AvgScore = domain.AvgScore,
            AssessmentSeasonId = domain.AssessmentSeasonId,
            CoordinationMembers = domain.CoordinationMembers.Select(cm => new CoordinationMemberEntity()
            {
                Id = cm.Id,
                Role = cm.Role,
                UserId = cm.UserId,
                CoordinationId = cm.CoordinationId
            }).ToList()
        };
    }
}