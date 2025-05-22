using AssetManagement.Contracts.Enums;

namespace AssetManagement.Contracts.DTOs.Resquest;

public class CreateUserRequestDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string DateOfBirth { get; set; } = string.Empty;
    public string JoinedDate { get; set; } = string.Empty;
    public UserTypeDtoEnum Type { get; set; }
    public GenderDtoEnum Gender { get; set; }
}