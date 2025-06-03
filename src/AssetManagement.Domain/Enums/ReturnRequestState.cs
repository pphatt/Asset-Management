using System.ComponentModel.DataAnnotations;

namespace AssetManagement.Contracts.Enums
{
    public enum ReturnRequestState
    {
        [Display(Name = "Completed")]
        Completed,
        [Display(Name = "Waiting for returning")]
        WaitingForReturning,
    }
}