using AssetManagement.Contracts.Enums;

namespace AssetManagement.Contracts.DTOs.Requests
{
    public class UpdateUserRequestDto
    {
        public GenderDtoEnum? Gender { get; set; }
        public string? DateOfBirth { get; set; }
        public string? JoinedDate { get; set; }
        public UserTypeDtoEnum? Type { get; set; }
    }
}
