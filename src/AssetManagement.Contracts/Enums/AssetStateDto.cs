using System.ComponentModel.DataAnnotations;

namespace AssetManagement.Contracts.Enums
{
    public enum AssetStateDto
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
}