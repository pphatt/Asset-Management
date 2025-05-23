namespace AssetManagement.Contracts.DTOs
{
    public class UserDetailsDto
    {
        public string? StaffCode { get; set; } = string.Empty;
        public string? FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; } = string.Empty;
        public string? Username { get; set; } = string.Empty;
        public string? DateOfBirth { get; set; } = DateTime.Now.ToString("dd/MM/yyyy");
        public string? JoinedDate { get; set; } = DateTime.Now.ToString("dd/MM/yyyy");
        public int? Gender { get; set; }
        public int? Type { get; set; }
        public int? Location { get; set; }
    }
}