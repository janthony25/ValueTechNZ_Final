﻿using Microsoft.EntityFrameworkCore;

namespace ValueTechNZ_Final.Models
{
    public class Order
    {
        public int Id { get; set; }

        public string ClientId { get; set; } = "";
        public ApplicationUser Client = null!;

        public List<OrderItem> Items { get; set; } = new List<OrderItem>();

        [Precision(16, 2)]
        public decimal ShippingFee { get; set; }

        public string DeliveryAddress { get; set; } = "";
        public string PaymentMethod { get; set; } = "";
        public string PaymentStatus { get; set; } = "";
        public string PaymentDetails { get; set; } = ""; // to store paypal details
        public string MyProperty { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.Now; 

    }
}