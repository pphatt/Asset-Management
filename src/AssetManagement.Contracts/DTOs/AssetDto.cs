namespace AssetManagement.Contracts.DTOs;

public class AssetDto
{
    public Guid Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? State { get; set; }
    public string? CategoryName { get; set; }
}