using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PTDA_Demo.Models
{
    public class CreateCategoryViewModel
    {
        // Kịch bản 3 & 6: Bắt buộc nhập và không được nhập toàn khoảng trắng
        [Required(ErrorMessage = "Vui lòng nhập tên danh mục.")]
        // Kịch bản 5: Giới hạn độ dài theo CSDL
        [StringLength(100, ErrorMessage = "Tên danh mục không được vượt quá 100 ký tự.")]
        public string CategoryName { get; set; }

        // Kịch bản 2 & 5: Mô tả là không bắt buộc, nhưng có giới hạn độ dài
        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự.")]
        public string Description { get; set; }

        // Kịch bản 1: Trạng thái mặc định là Đang hoạt động
        public bool IsActive { get; set; } = true;


    }
}