using LearnWellUniversity_CMS.Application.Abstractions;
using LearnWellUniversity_CMS.Application.DTOs.Responses;
using LearnWellUniversity_CMS.Application.Repositories;
using LearnWellUniversity_CMS.Domain.Models;
using LearnWellUniversity_CMS.Infrastructure.DataAccess;
using LearnWellUniversity_CMS.Shared.Exceptions;
using LearnWellUniversity_CMS.Shared.Utilities;
using Microsoft.EntityFrameworkCore;

namespace LearnWellUniversity_CMS.Infrastructure.Repositories;

public class StudentRepository(AppDbContext dbContext, IMappingHelper mappingHelper, ICurrentUser currentUser) : Repository<Student>(dbContext, mappingHelper), IStudentRepository
{
    public async Task<Guid?> GetIdByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => (await _dbContext.Students.FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken))?.StudentId ?? Guid.Empty;

    public async Task<List<string>> GetOtherStudentNamesByClassIdAsync(Guid classId, CancellationToken cancellationToken = default)
    {
        var studentId = currentUser.StudentId ?? throw new NotFoundException(ErrorMessageGenerator.NotFoundErrorMessage<Student>());

        var students = await _dbContext.StudentClasses
            .Where(sc => sc.ClassId == classId)
            .Select(sc => new
            {
                sc.StudentId,
                sc.Student.FirstName,
                sc.Student.LastName
            })
            .ToListAsync();

        return students
            .Where(w => w.StudentId != studentId)
            .Select(s => string.Join(" ", s.FirstName, s.LastName))
            .ToList();
    }

    public async Task<List<StudentClassResponse>> GetClassesAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.StudentClasses
            .Where(w => w.StudentId == studentId)
            .Select(s => new StudentClassResponse
            {
                ClassId = s.ClassId,
                Name = s.Class.Name,
                AssignedAt = s.AssignedAt,
                AssignedBy = s.AssignedByUser.FirstName + " " + s.AssignedByUser.LastName
            })
            .ToListAsync();
    }

    public async Task<Guid> GetUserIdAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Students
            .Where(w => w.StudentId == studentId)
            .Select(s => s.UserId)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
