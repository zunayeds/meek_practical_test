using System.ComponentModel.DataAnnotations;

namespace LearnWellUniversity_CMS.Application.DTOs.Requests;

public class CreateUpdateStudentRequest
{
    [StringLength(50)]
    public required string FirstName { get; set; }

    [StringLength(50)]
    public required string LastName { get; set; }

    [EmailAddress]
    [StringLength(50)]
    public required string EmailAddress { get; set; }

    [Phone]
    [StringLength(20)]
    public required string PhoneNumber { get; set; }

    [StringLength(200)]
    public required string Address { get; set; }
}
