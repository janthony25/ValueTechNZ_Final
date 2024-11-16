using System.ComponentModel;

namespace ValueTechNZ_Final.Models.Dto
{
    public class CategoryListDto
    {
        public int CategoryId { get; set; }

        [DisplayName("Category")]
        public required string CategoryName { get; set; }

    }
}
