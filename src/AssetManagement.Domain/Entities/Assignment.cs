using AssetManagement.Domain.Enums;

namespace AssetManagement.Domain.Entities
{
    public class Assignment : BaseEntity
    {
        public Guid AssetId { get; set; }
        public Asset Asset { get; set; } = null!;

        public Guid AssignorId { get; set; }
        public User Assignor { get; set; } = null!;

        public Guid AssigneeId { get; set; }
        public User Assignee { get; set; } = null!;

        public DateTimeOffset AssignedDate { get; set; }
        public string? Note { get; set; }
        public AssignmentState State { get; set; }
    }
}