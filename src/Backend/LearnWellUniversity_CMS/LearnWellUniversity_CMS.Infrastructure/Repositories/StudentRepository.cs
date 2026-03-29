using LearnWellUniversity_CMS.Application.Abstractions;
using LearnWellUniversity_CMS.Application.Repositories;
using LearnWellUniversity_CMS.Domain.Models;
using LearnWellUniversity_CMS.Infrastructure.DataAccess;
using System.Data.Entity;

namespace LearnWellUniversity_CMS.Infrastructure.Repositories;

public class StudentRepository(AppDbContext dbContext, ICurrentUser currentUser, IMappingHelper mappingHelper) : Repository<Student>(dbContext, currentUser, mappingHelper), IStudentRepository
{
    public async Task<Guid> GetIdByUserIdAsync(Guid userId)
        => (await _dbContext.Students.FirstOrDefaultAsync(s => s.UserId == userId))?.StudentId ?? Guid.Empty;
}
