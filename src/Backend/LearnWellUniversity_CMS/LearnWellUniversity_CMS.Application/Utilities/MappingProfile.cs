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
        CreateMap<Course, CreatedEntityResponse>()
            .ForMember(dest => dest.Id, src => src.MapFrom(s => s.CourseId));

        CreateMap<Class, ClassResponseBase>();
        CreateMap<Class, ClassResponse>();
        CreateMap<CreateUpdateClassRequest, Class>();
        CreateMap<Class, CreatedEntityResponse>()
            .ForMember(dest => dest.Id, src => src.MapFrom(s => s.ClassId));

        CreateMap<Student, StudentResponseBase>();
        CreateMap<Student, StudentResponse>();
        CreateMap<CreateUpdateStudentRequest, Student>();
        CreateMap<Student, CreateStudentResponse>()
            .ForMember(dest => dest.Id, src => src.MapFrom(s => s.StudentId));
        CreateMap<Student, StudentOwnResponse>();

        CreateMap<CreateUpdateUserRequest, ApplicationUser>();
    }
}
