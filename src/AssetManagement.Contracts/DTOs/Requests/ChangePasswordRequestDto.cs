namespace AssetManagement.Contracts.DTOs.Requests;

public class ChangePasswordRequestDto
{
    public string? Password { get; set; }
    public required string NewPassword { get; set; }
}
