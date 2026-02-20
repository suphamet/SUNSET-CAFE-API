using System.ComponentModel.DataAnnotations.Schema;

namespace Learning.Models
{
    public class DetailOrder
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string Category { get; set; }
        public string ProductName { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public double TotalPrice { get; set; }
    }
}
