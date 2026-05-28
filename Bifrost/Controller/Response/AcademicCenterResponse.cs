using Bifrost.Controller.Response;
using Bifrost.Core.Domain.AcademicCenter;

namespace Bifrost.Response;

public class AcademicCenterResponse(AcademicCenter academicCenter)
{
    public Guid Id { get; set; } = academicCenter.Id;
    public string Name { get; set; } = academicCenter.Name;
    public decimal? AvgScore { get; set; } = academicCenter.AvgScore;
    public AssessmentSeasonMinimalResponse AssessmentSeason { get; set; } = new(academicCenter.assessmentSeason);
    public IEnumerable<AcademicCenterMemberResponse> AcademicCenterMembers { get; set; } =
        academicCenter.AcademicCenterMembers.Select(m => new AcademicCenterMemberResponse(m));
}
