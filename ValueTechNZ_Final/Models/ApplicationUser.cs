using Microsoft.AspNetCore.Identity;
using Microsoft.VisualBasic;

namespace ValueTechNZ_Final.Models
{
    public class ApplicationUser : IdentityUser
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Address { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
