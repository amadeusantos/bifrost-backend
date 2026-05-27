using Bifrost.Core.Domain;
using Bifrost.Core.Domain.AssessmentSeason;
using Bifrost.Core.Exception.AssessmentSeason;
using Bifrost.Core.Exception.Course;
using Bifrost.Core.Port.Repository;
using Bifrost.Core.Service;
using FluentAssertions;
using NSubstitute;

namespace Bifrost.Test.Core;

public class AssessmentSeasonServiceTests
{
    private readonly IAssessmentSeasonRepository _repository = Substitute.For<IAssessmentSeasonRepository>();
    private readonly ICourseRepository _courseRepository = Substitute.For<ICourseRepository>();
    private readonly AssessmentSeasonService _sut;

    public AssessmentSeasonServiceTests()
    {
        _sut = new AssessmentSeasonService(_repository, _courseRepository);
    }

    // --- Create ---

    [Fact]
    public async Task CreateAssessmentSeason_WhenCourseDoesNotExist_ThrowsCourseNotFoundException()
    {
        var dto = new AssessmentSeasonCreateDto("2025.1", Guid.NewGuid(), null, null);
        _courseRepository.IdExists(dto.CourseId).Returns(false);

        var act = () => _sut.CreateAssessmentSeason(dto);

        await act.Should().ThrowAsync<CourseNotFoundException>();
    }

    [Fact]
    public async Task CreateAssessmentSeason_WhenPeriodAlreadyExists_ThrowsAssessmentSeasonPeriodAlreadyExistsException()
    {
        var dto = new AssessmentSeasonCreateDto("2025.1", Guid.NewGuid(), null, null);
        _courseRepository.IdExists(dto.CourseId).Returns(true);
        _repository.ExistsAssessmentSeasonByPeriod(dto.Period).Returns(true);

        var act = () => _sut.CreateAssessmentSeason(dto);

        await act.Should().ThrowAsync<AssessmentSeasonPeriodAlreadyExistsException>();
    }

    [Fact]
    public async Task CreateAssessmentSeason_WhenValid_ReturnsSavedAssessmentSeason()
    {
        var courseId = Guid.NewGuid();
        var dto = new AssessmentSeasonCreateDto("2025.1", courseId, DateTime.UtcNow, DateTime.UtcNow.AddMonths(6));
        var saved = new AssessmentSeason { Id = Guid.NewGuid(), Period = dto.Period, CourseId = courseId };

        _courseRepository.IdExists(courseId).Returns(true);
        _repository.ExistsAssessmentSeasonByPeriod(dto.Period).Returns(false);
        _repository.Add(Arg.Any<AssessmentSeason>()).Returns(saved);

        var result = await _sut.CreateAssessmentSeason(dto);

        result.Should().BeEquivalentTo(saved);
    }

    // --- GetById ---

    [Fact]
    public async Task GetAssessmentSeason_WhenNotFound_ThrowsAssessmentSeasonNotFoundException()
    {
        var id = Guid.NewGuid();
        _repository.FindById(id).Returns((AssessmentSeason?)null);

        var act = () => _sut.GetAssessmentSeason(id);

        await act.Should().ThrowAsync<AssessmentSeasonNotFoundException>();
    }

    [Fact]
    public async Task GetAssessmentSeason_WhenExists_ReturnsAssessmentSeason()
    {
        var id = Guid.NewGuid();
        var season = new AssessmentSeason { Id = id, Period = "2025.1", CourseId = Guid.NewGuid() };
        _repository.FindById(id).Returns(season);

        var result = await _sut.GetAssessmentSeason(id);

        result.Should().BeEquivalentTo(season);
    }

    // --- GetAll ---

    [Fact]
    public async Task GetAllAssessmentSeasons_ReturnsAllFromRepository()
    {
        var seasons = new List<AssessmentSeason>
        {
            new() { Id = Guid.NewGuid(), Period = "2025.1", CourseId = Guid.NewGuid() },
            new() { Id = Guid.NewGuid(), Period = "2025.2", CourseId = Guid.NewGuid() }
        };
        _repository.GetAll().Returns(seasons);

        var result = await _sut.GetAllAssessmentSeasons();

        result.Should().BeEquivalentTo(seasons);
    }

    // --- Paginate ---

    [Fact]
    public async Task GetAssessmentSeasons_WithoutCourseFilter_ReturnsPaginatedResult()
    {
        var pagination = new Pagination<AssessmentSeason>(1, 10, 0, []);
        _repository.GetAssessmentSeasons(1, 10, null).Returns(pagination);

        var result = await _sut.GetAssessmentSeasons(1, 10, null);

        result.Should().BeEquivalentTo(pagination);
    }

    [Fact]
    public async Task GetAssessmentSeasons_WithCourseFilter_ForwardsCourseIdToRepository()
    {
        var courseId = Guid.NewGuid();
        var pagination = new Pagination<AssessmentSeason>(1, 10, 0, []);
        _repository.GetAssessmentSeasons(1, 10, courseId).Returns(pagination);

        var result = await _sut.GetAssessmentSeasons(1, 10, courseId);

        result.Should().BeEquivalentTo(pagination);
        await _repository.Received(1).GetAssessmentSeasons(1, 10, courseId);
    }

    // --- Update ---

    [Fact]
    public async Task UpdateAssessmentSeason_WhenNotFound_ThrowsAssessmentSeasonNotFoundException()
    {
        var id = Guid.NewGuid();
        _repository.FindById(id).Returns((AssessmentSeason?)null);

        var act = () => _sut.UpdateAssessmentSeason(id, new AssessmentSeasonUpdateDto("2025.2", null, null));

        await act.Should().ThrowAsync<AssessmentSeasonNotFoundException>();
    }

    [Fact]
    public async Task UpdateAssessmentSeason_WhenNewPeriodAlreadyExists_ThrowsAssessmentSeasonPeriodAlreadyExistsException()
    {
        var id = Guid.NewGuid();
        var existing = new AssessmentSeason { Id = id, Period = "2025.1", CourseId = Guid.NewGuid() };
        var dto = new AssessmentSeasonUpdateDto("2025.2", null, null);

        _repository.FindById(id).Returns(existing);
        _repository.ExistsAssessmentSeasonByPeriod(dto.Period).Returns(true);

        var act = () => _sut.UpdateAssessmentSeason(id, dto);

        await act.Should().ThrowAsync<AssessmentSeasonPeriodAlreadyExistsException>();
    }

    [Fact]
    public async Task UpdateAssessmentSeason_WhenPeriodUnchanged_DoesNotCheckForDuplicate()
    {
        var id = Guid.NewGuid();
        var existing = new AssessmentSeason { Id = id, Period = "2025.1", CourseId = Guid.NewGuid() };
        var dto = new AssessmentSeasonUpdateDto("2025.1", null, null);
        var updated = new AssessmentSeason { Id = id, Period = dto.Period, CourseId = existing.CourseId };

        _repository.FindById(id).Returns(existing);
        _repository.Update(Arg.Any<AssessmentSeason>()).Returns(updated);

        await _sut.UpdateAssessmentSeason(id, dto);

        await _repository.DidNotReceive().ExistsAssessmentSeasonByPeriod(Arg.Any<string>());
    }

    [Fact]
    public async Task UpdateAssessmentSeason_WhenValid_ReturnsUpdatedAssessmentSeason()
    {
        var id = Guid.NewGuid();
        var existing = new AssessmentSeason { Id = id, Period = "2025.1", CourseId = Guid.NewGuid() };
        var dto = new AssessmentSeasonUpdateDto("2025.2", DateTime.UtcNow, DateTime.UtcNow.AddMonths(6));
        var updated = new AssessmentSeason { Id = id, Period = dto.Period, CourseId = existing.CourseId, StartDateTime = dto.StartDateTime, EndDateTime = dto.EndDateTime };

        _repository.FindById(id).Returns(existing);
        _repository.ExistsAssessmentSeasonByPeriod(dto.Period).Returns(false);
        _repository.Update(Arg.Any<AssessmentSeason>()).Returns(updated);

        var result = await _sut.UpdateAssessmentSeason(id, dto);

        result.Period.Should().Be(dto.Period);
        result.StartDateTime.Should().Be(dto.StartDateTime);
        result.EndDateTime.Should().Be(dto.EndDateTime);
    }

    // --- Delete ---

    [Fact]
    public async Task DeleteAssessmentSeason_WhenNotFound_ThrowsAssessmentSeasonNotFoundException()
    {
        var id = Guid.NewGuid();
        _repository.IdExists(id).Returns(false);

        var act = () => _sut.DeleteAssessmentSeason(id);

        await act.Should().ThrowAsync<AssessmentSeasonNotFoundException>();
    }

    [Fact]
    public async Task DeleteAssessmentSeason_WhenExists_CallsDeleteById()
    {
        var id = Guid.NewGuid();
        _repository.IdExists(id).Returns(true);

        await _sut.DeleteAssessmentSeason(id);

        await _repository.Received(1).DeleteById(id);
    }
}
