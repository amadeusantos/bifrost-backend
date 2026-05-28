namespace Bifrost.Core.Exception.Discipline;

public class DisciplineStudentDuplicatedException()
    : CoreException(409, "Discipline students must not have duplicate users");
