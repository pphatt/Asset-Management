namespace AssetManagement.Contracts.DTOs.Responses;

public class LoginResponse
{
    public required string AccessToken { get; set; }
    public required UserDto UserInfo { get; set; }
}
