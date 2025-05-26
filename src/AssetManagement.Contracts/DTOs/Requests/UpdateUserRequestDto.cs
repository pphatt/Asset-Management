using AssetManagement.Contracts.Enums;

namespace AssetManagement.Contracts.DTOs.Requests
{
    public class UpdateUserRequestDto
    {
        public GenderDto? Gender { get; set; }
        public string? DateOfBirth { get; set; }
        public string? JoinedDate { get; set; }
        public UserTypeDto? Type { get; set; }
    }
}
