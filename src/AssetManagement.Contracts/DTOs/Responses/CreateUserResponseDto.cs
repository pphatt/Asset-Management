using AssetManagement.Contracts.Enums;

namespace AssetManagement.Contracts.DTOs.Responses;

public class CreateUserResponseDto
{
    public string StaffCode { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public LocationDtoEnum Location { get; set; }
}
