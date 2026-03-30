using LearnWellUniversity_CMS.Application.Abstractions;
using LearnWellUniversity_CMS.Application.DTOs.Responses;
using LearnWellUniversity_CMS.Domain.Models;

namespace LearnWellUniversity_CMS.Application.Repositories;

public interface IStudentRepository : IRepository<Student>
{
    public Task<Guid> GetIdByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<List<string>> GetOtherStudentNamesByClassIdAsync(Guid classId, CancellationToken cancellationToken = default);
    Task<List<StudentClassResponse>> GetClassesAsync(Guid studentId, CancellationToken cancellationToken = default);
}
