using AutoMapper;
using LearnWellUniversity_CMS.Application.DTOs.Requests;
using LearnWellUniversity_CMS.Application.DTOs.Responses;
using LearnWellUniversity_CMS.Domain.Models;

namespace LearnWellUniversity_CMS.Application.Utilities;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Course, CourseResponseBase>();
        CreateMap<Course, CourseResponse>();
        CreateMap<CreateUpdateCourseRequest, Course>();
    }
}
