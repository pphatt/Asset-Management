namespace AssetManagement.Contracts.DTOs.Requests
{
    public class UpdateUserRequest
    {
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public DateTimeOffset? JoinedDate { get; set; }
        public string? Type { get; set; }
    }
}
