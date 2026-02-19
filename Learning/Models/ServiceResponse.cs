using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Learning.Models
{
    public class ServiceResponse<T>
    {
        public int Id { get; set; } = 0;
        public bool Success { get; set; } = true;
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new List<string>();
        public T? ResponseObject { get; set; } // ข้อมูลหลักที่จะส่งกลับ
    }
}