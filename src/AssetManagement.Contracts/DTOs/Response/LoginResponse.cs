namespace AssetManagement.Contracts.DTOs.Response;

public class LoginResponse
{
    public required string AccessToken { get; set; }
    public required UserDto UserInfo { get; set; }
}
