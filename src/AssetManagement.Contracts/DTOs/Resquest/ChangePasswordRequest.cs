namespace AssetManagement.Contracts.DTOs.Resquest;

public class ChangePasswordRequest
{
    public required string Password { get; set; }
    public required string NewPassword { get; set; }
}
