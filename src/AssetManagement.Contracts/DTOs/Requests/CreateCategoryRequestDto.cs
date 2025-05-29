using System.ComponentModel.DataAnnotations;

namespace AssetManagement.Contracts.DTOs.Requests
{
    public class CreateCategoryRequestDto
    {
        public string Name { get; set; } = null!;

        public string Prefix { get; set; } = null!;
    }
}
