using Microsoft.AspNetCore.Mvc;
using Learning.Models;
using BCrypt.Net;

namespace Learning.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        [HttpGet("GetProduct")]
        public IActionResult GetProduct()
        {
            var response = new ServiceResponse<object>();
            try
            {
                using (var db = new AppDbContext())
                {
                    var productsFromDb = db.Products.Select(p => new {
                        p.Id,
                        p.Name,
                        p.Price,
                        p.Category,
                        p.Img,
                        p.IsSignature
                    }).ToList();

                    response.ResponseObject = productsFromDb;
                    response.Success = true;
                    response.Message = "ดึงข้อมูลสินค้าทั้งหมดสำเร็จ";

                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "เกิดข้อผิดพลาดในการดึงข้อมูล";
                response.Errors.Add(ex.Message);

                return BadRequest(response);
            }
        }
        [HttpPost("PlaceOrder")]
        public IActionResult PlaceOrder([FromBody] OrderRequest request)
        {
            var response = new ServiceResponse<object>();
            try
            {
                using (var db = new AppDbContext())
                {
                    // เริ่ม Transaction เพื่อความปลอดภัยของข้อมูล
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            // 1. สร้างหัวข้อ OrderMenu
                            var order = new OrderMenu
                            {
                                OrderNumber = $"ORD-{DateTime.Now:yyyyMMddHHmmss}",
                                CustomerName = request.CustomerName,
                                TotalAmount = request.TotalAmount,
                                PhoneNumber = request.PhoneNumber,
                                CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            };

                            db.OrderMenus.Add(order);
                            db.SaveChanges(); // บันทึกเพื่อให้ได้ order.Id มาใช้ต่อ

                            // 2. บันทึกรายการอาหาร (Details)
                            foreach (var item in request.Items)
                            {
                                var detail = new DetailOrder
                                {
                                    OrderId = order.Id,
                                    Category = item.Category,
                                    ProductName = item.ProductName,
                                    Price = item.Price,
                                    Quantity = item.Quantity,
                                    TotalPrice = item.Price * item.Quantity
                                };
                                db.DetailOrders.Add(detail);
                            }

                            db.SaveChanges();

                            // ยืนยันการบันทึกทั้งหมดลง Database
                            transaction.Commit();

                            // กำหนดค่า response ให้คล้ายกับ GetProduct
                            response.Success = true;
                            response.Message = "บันทึกออเดอร์สำเร็จ!";
                            response.ResponseObject = new { OrderNumber = order.OrderNumber }; // ส่งเลขที่ออเดอร์กลับไปดูเล่นๆ

                            return Ok(response);
                        }
                        catch (Exception ex)
                        {
                            // หากเกิดข้อผิดพลาดภายใน ให้ Rollback ข้อมูล
                            transaction.Rollback();
                            throw; // โยน Exception ออกไปให้ Catch ตัวนอกจัดการ
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // จัดการ Error format ให้เหมือนกับ GetProduct
                response.Success = false;
                response.Message = "เกิดข้อผิดพลาดในการบันทึกออเดอร์";

                // ดึง InnerException มาโชว์ถ้ามี (เหมือนที่คุณเคยทำ)
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                response.Errors.Add(innerMessage);

                return BadRequest(response);
            }
        }
        [HttpGet("GetGroupedOrders")]
        public IActionResult GetGroupedOrders()
        {
            using (var db = new AppDbContext())
            {
                // 1. Join ข้อมูลก่อน
                var flatData = from order in db.OrderMenus
                               join detail in db.DetailOrders on order.Id equals detail.OrderId
                               select new
                               {
                                   order.OrderNumber,
                                   order.CustomerName,
                                   order.TotalAmount,
                                   order.CreatedAt,
                                   Detail = detail
                               };

                // 2. Group ข้อมูลด้วย OrderNumber
                var grouped = flatData.ToList() // ดึงมาจัดการใน Memory เพื่อความง่าย
                    .GroupBy(o => o.OrderNumber)
                    .Select(g => new
                    {
                        OrderNo = g.Key,
                        Customer = g.First().CustomerName,
                        Total = g.First().TotalAmount,
                        Date = g.First().CreatedAt,
                        Items = g.Select(i => new
                        {
                            Product = i.Detail.ProductName,
                            Qty = i.Detail.Quantity,
                            Price = i.Detail.Price
                        }).ToList()
                    }).ToList();

                return Ok(grouped);
            }
        }

    }
}