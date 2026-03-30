using LearnWellUniversity_CMS.Application.Abstractions;
using LearnWellUniversity_CMS.Application.Repositories;
using LearnWellUniversity_CMS.Domain.Models;
using LearnWellUniversity_CMS.Infrastructure.DataAccess;
using LearnWellUniversity_CMS.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LearnWellUniversity_CMS.Infrastructure.Repositories;

public class ClassRepository(AppDbContext dbContext, IMappingHelper mappingHelper, ILogger<ClassRepository> logger) : Repository<Class>(dbContext, mappingHelper), IClassRepository
{
    public async Task AddRemoveStudentsAsync(Guid classId, List<Guid> addStudentIds, List<Guid> removeStudentIds, CancellationToken cancellationToken = default)
    {
        if (addStudentIds.Count + removeStudentIds.Count == 0) return;

        if (removeStudentIds.Count > 0)
        {
            logger.LogInformation("Removing provided students from class with id '{classId}'", classId);
            removeStudentIds = removeStudentIds.Distinct().ToList();
            await _dbContext.StudentClasses
                .Where(w => removeStudentIds.Contains(w.StudentId))
                .ExecuteDeleteAsync(cancellationToken);
            logger.LogWarning("Removed provided students from class with id '{classId}'", classId);
        }

        if (addStudentIds.Count > 0)
        {
            addStudentIds = addStudentIds.Distinct().ToList();

            var existingStudentIds = await _dbContext.StudentClasses
                .Where(sc => sc.ClassId == classId && addStudentIds.Contains(sc.StudentId))
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

                logger.LogInformation("Enrolling provided students in class with id '{classId}'", classId);
                var newStudents = newStudentIds
                    .Select(studentId => new StudentClass
                    {
                        StudentId = studentId,
                        ClassId = classId
                    });
                await _dbContext.StudentClasses.AddRangeAsync(newStudents, cancellationToken);
                logger.LogInformation("Enrolled provided students in class with id '{classId}'", classId);
            }
        }
    }
}
