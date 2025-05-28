using System.ComponentModel.DataAnnotations.Schema;
using AssetManagement.Domain.Enums;

namespace AssetManagement.Domain.Entities
{
    public class User : BaseEntity
    {
        public string StaffCode { get; set; } = null!;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public required string Username { get; set; }
        public required string Password { get; set; }
        public bool IsPasswordUpdated { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTimeOffset JoinedDate { get; set; }
        public UserType Type { get; set; }
        public Location Location { get; set; }
        public Gender Gender { get; set; }
        public bool IsActive { get; set; } = true;

        [InverseProperty(nameof(Assignment.Assignee))]
        public ICollection<Assignment> Assignments = new List<Assignment>();
    }
}