using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PTDA_Demo.Models
{
    public class CreateAccountViewModel
    {
        // Kịch bản 4: Không bỏ trống
        [Required(ErrorMessage = "Vui lòng không bỏ trống trường này.")]
        [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự.")]
        public string FullName { get; set; }

        // Kịch bản 4 & 5: Không bỏ trống và phải đúng chuẩn Email
        [Required(ErrorMessage = "Vui lòng không bỏ trống trường này.")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
        public string Email { get; set; }

        // Kịch bản 4 & 5: Bắt buộc, độ dài 10 số, bắt đầu bằng 0 hoặc +84
        [Required(ErrorMessage = "Vui lòng không bỏ trống trường này.")]
        [RegularExpression(@"^(0|\+84)[3|5|7|8|9][0-9]{8}$", ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string PhoneNumber { get; set; }

        // Kịch bản 4 & 6: Bắt buộc, ít nhất 6 ký tự, có chữ và số
        [Required(ErrorMessage = "Vui lòng không bỏ trống trường này.")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d).{6,}$", ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự, bao gồm chữ và số.")]
        public string Password { get; set; }

        // Kịch bản 4: Bắt buộc chọn vai trò
        [Required(ErrorMessage = "Vui lòng chọn vai trò cho tài khoản.")]
        public int RoleID { get; set; }
    }
}