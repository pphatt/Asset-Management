namespace AssetManagement.Contracts.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string? StaffCode { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Username { get; set; }
        public DateTimeOffset JoinedDate { get; set; }
        public string? Type { get; set; }
        public bool? IsPasswordUpdated { get; set; }
        public bool? HasAssignments { get; set; }
    }
}