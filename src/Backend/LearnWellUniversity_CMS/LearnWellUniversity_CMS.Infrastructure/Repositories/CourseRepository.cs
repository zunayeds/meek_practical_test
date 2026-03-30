using LearnWellUniversity_CMS.Application.Abstractions;
using LearnWellUniversity_CMS.Application.Repositories;
using LearnWellUniversity_CMS.Domain.Models;
using LearnWellUniversity_CMS.Infrastructure.DataAccess;
using LearnWellUniversity_CMS.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace LearnWellUniversity_CMS.Infrastructure.Repositories;

public class CourseRepository(AppDbContext dbContext, IMappingHelper mappingHelper) : Repository<Course>(dbContext, mappingHelper), ICourseRepository
{
    public async Task AddRemoveStudentsAsync(Guid courseId, List<Guid> addStudentIds, List<Guid> removeStudentIds, CancellationToken cancellationToken = default)
    {
        if (addStudentIds.Count + removeStudentIds.Count == 0) return;

        var classIds = await _dbContext.Classes
            .Where(w => w.CourseClasses.Any(w => w.CourseId == courseId))
            .Select(s => s.ClassId)
            .ToListAsync();

        if (removeStudentIds.Count > 0)
        {
            removeStudentIds = removeStudentIds.Distinct().ToList();

            await _dbContext.StudentClasses
                .Where(w => classIds.Contains(w.ClassId) && removeStudentIds.Contains(w.StudentId))
                .ExecuteDeleteAsync(cancellationToken);

            await _dbContext.StudentCourses
                .Where(w => removeStudentIds.Contains(w.StudentId))
                .ExecuteDeleteAsync(cancellationToken);
        }

        if (addStudentIds.Count > 0)
        {
            addStudentIds = addStudentIds.Distinct().ToList();

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
                if (await _dbContext.Students.CountAsync(w => newStudentIds.Contains(w.StudentId)) < newStudentIds.Count)
                {
                    throw new NotFoundException("Some of the student id(s) are not valid");
                }

                var newStudentsInCourse = newStudentIds
                    .Select(studentId => new StudentCourse
                    {
                        StudentId = studentId,
                        CourseId = courseId,
                    });
                await _dbContext.StudentCourses.AddRangeAsync(newStudentsInCourse, cancellationToken);

                var newStudentsInClass = newStudentIds
                    .SelectMany(studentId => classIds, (studentId, classId) => new StudentClass
                    {
                        StudentId = studentId,
                        ClassId = classId
                    });
                await _dbContext.StudentClasses.AddRangeAsync(newStudentsInClass, cancellationToken);
            }
        }
    }

    public async Task AddRemoveClassesAsync(Guid courseId, List<Guid> addClassIds, List<Guid> removeClassIds, CancellationToken cancellationToken = default)
    {
        if (addClassIds.Count + removeClassIds.Count == 0) return;

        if (removeClassIds.Count > 0)
        {
            removeClassIds = removeClassIds.Distinct().ToList();
            await _dbContext.CourseClasses
                .Where(w => removeClassIds.Contains(w.ClassId))
                .ExecuteDeleteAsync(cancellationToken);
        }

        if (addClassIds.Count > 0)
        {
            addClassIds = addClassIds.Distinct().ToList();

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
                if (await _dbContext.Classes.CountAsync(w => newClassIds.Contains(w.ClassId)) < newClassIds.Count)
                {
                    throw new NotFoundException("Some of the class id(s) are not valid");
                }

                var newClasses = newClassIds
                    .Select(classId => new CourseClass
                    {
                        CourseId = courseId,
                        ClassId = classId
                    });
                await _dbContext.CourseClasses.AddRangeAsync(newClasses, cancellationToken);
            }
        }
    }
}
