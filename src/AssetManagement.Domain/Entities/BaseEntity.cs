using MassTransit;
using System.ComponentModel.DataAnnotations;

namespace AssetManagement.Domain.Entities;
public class BaseEntity
{
    [Key]
    public Guid Id { get; set; } = NewId.Next().ToGuid();
    public Guid? CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public Guid? LastModificatedBy { get; set; }
    public DateTime LastModificatedDate { get; set; } = DateTime.UtcNow;

    // For soft delete
    public Guid? DeletedBy { get; set; }
    public DateTime? DeletedDate { get; set; }
}
