using System.ComponentModel.DataAnnotations;

namespace AssetManagement.Contracts.DTOs.Requests
{
    public class CategoryRequestDto
    {
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; } = null!;

        [Required(AllowEmptyStrings = false)]
        public string Prefix { get; set; } = null!;
    }
}
