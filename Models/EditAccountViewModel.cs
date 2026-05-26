using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace PTDA_Demo.Models
{
    public class EditAccountViewModel
    {
        // Bắt buộc phải có UserID để lúc Submit hệ thống biết đang cập nhật dòng nào
        public int UserID { get; set; }

        //[Required(ErrorMessage = "Trường này không được để trống.")]
        //[StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự.")]
        public string FullName { get; set; }

        //[Required(ErrorMessage = "Trường này không được để trống.")]
        //[EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
        public string Email { get; set; }

        //[Required(ErrorMessage = "Trường này không được để trống.")]
        //[RegularExpression(@"^(0|\+84)[3|5|7|8|9][0-9]{8}$", ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn Vai trò.")]
        public int RoleID { get; set; }

        // Dùng để hiển thị trạng thái hiện tại (không cho phép sửa trực tiếp qua ô input)
        public bool IsActive { get; set; }

    }
}