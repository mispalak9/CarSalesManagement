namespace CarSalesManagementAPI.Application.DTOs;

public class ApiErrorResponse
{
    public bool Success { get; set; } = false;
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public string? StackTrace { get; set; }
}
