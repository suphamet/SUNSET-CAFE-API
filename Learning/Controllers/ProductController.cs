using Microsoft.AspNetCore.Mvc;
using Learning.Models;

namespace Learning.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        [HttpGet("GetProduct")]
        public IActionResult GetProduct()
        {
            var response = new ServiceResponse<List<Product>>();
            try
            {
                using (var db = new AppDbContext())
                {
                    var productsFromDb = db.Products.ToList();
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

        [HttpPost("CreateProduct")]
        public IActionResult CreateProduct(string name, double price)
        {
            var response = new ServiceResponse<Product>();
            try
            {
                using (var db = new AppDbContext())
                {
                    var newProduct = new Product
                    {
                        Name = name,
                        Price = price
                    };

                    db.Products.Add(newProduct);
                    db.SaveChanges();

                    response.ResponseObject = newProduct;
                    response.Success = true;
                    response.Message = "เพิ่มข้อมูลสินค้าเรียบร้อยแล้ว";

                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "เกิดข้อผิดพลาดในการบันทึกข้อมูล";
                response.Errors.Add(ex.Message);

                return BadRequest(response);
            }
        }
    }
}