using Bifrost.Core.Domain;
using Bifrost.Core.Domain.AssessmentSeason;
using Bifrost.Core.Domain.Enum;
using Bifrost.Core.Domain.Coordination;
using Bifrost.Core.Domain.User;
using Bifrost.Core.Exception.AssessmentSeason;
using Bifrost.Core.Exception.Coordination;
using Bifrost.Core.Port.Repository;
using Bifrost.Core.Service;
using FluentAssertions;
using NSubstitute;

namespace Bifrost.Test.Core;

public class CoordinationServiceTests
{
    private readonly ICoordinationRepository _coordinationRepository = Substitute.For<ICoordinationRepository>();
    private readonly IAssessmentSeasonRepository _assessmentSeasonRepository = Substitute.For<IAssessmentSeasonRepository>();
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly CoordinationService _sut;

    public CoordinationServiceTests()
    {
        _sut = new CoordinationService(_coordinationRepository, _assessmentSeasonRepository, _userRepository);
    }

    // --- Create ---

    [Fact]
    public async Task CreateCoordination_WhenAssessmentSeasonNotFound_ThrowsAssessmentSeasonNotFoundException()
    {
        var dto = new CoordinationCreateDto("Coord A", Guid.NewGuid(), []);
        _assessmentSeasonRepository.FindById(dto.AssessmentSeasonId).Returns((AssessmentSeason?)null);

        var act = () => _sut.CreateCoordination(dto);

        await act.Should().ThrowAsync<AssessmentSeasonNotFoundException>();
    }

    [Fact]
    public async Task CreateCoordination_WhenCoordinationAlreadyExistsForSeason_ThrowsCoordinationPeriodAlreadyExistsException()
    {
        var seasonId = Guid.NewGuid();
        var season = new AssessmentSeason { Period = "2025.1", CourseId = Guid.NewGuid() };
        var dto = new CoordinationCreateDto("Coord A", seasonId, []);

        _assessmentSeasonRepository.FindById(seasonId).Returns(season);
        _coordinationRepository.ExistsCoordinationByAssessmentSeasonId(seasonId).Returns(true);

        var act = () => _sut.CreateCoordination(dto);

        await act.Should().ThrowAsync<CoordinationPeriodAlreadyExistsException>();
    }

    [Fact]
    public async Task CreateCoordination_WhenMembersHaveDuplicateUsers_ThrowsCoordinationMemberDuplicatedException()
    {
        var seasonId = Guid.NewGuid();
        var season = new AssessmentSeason { Period = "2025.1", CourseId = Guid.NewGuid() };
        var duplicateId = Guid.NewGuid();
        var members = new List<CoordinationMemberDto>
        {
            new("coordinator", duplicateId),
            new("member", duplicateId)
        };
        var dto = new CoordinationCreateDto("Coord A", seasonId, members);

        _assessmentSeasonRepository.FindById(seasonId).Returns(season);
        _coordinationRepository.ExistsCoordinationByAssessmentSeasonId(seasonId).Returns(false);

        var act = () => _sut.CreateCoordination(dto);

        await act.Should().ThrowAsync<CoordinationMemberDuplicatedException>();
    }

    [Fact]
    public async Task CreateCoordination_WhenMembersNotAllowedProfessors_ExcludesThemFromCoordination()
    {
        var courseId = Guid.NewGuid();
        var seasonId = Guid.NewGuid();
        var season = new AssessmentSeason { Period = "2025.1", CourseId = courseId };

        var allowedId = Guid.NewGuid();
        var notAllowedId = Guid.NewGuid();
        var members = new List<CoordinationMemberDto>
        {
            new("coordinator", allowedId),
            new("member", notAllowedId)
        };
        var dto = new CoordinationCreateDto("Coord A", seasonId, members);
        var saved = new Coordination { Id = Guid.NewGuid(), Name = dto.Name, AssessmentSeasonId = seasonId, CoordinationMembers = [] };

        _assessmentSeasonRepository.FindById(seasonId).Returns(season);
        _coordinationRepository.ExistsCoordinationByAssessmentSeasonId(seasonId).Returns(false);
        _userRepository.FindManyUsers(Arg.Any<List<Guid>>(), Arg.Any<UserProfileEnum>(), courseId)
            .Returns([new User { Id = allowedId, Email = "prof@test.com" }]);
        _coordinationRepository.Add(Arg.Any<Coordination>()).Returns(callInfo =>
        {
            var c = callInfo.Arg<Coordination>();
            c.CoordinationMembers.Should().ContainSingle(m => m.UserId == allowedId);
            c.CoordinationMembers.Should().NotContain(m => m.UserId == notAllowedId);
            return saved;
        });

        await _sut.CreateCoordination(dto);

        await _coordinationRepository.Received(1).Add(Arg.Any<Coordination>());
    }

    [Fact]
    public async Task CreateCoordination_WhenValid_ReturnsSavedCoordination()
    {
        var courseId = Guid.NewGuid();
        var seasonId = Guid.NewGuid();
        var season = new AssessmentSeason { Period = "2025.1", CourseId = courseId };
        var professorId = Guid.NewGuid();
        var dto = new CoordinationCreateDto("Coord A", seasonId, [new("coordinator", professorId)]);
        var saved = new Coordination { Id = Guid.NewGuid(), Name = dto.Name, AssessmentSeasonId = seasonId, CoordinationMembers = [] };

        _assessmentSeasonRepository.FindById(seasonId).Returns(season);
        _coordinationRepository.ExistsCoordinationByAssessmentSeasonId(seasonId).Returns(false);
        _userRepository.FindManyUsers(Arg.Any<List<Guid>>(), Arg.Any<UserProfileEnum>(), courseId)
            .Returns([new User { Id = professorId, Email = "prof@test.com" }]);
        _coordinationRepository.Add(Arg.Any<Coordination>()).Returns(saved);

        var result = await _sut.CreateCoordination(dto);

        result.Should().BeEquivalentTo(saved);
    }

    // --- GetById ---

    [Fact]
    public async Task GetCoordination_WhenNotFound_ThrowsCoordinationNotFoundException()
    {
        var id = Guid.NewGuid();
        _coordinationRepository.FindById(id).Returns((Coordination?)null);

        var act = () => _sut.GetCoordination(id);

        await act.Should().ThrowAsync<CoordinationNotFoundException>();
    }

    [Fact]
    public async Task GetCoordination_WhenExists_ReturnsCoordination()
    {
        var id = Guid.NewGuid();
        var coordination = new Coordination { Id = id, Name = "Coord A", AssessmentSeasonId = Guid.NewGuid(), CoordinationMembers = [] };
        _coordinationRepository.FindById(id).Returns(coordination);

        var result = await _sut.GetCoordination(id);

        result.Should().BeEquivalentTo(coordination);
    }

    // --- Paginate ---

    [Fact]
    public async Task GetCoordinations_ReturnsPaginatedResult()
    {
        var pagination = new Pagination<Coordination>(1, 10, 0, []);
        _coordinationRepository.GetCoordinations(1, 10).Returns(pagination);

        var result = await _sut.GetCoordinations(1, 10);

        result.Should().BeEquivalentTo(pagination);
    }

    // --- Update ---

    [Fact]
    public async Task UpdateCoordination_WhenNotFound_ThrowsCoordinationNotFoundException()
    {
        var id = Guid.NewGuid();
        _coordinationRepository.FindById(id).Returns((Coordination?)null);

        var act = () => _sut.UpdateCoordination(id, new CoordinationUpdateDto("Novo Nome", []));

        await act.Should().ThrowAsync<CoordinationNotFoundException>();
    }

    [Fact]
    public async Task UpdateCoordination_WhenMembersHaveDuplicateUsers_ThrowsCoordinationMemberDuplicatedException()
    {
        var id = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var season = new AssessmentSeason { Period = "2025.1", CourseId = courseId };
        var coordination = new Coordination
        {
            Id = id, Name = "Coord A", AssessmentSeasonId = Guid.NewGuid(),
            CoordinationMembers = [], assessmentSeason = season
        };
        var duplicateId = Guid.NewGuid();
        var dto = new CoordinationUpdateDto("Coord A", [new("coordinator", duplicateId), new("member", duplicateId)]);

        _coordinationRepository.FindById(id).Returns(coordination);

        var act = () => _sut.UpdateCoordination(id, dto);

        await act.Should().ThrowAsync<CoordinationMemberDuplicatedException>();
    }

    [Fact]
    public async Task UpdateCoordination_WhenValid_UpdatesNameAndMembers()
    {
        var id = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var professorId = Guid.NewGuid();
        var season = new AssessmentSeason { Period = "2025.1", CourseId = courseId };
        var coordination = new Coordination
        {
            Id = id, Name = "Antigo", AssessmentSeasonId = Guid.NewGuid(),
            CoordinationMembers = [], assessmentSeason = season
        };
        var dto = new CoordinationUpdateDto("Novo Nome", [new("coordinator", professorId)]);
        var updated = new Coordination { Id = id, Name = dto.Name, AssessmentSeasonId = coordination.AssessmentSeasonId, CoordinationMembers = [] };

        _coordinationRepository.FindById(id).Returns(coordination);
        _userRepository.FindManyUsers(Arg.Any<List<Guid>>(), Arg.Any<UserProfileEnum>(), courseId)
            .Returns([new User { Id = professorId, Email = "prof@test.com" }]);
        _coordinationRepository.Update(Arg.Any<Coordination>()).Returns(updated);

        var result = await _sut.UpdateCoordination(id, dto);

        result.Name.Should().Be(dto.Name);
    }

    [Fact]
    public async Task UpdateCoordination_WhenMembersNotAllowedProfessors_ExcludesThemFromUpdate()
    {
        var id = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var allowedId = Guid.NewGuid();
        var notAllowedId = Guid.NewGuid();
        var season = new AssessmentSeason { Period = "2025.1", CourseId = courseId };
        var coordination = new Coordination
        {
            Id = id, Name = "Coord A", AssessmentSeasonId = Guid.NewGuid(),
            CoordinationMembers = [], assessmentSeason = season
        };
        var dto = new CoordinationUpdateDto("Coord A", [new("coordinator", allowedId), new("member", notAllowedId)]);

        _coordinationRepository.FindById(id).Returns(coordination);
        _userRepository.FindManyUsers(Arg.Any<List<Guid>>(), Arg.Any<UserProfileEnum>(), courseId)
            .Returns([new User { Id = allowedId, Email = "prof@test.com" }]);
        _coordinationRepository.Update(Arg.Any<Coordination>()).Returns(callInfo =>
        {
            var c = callInfo.Arg<Coordination>();
            c.CoordinationMembers.Should().ContainSingle(m => m.UserId == allowedId);
            c.CoordinationMembers.Should().NotContain(m => m.UserId == notAllowedId);
            return (Coordination?)c;
        });

        await _sut.UpdateCoordination(id, dto);
    }

    // --- Delete ---

    [Fact]
    public async Task DeleteCoordination_WhenNotFound_ThrowsCoordinationNotFoundException()
    {
        var id = Guid.NewGuid();
        _coordinationRepository.IdExists(id).Returns(false);

        var act = () => _sut.DeleteCoordination(id);

        await act.Should().ThrowAsync<CoordinationNotFoundException>();
    }

    [Fact]
    public async Task DeleteCoordination_WhenExists_CallsDeleteById()
    {
        var id = Guid.NewGuid();
        _coordinationRepository.IdExists(id).Returns(true);

        await _sut.DeleteCoordination(id);

        await _coordinationRepository.Received(1).DeleteById(id);
    }
}
