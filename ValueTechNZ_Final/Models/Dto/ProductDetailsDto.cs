using System.ComponentModel;

namespace ValueTechNZ_Final.Models.Dto
{
    public class ProductDetailsDto
    {
        public int ProductId { get; set; }
        [DisplayName("Name")]
        public required string ProductName { get; set; }
        public required string Brand { get; set; }
        public required decimal Price { get; set; }
        public string? Description { get; set; }

        [DisplayName("Image")]
        public string? ImageFileName { get; set; }
        public int CategoryId { get; set; }

        [DisplayName("Category")]
        public required string CategoryName { get; set; }
    }
}
