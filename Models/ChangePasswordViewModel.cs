using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PTDA_Demo.Models
{
    public class ChangePasswordViewModel
    {
        // Kịch bản 6: Bỏ trống trường thông tin
        [Required(ErrorMessage = "Vui lòng không bỏ trống trường này.")]
        public string CurrentPassword { get; set; }

        // Kịch bản 6: Bỏ trống trường thông tin
        [Required(ErrorMessage = "Vui lòng không bỏ trống trường này.")]
        // Kịch bản 4: Độ mạnh mật khẩu (ít nhất 8 ký tự, có chữ và số)
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d).{8,}$", ErrorMessage = "Mật khẩu không đủ độ mạnh. Phải có tối thiểu 8 ký tự, bao gồm chữ cái và chữ số.")]
        public string NewPassword { get; set; }

        // Kịch bản 6: Bỏ trống trường thông tin
        [Required(ErrorMessage = "Vui lòng không bỏ trống trường này.")]
        // Kịch bản 3: Không trùng khớp
        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không trùng khớp với mật khẩu mới.")]
        public string ConfirmPassword { get; set; }
    }
}