using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using QuanLyCongViec.DataAccess;
using QuanLyCongViec.Helpers;

namespace QuanLyCongViec
{
    //Form đăng ký tài khoản mới cho hệ thống Quản lý Công việc
    public partial class frmDangKy : Form
    {

        #region Properties - Thuộc tính
        //Username đã đăng ký thành công (để trả về form đăng nhập)
        public string RegisteredUsername { get; private set; }
        #endregion

        #region Constructor - Hàm khởi tạo
        //Khởi tạo form đăng ký
        public frmDangKy()
        {
            InitializeComponent();
        }
        #endregion

        #region Event Handlers - Xử lý sự kiện
        //Xử lý sự kiện click nút Đăng ký
        //<param name="sender">Đối tượng gửi sự kiện</param>
        //<param name="e">Thông tin sự kiện</param>
        private void btnXacNhan_Click(object sender, EventArgs e)
        {
            PerformRegister();
        }
        //Xử lý sự kiện click link Đăng nhập
        //Đóng form và quay về form đăng nhập
        //<param name="sender">Đối tượng gửi sự kiện</param>
        //<param name="e">Thông tin sự kiện</param>
        private void linklblDangNhap_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CloseAndReturnToLogin();
        }
        #endregion

        #region Private Methods - Các phương thức riêng tư
        //Đóng form và trả về DialogResult.Cancel để báo hiệu người dùng hủy đăng ký
        private void CloseAndReturnToLogin()
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        //Thực hiện quá trình đăng ký tài khoản mới
        //Bao gồm: validate input, hash password, gọi stored procedure, xử lý kết quả
        private void PerformRegister()
        {
            try
            {
                // Validate dữ liệu đầu vào
                if (!ValidateInput())
                {
                    return;
                }
                // Lấy thông tin từ form
                RegistrationData duLieuDangKy = GetRegistrationData();
                // Hash mật khẩu
                string maHoaMatKhau = HashPassword(duLieuDangKy.Password);
                // Đăng ký tài khoản vào database
                int maNguoiDung = RegisterUserToDatabase(duLieuDangKy, maHoaMatKhau);
                // Xử lý kết quả đăng ký
                ProcessRegistrationResult(maNguoiDung, duLieuDangKy);
            }
            catch (Exception loi)
            {
                ShowRegistrationError(loi);
            }
        }

        
        //Lấy dữ liệu đăng ký từ các control trên form
        
        //<returns>Đối tượng chứa thông tin đăng ký</returns>
        private RegistrationData GetRegistrationData()
        {
            return new RegistrationData
            {
                Username = txtTaiKhoan.Text.Trim(),
                Password = txtMatKhau.Text,
                FullName = txtHoTen.Text.Trim(),
                Email = txtEmail.Text.Trim()
            };
        }

        
        //Hash mật khẩu bằng PasswordHelper
        
        //<param name="matKhau">Mật khẩu gốc</param>
        //<returns>Mật khẩu đã được hash</returns>
        private string HashPassword(string matKhau)
        {
            return PasswordHelper.HashPassword(matKhau);
        }

        
        //Đăng ký user vào database thông qua stored procedure
        
        //<param name="duLieuDangKy">Thông tin đăng ký</param>
        //<param name="maHoaMatKhau">Mật khẩu đã hash</param>
        //<returns>Mã người dùng nếu thành công, mã lỗi nếu thất bại</returns>
        private int RegisterUserToDatabase(RegistrationData duLieuDangKy, string maHoaMatKhau)
        {
            SqlParameter thamSoMaNguoiDung = new SqlParameter("@UserId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            SqlParameter[] thamSo = new SqlParameter[]
            {
                new SqlParameter("@Username", duLieuDangKy.Username),
                new SqlParameter("@PasswordHash", maHoaMatKhau),
                new SqlParameter("@FullName", duLieuDangKy.FullName),
                new SqlParameter("@Email", duLieuDangKy.Email),
                thamSoMaNguoiDung
            };

            DatabaseHelper.ExecuteStoredProcedureNonQuery("sp_UserRegister", thamSo);

            // Lấy giá trị mã người dùng từ output parameter
            if (thamSoMaNguoiDung.Value != null && thamSoMaNguoiDung.Value != DBNull.Value)
            {
                return Convert.ToInt32(thamSoMaNguoiDung.Value);
            }

            return 0;
        }

        
        //Xử lý kết quả đăng ký từ database
        
        //<param name="maNguoiDung">Mã người dùng hoặc mã lỗi từ stored procedure</param>
        //<param name="duLieuDangKy">Thông tin đăng ký</param>
        private void ProcessRegistrationResult(int maNguoiDung, RegistrationData duLieuDangKy)
        {
            // Lấy các mã lỗi từ database
            int maLoiUsernameTonTai = Helpers.SystemSettings.ErrorUsernameExists;
            int maLoiEmailTonTai = Helpers.SystemSettings.ErrorEmailExists;

            if (maNguoiDung > 0)
            {
                HandleSuccessfulRegistration(maNguoiDung, duLieuDangKy);
            }
            else if (maNguoiDung == maLoiUsernameTonTai)
            {
                HandleUsernameExistsError();
            }
            else if (maNguoiDung == maLoiEmailTonTai)
            {
                HandleEmailExistsError();
            }
            else
            {
                HandleUnknownRegistrationError();
            }
        }

        
        //Xử lý khi đăng ký thành công
        
        //<param name="maNguoiDung">Mã người dùng của tài khoản mới</param>
        //<param name="duLieuDangKy">Thông tin đăng ký</param>
        private void HandleSuccessfulRegistration(int maNguoiDung, RegistrationData duLieuDangKy)
        {
            RegisteredUsername = duLieuDangKy.Username;

            string thongBaoThanhCong = $"Đăng ký thành công!\n\n" +
                                   $"Tài khoản: {duLieuDangKy.Username}\n" +
                                   $"Họ tên: {duLieuDangKy.FullName}\n\n" +
                                   $"Bạn có thể đăng nhập ngay bây giờ.";

            MessageBox.Show(
                thongBaoThanhCong,
                "Thành công",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        
        //Xử lý lỗi username đã tồn tại
        
        private void HandleUsernameExistsError()
        {
            string thongBaoLoi = "Tên đăng nhập đã được sử dụng!\n\nVui lòng chọn tên đăng nhập khác.";

            MessageBox.Show(
                thongBaoLoi,
                "Lỗi đăng ký",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );

            FocusAndSelectAll(txtTaiKhoan);
        }

        
        //Xử lý lỗi email đã tồn tại
        
        private void HandleEmailExistsError()
        {
            string thongBaoLoi = "Email đã được sử dụng!\n\nVui lòng sử dụng email khác.";

            MessageBox.Show(
                thongBaoLoi,
                "Lỗi đăng ký",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );

            FocusAndSelectAll(txtEmail);
        }

        
        //Xử lý lỗi không xác định khi đăng ký
        
        private void HandleUnknownRegistrationError()
        {
            MessageBox.Show(
                "Đăng ký thất bại!\n\nVui lòng thử lại sau.",
                "Lỗi đăng ký",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }

        
        //Focus vào control và select all text
        
        //<param name="control">Control cần focus</param>
        private void FocusAndSelectAll(Control control)
        {
            control.Focus();
            if (control is TextBox textBox)
            {
                textBox.SelectAll();
            }
        }

        
        //Hiển thị thông báo lỗi khi có exception xảy ra
        
        //<param name="loi">Exception xảy ra</param>
        private void ShowRegistrationError(Exception loi)
        {
            MessageBox.Show(
                $"Lỗi khi đăng ký!\n\nChi tiết: {loi.Message}",
                "Lỗi",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }

        
        //Validate toàn bộ dữ liệu đầu vào của form đăng ký
        
        //<returns>True nếu tất cả dữ liệu hợp lệ, False nếu có lỗi</returns>
        private bool ValidateInput()
        {
            return ValidateUsername() &&
                   ValidatePassword() &&
                   ValidatePasswordConfirmation() &&
                   ValidateFullName() &&
                   ValidateEmail() &&
                   ValidateTermsAndConditions();
        }

        
        //Validate tên đăng nhập
        
        //<returns>True nếu hợp lệ, False nếu không</returns>
        private bool ValidateUsername()
        {
            if (string.IsNullOrWhiteSpace(txtTaiKhoan.Text))
            {
                ShowValidationError("Vui lòng nhập tên đăng nhập!", txtTaiKhoan);
                return false;
            }

            string tenDangNhap = txtTaiKhoan.Text.Trim();

            int doDaiToiThieu = Helpers.ValidationLimits.MinUsernameLength;
            int doDaiToiDa = Helpers.ValidationLimits.MaxUsernameLength;

            if (tenDangNhap.Length < doDaiToiThieu)
            {
                ShowValidationError($"Tên đăng nhập phải có ít nhất {doDaiToiThieu} ký tự!", txtTaiKhoan);
                return false;
            }

            if (tenDangNhap.Length > doDaiToiDa)
            {
                ShowValidationError($"Tên đăng nhập không được vượt quá {doDaiToiDa} ký tự!", txtTaiKhoan);
                return false;
            }

            if (tenDangNhap.Contains(" "))
            {
                ShowValidationError("Tên đăng nhập không được chứa khoảng trắng!", txtTaiKhoan);
                return false;
            }

            return true;
        }

        
        //Validate mật khẩu
        
        //<returns>True nếu hợp lệ, False nếu không</returns>
        private bool ValidatePassword()
        {
            if (string.IsNullOrWhiteSpace(txtMatKhau.Text))
            {
                ShowValidationError("Vui lòng nhập mật khẩu!", txtMatKhau);
                return false;
            }

            string matKhau = txtMatKhau.Text;

            int doDaiToiThieu = Helpers.ValidationLimits.MinPasswordLength;
            int doDaiToiDa = Helpers.ValidationLimits.MaxPasswordLength;

            if (matKhau.Length < doDaiToiThieu)
            {
                ShowValidationError($"Mật khẩu phải có ít nhất {doDaiToiThieu} ký tự!", txtMatKhau);
                return false;
            }

            if (matKhau.Length > doDaiToiDa)
            {
                ShowValidationError($"Mật khẩu không được vượt quá {doDaiToiDa} ký tự!", txtMatKhau);
                return false;
            }

            // Kiểm tra độ phức tạp mật khẩu
            string loiMatKhau = ValidatePasswordStrength(matKhau);
            if (!string.IsNullOrEmpty(loiMatKhau))
            {
                ShowValidationError(loiMatKhau, txtMatKhau);
                return false;
            }

            return true;
        }

        
        //Validate xác nhận mật khẩu
        
        //<returns>True nếu hợp lệ, False nếu không</returns>
        private bool ValidatePasswordConfirmation()
        {
            if (string.IsNullOrWhiteSpace(txtXacNhanMatKhau.Text))
            {
                ShowValidationError("Vui lòng xác nhận mật khẩu!", txtXacNhanMatKhau);
                return false;
            }

            if (txtMatKhau.Text != txtXacNhanMatKhau.Text)
            {
                ShowValidationError("Mật khẩu xác nhận không khớp!\n\nVui lòng nhập lại.", txtXacNhanMatKhau);
                return false;
            }

            return true;
        }

        
        //Validate họ tên
        
        //<returns>True nếu hợp lệ, False nếu không</returns>
        private bool ValidateFullName()
        {
            if (string.IsNullOrWhiteSpace(txtHoTen.Text))
            {
                ShowValidationError("Vui lòng nhập họ tên!", txtHoTen);
                return false;
            }

            string hoTen = txtHoTen.Text.Trim();
            int doDaiToiDa = Helpers.ValidationLimits.MaxFullNameLength;
            if (hoTen.Length > doDaiToiDa)
            {
                ShowValidationError($"Họ tên không được vượt quá {doDaiToiDa} ký tự!", txtHoTen);
                return false;
            }

            return true;
        }

        
        //Validate email
        
        //<returns>True nếu hợp lệ, False nếu không</returns>
        private bool ValidateEmail()
        {
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                ShowValidationError("Vui lòng nhập email!", txtEmail);
                return false;
            }

            string thuDienTu = txtEmail.Text.Trim();

            int doDaiToiDa = Helpers.ValidationLimits.MaxEmailLength;
            if (thuDienTu.Length > doDaiToiDa)
            {
                ShowValidationError($"Email không được vượt quá {doDaiToiDa} ký tự!", txtEmail);
                return false;
            }

            if (!IsValidEmail(thuDienTu))
            {
                ShowValidationError("Email không hợp lệ!\n\nVui lòng nhập đúng định dạng email.\nVí dụ: example@email.com", txtEmail);
                return false;
            }

            return true;
        }

        
        //Validate checkbox đồng ý điều khoản
        
        //<returns>True nếu đã check, False nếu chưa</returns>
        private bool ValidateTermsAndConditions()
        {
            if (!chkDongYDieuKhoan.Checked)
            {
                ShowValidationError("Bạn phải đồng ý với các điều khoản sử dụng để tiếp tục đăng ký!", chkDongYDieuKhoan);
                return false;
            }

            return true;
        }

        
        //Hiển thị thông báo lỗi validation và focus vào control tương ứng
        
        //<param name="message">Thông báo lỗi</param>
        //<param name="control">Control cần focus</param>
        private void ShowValidationError(string message, Control control)
        {
            MessageBox.Show(
                message,
                "Thông báo",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );

            control.Focus();

            // Select all text nếu là TextBox
            if (control is TextBox textBox)
            {
                textBox.SelectAll();
            }
        }

        
        //Kiểm tra độ phức tạp mật khẩu
        //Hiện tại chỉ yêu cầu độ dài tối thiểu
        //Có thể mở rộng thêm yêu cầu: chữ hoa, chữ thường, số, ký tự đặc biệt
        
        //<param name="matKhau">Mật khẩu cần kiểm tra</param>
        //<returns>Thông báo lỗi nếu không hợp lệ, null nếu hợp lệ</returns>
        private string ValidatePasswordStrength(string matKhau)
        {
            // Hiện tại chỉ yêu cầu độ dài tối thiểu
            // Có thể mở rộng thêm yêu cầu: chữ hoa, chữ thường, số, ký tự đặc biệt
            return null;
        }

        
        //Kiểm tra email có hợp lệ không bằng regex pattern
        
        //<param name="thuDienTu">Email cần kiểm tra</param>
        //<returns>True nếu email hợp lệ, False nếu không</returns>
        private bool IsValidEmail(string thuDienTu)
        {
            if (string.IsNullOrWhiteSpace(thuDienTu))
            {
                return false;
            }

            try
            {
                // Regex pattern để kiểm tra định dạng email
                string mauRegex = @"^[a-zA-Z0-9]([a-zA-Z0-9._-]*[a-zA-Z0-9])?@[a-zA-Z0-9]([a-zA-Z0-9.-]*[a-zA-Z0-9])?\.[a-zA-Z]{2,}$";

                // Kiểm tra cơ bản trước
                if (!thuDienTu.Contains("@") || !thuDienTu.Contains("."))
                {
                    return false;
                }

                // Kiểm tra không có khoảng trắng
                if (thuDienTu.Contains(" "))
                {
                    return false;
                }

                // Kiểm tra @ không ở đầu hoặc cuối
                if (thuDienTu.StartsWith("@") || thuDienTu.EndsWith("@"))
                {
                    return false;
                }

                // Kiểm tra . không ở đầu hoặc cuối
                if (thuDienTu.StartsWith(".") || thuDienTu.EndsWith("."))
                {
                    return false;
                }

                // Kiểm tra regex pattern
                return Regex.IsMatch(thuDienTu, mauRegex, RegexOptions.IgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Nested Classes - Lớp lồng nhau

        //Lớp chứa thông tin đăng ký từ form
        private class RegistrationData
        {
            //Tên đăng nhập
            public string Username { get; set; }
            //Mật khẩu
            public string Password { get; set; }
            //Họ tên đầy đủ
            public string FullName { get; set; }
            //Email
            public string Email { get; set; }
        }

        #endregion
    }
}
