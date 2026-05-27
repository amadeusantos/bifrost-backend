using Bifrost.Core.Domain;
using Bifrost.Core.Domain.Course;

namespace Bifrost.Core.Adapter;

public interface ICourseService
{
    Task<Pagination<Course>> PaginationCoursesAsync(int page, int size);
    
    Task<List<Course>> GetAllCoursesAsync();
    
    Task<Course> GetCourseByIdAsync(Guid id);
    
    Task<Course> CreateCourseAsync(CourseDto courseDto);
    
    Task<Course> UpdateCourseAsync(Guid id, CourseDto courseDto);
    
    Task DeleteCourseByIdAsync(Guid id);
}