namespace LearnWellUniversity_CMS.Application.DTOs.Requests;

public class GetByFiltersBaseRequest
{
    public string Name { get; set; } = string.Empty;
    public int Page { get; set; }
    public int? PageSize { get; set; }
}
