namespace LearnWellUniversity_CMS.Application.DTOs.Responses;

public class PaginatedResponse<T>
{
    public int TotalRecords { get; set; }
    public List<T> Records { get; set; } = [];
}
