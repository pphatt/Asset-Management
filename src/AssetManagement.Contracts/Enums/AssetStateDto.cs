using System.ComponentModel.DataAnnotations;

namespace AssetManagement.Contracts.Enums;

public enum AssetStateDto
{
    [Display(Name = "Available")]
    Available = 1,
    [Display(Name = "Not available")]
    NotAvailable = 2,
}