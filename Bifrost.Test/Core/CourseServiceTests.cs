using Bifrost.Core.Domain.Course;
using Bifrost.Core.Exception.Course;
using Bifrost.Core.Port.Repository;
using Bifrost.Core.Service;
using FluentAssertions;
using NSubstitute;

namespace Bifrost.Test.Core;

public class CourseServiceTests
{
    private readonly ICourseRepository _repository = Substitute.For<ICourseRepository>();
    private readonly CourseService _sut;

    public CourseServiceTests()
    {
        _sut = new CourseService(_repository);
    }

    [Fact]
    public async Task CreateCourseAsync_WhenCodeAlreadyExists_ThrowsCourseCodeAlreadyExistsException()
    {
        var dto = new CourseDto("Engenharia de Software", "ES101");
        _repository.ExistsByCode(dto.Code).Returns(true);

        var act = () => _sut.CreateCourseAsync(dto);

        await act.Should().ThrowAsync<CourseCodeAlreadyExistsException>();
    }

    [Fact]
    public async Task CreateCourseAsync_WhenCodeIsUnique_ReturnsSavedCourse()
    {
        var dto = new CourseDto("Engenharia de Software", "ES101");
        var saved = new Course { Id = Guid.NewGuid(), Name = dto.Name, Code = dto.Code };

        _repository.ExistsByCode(dto.Code).Returns(false);
        _repository.Add(Arg.Any<Course>()).Returns(saved);

        var result = await _sut.CreateCourseAsync(dto);

        result.Should().BeEquivalentTo(saved);
    }

    [Fact]
    public async Task UpdateCourseAsync_WhenCourseDoesNotExist_ThrowsCourseNotFoundException()
    {
        var id = Guid.NewGuid();
        _repository.FindById(id).Returns((Course?)null);

        var act = () => _sut.UpdateCourseAsync(id, new CourseDto("Novo Nome", "NOVO01"));

        await act.Should().ThrowAsync<CourseNotFoundException>();
    }

    [Fact]
    public async Task UpdateCourseAsync_WhenCodeBelongsToAnotherCourse_ThrowsCourseCodeAlreadyExistsException()
    {
        var id = Guid.NewGuid();
        var existing = new Course { Id = id, Name = "Antigo", Code = "ANT01" };
        var dto = new CourseDto("Novo Nome", "OUTRO01");

        _repository.FindById(id).Returns(existing);
        _repository.ExistsByCode(dto.Code).Returns(true);

        var act = () => _sut.UpdateCourseAsync(id, dto);

        await act.Should().ThrowAsync<CourseCodeAlreadyExistsException>();
    }

    [Fact]
    public async Task UpdateCourseAsync_WhenCodeIsUnique_ReturnsUpdatedCourse()
    {
        var id = Guid.NewGuid();
        var existing = new Course { Id = id, Name = "Antigo", Code = "ANT01" };
        var dto = new CourseDto("Novo Nome", "NOVO01");
        var updated = new Course { Id = id, Name = dto.Name, Code = dto.Code };

        _repository.FindById(id).Returns(existing);
        _repository.ExistsByCodeExcludingId(dto.Code, id).Returns(false);
        _repository.Update(Arg.Any<Course>()).Returns(updated);

        var result = await _sut.UpdateCourseAsync(id, dto);

        result.Name.Should().Be(dto.Name);
        result.Code.Should().Be(dto.Code);
    }

    [Fact]
    public async Task GetCourseByIdAsync_WhenCourseDoesNotExist_ThrowsCourseNotFoundException()
    {
        var id = Guid.NewGuid();
        _repository.FindById(id).Returns((Course?)null);

        var act = () => _sut.GetCourseByIdAsync(id);

        await act.Should().ThrowAsync<CourseNotFoundException>();
    }

    [Fact]
    public async Task GetCourseByIdAsync_WhenCourseExists_ReturnsCourse()
    {
        var id = Guid.NewGuid();
        var course = new Course { Id = id, Name = "Matemática", Code = "MAT01" };
        _repository.FindById(id).Returns(course);

        var result = await _sut.GetCourseByIdAsync(id);

        result.Should().BeEquivalentTo(course);
    }

    [Fact]
    public async Task DeleteCourseByIdAsync_WhenCourseDoesNotExist_ThrowsCourseNotFoundException()
    {
        var id = Guid.NewGuid();
        _repository.IdExists(id).Returns(false);

        var act = () => _sut.DeleteCourseByIdAsync(id);

        await act.Should().ThrowAsync<CourseNotFoundException>();
    }

    [Fact]
    public async Task DeleteCourseByIdAsync_WhenCourseExists_CallsDeleteById()
    {
        var id = Guid.NewGuid();
        _repository.IdExists(id).Returns(true);

        await _sut.DeleteCourseByIdAsync(id);

        await _repository.Received(1).DeleteById(id);
    }
}
