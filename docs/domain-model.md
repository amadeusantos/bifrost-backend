# Domain Model

Class diagram of the core domain entities (`Core/Domain`). DTOs and records are omitted — this shows only the runtime model that services operate on.

```mermaid
classDiagram
    direction TB

    class UserProfileEnum {
        <<enumeration>>
        Professor
        Student
    }

    class UserMinimal {
        +Guid Id
        +string? FullName
        +string Email
        +string? GoogleOpenid
        +UserProfileEnum Profile
        +bool IsAdmin
        +Guid? CourseId
    }

    class User {
        +Course? Course
    }

    class Course {
        +Guid Id
        +string Name
        +string Code
    }

    class AssessmentSeasonMinimal {
        +Guid Id
        +string Period
        +Guid CourseId
        +DateTime? StartDateTime
        +DateTime? EndDateTime
    }

    class AssessmentSeason {
        +Course Course
    }

    class Coordination {
        +Guid Id
        +string Name
        +decimal? AvgScore
        +Guid AssessmentSeasonId
    }

    class CoordinationMember {
        +Guid Id
        +string Role
        +Guid UserId
        +Guid CoordinationId
    }

    class AcademicCenter {
        +Guid Id
        +string Name
        +decimal? AvgScore
        +Guid AssessmentSeasonId
    }

    class AcademicCenterMember {
        +Guid Id
        +string Role
        +Guid UserId
        +Guid AcademicCenterId
    }

    class Discipline {
        +Guid Id
        +string Name
        +string Code
        +Guid AssessmentSeasonId
        +Guid ProfessorId
        +decimal? AvgScore
    }

    %% Inheritance
    UserMinimal <|-- User
    AssessmentSeasonMinimal <|-- AssessmentSeason

    %% User
    UserMinimal --> UserProfileEnum

    %% Course associations
    User "0..1" --> "1" Course
    AssessmentSeason "1" --> "1" Course

    %% Coordination
    Coordination "1" --> "1" AssessmentSeasonMinimal
    Coordination "1" *-- "1..*" CoordinationMember
    CoordinationMember "1" --> "1" UserMinimal

    %% Academic Center
    AcademicCenter "1" --> "1" AssessmentSeasonMinimal
    AcademicCenter "1" *-- "1..*" AcademicCenterMember
    AcademicCenterMember "1" --> "1" UserMinimal

    %% Discipline
    Discipline "1" --> "1" AssessmentSeasonMinimal
    Discipline "1" --> "1" UserMinimal : professor
    Discipline "1" --> "*" UserMinimal : students
```

## Notes

- **`UserMinimal`** is the base projection of a user used by aggregates (`CoordinationMember`, `AcademicCenterMember`, `Discipline`). **`User`** is the full form that includes the resolved `Course`.
- **`AssessmentSeasonMinimal`** is the base projection used by aggregates. **`AssessmentSeason`** is the full form that includes the resolved `Course`.
- `Coordination` and `AcademicCenter` carry an `AvgScore` populated by the scoring pipeline.
- `Discipline` references the professor via `UserMinimal` and a separate `Students` list of `UserMinimal`.
