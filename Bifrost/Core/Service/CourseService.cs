using Bifrost.Core.Adapter;
using Bifrost.Core.Domain;
using Bifrost.Core.Domain.Course;
using Bifrost.Core.Exception.Course;
using Bifrost.Core.Port.Repository;

namespace Bifrost.Core.Service;

public class CourseService(ICourseRepository courseRepository) : ICourseService
{
    public async Task<Pagination<Course>> PaginationCoursesAsync(int page, int size)
    {
        return await courseRepository.GetCourses(page, size);
    }

    public async Task<List<Course>> GetAllCoursesAsync()
    {
        return await courseRepository.GetAll();
    }

    public async Task<Course> GetCourseByIdAsync(Guid id)
    {
        Course? course = await courseRepository.FindById(id);
        return course ?? throw new CourseNotFoundException();
    }

    public async Task<Course> CreateCourseAsync(CourseDto courseDto)
    {
        if (await courseRepository.ExistsByCode(courseDto.Code))
            throw new CourseCodeAlreadyExistsException();

        return await courseRepository.Add(new Course { Name = courseDto.Name, Code = courseDto.Code });
    }

    public async Task<Course> UpdateCourseAsync(Guid id, CourseDto courseDto)
    {
        Course? course = await courseRepository.FindById(id);
        if (course is null) throw new CourseNotFoundException();

        if (course.Code != courseDto.Code && await courseRepository.ExistsByCode(courseDto.Code))
            throw new CourseCodeAlreadyExistsException();

        course.Name = courseDto.Name;
        course.Code = courseDto.Code;
        return await courseRepository.Update(course);
    }

    public async Task DeleteCourseByIdAsync(Guid id)
    {
        if (!await courseRepository.IdExists(id)) throw new CourseNotFoundException();
        await courseRepository.DeleteById(id);
    }
}
