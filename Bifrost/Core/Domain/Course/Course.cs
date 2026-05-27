namespace Bifrost.Core.Domain.Course;

public class Course
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Code { get; set; }
}