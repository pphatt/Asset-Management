namespace AssetManagement.Contracts.Common;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public required string Message { get; set; }
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
}