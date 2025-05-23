namespace AssetManagement.Contracts.DTOs;

public class AssetDetailsDto: AssetDto
{
    public DateTimeOffset InstallDate { get; set; }
    public string Location { get; set; }
    public string Specification { get; set; }
    public List<History> History { get; set; }
    
    public AssetDetailsDto(Guid id, string assetCode, string name, string state, string categoryName,
    DateTimeOffset installDate, string location, string specification)
    {
        Id = id;
        AssetCode = assetCode;
        Name = name;
        State = state;
        CategoryName = categoryName;
        InstallDate = installDate;
        Location = location;
        Specification = specification;
        History = new List<History>();
    }
}

public class History
{
    public DateTimeOffset AssignDate { get; set; }
    public UserDto AssignedTo { get; set; }
    public UserDto AssignedBy { get; set; }
    public DateTimeOffset ReturnedDate { get; set; }
}