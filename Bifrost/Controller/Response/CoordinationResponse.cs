using Bifrost.Core.Domain.Coordination;
using Bifrost.Core.Domain.CoordinationMember;

namespace Bifrost.Response;

public class CoordinationResponse(Coordination coordination)
{
    public Guid Id { get; set; } = coordination.Id;
    public string Name { get; set; } = coordination.Name;
    public decimal? AvgScore { get; set; } = coordination.AvgScore;
    public AssessmentSeasonMinimalResponse AssessmentSeason { get; set; } = new(coordination.assessmentSeason);
    public IEnumerable<CoordinationMemberResponse> CoordinationMembers { get; set; } =
        coordination.CoordinationMembers.Select(cm => new CoordinationMemberResponse(cm));
}