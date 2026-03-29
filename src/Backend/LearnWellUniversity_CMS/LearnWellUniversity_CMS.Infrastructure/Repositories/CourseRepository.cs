using LearnWellUniversity_CMS.Application.Abstractions;
using LearnWellUniversity_CMS.Application.Repositories;
using LearnWellUniversity_CMS.Domain.Models;
using LearnWellUniversity_CMS.Infrastructure.DataAccess;

namespace LearnWellUniversity_CMS.Infrastructure.Repositories;

public class CourseRepository(AppDbContext dbContext, ICurrentUser currentUser) : Repository<Course>(dbContext, currentUser), ICourseRepository
{

}
