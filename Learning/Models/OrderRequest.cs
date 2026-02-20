using System.Collections.Generic;

namespace Learning.Models
{
    public class OrderRequest
    {
        public string CustomerName { get; set; } = string.Empty;
        public double TotalAmount { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public List<DetailOrderRequest> Items { get; set; } = new List<DetailOrderRequest>();
    }

    public class DetailOrderRequest
    {
        public string Category { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public double Price { get; set; }
        public int Quantity { get; set; }
    }
}