namespace AssetManagement.Contracts.DTOs
{
    public class ReturnRequestDto
    {
        public Guid Id { get; set; }
        public int No { get; set; }
        public string? AssetCode { get; set; }
        public string? AssetName { get; set; }
        public string? RequestedBy { get; set; }
        public string? AssignedDate { get; set; }
        public string? AcceptedBy { get; set; }
        public string? ReturnedDate { get; set; }
        public string? State { get; set; }
    }
}