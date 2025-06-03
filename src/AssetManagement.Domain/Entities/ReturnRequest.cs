using AssetManagement.Contracts.Enums;

namespace AssetManagement.Domain.Entities
{
    public class ReturnRequest : BaseEntity
    {
        public Guid AssignmentId { get; set; }
        public Assignment Assignment { get; set; } = null!;

        public Guid RequesterId { get; set; }
        public User Requester { get; set; } = null!;

        public Guid AcceptorId { get; set; }
        public User Acceptor { get; set; } = null!;

        public DateTimeOffset ReturnedDate { get; set; }
        public ReturnRequestState State { get; set; }
    }
}