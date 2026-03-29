erDiagram

AspNetUsers {
    uuid    Id                  PK
    varchar UserName
    varchar Email
    boolean EmailConfirmed
    varchar PasswordHash
    varchar FirstName
    varchar LastName
    timestamptz CreatedAt
    uuid    CreatedBy           FK
    timestamptz ModifiedAt
    uuid    ModifiedBy          FK
}

Students {
    uuid    UserId              PK
    uuid    StudentId
    varchar FirstName
    varchar LastName
    varchar EmailAddress
    varchar PhoneNumber
    varchar Address
    timestamptz CreatedAt
    uuid    CreatedBy           FK
    timestamptz ModifiedAt
    uuid    ModifiedBy          FK
    boolean IsActive
}

Courses {
    uuid    CourseId            PK
    varchar Name
    varchar Description
    timestamptz CreatedAt
    uuid    CreatedBy           FK
    timestamptz ModifiedAt
    uuid    ModifiedBy          FK
    boolean IsActive
}

Classes {
    uuid    ClassId             PK
    varchar Name
    varchar Description
    timestamptz CreatedAt
    uuid    CreatedBy           FK
    timestamptz ModifiedAt
    uuid    ModifiedBy          FK
    boolean IsActive
}



CourseClasses {
    uuid        CourseClassId   PK
    uuid        CourseId        FK
    uuid        ClassId         FK
    timestamptz AssignedAt
    uuid        AssignedBy      FK
}

StudentCourses {
    uuid        StudentCourseId PK
    uuid        StudentId       FK
    uuid        CourseId        FK
    timestamptz AssignedAt
    uuid        AssignedBy      FK
}

StudentClasses {
    uuid        StudentClassId  PK
    uuid        StudentId       FK
    uuid        ClassId         FK
    timestamptz AssignedAt
    uuid        AssignedBy      FK
}

AspNetRoles {
    uuid    Id                  PK
    varchar Name
    varchar NormalizedName
}

AspNetUserRoles {
    uuid UserId                 FK
    uuid RoleId                 FK
}

AspNetUsers ||--|| Students : "identity"
AspNetUsers ||--o{ AspNetUserRoles : "has"

AspNetRoles ||--o{ AspNetUserRoles : "assigned via"
Courses     ||--o{ CourseClasses  : "includes"
Classes     ||--o{ CourseClasses  : "belongs to"
Students    ||--o{ StudentCourses : "enrolled in"
Courses     ||--o{ StudentCourses : "has"
Students    ||--o{ StudentClasses : "attends"
Classes     ||--o{ StudentClasses : "has"

AspNetUsers |o..o{ AspNetUsers : "created/modified by"
AspNetUsers ||..o{ Courses       : "created/modified by"
AspNetUsers ||..o{ Classes       : "created/modified by"
AspNetUsers ||..o{ Students      : "created/modified by"

AspNetUsers ||..o{ CourseClasses  : "assigned by"
AspNetUsers ||..o{ StudentCourses : "assigned by"
AspNetUsers ||..o{ StudentClasses : "assigned by"