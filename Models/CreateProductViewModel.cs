using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web; // Thư viện để dùng HttpPostedFileBase

namespace PTDA_Demo.Models
{
    public class CreateProductViewModel
    {
        // Kịch bản 3: Bắt buộc
        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm.")]
        [StringLength(200, ErrorMessage = "Tên sản phẩm không được vượt quá 200 ký tự.")]
        public string ProductName { get; set; }

        // Kịch bản 3: Bắt buộc chọn danh mục
        [Required(ErrorMessage = "Vui lòng chọn danh mục sản phẩm.")]
        public int CategoryID { get; set; }

        // Kịch bản 4: Giá bán > 0, kiểu decimal theo DB
        [Required(ErrorMessage = "Vui lòng nhập giá bán.")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá bán phải là một số hợp lệ và lớn hơn hoặc bằng 0.")]
        public decimal Price { get; set; }

        // Kịch bản 5: Tồn kho số nguyên >= 0
       // [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn kho phải là số nguyên lớn hơn hoặc bằng 0.")]
        public int StockQuantity { get; set; } = 0; // Mặc định là 0 (Kịch bản 2)

        public string Description { get; set; }

        // Kịch bản 6: File ảnh tải lên
        public HttpPostedFileBase ImageUpload { get; set; }

        // Kịch bản 1: Mặc định đang kinh doanh
        public bool IsActive { get; set; } = true;

    }
}