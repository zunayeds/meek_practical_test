using System.ComponentModel.DataAnnotations;

namespace LearnWellUniversity_CMS.Application.DTOs.Requests;

public class CreateClassRequest
{
    [StringLength(20)]
    public required string Name { get; set; }

    [StringLength(100)]
    public string? Description { get; set; }
}
