namespace AssetManagement.Contracts.DTOs.Requests;

public class ChangePasswordRequest
{
    public string? Password { get; set; }
    public required string NewPassword { get; set; }
}
