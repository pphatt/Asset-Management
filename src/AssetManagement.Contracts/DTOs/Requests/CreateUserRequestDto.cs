using AssetManagement.Contracts.Enums;

namespace AssetManagement.Contracts.DTOs.Requests;

public class CreateUserRequestDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string DateOfBirth { get; set; } = string.Empty;
    public string JoinedDate { get; set; } = string.Empty;
    public UserTypeDto Type { get; set; }
    public GenderDto Gender { get; set; }
}