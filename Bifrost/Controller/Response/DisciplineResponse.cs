using Bifrost.Core.Domain.Discipline;
namespace Bifrost.Response;

public class DisciplineResponse(Discipline discipline)
{
    public Guid Id { get; set; } = discipline.Id;
    public string Name { get; set; } = discipline.Name;
    public string Code { get; set; } = discipline.Code;
    public decimal? AvgScore { get; set; } = discipline.AvgScore;
    public AssessmentSeasonMinimalResponse AssessmentSeason { get; set; } = new(discipline.assessmentSeason);
    public UserMinimalResponse Professor { get; set; } = new(discipline.Professor);
    public IEnumerable<UserMinimalResponse> Students { get; set; } =
        discipline.Students.Select(s => new UserMinimalResponse(s));
}
