using Microsoft.AspNetCore.Antiforgery;
using System.ComponentModel.DataAnnotations;

namespace ValueTechNZ_Final.Models.Dto
{
    public class CheckoutDto
    {
        [Required(ErrorMessage = "The delivery address is required.")]
        [MaxLength(200)]
        public string DeliveryAddress { get; set; } = "";
        public string PaymentMethod { get; set; } = "";
    }
}
