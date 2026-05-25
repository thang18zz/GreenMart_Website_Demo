using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Security.Cryptography;
using System.Text;

using System.Data.Entity;


using PTDA_Demo.Models;

namespace PTDA_Demo.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin



        private string GetSHA256Hash(string input)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private GreenMartDBContext db = new GreenMartDBContext(); // Context kết nối CSDL

        [HttpGet]
        public ActionResult Login()
        {
            if (Session["TaiKhoan"] != null )
            {
                if ((int)Session["RoleID"] == 1 || (int)Session["RoleID"] == 2 || (int)Session["RoleID"] == 3)
                    return RedirectToAction("Dashboard", "Admin");
                else
                    return RedirectToAction("Homepage", "User");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Login(string Email, string Password)
        {
            // Scenario 5: Fallback kiểm tra rỗng ở phía Server [cite: 35, 37]
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ Email và Mật khẩu."; // [cite: 38]
                return View();
            }

            // Mã hóa mật khẩu người dùng nhập vào để so sánh với PasswordHash trong DB
            string hashedPassword = GetSHA256Hash(Password);

            // Tìm user theo Email và PasswordHash
            var user = db.Users.SingleOrDefault(x => x.Email == Email && x.PasswordHash == hashedPassword);

            // Scenario 2: Sai Email hoặc Mật khẩu [cite: 14]
            if (user == null)
            {
                ViewBag.Error = "Email hoặc mật khẩu không chính xác."; // [cite: 17]
                return View();
            }

            // Scenario 3: Tài khoản bị vô hiệu hóa (IsActive = 0) [cite: 21]
            if (user.IsActive == false)
            {
                ViewBag.Error = "Tài khoản của bạn đã bị khóa hoặc vô hiệu hóa. Vui lòng liên hệ Quản trị viên để biết thêm chi tiết."; // [cite: 25]
                return View();
            }

            // Scenario 4: Tài khoản Khách hàng (RoleID = 4) [cite: 28]
            if (user.RoleID == 4)
            {
                return RedirectToAction("Homepage", "User");
            }

            // Scenario 1: Đăng nhập thành công với vai trò Admin (1), Manager (2), Staff (3) [cite: 4]
            if (user.RoleID == 1 || user.RoleID == 2 || user.RoleID == 3)
            {
                // Lưu phiên đăng nhập (Session) [cite: 10]
                Session["TaiKhoan"] = user;
                Session["RoleID"] = user.RoleID;
                Session["FullName"] = user.FullName;
                Session["UserID"] = user.UserID;

                // Chuyển hướng vào Dashboard [cite: 9]
                return RedirectToAction("Dashboard", "Admin");
            }

            return View();
        }

        public ActionResult Dashboard()
        {
            // 1. KIỂM TRA BẢO MẬT (Xác thực & Phân quyền)
            if (Session["TaiKhoan"] == null)
            {
                // Chưa đăng nhập -> Đuổi về trang Login
                return RedirectToAction("Login", "Admin");
            }

            int roleId = Convert.ToInt32(Session["RoleID"]);
            if (roleId == 4)
            {
                // Là khách hàng nhưng cố tình vào Admin -> Đuổi về Login hoặc trang chủ
                return RedirectToAction("Login", "Admin");
            }

            
            return View();
        }

        public ActionResult Logout()
        {
            return View();

        }

        public ActionResult ClearSessions()
        {
            if (Session["TaiKhoan"] == null)
            {
                // Dùng TempData để giữ thông báo khi chuyển trang
                TempData["WarningMessage"] = "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.";
                return RedirectToAction("Login", "Admin");
            }

            Session.Clear();
            TempData["WarningMessage"] = "Đăng xuất thành công!";
            return RedirectToAction("Login","Admin");

        }

        public ActionResult AutoLogoutDueToTimeout()
        {
            // Dọn dẹp session cho chắc chắn
            Session.Clear();

            // Gắn thông báo bằng TempData y hệt như Scenario 5
            TempData["WarningMessage"] = "Phiên đăng nhập đã tự động đóng do bạn không thao tác trong một thời gian dài.";

            // Đẩy về trang đăng nhập
            return RedirectToAction("Login", "Admin");
        }

        public ActionResult Profile()
        {
            //scenario 2
            if (Session["TaiKhoan"] == null)
            {
                // Dùng TempData để giữ thông báo khi chuyển trang
                TempData["WarningMessage"] = "Vui lòng đăng nhập để tiếp tục!";
                return RedirectToAction("Login", "Admin");
            }

            //scenario 1
            int userID = (int)Session["UserID"];
            var UserProfile= db.Users.SingleOrDefault(u => u.UserID == userID);

            
            //scenario 4
            if (UserProfile == null)
            {
                ViewBag.Error = "Không thể tải thông tin lúc này. Vui lòng thử lại sau.";
            }

            //scenario 3
            int roleId = Convert.ToInt32(Session["RoleID"]);
            if (roleId == 4)
            {
                // Là khách hàng 
                return RedirectToAction("Profile", "User", new {id = UserProfile.UserID});
            }

            return View(UserProfile);
        }

        // GET: Hiển thị form đổi mật khẩu
        [HttpGet]
        public ActionResult ChangePassword()
        {
            // Kịch bản 7: Session hết hạn khi vừa bấm vào link
            if (Session["TaiKhoan"] == null)
            {
                TempData["WarningMessage"] = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại.";
                return RedirectToAction("Login", "Admin");
            }

            return View();
        }

        // POST: Xử lý dữ liệu khi bấm nút "Cập nhật"
        [HttpPost]
        [ValidateAntiForgeryToken] // Bảo mật: Chống tấn công giả mạo (CSRF)
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            // Kịch bản 7: Session hết hạn khi đang nhập dở tay rồi mới bấm nút
            if (Session["TaiKhoan"] == null)
            {
                TempData["WarningMessage"] = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại.";
                return RedirectToAction("Login", "Admin");
            }

            // Nếu dữ liệu vi phạm Kịch bản 3, 4, 6 (do Model kiểm tra) thì trả về View luôn
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            int userId = (int)Session["UserID"];
            // Giả sử context database của bạn là db
            var user = db.Users.SingleOrDefault(u => u.UserID == userId);

            if (user != null)
            {
                // Hàm băm của bạn (nhớ thay bằng hàm băm MD5/SHA256 bạn đang dùng ở trang Login)
                string currentHashedInput = GetSHA256Hash(model.CurrentPassword);

                // Kịch bản 2: Sai mật khẩu hiện tại
                if (user.PasswordHash != currentHashedInput)
                {
                    ModelState.AddModelError("CurrentPassword", "Mật khẩu hiện tại không chính xác. Vui lòng thử lại.");
                    return View(model);
                }

                // Kịch bản 5: Mật khẩu mới trùng mật khẩu hiện tại
                if (model.CurrentPassword == model.NewPassword)
                {
                    ModelState.AddModelError("NewPassword", "Mật khẩu mới không được trùng với mật khẩu hiện tại đang sử dụng.");
                    return View(model);
                }

                // Kịch bản 1 (Happy Path): Thành công
                string newHashedPassword = GetSHA256Hash(model.NewPassword);
                user.PasswordHash = newHashedPassword;
                db.SaveChanges();

                // Clear Session và yêu cầu đăng nhập lại
                Session.Clear();
                TempData["WarningMessage"] = "Thay đổi mật khẩu thành công. Vui lòng đăng nhập lại bằng mật khẩu mới.";
                return RedirectToAction("Login", "Admin");
            }

            return View(model);
        }

        // Thêm tham số page để xử lý Kịch bản 2 (Phân trang)
        //liet ke account us 20
        public ActionResult AccountManagement(int page = 1)
        {
            // Scenario 5: Chưa đăng nhập hoặc hết hạn phiên
            if (Session["TaiKhoan"] == null)
            {
                TempData["WarningMessage"] = "Vui lòng đăng nhập để tiếp tục.";
                return RedirectToAction("Login", "Admin");
            }

            // Scenario 4: Khách hàng hoặc nhân viên cố tình truy cập
            int roleId = Convert.ToInt32(Session["RoleID"]);
            if (roleId == 3 )
            {
                TempData["WarningMessage"] = "Bạn không có quyền thực hiện chức năng này.";
                return RedirectToAction("Dashboard", "Admin");
            }

            if (roleId == 4)
            {
                TempData["WarningMessage"] = "Bạn không có quyền truy cập vào trang này.";
                // Đuổi về trang chủ của khách
                return RedirectToAction("Homepage", "User");
            }

            try
            {
                // Mặc định số lượng hiển thị trên 1 trang
                int pageSize = 10;

                // Scenario 1: Lấy danh sách, sắp xếp Mới nhất lên đầu (ORDER BY CreatedAt DESC)
                var query = db.Users.OrderByDescending(u => u.CreatedAt).ToList();

                // Tính toán cho việc phân trang (Truyền số liệu ra View để vẽ nút bấm)
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = (int)Math.Ceiling((double)query.Count() / pageSize);

                // Cắt lấy đúng số lượng dữ liệu của trang hiện tại
                var usersOnPage = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                return View(usersOnPage);
            }
            catch (Exception ex)
            {
                // Scenario 6: Lỗi tải dữ liệu (Mất kết nối server/CSDL)
                ViewBag.Error = "Không thể tải danh sách tài khoản lúc này. Vui lòng tải lại trang hoặc thử lại sau.";

                // Trả về một list rỗng để View không bị lỗi NullReference
                return View(new System.Collections.Generic.List<User>());
            }
        }

        // Hàm hỗ trợ tạo danh sách thả xuống (Dropdown) cho Role
        private void PrepareAddRoleDropdown()
        {
            // Lấy RoleID = 2 (Manager) và RoleID = 3 (Staff) từ CSDL
            var roles = db.Roles.Where(r => r.RoleID == 2 || r.RoleID == 3).ToList();
            ViewBag.RoleList = new SelectList(roles, "RoleID", "RoleName");
        }

        // GET: Hiển thị giao diện Tạo tài khoản
        [HttpGet]
        public ActionResult CreateAccount()
        {
            // KỊCH BẢN 7: Kiểm tra phân quyền (Chỉ Admin mới được vào)
            if (Session["TaiKhoan"] == null)
            {
                TempData["WarningMessage"] = "Vui lòng đăng nhập để tiếp tục.";
                return RedirectToAction("Login", "Admin");
            }

            int roleId = Convert.ToInt32(Session["RoleID"]);
            // Nếu là Staff (3) hoặc Customer (4) -> Đuổi về Dashboard/Trang chủ
            if (roleId == 3 || roleId == 4)
            {
                TempData["WarningMessage"] = "Bạn không có quyền thực hiện chức năng này.";
                return RedirectToAction("Dashboard", "Admin");
            }

            PrepareAddRoleDropdown();
            return View();
        }

        // POST: Nhận dữ liệu và Lưu vào CSDL
        [HttpPost]
        [ValidateAntiForgeryToken] // Bảo mật: Chống giả mạo request
        public ActionResult CreateAccount(CreateAccountViewModel model)
        {
            // KỊCH BẢN 7 (Check lại lần 2 đề phòng dùng tool gửi POST lậu)
            int roleId = Convert.ToInt32(Session["RoleID"]);
            if (Session["TaiKhoan"] == null || roleId == 3 || roleId == 4)
            {
                TempData["WarningMessage"] = "Bạn không có quyền thực hiện chức năng này.";
                return RedirectToAction("Dashboard", "Admin");
            }

            // Nếu dữ liệu qua được vòng kiểm tra của ViewModel (Kịch bản 4, 5, 6)
            if (ModelState.IsValid)
            {
                // KỊCH BẢN 2: Kiểm tra Email trùng
                bool isEmailExist = db.Users.Any(u => u.Email == model.Email);
                if (isEmailExist)
                {
                    ModelState.AddModelError("Email", "Email này đã tồn tại trong hệ thống. Vui lòng sử dụng email khác.");
                }

                // KỊCH BẢN 3: Kiểm tra Số điện thoại trùng
                bool isPhoneExist = db.Users.Any(u => u.PhoneNumber == model.PhoneNumber);
                if (isPhoneExist)
                {
                    ModelState.AddModelError("PhoneNumber", "Số điện thoại này đã tồn tại trong hệ thống.");
                }

                // KỊCH BẢN 1 (Happy Path): Nếu không trùng gì cả -> Lưu DB
                if (!isEmailExist && !isPhoneExist)
                {
                    var newUser = new User
                    {
                        FullName = model.FullName,
                        Email = model.Email,
                        PhoneNumber = model.PhoneNumber,
                        RoleID = model.RoleID,
                        IsActive = true, // Mặc định là 1 (Đang hoạt động)
                        CreatedAt = DateTime.Now,
                        PasswordHash = GetSHA256Hash(model.Password) // Hàm băm bạn đã viết sẵn
                    };

                    db.Users.Add(newUser);
                    db.SaveChanges();

                    TempData["WarningMessage"] = "Tạo tài khoản thành công!";
                    // Redirect về trang danh sách tài khoản (Giả sử bạn có hàm Index cho quản lý User)
                    return RedirectToAction("AccountManagement", "Admin");
                }
            }

            // Nếu code chạy đến đây nghĩa là có lỗi. Trả về View kèm thông báo lỗi và giữ nguyên dữ liệu đã nhập
            PrepareAddRoleDropdown();
            return View(model);
        }


        private void PrepareEditRoleDropdown()
        {
            // Lấy danh sách Role từ CSDL (Admin, Manager, Staff, Customer)
            var roles = db.Roles.ToList();
            ViewBag.RoleList = new SelectList(roles, "RoleID", "RoleName");
        }
        // ==========================================
        // 1. GET: Hiển thị form chỉnh sửa
        // ==========================================
        [HttpGet]
        public ActionResult EditAccount(int id)
        {
            // KỊCH BẢN 8: Kiểm tra quyền (Chỉ Admin/Manager mới được sửa)
            if (Session["TaiKhoan"] == null) return RedirectToAction("Login", "Admin");
            int roleIdSession = Convert.ToInt32(Session["RoleID"]);
            if (roleIdSession == 3 || roleIdSession == 4)
            {
                TempData["WarningMessage"] = "Bạn không có quyền thực hiện chức năng này.";
                return RedirectToAction("Dashboard", "Admin");
            }

            // Tìm user cần sửa trong CSDL
            var user = db.Users.SingleOrDefault(u => u.UserID == id);
            if (user == null) return HttpNotFound();

            // Chuyển dữ liệu từ DB sang ViewModel để đẩy ra giao diện
            var model = new EditAccountViewModel
            {
                UserID = user.UserID,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                RoleID = user.RoleID,
                IsActive = user.IsActive ?? false
            };

            PrepareEditRoleDropdown();
            return View(model);
        }

        // ==========================================
        // 2. POST: Xử lý lưu thông tin chỉnh sửa (Scenarios 1, 2, 3, 4)
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAccount(EditAccountViewModel model)
        {
            // KỊCH BẢN 8: Check phân quyền
            int roleIdSession = Convert.ToInt32(Session["RoleID"]);
            if (Session["TaiKhoan"] == null || roleIdSession == 3 || roleIdSession == 4)
            {
                TempData["WarningMessage"] = "Bạn không có quyền thực hiện chức năng này.";
                return RedirectToAction("Dashboard", "Admin");
            }

            if (ModelState.IsValid)
            {
                // KỊCH BẢN 2 & 3: Kiểm tra trùng Email/SĐT NHƯNG PHẢI LOẠI TRỪ CHÍNH USER ĐÓ RA
                // (Nếu không loại trừ UserID hiện tại, khi họ bấm Lưu mà không đổi Email, hệ thống sẽ báo trùng)
                bool isEmailExist = db.Users.Any(u => u.Email == model.Email && u.UserID != model.UserID);
                if (isEmailExist)
                    ModelState.AddModelError("Email", "Email này đã được sử dụng bởi một tài khoản khác. Vui lòng chọn email khác.");

                bool isPhoneExist = db.Users.Any(u => u.PhoneNumber == model.PhoneNumber && u.UserID != model.UserID);
                if (isPhoneExist)
                    ModelState.AddModelError("PhoneNumber", "Số điện thoại này đã tồn tại trong hệ thống.");

                // KỊCH BẢN 1: Lưu thành công
                if (!isEmailExist && !isPhoneExist)
                {
                    var userToUpdate = db.Users.SingleOrDefault(u => u.UserID == model.UserID);
                    if (userToUpdate != null)
                    {
                        userToUpdate.FullName = model.FullName;
                        userToUpdate.Email = model.Email;
                        userToUpdate.PhoneNumber = model.PhoneNumber;
                        userToUpdate.RoleID = model.RoleID;
                        // Lưu ý: Không cập nhật IsActive và Password ở đây

                        db.SaveChanges();
                        TempData["WarningMessage"] = "Cập nhật thông tin tài khoản thành công.";
                        return RedirectToAction("AccountManagement", "Admin"); 
                    }
                }
            }

            PrepareEditRoleDropdown();
            return View(model);
        }

        // ==========================================
        // 3. POST: Xử lý Khóa/Mở khóa tài khoản (Scenarios 5, 6, 7)
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ToggleAccountStatus(int targetUserId)
        {
            // Check phân quyền như trên...
            int currentUserId = Convert.ToInt32(Session["UserID"]);

            // KỊCH BẢN 7: Ngăn chặn tự khóa chính mình
            if (currentUserId == targetUserId)
            {
                TempData["WarningMessage"] = "Bạn không thể khóa hoặc thay đổi trạng thái tài khoản đang đăng nhập.";
                return RedirectToAction("EditAccount", new { id = targetUserId });
            }

            var user = db.Users.SingleOrDefault(u => u.UserID == targetUserId);
            if (user != null)
            {
                // KỊCH BẢN 5 & 6: Đảo ngược trạng thái hiện tại (Đang 1 thì thành 0, Đang 0 thì thành 1)
                user.IsActive = !(user.IsActive ?? false);
                db.SaveChanges();

                if (user.IsActive == true)
                    TempData["WarningMessage"] = "Đã mở khóa tài khoản thành công.";
                else
                    TempData["WarningMessage"] = "Đã khóa tài khoản thành công.";
            }

            return RedirectToAction("EditAccount", new { id = targetUserId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Chống giả mạo Request
        public ActionResult DeleteAccount(int targetUserId)
        {
            // ==========================================
            // SCENARIO 6: Phân quyền (Chỉ Admin mới được xóa)
            // ==========================================
            if (Session["TaiKhoan"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }

            int currentUserId = Convert.ToInt32(Session["UserID"]);
            int roleIdSession = Convert.ToInt32(Session["RoleID"]);

            // Chỉ cho phép Admin và quản lý (RoleID = 1 hoặc 2) thực hiện chức năng xóa cứng
            if (roleIdSession != 1 && roleIdSession != 2)
            {
                TempData["WarningMessage"] = "Bạn không có quyền thực hiện chức năng xóa tài khoản.";
                if(roleIdSession==3)
                return RedirectToAction("AccountManagement", "Admin");
                if(roleIdSession==4)
                return RedirectToAction("Homepage", "User");
            }

            // ==========================================
            // SCENARIO 5: Ngăn Admin tự xóa chính mình
            // ==========================================
            if (currentUserId == targetUserId)
            {
                TempData["WarningMessage"] = "Bạn không thể xóa tài khoản đang đăng nhập.";
                return RedirectToAction("AccountManagement", "Admin");
            }

            // Tìm tài khoản cần xóa
            var userToDelete = db.Users.SingleOrDefault(u => u.UserID == targetUserId);
            if (userToDelete == null)
            {
                TempData["WarningMessage"] = "Không tìm thấy tài khoản cần xóa.";
                return RedirectToAction("AccountManagement", "Admin");
            }

            // ==========================================
            // SCENARIO 2: Ràng buộc Khách hàng (Đơn hàng)
            // ==========================================
            bool hasOrders = db.Orders.Any(o => o.UserID == targetUserId);
            if (hasOrders)
            {
                TempData["WarningMessage"] = "Không thể xóa tài khoản này vì đã phát sinh lịch sử giao dịch (Đơn hàng). Vui lòng sử dụng tính năng Khóa tài khoản.";
                return RedirectToAction("AccountManagement", "Admin");
            }

            // ==========================================
            // SCENARIO 3: Ràng buộc Nội bộ (Phiếu nhập kho)
            // ==========================================
            bool hasReceipts = db.PurchaseReceipts.Any(p => p.CreatedBy == targetUserId);
            if (hasReceipts)
            {
                TempData["WarningMessage"] = "Không thể xóa tài khoản này vì có liên kết với các chứng từ hệ thống (Phiếu nhập kho). Vui lòng Khóa tài khoản để bảo toàn dữ liệu.";
                return RedirectToAction("AccountManagement", "Admin");
            }

            // ==========================================
            // SCENARIO 1: Xóa thành công (Happy Path)
            // ==========================================
            try
            {
                // 1. Xóa các dữ liệu phụ trước (Sổ địa chỉ, Giỏ hàng) để không vi phạm Khóa ngoại
                var userAddresses = db.Addresses.Where(a => a.UserID == targetUserId).ToList();
                if (userAddresses.Any()) db.Addresses.RemoveRange(userAddresses);

                var userCartItems = db.CartItems.Where(c => c.UserID == targetUserId).ToList();
                if (userCartItems.Any()) db.CartItems.RemoveRange(userCartItems);

                // 2. Xóa tài khoản chính
                db.Users.Remove(userToDelete);

                // 3. Lưu vào CSDL
                db.SaveChanges();

                TempData["WarningMessage"] = "Xóa tài khoản thành công.";
            }
            catch (System.Exception ex)
            {
                // Bắt lỗi dự phòng nếu có bảng nào đó phát sinh FK mà ta chưa tính tới
                TempData["WarningMessage"] = "Lỗi hệ thống: Không thể xóa do dữ liệu đang được ràng buộc ở bảng khác. Chi tiết: " + ex.Message;
            }

            return RedirectToAction("AccountManagement", "Admin");
        }

        public ActionResult CategoryManagement(int page = 1)
        {
            // ==========================================
            // SCENARIO 6: Bảo mật & Phân quyền (Chặn Khách hàng)
            // ==========================================
            if (Session["TaiKhoan"] == null)
            {
                TempData["WarningMessage"] = "Vui lòng đăng nhập để tiếp tục.";
                return RedirectToAction("Login", "Admin");
            }

            int roleId = Convert.ToInt32(Session["RoleID"]);
            // RoleID = 4 là Customer (Khách hàng)
            if (roleId == 4)
            {
                TempData["WarningMessage"] = "Bạn không có quyền truy cập vào trang này.";
                // Đuổi về trang login admin
                return RedirectToAction("Login", "Admin");
            }

            // ==========================================
            // SCENARIO 7: Xử lý lỗi mất kết nối CSDL
            // ==========================================
            try
            {
                int pageSize = 10; // Số lượng danh mục trên 1 trang

                // Lấy danh sách danh mục, sắp xếp theo ID mới nhất (giảm dần)
                var query = db.Categories.OrderByDescending(c => c.CategoryID).ToList();

                // SCENARIO 2: Tính toán phân trang
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = (int)Math.Ceiling((double)query.Count() / pageSize);

                // Cắt dữ liệu đúng trang hiện tại
                var categoriesOnPage = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                // SCENARIO 1: Trả dữ liệu về View
                return View(categoriesOnPage);
            }
            catch (Exception)
            {
                // Khi DB lỗi, gán câu thông báo vào ViewBag và trả về danh sách rỗng để không bị sập trang
                ViewBag.Error = "Không thể tải dữ liệu danh mục lúc này. Vui lòng tải lại trang hoặc thử lại sau.";
                return View(new System.Collections.Generic.List<Category>());
            }
        }

        // ==========================================
        // GET: Hiển thị form thêm mới danh mục
        // ==========================================
        [HttpGet]
        public ActionResult CreateCategory()
        {
            // KỊCH BẢN 7: Phân quyền (Chặn Role Customer)
            if (Session["TaiKhoan"] == null) return RedirectToAction("Login", "Admin");

            int roleId = Convert.ToInt32(Session["RoleID"]);
            if (roleId == 4 || roleId==3) // Customer có RoleID là 4
            {
                TempData["WarningMessage"] = "Bạn không có quyền thực hiện chức năng này.";
                return RedirectToAction("Dashboard", "Admin");
            }

            // Truyền ViewModel trống với IsActive mặc định = true
            return View(new CreateCategoryViewModel { IsActive = true });
        }

        // ==========================================
        // POST: Xử lý lưu danh mục mới
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken] // Kịch bản 7: Chống tấn công giả mạo (CSRF)
        public ActionResult CreateCategory(CreateCategoryViewModel model)
        {
            // KỊCH BẢN 7: Kiểm tra quyền lần 2 (Đề phòng dùng tools)
            if (Session["TaiKhoan"] == null) return RedirectToAction("Login", "Admin");
            int roleId = Convert.ToInt32(Session["RoleID"]);
            if (roleId == 4 || roleId==3) //
            {
                TempData["WarningMessage"] = "Bạn không có quyền thực hiện chức năng này.";
                return RedirectToAction("Dashboard", "Admin");
            }

            // KỊCH BẢN 6: Loại bỏ khoảng trắng thừa ở 2 đầu Tên danh mục
            if (!string.IsNullOrWhiteSpace(model.CategoryName))
            {
                model.CategoryName = model.CategoryName.Trim();
            }
            else
            {
                // Nếu sau khi Trim mà rỗng, báo lỗi
                ModelState.AddModelError("CategoryName", "Vui lòng nhập tên danh mục.");
            }

            if (ModelState.IsValid)
            {
                // KỊCH BẢN 4: Kiểm tra trùng tên danh mục (Chuyển về chữ thường để so sánh chuẩn xác)
                bool isExist = db.Categories.Any(c => c.CategoryName.ToLower() == model.CategoryName.ToLower());
                if (isExist)
                {
                    ModelState.AddModelError("CategoryName", "Tên danh mục này đã tồn tại. Vui lòng chọn tên khác.");
                    return View(model);
                }

                // KỊCH BẢN 1 & 2: Lưu vào cơ sở dữ liệu
                var newCategory = new Category
                {
                    CategoryName = model.CategoryName,
                    Description = model.Description, // Có thể null
                    IsActive = model.IsActive
                };

                db.Categories.Add(newCategory);
                db.SaveChanges();

                TempData["WarningMessage"] = "Thêm mới danh mục thành công.";
                return RedirectToAction("CategoryManagement", "Admin"); // Đổi về action danh sách của bạn
            }

            // Nếu có lỗi (Kịch bản 3, 5), hiển thị lại form kèm câu thông báo lỗi
            return View(model);
        }


        // ==========================================
        // 1. GET: Hiển thị form chỉnh sửa
        // ==========================================
        [HttpGet]
        public ActionResult EditCategory(int? id) // Dùng int? để bắt trường hợp URL không có ID
        {
            // KỊCH BẢN 8: Kiểm tra phân quyền (Khách hàng RoleID = 4 bị chặn)
            if (Session["TaiKhoan"] == null) return RedirectToAction("Login", "Admin");
            if (Convert.ToInt32(Session["RoleID"]) == 4 || Convert.ToInt32(Session["RoleID"]) == 3)
            {
                TempData["WarningMessage"] = "Bạn không có quyền thực hiện chức năng này.";
                return RedirectToAction("Dashboard", "Admin");
            }

            // KỊCH BẢN 7: ID Manipulation (Gõ ID bậy bạ lên URL)
            if (id == null) return RedirectToAction("CategoryManagement", "Admin");

            var category = db.Categories.SingleOrDefault(c => c.CategoryID == id);
            if (category == null)
            {
                // Không tìm thấy trong CSDL
                TempData["WarningMessage"] = "Không tìm thấy danh mục yêu cầu.";
                return RedirectToAction("CategoryManagement", "Admin");
            }

            // Chuyển đổi dữ liệu từ Entity sang ViewModel
            var model = new EditCategoryViewModel
            {
                CategoryID = category.CategoryID,
                CategoryName = category.CategoryName,
                Description = category.Description,
                IsActive = category.IsActive ?? false
            };

            return View(model);
        }

        // ==========================================
        // 2. POST: Nhận dữ liệu cập nhật
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken] // Chống giả mạo request
        public ActionResult EditCategory(EditCategoryViewModel model)
        {
            // KỊCH BẢN 8: Kiểm tra lại quyền (Chặn API POST lậu)
            if (Session["TaiKhoan"] == null || Convert.ToInt32(Session["RoleID"]) == 4)
            {
                TempData["WarningMessage"] = "Bạn không có quyền thực hiện chức năng này.";
                return RedirectToAction("Dashboard", "Admin");
            }

            // Loại bỏ khoảng trắng thừa
            if (!string.IsNullOrWhiteSpace(model.CategoryName))
                model.CategoryName = model.CategoryName.Trim();

            if (ModelState.IsValid)
            {
                // KỊCH BẢN 4: Trùng tên danh mục (LƯU Ý: Phải loại trừ chính nó)
                bool isDuplicate = db.Categories.Any(c =>
                    c.CategoryName.ToLower() == model.CategoryName.ToLower() &&
                    c.CategoryID != model.CategoryID);

                if (isDuplicate)
                {
                    ModelState.AddModelError("CategoryName", "Tên danh mục này đã tồn tại. Vui lòng chọn tên khác.");
                    return View(model);
                }

                // KỊCH BẢN 1 & 2: Cập nhật thông tin & trạng thái
                var categoryToUpdate = db.Categories.SingleOrDefault(c => c.CategoryID == model.CategoryID);
                if (categoryToUpdate != null)
                {
                    categoryToUpdate.CategoryName = model.CategoryName;
                    categoryToUpdate.Description = model.Description;
                    categoryToUpdate.IsActive = model.IsActive;

                    // KỊCH BẢN 6: EF tự động bắt được nếu không có thay đổi (No changes made)
                    // Hàm SaveChanges() sẽ bỏ qua câu lệnh UPDATE nếu dữ liệu cũ và mới y hệt nhau.
                    db.SaveChanges();

                    TempData["WarningMessage"] = "Cập nhật danh mục thành công.";
                    return RedirectToAction("CategoryManagement", "Admin");
                }
            }

            // Trả về View nếu có lỗi Validation (Kịch bản 3, 5)
            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken] // Chống giả mạo request từ bên ngoài
        public ActionResult DeleteCategory(int targetCategoryId)
        {
            // ==========================================
            // SCENARIO 5: Kiểm tra phân quyền (Chỉ Admin/Manager mới được xóa)
            // ==========================================
            if (Session["TaiKhoan"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }

            int roleIdSession = Convert.ToInt32(Session["RoleID"]);
            // Giả sử RoleID = 3 (Staff) và 4 (Customer) không được phép xóa
            if (roleIdSession == 3 || roleIdSession == 4)
            {
                TempData["WarningMessage"] = "Bạn không có quyền thực hiện chức năng xóa danh mục.";
                return RedirectToAction("CategoryManagement", "Admin");
            }

            // Tìm danh mục cần xóa trong CSDL
            var categoryToDelete = db.Categories.SingleOrDefault(c => c.CategoryID == targetCategoryId);
            if (categoryToDelete == null)
            {
                TempData["WarningMessage"] = "Không tìm thấy danh mục cần xóa.";
                return RedirectToAction("CategoryManagement", "Admin");
            }

            // ==========================================
            // SCENARIO 2: Ràng buộc dữ liệu (Sad Path)
            // Kiểm tra xem danh mục này có đang chứa sản phẩm nào không
            // ==========================================
            bool hasProducts = db.Products.Any(p => p.CategoryID == targetCategoryId);
            if (hasProducts)
            {
                TempData["WarningMessage"] = "Không thể xóa danh mục này vì đang có sản phẩm thuộc danh mục. Vui lòng di chuyển sản phẩm sang danh mục khác hoặc chuyển trạng thái danh mục thành 'Đã ẩn'.";
                return RedirectToAction("CategoryManagement", "Admin");
            }

            // ==========================================
            // SCENARIO 1: Xóa thành công (Happy Path)
            // ==========================================
            try
            {
                db.Categories.Remove(categoryToDelete);
                db.SaveChanges();
                TempData["WarningMessage"] = "Xóa danh mục sản phẩm thành công.";
            }
            catch (System.Exception ex)
            {
                TempData["WarningMessage"] = "Lỗi hệ thống: Không thể xóa do lỗi cơ sở dữ liệu. Chi tiết: " + ex.Message;
            }

            return RedirectToAction("CategoryManagement", "Admin");
        }

        // ==========================================
        // SCENARIO 1 & 2: GET danh sách sản phẩm và Phân trang
        // ==========================================
        [HttpGet]
        public ActionResult ProductManagement(int page = 1)
        {
            // SCENARIO 7: Phân quyền bảo mật (Khách hàng RoleID = 4 bị chặn)
            if (Session["TaiKhoan"] == null)
            {
                TempData["WarningMessage"] = "Vui lòng đăng nhập để tiếp tục.";
                return RedirectToAction("Login", "Admin");
            }

            int roleId = Convert.ToInt32(Session["RoleID"]);
            if (roleId == 4) // Khách hàng
            {
                Session.Clear();
                TempData["WarningMessage"] = "Bạn không có quyền truy cập vào trang quản lý sản phẩm.";
                return RedirectToAction("Login", "Admin");
            }

            try
            {
                int pageSize = 10; // Cài đặt hiển thị 10 dòng/trang

                // Lấy danh sách sản phẩm, KẾT NỐI với bảng Categories để lấy Tên danh mục
                // Sắp xếp mặc định: Mới nhất lên đầu (Dựa vào CreatedAt hoặc ProductID)
                var query = db.Products.Include(p => p.Category).OrderByDescending(p => p.ProductID).ToList();

                // Tính toán phân trang
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = (int)Math.Ceiling((double)query.Count() / pageSize);

                // Cắt dữ liệu đúng trang hiện tại
                var productsOnPage = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                return View(productsOnPage);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Không thể tải dữ liệu sản phẩm lúc này. Vui lòng thử lại sau.";
                return View(new System.Collections.Generic.List<Product>());
            }
        }


        // Hàm chuẩn bị Dropdown danh mục (Chỉ lấy các danh mục đang hoạt động)
        private void PrepareCategoryDropdown()
        {
            var categories = db.Categories.Where(c => c.IsActive == true).ToList();
            ViewBag.CategoryList = new SelectList(categories, "CategoryID", "CategoryName");
        }

        // ==========================================
        // GET: Hiển thị form thêm sản phẩm
        // ==========================================
        [HttpGet]
        public ActionResult CreateProduct()
        {
            // KỊCH BẢN 8: Phân quyền (Chặn Staff = 3 và Customer = 4)
            if (Session["TaiKhoan"] == null) return RedirectToAction("Login", "Admin");

            int roleId = Convert.ToInt32(Session["RoleID"]);
            if (roleId == 3 || roleId == 4)
            {
                TempData["WarningMessage"] = "Bạn không có quyền truy cập vào chức năng này.";
                return RedirectToAction("ProductManagement", "Admin");
            }

            PrepareCategoryDropdown();
            return View(new CreateProductViewModel { IsActive = true, StockQuantity = 0 });
        }

        // ==========================================
        // POST: Xử lý lưu sản phẩm và Upload Ảnh
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateProduct(CreateProductViewModel model)
        {
            //Tạo thư mục ảnh, nếu chưa có
            if (model.ImageUpload != null && model.ImageUpload.ContentLength > 0)
            {
                // 1. Xác định đường dẫn thư mục
                string folderPath = Server.MapPath("~/Content/Images/Products/");

                // 2. QUAN TRỌNG: Kiểm tra nếu thư mục chưa tồn tại thì tạo mới
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // 3. Xử lý tên file và lưu
                string extension = Path.GetExtension(model.ImageUpload.FileName).ToLower();
                string fileName = DateTime.Now.Ticks.ToString() + extension;
                string path = Path.Combine(folderPath, fileName);

                model.ImageUpload.SaveAs(path);
            }

                // KỊCH BẢN 8: Kiểm tra quyền lại
                if (Session["TaiKhoan"] == null || Convert.ToInt32(Session["RoleID"]) == 3 || Convert.ToInt32(Session["RoleID"]) == 4)
            {
                TempData["WarningMessage"] = "Bạn không có quyền thực hiện chức năng này.";
                return RedirectToAction("Dashboard", "Admin");
            }

            if (ModelState.IsValid)
            {
                // KỊCH BẢN 7: Kiểm tra trùng tên sản phẩm
                bool isExist = db.Products.Any(p => p.ProductName.ToLower() == model.ProductName.Trim().ToLower());
                if (isExist)
                {
                    ModelState.AddModelError("ProductName", "Sản phẩm với tên này đã tồn tại trong hệ thống. Vui lòng chọn tên khác.");
                    PrepareCategoryDropdown();
                    return View(model);
                }

                string fileName = null;

                // KỊCH BẢN 6: Xử lý Upload Hình ảnh
                if (model.ImageUpload != null && model.ImageUpload.ContentLength > 0)
                {
                    // Kiểm tra dung lượng (5MB = 5 * 1024 * 1024 bytes)
                    if (model.ImageUpload.ContentLength > 5242880)
                    {
                        ModelState.AddModelError("ImageUpload", "Dung lượng ảnh không được vượt quá 5MB.");
                        PrepareCategoryDropdown();
                        return View(model);
                    }

                    // Kiểm tra định dạng (Chỉ cho phép .jpg, .jpeg, .png)
                    string extension = Path.GetExtension(model.ImageUpload.FileName).ToLower();
                    if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
                    {
                        ModelState.AddModelError("ImageUpload", "Định dạng tệp không hỗ trợ. Vui lòng chọn ảnh JPG hoặc PNG.");
                        PrepareCategoryDropdown();
                        return View(model);
                    }

                    // Đổi tên file để tránh trùng lặp khi nhiều người cùng up ảnh có tên giống nhau
                    fileName = DateTime.Now.Ticks.ToString() + extension;

                    // Đường dẫn lưu file trên Server (Yêu cầu phải tạo sẵn thư mục này trong Project)
                    string path = Path.Combine(Server.MapPath("~/Content/Images/Products/"), fileName);

                    // Lưu file
                    model.ImageUpload.SaveAs(path);
                }

                // KỊCH BẢN 1 & 2: Lưu vào Database
                var newProduct = new Product
                {
                    ProductName = model.ProductName.Trim(),
                    CategoryID = model.CategoryID,
                    Price = model.Price,
                    StockQuantity = model.StockQuantity,
                    Description = model.Description,
                    ImageURL = fileName, // Có thể null nếu Kịch bản 2 xảy ra
                    IsActive = model.IsActive,
                    CreatedAt = DateTime.Now
                };

                db.Products.Add(newProduct);
                db.SaveChanges();

                TempData["WarningMessage"] = "Thêm sản phẩm mới thành công.";
                return RedirectToAction("ProductManagement", "Admin");
            }

            PrepareCategoryDropdown();
            return View(model);
        }

        // ==========================================
        // 1. GET: Hiển thị form chỉnh sửa
        // ==========================================
        [HttpGet]
        public ActionResult EditProduct(int? id)
        {
            // KỊCH BẢN 9: Phân quyền (Chặn Role Customer = 4 và Staff = 3)
            if (Session["TaiKhoan"] == null) return RedirectToAction("Login", "Admin");
            int roleId = Convert.ToInt32(Session["RoleID"]);
            if (roleId == 3 || roleId == 4)
            {
                TempData["WarningMessage"] = "Bạn không có quyền thực hiện chức năng này.";
                return RedirectToAction("ProductManagement", "Admin");
            }

            if (id == null) return RedirectToAction("ProductManagement", "Admin");

            var product = db.Products.SingleOrDefault(p => p.ProductID == id);
            if (product == null) return HttpNotFound();

            // Đổ dữ liệu từ DB sang ViewModel
            var model = new EditProductViewModel
            {
                ProductID = product.ProductID,
                ProductName = product.ProductName,
                CategoryID = product.CategoryID,
                Price = product.Price,
                StockQuantity = product.StockQuantity ?? 0,
                Description = product.Description,
                IsActive = product.IsActive ?? false,
                ExistingImageURL = product.ImageURL // Gắn ảnh cũ vào model
            };

            PrepareCategoryDropdown();
            return View(model);
        }

        // ==========================================
        // 2. POST: Lưu thông tin chỉnh sửa
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProduct(EditProductViewModel model)
        {
            // KỊCH BẢN 9: Kiểm tra quyền lần nữa
            if (Session["TaiKhoan"] == null || Convert.ToInt32(Session["RoleID"]) == 3 || Convert.ToInt32(Session["RoleID"]) == 4)
            {
                TempData["WarningMessage"] = "Bạn không có quyền thực hiện chức năng này.";
                return RedirectToAction("Dashboard", "Admin");
            }

            if (ModelState.IsValid)
            {
                // KỊCH BẢN 7: Trùng tên (Loại trừ ID của chính sản phẩm đang sửa)
                bool isExist = db.Products.Any(p => p.ProductName.ToLower() == model.ProductName.Trim().ToLower() && p.ProductID != model.ProductID);
                if (isExist)
                {
                    ModelState.AddModelError("ProductName", "Tên sản phẩm này đã tồn tại trong hệ thống. Vui lòng chọn tên khác.");
                    PrepareCategoryDropdown();
                    return View(model);
                }

                var productToUpdate = db.Products.SingleOrDefault(p => p.ProductID == model.ProductID);
                if (productToUpdate == null) return HttpNotFound();

                // KỊCH BẢN 2 & 6: Xử lý Upload ảnh mới
                if (model.ImageUpload != null && model.ImageUpload.ContentLength > 0)
                {
                    if (model.ImageUpload.ContentLength > 5242880) // 5MB
                    {
                        ModelState.AddModelError("ImageUpload", "Dung lượng ảnh không được vượt quá 5MB.");
                        PrepareCategoryDropdown(); return View(model);
                    }

                    string extension = Path.GetExtension(model.ImageUpload.FileName).ToLower();
                    if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
                    {
                        ModelState.AddModelError("ImageUpload", "Định dạng tệp không hỗ trợ (chỉ nhận .jpg, .png).");
                        PrepareCategoryDropdown(); return View(model);
                    }

                    string folderPath = Server.MapPath("~/Content/Images/Products/");
                    if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                    string newFileName = DateTime.Now.Ticks.ToString() + extension;
                    string savePath = Path.Combine(folderPath, newFileName);
                    model.ImageUpload.SaveAs(savePath);

                    // Xóa file ảnh cũ trên server để tiết kiệm dung lượng
                    if (!string.IsNullOrEmpty(model.ExistingImageURL))
                    {
                        string oldFilePath = Path.Combine(folderPath, model.ExistingImageURL);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    // Gán tên ảnh mới cho DB
                    productToUpdate.ImageURL = newFileName;
                }
                else
                {
                    // Nếu không upload ảnh mới, giữ nguyên ảnh cũ
                    productToUpdate.ImageURL = model.ExistingImageURL;
                }

                // KỊCH BẢN 1 & 3: Cập nhật các trường còn lại
                productToUpdate.ProductName = model.ProductName.Trim();
                productToUpdate.CategoryID = model.CategoryID;
                productToUpdate.Price = model.Price;
                productToUpdate.StockQuantity = model.StockQuantity;
                productToUpdate.Description = model.Description;
                productToUpdate.IsActive = model.IsActive;

                // KỊCH BẢN 8: EF sẽ tự bỏ qua nếu không có gì thay đổi (No changes made)
                db.SaveChanges();

                TempData["SuccessMessage"] = "Cập nhật thông tin sản phẩm thành công.";
                return RedirectToAction("ProductManagement", "Admin");
            }

            PrepareCategoryDropdown();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Bảo mật: Chống giả mạo request từ bên ngoài
        public ActionResult DeleteProduct(int id)
        {
            // ==========================================
            // SCENARIO 6: Bảo mật phân quyền (Chỉ Admin/Manager mới được xóa)
            // ==========================================
            if (Session["TaiKhoan"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }

            int roleId = Convert.ToInt32(Session["RoleID"]);
            // Giả sử Staff (3) và Customer (4) không có quyền xóa
            if (roleId == 3 || roleId == 4)
            {
                TempData["WarningMessage"] = "Bạn không có quyền thực hiện chức năng xóa sản phẩm.";
                return RedirectToAction("ProductManagement", "Admin");
            }

            var productToDelete = db.Products.SingleOrDefault(p => p.ProductID == id);
            if (productToDelete == null)
            {
                TempData["WarningMessage"] = "Không tìm thấy sản phẩm cần xóa.";
                return RedirectToAction("ProductManagement", "Admin");
            }

            // ==========================================
            // SCENARIO 2: Kiểm tra lịch sử Đơn hàng (OrderDetails)
            // ==========================================
            bool hasOrders = db.OrderDetails.Any(od => od.ProductID == id); //
            if (hasOrders)
            {
                TempData["WarningMessage"] = "Không thể xóa sản phẩm này vì đã tồn tại trong lịch sử Đơn hàng. Vui lòng Sửa và chuyển trạng thái thành 'Ngừng bán'.";
                return RedirectToAction("ProductManagement", "Admin");
            }

            // ==========================================
            // SCENARIO 3: Kiểm tra lịch sử Nhập kho (PurchaseReceiptDetails)
            // ==========================================
            bool hasReceipts = db.PurchaseReceiptDetails.Any(pr => pr.ProductID == id); //
            if (hasReceipts)
            {
                TempData["WarningMessage"] = "Không thể xóa sản phẩm này vì có liên kết với dữ liệu Phiếu nhập kho.";
                return RedirectToAction("ProductManagement", "Admin");
            }

            try
            {
                // ==========================================
                // SCENARIO 4: Cascade Delete - Xóa sản phẩm khỏi Giỏ hàng của khách (CartItems)
                // ==========================================
                var cartItemsToRemove = db.CartItems.Where(c => c.ProductID == id).ToList(); //
                if (cartItemsToRemove.Any())
                {
                    db.CartItems.RemoveRange(cartItemsToRemove);
                }

                // ==========================================
                // SCENARIO 1: Xóa thành công (Happy Path)
                // ==========================================

                // (Tùy chọn) Xóa file ảnh trên server để giải phóng dung lượng
                if (!string.IsNullOrEmpty(productToDelete.ImageURL))
                {
                    string filePath = Path.Combine(Server.MapPath("~/Content/Images/Products/"), productToDelete.ImageURL);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                // Xóa bản ghi trong database
                db.Products.Remove(productToDelete);
                db.SaveChanges();

                TempData["WarningMessage"] = "Xóa sản phẩm thành công.";
            }
            catch (Exception ex)
            {
                TempData["WarningMessage"] = "Lỗi hệ thống: Không thể xóa do lỗi cơ sở dữ liệu. Chi tiết: " + ex.Message;
            }

            return RedirectToAction("ProductManagement", "Admin");
        }




    }
}