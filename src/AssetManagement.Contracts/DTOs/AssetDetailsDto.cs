namespace AssetManagement.Contracts.DTOs;

public class AssetDetailsDto: AssetDto
{
    public DateTimeOffset InstalledDate { get; set; }
    public string Location { get; set; }
    public string Specification { get; set; }
    public List<History> History { get; set; }
    
    public AssetDetailsDto(Guid id, string code, string name, string state, string categoryName,
    DateTimeOffset installedDate, string location, string specification)
    {
        Id = id;
        Code = code;
        Name = name;
        State = state;
        CategoryName = categoryName;
        InstalledDate = installedDate;
        Location = location;
        Specification = specification;
        History = new List<History>();
    }
}

public class History
{
    public DateTimeOffset AssignedDate { get; set; }
    public UserDto? AssignedTo { get; set; }
    public UserDto? AssignedBy { get; set; }
    public DateTimeOffset ReturnedDate { get; set; }
}