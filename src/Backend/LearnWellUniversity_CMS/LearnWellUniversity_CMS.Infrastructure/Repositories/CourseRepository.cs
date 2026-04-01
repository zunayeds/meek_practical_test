using LearnWellUniversity_CMS.Application.Abstractions;
using LearnWellUniversity_CMS.Application.Repositories;
using LearnWellUniversity_CMS.Domain.Models;
using LearnWellUniversity_CMS.Infrastructure.DataAccess;
using LearnWellUniversity_CMS.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LearnWellUniversity_CMS.Infrastructure.Repositories;

public class CourseRepository(AppDbContext dbContext, IMappingHelper mappingHelper, ILogger<CourseRepository> logger) : Repository<Course>(dbContext, mappingHelper), ICourseRepository
{
    public async Task AddRemoveStudentsAsync(Guid courseId, List<Guid> addStudentIds, List<Guid> removeStudentIds, CancellationToken cancellationToken = default)
    {
        if (addStudentIds.Count + removeStudentIds.Count == 0) return;

        var classIds = await _dbContext.Classes
            .Where(w => w.CourseClasses.Any(w => w.CourseId == courseId))
            .Select(s => s.ClassId)
            .ToListAsync(cancellationToken);

        if (removeStudentIds.Count > 0)
        {
            removeStudentIds = [.. removeStudentIds.Distinct()];
            var otherCourseClassesByStudent = await _dbContext.StudentCourses
                .Where(w => removeStudentIds.Contains(w.StudentId) && w.CourseId != courseId)
                .GroupBy(g => g.StudentId)
                .Select(s => new
                {
                    StudentId = s.Key,
                    ClassIds = s
                        .SelectMany(sc => sc.Course.CourseClasses.Select(cc => cc.ClassId))
                        .Distinct()
                        .ToList()
                })
                .ToDictionaryAsync(x => x.StudentId, x => x.ClassIds, cancellationToken);

            var removableStudentClasses = await _dbContext.StudentClasses
                .Where(w => removeStudentIds.Contains(w.StudentId) && classIds.Contains(w.ClassId))
                .Select(s => new
                {
                    s.StudentClassId,
                    s.StudentId,
                    s.ClassId
                })
                .ToListAsync(cancellationToken);

            var studentClassIdsToDelete = removableStudentClasses
                .Where(w => !otherCourseClassesByStudent.TryGetValue(w.StudentId, out var keepClassIds) || !keepClassIds.Contains(w.ClassId))
                .Select(s => s.StudentClassId)
                .ToList();

            if (studentClassIdsToDelete.Count > 0)
            {
                logger.LogInformation("Removing provided students from classes in course with id '{course}'", courseId);
                await _dbContext.StudentClasses
                    .Where(w => studentClassIdsToDelete.Contains(w.StudentClassId))
                    .ExecuteDeleteAsync(cancellationToken);
                logger.LogWarning("Removed provided students from classes in course with id '{course}'", courseId);
            }

            logger.LogInformation("Removing provided students from course with id '{course}'", courseId);
            await _dbContext.StudentCourses
                .Where(w => removeStudentIds.Contains(w.StudentId))
                .ExecuteDeleteAsync(cancellationToken);
            logger.LogWarning("Removed provided students from course with id '{course}'", courseId);
        }

        if (addStudentIds.Count > 0)
        {
            addStudentIds = [.. addStudentIds.Distinct()];

            var existingStudentIds = await _dbContext.StudentCourses
                .Where(sc => sc.CourseId == courseId && addStudentIds.Contains(sc.StudentId))
                .Select(sc => sc.StudentId)
                .ToListAsync(cancellationToken);

            var existingSet = existingStudentIds.ToHashSet();

            var newStudentIds = addStudentIds
                .Where(id => !existingSet.Contains(id))
                .ToList();

            if (newStudentIds.Count > 0)
            {
                if (await _dbContext.Students.CountAsync(w => newStudentIds.Contains(w.StudentId), cancellationToken) < newStudentIds.Count)
                {
                    throw new NotFoundException("Some of the student id(s) are not valid");
                }

                logger.LogInformation("Enrolling provided students in course with id '{courseId}'", courseId);
                var newStudentsInCourse = newStudentIds
                    .Select(studentId => new StudentCourse
                    {
                        StudentId = studentId,
                        CourseId = courseId,
                    });
                await _dbContext.StudentCourses.AddRangeAsync(newStudentsInCourse, cancellationToken);
                logger.LogInformation("Enrolled provided students in course with id '{courseId}'", courseId);

                var existingStudentClasses = await _dbContext.StudentClasses
                    .Where(w => newStudentIds.Contains(w.StudentId) && classIds.Contains(w.ClassId))
                    .Select(s => new { s.StudentId, s.ClassId })
                    .ToListAsync(cancellationToken);

                var existingPairSet = existingStudentClasses
                    .Select(x => (x.StudentId, x.ClassId))
                    .ToHashSet();

                var newStudentsInClass = newStudentIds
                    .SelectMany(studentId => classIds, (studentId, classId) => new { studentId, classId })
                    .Where(x => !existingPairSet.Contains((x.studentId, x.classId)))
                    .Select(x => new StudentClass
                    {
                        StudentId = x.studentId,
                        ClassId = x.classId
                    })
                    .ToList();

                if (newStudentsInClass.Count > 0)
                {
                    logger.LogInformation("Enrolling provided students in classes of course with id '{courseId}'", courseId);
                    await _dbContext.StudentClasses.AddRangeAsync(newStudentsInClass, cancellationToken);
                    logger.LogInformation("Enrolled provided students in classes of course with id '{courseId}'", courseId);
                }
            }
        }
    }

    public async Task AddRemoveClassesAsync(Guid courseId, List<Guid> addClassIds, List<Guid> removeClassIds, CancellationToken cancellationToken = default)
    {
        if (addClassIds.Count + removeClassIds.Count == 0) return;

        if (removeClassIds.Count > 0)
        {
            logger.LogInformation("Removing provided classes from course with id '{course}'", courseId);
            removeClassIds = [.. removeClassIds.Distinct()];
            await _dbContext.CourseClasses
                .Where(w => removeClassIds.Contains(w.ClassId))
                .ExecuteDeleteAsync(cancellationToken);
            logger.LogWarning("Removed provided classes from course with id '{course}'", courseId);
        }

        if (addClassIds.Count > 0)
        {
            addClassIds = [.. addClassIds.Distinct()];

            var existingClassIds = await _dbContext.CourseClasses
                .Where(sc => sc.CourseId == courseId && addClassIds.Contains(sc.ClassId))
                .Select(sc => sc.ClassId)
                .ToListAsync(cancellationToken);

            var existingSet = existingClassIds.ToHashSet();

            var newClassIds = addClassIds
                .Where(id => !existingSet.Contains(id))
                .ToList();

            if (newClassIds.Count > 0)
            {
                if (await _dbContext.Classes.CountAsync(w => newClassIds.Contains(w.ClassId), cancellationToken) < newClassIds.Count)
                {
                    throw new NotFoundException("Some of the class id(s) are not valid");
                }

                logger.LogInformation("Adding provided classes in course with id '{courseId}'", courseId);
                var newClasses = newClassIds
                    .Select(classId => new CourseClass
                    {
                        CourseId = courseId,
                        ClassId = classId
                    });
                await _dbContext.CourseClasses.AddRangeAsync(newClasses, cancellationToken);
                logger.LogInformation("Added provided classes in course with id '{courseId}'", courseId);

                var enrolledStudentIds = await _dbContext.StudentCourses
                    .Where(sc => sc.CourseId == courseId)
                    .Select(sc => sc.StudentId)
                    .Distinct()
                    .ToListAsync(cancellationToken);

                if (enrolledStudentIds.Count > 0)
                {
                    var existingStudentClasses = await _dbContext.StudentClasses
                        .Where(sc => enrolledStudentIds.Contains(sc.StudentId) && newClassIds.Contains(sc.ClassId))
                        .Select(sc => new { sc.StudentId, sc.ClassId })
                        .Select(sc => new { sc.StudentId, sc.ClassId })
                        .ToHashSetAsync(cancellationToken);

                    var newStudentClasses = enrolledStudentIds
                        .SelectMany(studentId => newClassIds, (studentId, classId) => new { studentId, classId })
                        .Where(x => !existingStudentClasses.Contains(new { StudentId = x.studentId, ClassId = x.classId }))
                        .Select(x => new StudentClass
                        {
                            StudentId = x.studentId,
                            ClassId = x.classId
                        })
                        .ToList();

                    if (newStudentClasses.Count > 0)
                    {
                        logger.LogInformation("Adding enrolled students to new classes in course with id '{courseId}'", courseId);
                        await _dbContext.StudentClasses.AddRangeAsync(newStudentClasses, cancellationToken);
                        logger.LogInformation("Added enrolled students to new classes in course with id '{courseId}'", courseId);
                    }
                }
            }
        }
    }
}
