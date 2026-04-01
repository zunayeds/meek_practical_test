using System.ComponentModel.DataAnnotations;

namespace LearnWellUniversity_CMS.Application.DTOs.Requests;

public class CreateUpdateStudentRequest : CreateUpdateUserRequest
{
    [StringLength(200)]
    public required string Address { get; set; }
}