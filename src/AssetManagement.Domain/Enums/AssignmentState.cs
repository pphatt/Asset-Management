using System.ComponentModel.DataAnnotations;

namespace AssetManagement.Domain.Enums
{
    public enum AssignmentState
    {
        [Display(Name = "Waiting for acceptance")]
        WaitingForAcceptance,
        [Display(Name = "Accepted")]
        Accepted,
        [Display(Name = "Declined")]
        Declined,
        [Display(Name = "Returned")]
        Returned
    }
}