using System.ComponentModel.DataAnnotations;

namespace AssetManagement.Domain.Enums;

public enum AssetState
{
    [Display(Name = "Assigned")]
    Assigned,
    [Display(Name = "Available")]
    Available,
    [Display(Name = "Not available")]
    NotAvailable,
    [Display(Name = "Waiting for recycling")]
    WaitingForRecycling,
    [Display(Name = "Recycled")]
    Recycled,
}