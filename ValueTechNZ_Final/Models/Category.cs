using System.ComponentModel;

namespace ValueTechNZ_Final.Models
{
    public class Category
    {
        public int CategoryId { get; set; }

        [DisplayName("Category")]
        public required string CategoryName { get; set; }

        public ICollection<ProductCategory> ProductCategory { get; set; }   
    }           
}
