using Bifrost.Core.Domain.AssessmentCoordination;

namespace Bifrost.Response;

public class AssessmentCoordinationResponse(AssessmentCoordination assessmentCoordination)
{
    public Guid Id { get; set; } = assessmentCoordination.Id;
    public decimal Score { get; set; } = assessmentCoordination.Score;
    public string? Note { get; set; } = assessmentCoordination.Note;
    public Guid UserId { get; set; } = assessmentCoordination.UserId;
    public Guid CoordinationId { get; set; } = assessmentCoordination.CoordinationId;
    public UserResponse User { get; set; } = new UserResponse(assessmentCoordination.User);
    public CoordinationResponse Coordination { get; set; } = new CoordinationResponse(assessmentCoordination.Coordination);
}
