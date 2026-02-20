using System.ComponentModel.DataAnnotations.Schema;

namespace Learning.Models
{
    public class OrderMenu
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public double TotalAmount { get; set; }
        public string CreatedAt { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
