namespace AssetManagement.Contracts.DTOs.Responses;

public class LoginResponseDto
{
    public required string AccessToken { get; set; }
    public required UserDto UserInfo { get; set; }
}
