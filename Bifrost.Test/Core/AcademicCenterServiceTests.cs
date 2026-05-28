using Bifrost.Core.Domain;
using Bifrost.Core.Domain.AcademicCenter;
using Bifrost.Core.Domain.AssessmentSeason;
using Bifrost.Core.Domain.Enum;
using Bifrost.Core.Domain.User;
using Bifrost.Core.Exception.AcademicCenter;
using Bifrost.Core.Exception.AssessmentSeason;
using Bifrost.Core.Port.Repository;
using Bifrost.Core.Service;
using FluentAssertions;
using NSubstitute;

namespace Bifrost.Test.Core;

public class AcademicCenterServiceTests
{
    private readonly IAcademicCenterRepository _academicCenterRepository = Substitute.For<IAcademicCenterRepository>();
    private readonly IAssessmentSeasonRepository _assessmentSeasonRepository = Substitute.For<IAssessmentSeasonRepository>();
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly AcademicCenterService _sut;

    public AcademicCenterServiceTests()
    {
        _sut = new AcademicCenterService(_academicCenterRepository, _assessmentSeasonRepository, _userRepository);
    }

    // --- Create ---

    [Fact]
    public async Task CreateAcademicCenter_WhenAssessmentSeasonNotFound_ThrowsAssessmentSeasonNotFoundException()
    {
        var dto = new AcademicCenterCreateDto("Center A", Guid.NewGuid(), []);
        _assessmentSeasonRepository.FindById(dto.AssessmentSeasonId).Returns((AssessmentSeason?)null);

        var act = () => _sut.CreateAcademicCenter(dto);

        await act.Should().ThrowAsync<AssessmentSeasonNotFoundException>();
    }

    [Fact]
    public async Task CreateAcademicCenter_WhenAcademicCenterAlreadyExistsForSeason_ThrowsAcademicCenterPeriodAlreadyExistsException()
    {
        var seasonId = Guid.NewGuid();
        var season = new AssessmentSeason { Period = "2025.1", CourseId = Guid.NewGuid() };
        var dto = new AcademicCenterCreateDto("Center A", seasonId, []);

        _assessmentSeasonRepository.FindById(seasonId).Returns(season);
        _academicCenterRepository.ExistsAcademicCenterByAssessmentSeasonId(seasonId).Returns(true);

        var act = () => _sut.CreateAcademicCenter(dto);

        await act.Should().ThrowAsync<AcademicCenterPeriodAlreadyExistsException>();
    }

    [Fact]
    public async Task CreateAcademicCenter_WhenMembersHaveDuplicateUsers_ThrowsAcademicCenterMemberDuplicatedException()
    {
        var seasonId = Guid.NewGuid();
        var season = new AssessmentSeason { Period = "2025.1", CourseId = Guid.NewGuid() };
        var duplicateId = Guid.NewGuid();
        var members = new List<AcademicCenterMemberDto>
        {
            new("president", duplicateId),
            new("member", duplicateId)
        };
        var dto = new AcademicCenterCreateDto("Center A", seasonId, members);

        _assessmentSeasonRepository.FindById(seasonId).Returns(season);
        _academicCenterRepository.ExistsAcademicCenterByAssessmentSeasonId(seasonId).Returns(false);

        var act = () => _sut.CreateAcademicCenter(dto);

        await act.Should().ThrowAsync<AcademicCenterMemberDuplicatedException>();
    }

    [Fact]
    public async Task CreateAcademicCenter_WhenMembersNotAllowedStudents_ExcludesThemFromAcademicCenter()
    {
        var courseId = Guid.NewGuid();
        var seasonId = Guid.NewGuid();
        var season = new AssessmentSeason { Period = "2025.1", CourseId = courseId };

        var allowedId = Guid.NewGuid();
        var notAllowedId = Guid.NewGuid();
        var members = new List<AcademicCenterMemberDto>
        {
            new("president", allowedId),
            new("member", notAllowedId)
        };
        var dto = new AcademicCenterCreateDto("Center A", seasonId, members);
        var saved = new AcademicCenter { Id = Guid.NewGuid(), Name = dto.Name, AssessmentSeasonId = seasonId, AcademicCenterMembers = [] };

        _assessmentSeasonRepository.FindById(seasonId).Returns(season);
        _academicCenterRepository.ExistsAcademicCenterByAssessmentSeasonId(seasonId).Returns(false);
        _userRepository.FindManyUsers(Arg.Any<List<Guid>>(), UserProfileEnum.Student, courseId)
            .Returns([new User { Id = allowedId, Email = "student@test.com" }]);
        _academicCenterRepository.Add(Arg.Any<AcademicCenter>()).Returns(callInfo =>
        {
            var ac = callInfo.Arg<AcademicCenter>();
            ac.AcademicCenterMembers.Should().ContainSingle(m => m.UserId == allowedId);
            ac.AcademicCenterMembers.Should().NotContain(m => m.UserId == notAllowedId);
            return saved;
        });

        await _sut.CreateAcademicCenter(dto);

        await _academicCenterRepository.Received(1).Add(Arg.Any<AcademicCenter>());
    }

    [Fact]
    public async Task CreateAcademicCenter_WhenValid_ReturnsSavedAcademicCenter()
    {
        var courseId = Guid.NewGuid();
        var seasonId = Guid.NewGuid();
        var season = new AssessmentSeason { Period = "2025.1", CourseId = courseId };
        var studentId = Guid.NewGuid();
        var dto = new AcademicCenterCreateDto("Center A", seasonId, [new("president", studentId)]);
        var saved = new AcademicCenter { Id = Guid.NewGuid(), Name = dto.Name, AssessmentSeasonId = seasonId, AcademicCenterMembers = [] };

        _assessmentSeasonRepository.FindById(seasonId).Returns(season);
        _academicCenterRepository.ExistsAcademicCenterByAssessmentSeasonId(seasonId).Returns(false);
        _userRepository.FindManyUsers(Arg.Any<List<Guid>>(), UserProfileEnum.Student, courseId)
            .Returns([new User { Id = studentId, Email = "student@test.com" }]);
        _academicCenterRepository.Add(Arg.Any<AcademicCenter>()).Returns(saved);

        var result = await _sut.CreateAcademicCenter(dto);

        result.Should().BeEquivalentTo(saved);
    }

    // --- GetById ---

    [Fact]
    public async Task GetAcademicCenter_WhenNotFound_ThrowsAcademicCenterNotFoundException()
    {
        var id = Guid.NewGuid();
        _academicCenterRepository.FindById(id).Returns((AcademicCenter?)null);

        var act = () => _sut.GetAcademicCenter(id);

        await act.Should().ThrowAsync<AcademicCenterNotFoundException>();
    }

    [Fact]
    public async Task GetAcademicCenter_WhenExists_ReturnsAcademicCenter()
    {
        var id = Guid.NewGuid();
        var academicCenter = new AcademicCenter { Id = id, Name = "Center A", AssessmentSeasonId = Guid.NewGuid(), AcademicCenterMembers = [] };
        _academicCenterRepository.FindById(id).Returns(academicCenter);

        var result = await _sut.GetAcademicCenter(id);

        result.Should().BeEquivalentTo(academicCenter);
    }

    // --- Paginate ---

    [Fact]
    public async Task GetAcademicCenters_WithoutFilter_ReturnsPaginatedResult()
    {
        var pagination = new Pagination<AcademicCenter>(1, 10, 0, []);
        _academicCenterRepository.GetAcademicCenters(1, 10, null).Returns(pagination);

        var result = await _sut.GetAcademicCenters(1, 10, null);

        result.Should().BeEquivalentTo(pagination);
    }

    [Fact]
    public async Task GetAcademicCenters_WithAssessmentSeasonIdFilter_PassesFilterToRepository()
    {
        var seasonId = Guid.NewGuid();
        var pagination = new Pagination<AcademicCenter>(1, 10, 0, []);
        _academicCenterRepository.GetAcademicCenters(1, 10, seasonId).Returns(pagination);

        var result = await _sut.GetAcademicCenters(1, 10, seasonId);

        result.Should().BeEquivalentTo(pagination);
        await _academicCenterRepository.Received(1).GetAcademicCenters(1, 10, seasonId);
    }

    // --- Update ---

    [Fact]
    public async Task UpdateAcademicCenter_WhenNotFound_ThrowsAcademicCenterNotFoundException()
    {
        var id = Guid.NewGuid();
        _academicCenterRepository.FindById(id).Returns((AcademicCenter?)null);

        var act = () => _sut.UpdateAcademicCenter(id, new AcademicCenterUpdateDto("Novo Nome", []));

        await act.Should().ThrowAsync<AcademicCenterNotFoundException>();
    }

    [Fact]
    public async Task UpdateAcademicCenter_WhenMembersHaveDuplicateUsers_ThrowsAcademicCenterMemberDuplicatedException()
    {
        var id = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var season = new AssessmentSeason { Period = "2025.1", CourseId = courseId };
        var academicCenter = new AcademicCenter
        {
            Id = id, Name = "Center A", AssessmentSeasonId = Guid.NewGuid(),
            AcademicCenterMembers = [], assessmentSeason = season
        };
        var duplicateId = Guid.NewGuid();
        var dto = new AcademicCenterUpdateDto("Center A", [new("president", duplicateId), new("member", duplicateId)]);

        _academicCenterRepository.FindById(id).Returns(academicCenter);

        var act = () => _sut.UpdateAcademicCenter(id, dto);

        await act.Should().ThrowAsync<AcademicCenterMemberDuplicatedException>();
    }

    [Fact]
    public async Task UpdateAcademicCenter_WhenValid_UpdatesNameAndMembers()
    {
        var id = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var studentId = Guid.NewGuid();
        var season = new AssessmentSeason { Period = "2025.1", CourseId = courseId };
        var academicCenter = new AcademicCenter
        {
            Id = id, Name = "Antigo", AssessmentSeasonId = Guid.NewGuid(),
            AcademicCenterMembers = [], assessmentSeason = season
        };
        var dto = new AcademicCenterUpdateDto("Novo Nome", [new("president", studentId)]);
        var updated = new AcademicCenter { Id = id, Name = dto.Name, AssessmentSeasonId = academicCenter.AssessmentSeasonId, AcademicCenterMembers = [] };

        _academicCenterRepository.FindById(id).Returns(academicCenter);
        _userRepository.FindManyUsers(Arg.Any<List<Guid>>(), UserProfileEnum.Student, courseId)
            .Returns([new User { Id = studentId, Email = "student@test.com" }]);
        _academicCenterRepository.Update(Arg.Any<AcademicCenter>()).Returns(updated);

        var result = await _sut.UpdateAcademicCenter(id, dto);

        result.Name.Should().Be(dto.Name);
    }

    [Fact]
    public async Task UpdateAcademicCenter_WhenMembersNotAllowedStudents_ExcludesThemFromUpdate()
    {
        var id = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var allowedId = Guid.NewGuid();
        var notAllowedId = Guid.NewGuid();
        var season = new AssessmentSeason { Period = "2025.1", CourseId = courseId };
        var academicCenter = new AcademicCenter
        {
            Id = id, Name = "Center A", AssessmentSeasonId = Guid.NewGuid(),
            AcademicCenterMembers = [], assessmentSeason = season
        };
        var dto = new AcademicCenterUpdateDto("Center A", [new("president", allowedId), new("member", notAllowedId)]);

        _academicCenterRepository.FindById(id).Returns(academicCenter);
        _userRepository.FindManyUsers(Arg.Any<List<Guid>>(), UserProfileEnum.Student, courseId)
            .Returns([new User { Id = allowedId, Email = "student@test.com" }]);
        _academicCenterRepository.Update(Arg.Any<AcademicCenter>()).Returns(callInfo =>
        {
            var ac = callInfo.Arg<AcademicCenter>();
            ac.AcademicCenterMembers.Should().ContainSingle(m => m.UserId == allowedId);
            ac.AcademicCenterMembers.Should().NotContain(m => m.UserId == notAllowedId);
            return (AcademicCenter?)ac;
        });

        await _sut.UpdateAcademicCenter(id, dto);
    }

    // --- Delete ---

    [Fact]
    public async Task DeleteAcademicCenter_WhenNotFound_ThrowsAcademicCenterNotFoundException()
    {
        var id = Guid.NewGuid();
        _academicCenterRepository.IdExists(id).Returns(false);

        var act = () => _sut.DeleteAcademicCenter(id);

        await act.Should().ThrowAsync<AcademicCenterNotFoundException>();
    }

    [Fact]
    public async Task DeleteAcademicCenter_WhenExists_CallsDeleteById()
    {
        var id = Guid.NewGuid();
        _academicCenterRepository.IdExists(id).Returns(true);

        await _sut.DeleteAcademicCenter(id);

        await _academicCenterRepository.Received(1).DeleteById(id);
    }
}
