using Bifrost.Core.Adapter;
using Bifrost.Core.Domain;
using Bifrost.Core.Domain.Coordination;
using Bifrost.Core.Domain.Enum;
using Bifrost.Core.Exception.AssessmentSeason;
using Bifrost.Core.Exception.Coordination;
using Bifrost.Core.Port.Repository;

namespace Bifrost.Core.Service;

public class CoordinationService(
    ICoordinationRepository coordinationRepository,
    IAssessmentSeasonRepository assessmentSeasonRepository,
    IUserRepository userRepository) : ICoordinationService
{
    public async Task<Coordination> CreateCoordination(CoordinationCreateDto coordinationCreateDto)
    {
        var assessmentSeason = await assessmentSeasonRepository.FindById(coordinationCreateDto.AssessmentSeasonId);

        if (assessmentSeason is null)
        {
            throw new AssessmentSeasonNotFoundException();
        }
        if (await coordinationRepository.ExistsCoordinationByAssessmentSeasonId(coordinationCreateDto.AssessmentSeasonId))
            throw new CoordinationPeriodAlreadyExistsException();

        if (HasDuplicateMembers(coordinationCreateDto.CoordinationMembers.Select(cm => cm.userId)))
            throw new CoordinationMemberDuplicatedException();

        var allowedProfessors = await FilterAllowedProfessors(
            coordinationCreateDto.CoordinationMembers.Select(cm => cm.userId).ToList(),
            assessmentSeason.CourseId);

        var coordinationMembers =
            coordinationCreateDto.CoordinationMembers.Where(cm => allowedProfessors.Contains(cm.userId)).Select(
                dto => new CoordinationMember {UserId =  dto.userId, Role = dto.role}).ToList();
        
        Coordination coordination = new Coordination
        {
            Name =  coordinationCreateDto.Name, 
            AssessmentSeasonId =  coordinationCreateDto.AssessmentSeasonId, 
            CoordinationMembers = coordinationMembers
        };
        return await coordinationRepository.Add(coordination);
    }

    public async Task<Coordination> GetCoordination(Guid id)
    {
        Coordination? coordination = await coordinationRepository.FindById(id);

        return coordination ?? throw new CoordinationNotFoundException();
    }

    public async Task<Pagination<Coordination>> GetCoordinations(int page, int size)
    {
        return await coordinationRepository.GetCoordinations(page, size);
    }

    public async Task<Coordination> UpdateCoordination(Guid id, CoordinationUpdateDto coordinationUpdateDto)
    {
        Coordination? coordination = await coordinationRepository.FindById(id);
        
        if (coordination is null) throw new CoordinationNotFoundException();

        if (HasDuplicateMembers(coordinationUpdateDto.CoordinationMembers.Select(cm => cm.userId)))
            throw new CoordinationMemberDuplicatedException();

        var allowedProfessors = await FilterAllowedProfessors(
            coordinationUpdateDto.CoordinationMembers.Select(cm => cm.userId).ToList(),
            coordination.assessmentSeason.CourseId);
        
        await coordinationRepository.RemoveCoordinationMembers(id);
        
        coordination.Name = coordinationUpdateDto.Name;
        coordination.CoordinationMembers =
            coordinationUpdateDto.CoordinationMembers.Where(cm => allowedProfessors.Contains(cm.userId)).Select(dto => 
                new CoordinationMember {UserId =  dto.userId, Role = dto.role, CoordinationId = id}
            ).ToList();
        return await coordinationRepository.Update(coordination);

    }

    public async Task DeleteCoordination(Guid id)
    {
        if (!await coordinationRepository.IdExists(id)) throw new CoordinationNotFoundException();
        await coordinationRepository.DeleteById(id);
    }

    private static bool HasDuplicateMembers(IEnumerable<Guid> userIds)
    {
        var ids = userIds.ToList();
        return ids.Count != ids.Distinct().Count();
    }

    private async Task<List<Guid>> FilterAllowedProfessors(List<Guid> userIds, Guid courseId)
    {
        return (await userRepository.FindManyUsers(userIds, UserProfileEnum.Professor, courseId))
            .Select(u => u.Id).ToList();
    }
}