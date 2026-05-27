using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PTDA_Demo.Models
{
    public class EditCategoryViewModel
    {
        // Rất quan trọng: Phải có ID để biết đang chỉnh sửa bản ghi nào
        public int CategoryID { get; set; }

        // Kịch bản 3 & 5: Bắt buộc và giới hạn 100 ký tự
        [Required(ErrorMessage = "Vui lòng không bỏ trống tên danh mục.")]
        [StringLength(100, ErrorMessage = "Tên danh mục không được vượt quá 100 ký tự.")]
        public string CategoryName { get; set; }

        // Kịch bản 5: Mô tả giới hạn 500 ký tự (theo chuẩn SQL của bạn)
        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự.")]
        public string Description { get; set; }

        // Kịch bản 2: Thay đổi trạng thái hiển thị
       // public bool IsActive { get; set; }
    }
}