using Microsoft.AspNetCore.Mvc;
using Learning.Models;
using BCrypt.Net;

namespace Learning.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        // 1. API สำหรับสมัครสมาชิก (Register)
        [HttpPost("Register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            var response = new ServiceResponse<object>();
            try
            {
                using (var db = new AppDbContext())
                {
                    // ตรวจสอบว่ามี Username นี้หรือยัง
                    if (db.Users.Any(u => u.Username == request.Username))
                    {
                        response.Success = false;
                        response.Message = "ชื่อผู้ใช้งานนี้มีอยู่ในระบบแล้ว";
                        return BadRequest(response);
                    }

                    var user = new User
                    {
                        Username = request.Username,
                        // เข้ารหัสรหัสผ่านก่อนบันทึก
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                        FullName = request.FullName,
                        PhoneNumber = request.PhoneNumber,
                        Role = "Staff", // ค่าเริ่มต้น
                        IsActive = true,
                        CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    };

                    db.Users.Add(user);
                    db.SaveChanges();

                    response.Success = true;
                    response.Message = "สมัครสมาชิกสำเร็จ!";
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "เกิดข้อผิดพลาดในการสมัครสมาชิก";
                response.Errors.Add(ex.Message);
                return BadRequest(response);
            }
        }

        // 2. API สำหรับเข้าสู่ระบบ (Login)
        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var response = new ServiceResponse<object>();
            try
            {
                using (var db = new AppDbContext())
                {
                    // หา User จาก Username
                    var user = db.Users.FirstOrDefault(u => u.Username == request.Username);

                    // ตรวจสอบว่าพบ User และรหัสผ่านถูกต้องหรือไม่
                    if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                    {
                        response.Success = false;
                        response.Message = "ชื่อผู้ใช้งานหรือรหัสผ่านไม่ถูกต้อง";
                        return Unauthorized(response);
                    }

                    response.Success = true;
                    response.Message = "เข้าสู่ระบบสำเร็จ";
                    response.ResponseObject = new
                    {
                    };

                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "เกิดข้อผิดพลาดในการเข้าสู่ระบบ";
                response.Errors.Add(ex.Message);
                return BadRequest(response);
            }
        }
    }
}