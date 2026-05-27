using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web;
namespace PTDA_Demo.Models
{
    public class EditProductViewModel
    {
        public int ProductID { get; set; }

        // Kịch bản 4: Không bỏ trống
        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm.")]
        [StringLength(200, ErrorMessage = "Tên sản phẩm không được vượt quá 200 ký tự.")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn danh mục.")]
        public int CategoryID { get; set; }

        // Kịch bản 5: Phải là số dương
        [Required(ErrorMessage = "Vui lòng nhập giá bán.")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá bán phải là số lớn hơn hoặc bằng 0.")]
        public decimal Price { get; set; }

        // Kịch bản 5: Tồn kho >= 0
        //[Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn kho phải là số nguyên >= 0.")]
        public int StockQuantity { get; set; }

        public string Description { get; set; }

        // Kịch bản 3: Trạng thái hiển thị
        //public bool IsActive { get; set; }

        // --- XỬ LÝ HÌNH ẢNH ---
        // Giữ lại đường dẫn ảnh cũ từ Database
        public string ExistingImageURL { get; set; }
        // File ảnh mới người dùng upload lên (có thể null)
        public HttpPostedFileBase ImageUpload { get; set; }

    }
}