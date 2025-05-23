namespace AssetManagement.Contracts.DTOs.Requests;

public class ChangePasswordRequest
{
    public required string Password { get; set; }
    public required string NewPassword { get; set; }
}
