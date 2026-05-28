using Bifrost.Core.Adapter;
using Bifrost.Core.Domain;
using Bifrost.Core.Domain.AcademicCenter;
using Bifrost.Core.Domain.Enum;
using Bifrost.Core.Exception.AcademicCenter;
using Bifrost.Core.Exception.AssessmentSeason;
using Bifrost.Core.Port.Repository;

namespace Bifrost.Core.Service;

public class AcademicCenterService(
    IAcademicCenterRepository academicCenterRepository,
    IAssessmentSeasonRepository assessmentSeasonRepository,
    IUserRepository userRepository) : IAcademicCenterService
{
    public async Task<AcademicCenter> CreateAcademicCenter(AcademicCenterCreateDto createDto)
    {
        var assessmentSeason = await assessmentSeasonRepository.FindById(createDto.AssessmentSeasonId);

        if (assessmentSeason is null)
            throw new AssessmentSeasonNotFoundException();

        if (await academicCenterRepository.ExistsAcademicCenterByAssessmentSeasonId(createDto.AssessmentSeasonId))
            throw new AcademicCenterPeriodAlreadyExistsException();

        if (HasDuplicateMembers(createDto.AcademicCenterMembers.Select(m => m.userId)))
            throw new AcademicCenterMemberDuplicatedException();

        var allowedStudents = await FilterAllowedStudents(
            createDto.AcademicCenterMembers.Select(m => m.userId).ToList(),
            assessmentSeason.CourseId);

        var members = createDto.AcademicCenterMembers
            .Where(m => allowedStudents.Contains(m.userId))
            .Select(m => new AcademicCenterMember { UserId = m.userId, Role = m.role })
            .ToList();

        var academicCenter = new AcademicCenter
        {
            Name = createDto.Name,
            AssessmentSeasonId = createDto.AssessmentSeasonId,
            AcademicCenterMembers = members
        };
        return await academicCenterRepository.Add(academicCenter);
    }

    public async Task<AcademicCenter> GetAcademicCenter(Guid id)
    {
        AcademicCenter? academicCenter = await academicCenterRepository.FindById(id);
        return academicCenter ?? throw new AcademicCenterNotFoundException();
    }

    public async Task<Pagination<AcademicCenter>> GetAcademicCenters(int page, int size, Guid? assessmentSeasonId)
    {
        return await academicCenterRepository.GetAcademicCenters(page, size, assessmentSeasonId);
    }

    public async Task<AcademicCenter> UpdateAcademicCenter(Guid id, AcademicCenterUpdateDto updateDto)
    {
        AcademicCenter? academicCenter = await academicCenterRepository.FindById(id);

        if (academicCenter is null) throw new AcademicCenterNotFoundException();

        if (HasDuplicateMembers(updateDto.AcademicCenterMembers.Select(m => m.userId)))
            throw new AcademicCenterMemberDuplicatedException();

        var allowedStudents = await FilterAllowedStudents(
            updateDto.AcademicCenterMembers.Select(m => m.userId).ToList(),
            academicCenter.assessmentSeason.CourseId);

        await academicCenterRepository.RemoveAcademicCenterMembers(id);

        academicCenter.Name = updateDto.Name;
        academicCenter.AcademicCenterMembers = updateDto.AcademicCenterMembers
            .Where(m => allowedStudents.Contains(m.userId))
            .Select(m => new AcademicCenterMember { UserId = m.userId, Role = m.role, AcademicCenterId = id })
            .ToList();
        return await academicCenterRepository.Update(academicCenter);
    }

    public async Task DeleteAcademicCenter(Guid id)
    {
        if (!await academicCenterRepository.IdExists(id)) throw new AcademicCenterNotFoundException();
        await academicCenterRepository.DeleteById(id);
    }

    private static bool HasDuplicateMembers(IEnumerable<Guid> userIds)
    {
        var ids = userIds.ToList();
        return ids.Count != ids.Distinct().Count();
    }

    private async Task<List<Guid>> FilterAllowedStudents(List<Guid> userIds, Guid courseId)
    {
        return (await userRepository.FindManyUsers(userIds, UserProfileEnum.Student, courseId))
            .Select(u => u.Id).ToList();
    }
}
