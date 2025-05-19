using System.ComponentModel.DataAnnotations;
using MassTransit;

namespace AssetManagement.Domain.Entities;

public class BaseEntity
{
    [Key] public Guid Id { get; set; }

    public Guid? CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public Guid? LastModificatedBy { get; set; }
    public DateTime LastModificatedDate { get; set; }

    // For soft delete
    public Guid? DeletedBy { get; set; }
    public DateTime? DeletedDate { get; set; }
    public bool? IsDeleted { get; set; }
}