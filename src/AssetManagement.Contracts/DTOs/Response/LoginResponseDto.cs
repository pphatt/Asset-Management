namespace AssetManagement.Contracts.DTOs.Response;

public class LoginResponseDto
{
    public required string AccessToken { get; set; }
    public required UserDto UserInfo { get; set; }
}
