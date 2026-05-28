namespace Bifrost.Core.Exception.AcademicCenter;

public class AcademicCenterMemberDuplicatedException()
    : CoreException(409, "Academic center members must not have duplicate users");
