using Bifrost.Core.Domain;
using Bifrost.Core.Domain.AssessmentSeason;
using Bifrost.Core.Domain.Discipline;
using Bifrost.Core.Domain.Enum;
using Bifrost.Core.Domain.User;
using Bifrost.Core.Exception.AssessmentSeason;
using Bifrost.Core.Exception.Discipline;
using Bifrost.Core.Port.Repository;
using Bifrost.Core.Service;
using FluentAssertions;
using NSubstitute;

namespace Bifrost.Test.Core;

public class DisciplineServiceTests
{
    private readonly IDisciplineRepository _disciplineRepository = Substitute.For<IDisciplineRepository>();
    private readonly IAssessmentSeasonRepository _assessmentSeasonRepository = Substitute.For<IAssessmentSeasonRepository>();
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly DisciplineService _sut;

    public DisciplineServiceTests()
    {
        _sut = new DisciplineService(_disciplineRepository, _assessmentSeasonRepository, _userRepository);
    }

    // --- Create ---

    [Fact]
    public async Task CreateDiscipline_WhenAssessmentSeasonNotFound_ThrowsAssessmentSeasonNotFoundException()
    {
        var dto = new DisciplineCreateDto(Guid.Empty, "Math", "MAT101", Guid.NewGuid(), Guid.NewGuid(), []);
        _assessmentSeasonRepository.FindById(dto.AssessmentSeasonId).Returns((AssessmentSeason?)null);

        var act = () => _sut.CreateDiscipline(dto);

        await act.Should().ThrowAsync<AssessmentSeasonNotFoundException>();
    }

    [Fact]
    public async Task CreateDiscipline_WhenProfessorHasWrongProfile_ThrowsProfessorProfileRequiredException()
    {
        var courseId = Guid.NewGuid();
        var seasonId = Guid.NewGuid();
        var season = new AssessmentSeason { Period = "2025.1", CourseId = courseId };
        var dto = new DisciplineCreateDto(Guid.Empty, "Math", "MAT101", seasonId, Guid.NewGuid(), []);

        _assessmentSeasonRepository.FindById(seasonId).Returns(season);
        _userRepository.FindManyUsers(Arg.Any<List<Guid>>(), UserProfileEnum.Professor, courseId).Returns([]);

        var act = () => _sut.CreateDiscipline(dto);

        await act.Should().ThrowAsync<ProfessorProfileRequiredException>();
    }

    [Fact]
    public async Task CreateDiscipline_WhenStudentsHaveDuplicates_ThrowsDisciplineStudentDuplicatedException()
    {
        var courseId = Guid.NewGuid();
        var seasonId = Guid.NewGuid();
        var professorId = Guid.NewGuid();
        var duplicateStudentId = Guid.NewGuid();
        var season = new AssessmentSeason { Period = "2025.1", CourseId = courseId };
        var dto = new DisciplineCreateDto(Guid.Empty, "Math", "MAT101", seasonId, professorId,
            [duplicateStudentId, duplicateStudentId]);

        _assessmentSeasonRepository.FindById(seasonId).Returns(season);
        _userRepository.FindById(professorId)
            .Returns(new User { Id = professorId, Email = "prof@test.com", Profile = UserProfileEnum.Professor});

        var act = () => _sut.CreateDiscipline(dto);

        await act.Should().ThrowAsync<DisciplineStudentDuplicatedException>();
    }

    [Fact]
    public async Task CreateDiscipline_WhenStudentsNotAllowed_ExcludesThemFromDiscipline()
    {
        var courseId = Guid.NewGuid();
        var seasonId = Guid.NewGuid();
        var professorId = Guid.NewGuid();
        var allowedStudentId = Guid.NewGuid();
        var notAllowedStudentId = Guid.NewGuid();
        var season = new AssessmentSeason { Period = "2025.1", CourseId = courseId };
        var dto = new DisciplineCreateDto(Guid.Empty, "Math", "MAT101", seasonId, professorId,
            [allowedStudentId, notAllowedStudentId]);
        var saved = new Discipline { Id = Guid.NewGuid(), Name = dto.Name, Code = dto.Code, AssessmentSeasonId = seasonId, ProfessorId = professorId };

        _assessmentSeasonRepository.FindById(seasonId).Returns(season);
        _userRepository.FindById(professorId)
            .Returns(new User { Id = professorId, Email = "prof@test.com", Profile = UserProfileEnum.Professor});
        _userRepository.FindManyUsers(Arg.Is<List<Guid>>(l => l.Contains(allowedStudentId)), UserProfileEnum.Student, courseId)
            .Returns([new User { Id = allowedStudentId, Email = "student@test.com" }]);
        _disciplineRepository.Add(Arg.Any<Discipline>()).Returns(callInfo =>
        {
            var d = callInfo.Arg<Discipline>();
            d.Students.Should().ContainSingle(s => s.Id == allowedStudentId);
            d.Students.Should().NotContain(s => s.Id == notAllowedStudentId);
            return saved;
        });

        await _sut.CreateDiscipline(dto);

        await _disciplineRepository.Received(1).Add(Arg.Any<Discipline>());
    }

    [Fact]
    public async Task CreateDiscipline_WhenValid_ReturnsSavedDiscipline()
    {
        var courseId = Guid.NewGuid();
        var seasonId = Guid.NewGuid();
        var professorId = Guid.NewGuid();
        var season = new AssessmentSeason { Period = "2025.1", CourseId = courseId };
        var dto = new DisciplineCreateDto(Guid.Empty, "Math", "MAT101", seasonId, professorId, []);
        var saved = new Discipline { Id = Guid.NewGuid(), Name = dto.Name, Code = dto.Code, AssessmentSeasonId = seasonId, ProfessorId = professorId };

        _assessmentSeasonRepository.FindById(seasonId).Returns(season);
        _userRepository.FindById(professorId)
            .Returns(new User { Id = professorId, Email = "prof@test.com", Profile = UserProfileEnum.Professor});
        _userRepository.FindManyUsers(Arg.Any<List<Guid>>(), UserProfileEnum.Student, courseId).Returns([]);
        _disciplineRepository.Add(Arg.Any<Discipline>()).Returns(saved);

        var result = await _sut.CreateDiscipline(dto);

        result.Should().BeEquivalentTo(saved);
    }

    // --- GetById ---

    [Fact]
    public async Task GetDiscipline_WhenNotFound_ThrowsDisciplineNotFoundException()
    {
        var id = Guid.NewGuid();
        _disciplineRepository.FindById(id).Returns((Discipline?)null);

        var act = () => _sut.GetDiscipline(id);

        await act.Should().ThrowAsync<DisciplineNotFoundException>();
    }

    [Fact]
    public async Task GetDiscipline_WhenExists_ReturnsDiscipline()
    {
        var id = Guid.NewGuid();
        var discipline = new Discipline { Id = id, Name = "Math", Code = "MAT101", AssessmentSeasonId = Guid.NewGuid(), ProfessorId = Guid.NewGuid() };
        _disciplineRepository.FindById(id).Returns(discipline);

        var result = await _sut.GetDiscipline(id);

        result.Should().BeEquivalentTo(discipline);
    }

    // --- Update ---

    [Fact]
    public async Task UpdateDiscipline_WhenNotFound_ThrowsDisciplineNotFoundException()
    {
        var id = Guid.NewGuid();
        _disciplineRepository.FindById(id).Returns((Discipline?)null);

        var act = () => _sut.UpdateDiscipline(id, new DisciplineUpdateDto(id, "Math", "MAT101", Guid.NewGuid(), []));

        await act.Should().ThrowAsync<DisciplineNotFoundException>();
    }

    [Fact]
    public async Task UpdateDiscipline_WhenProfessorHasWrongProfile_ThrowsProfessorProfileRequiredException()
    {
        var id = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var season = new AssessmentSeason { Period = "2025.1", CourseId = courseId };
        var discipline = new Discipline
        {
            Id = id, Name = "Math", Code = "MAT101",
            AssessmentSeasonId = Guid.NewGuid(), ProfessorId = Guid.NewGuid(),
            assessmentSeason = season
        };
        var dto = new DisciplineUpdateDto(id, "Math", "MAT101", Guid.NewGuid(), []);

        _disciplineRepository.FindById(id).Returns(discipline);
        _userRepository.FindManyUsers(Arg.Any<List<Guid>>(), UserProfileEnum.Professor, courseId).Returns([]);

        var act = () => _sut.UpdateDiscipline(id, dto);

        await act.Should().ThrowAsync<ProfessorProfileRequiredException>();
    }

    [Fact]
    public async Task UpdateDiscipline_WhenStudentsHaveDuplicates_ThrowsDisciplineStudentDuplicatedException()
    {
        var id = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var professorId = Guid.NewGuid();
        var duplicateStudentId = Guid.NewGuid();
        var season = new AssessmentSeason { Period = "2025.1", CourseId = courseId };
        var discipline = new Discipline
        {
            Id = id, Name = "Math", Code = "MAT101",
            AssessmentSeasonId = Guid.NewGuid(), ProfessorId = Guid.NewGuid(),
            assessmentSeason = season
        };
        var dto = new DisciplineUpdateDto(id, "Math", "MAT101", professorId, [duplicateStudentId, duplicateStudentId]);

        _disciplineRepository.FindById(id).Returns(discipline);
        _userRepository.FindById(professorId)
            .Returns(new User { Id = professorId, Email = "prof@test.com", Profile = UserProfileEnum.Professor});

        var act = () => _sut.UpdateDiscipline(id, dto);

        await act.Should().ThrowAsync<DisciplineStudentDuplicatedException>();
    }

    [Fact]
    public async Task UpdateDiscipline_WhenValid_UpdatesFields()
    {
        var id = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var professorId = Guid.NewGuid();
        var season = new AssessmentSeason { Period = "2025.1", CourseId = courseId };
        var discipline = new Discipline
        {
            Id = id, Name = "Old", Code = "OLD101",
            AssessmentSeasonId = Guid.NewGuid(), ProfessorId = Guid.NewGuid(),
            assessmentSeason = season
        };
        var dto = new DisciplineUpdateDto(id, "Math", "MAT101", professorId, []);
        var updated = new Discipline { Id = id, Name = dto.Name, Code = dto.Code, AssessmentSeasonId = discipline.AssessmentSeasonId, ProfessorId = professorId };

        _disciplineRepository.FindById(id).Returns(discipline);
        _userRepository.FindById(professorId)
            .Returns(new User { Id = professorId, Email = "prof@test.com", Profile = UserProfileEnum.Professor});
        _userRepository.FindManyUsers(Arg.Any<List<Guid>>(), UserProfileEnum.Student, courseId).Returns([]);
        _disciplineRepository.Update(Arg.Any<Discipline>()).Returns(updated);

        var result = await _sut.UpdateDiscipline(id, dto);

        result.Name.Should().Be(dto.Name);
        result.Code.Should().Be(dto.Code);
    }

    // --- Delete ---

    [Fact]
    public async Task DeleteDiscipline_WhenNotFound_ThrowsDisciplineNotFoundException()
    {
        var id = Guid.NewGuid();
        _disciplineRepository.IdExists(id).Returns(false);

        var act = () => _sut.DeleteDiscipline(id);

        await act.Should().ThrowAsync<DisciplineNotFoundException>();
    }

    [Fact]
    public async Task DeleteDiscipline_WhenExists_CallsDeleteById()
    {
        var id = Guid.NewGuid();
        _disciplineRepository.IdExists(id).Returns(true);

        await _sut.DeleteDiscipline(id);

        await _disciplineRepository.Received(1).DeleteById(id);
    }
}
