using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using QuanLyCongViec.DataAccess;
using QuanLyCongViec.Helpers;

namespace QuanLyCongViec
{
    //Form đăng nhập vào hệ thống Quản lý Công việc
    public partial class frmDangNhap : Form
    {
        #region Private Fields - Các trường dữ liệu riêng tư
        //Số lần đăng nhập thất bại liên tiếp
        private int _soLanDangNhapSai = 0;
        //Thời điểm tài khoản bị khóa đến khi nào (null nếu không bị khóa)
        private DateTime? _thoiGianKhoaDen = null;
        #endregion

        #region Constructor - Hàm khởi tạo

        //Khởi tạo form đăng nhập
        public frmDangNhap()
        {
            InitializeComponent();
            TestDatabaseConnection();
            LoadRememberedCredentials();
        }

        #endregion

        #region Event Handlers - Xử lý sự kiện
        //Xử lý sự kiện click nút Đăng nhập
        //<param name="sender">Đối tượng gửi sự kiện</param>
        //<param name="e">Thông tin sự kiện</param>
        private void btnXacNhan_Click(object sender, EventArgs e)
        {
            PerformLogin();
        }
        //Xử lý sự kiện click link Đăng ký
        //Mở form đăng ký và xử lý kết quả sau khi đóng
        //<param name="sender">Đối tượng gửi sự kiện</param>
        //<param name="e">Thông tin sự kiện</param>
        private void linklblDangKy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenRegistrationForm();
        }
        //Xử lý sự kiện nhấn phím Enter trong textbox Tài khoản
        //Chuyển focus sang textbox Mật khẩu
        //<param name="sender">Đối tượng gửi sự kiện</param>
        //<param name="e">Thông tin sự kiện phím</param>
        private void txtTaiKhoan_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                txtMatKhau.Focus();
            }
        }
        //Xử lý sự kiện nhấn phím Enter trong textbox Mật khẩu
        //Thực hiện đăng nhập
        //<param name="sender">Đối tượng gửi sự kiện</param>
        //<param name="e">Thông tin sự kiện phím</param>
        private void txtMatKhau_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                PerformLogin();
            }
        }
        #endregion

        #region Private Methods - Các phương thức riêng tư
        //Kiểm tra kết nối database khi form được load
        //Hiển thị thông báo lỗi nếu không kết nối được
        private void TestDatabaseConnection()
        {
            try
            {
                DatabaseHelper.TestConnection();
            }
            catch (Exception loi)
            {
                ShowDatabaseConnectionError(loi);
            }
        }
        //Hiển thị thông báo lỗi kết nối database
        //<param name="loi">Exception xảy ra khi kết nối</param>
        private void ShowDatabaseConnectionError(Exception loi)
        {
            string thongBaoLoi = "❌ Lỗi kết nối database!\n\n" +
                                "Chi tiết: " + loi.Message + "\n\n" +
                                "Vui lòng kiểm tra:\n" +
                                "1. SQL Server đang chạy\n" +
                                "2. Database 'QuanLyCongViec' đã được tạo\n" +
                                "3. Connection string trong App.config đúng\n\n" +
                                "Xem file README_SHARE.md trong project để biết cách setup.";

            MessageBox.Show(
                thongBaoLoi,
                "Lỗi kết nối",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
        //Mở form đăng ký và xử lý kết quả
        //Nếu đăng ký thành công, tự động điền username vào form đăng nhập
        private void OpenRegistrationForm()
        {
            frmDangKy frmDangKy = new frmDangKy();
            this.Hide();

            DialogResult ketQua = frmDangKy.ShowDialog();
            this.Show();

            if (ketQua == DialogResult.OK && !string.IsNullOrWhiteSpace(frmDangKy.RegisteredUsername))
            {
                FillUsernameAfterRegistration(frmDangKy.RegisteredUsername);
            }
        }
        //Điền tên đăng nhập vào textbox sau khi đăng ký thành công
        //<param name="tenDangNhap">Tên đăng nhập đã đăng ký</param>
        private void FillUsernameAfterRegistration(string tenDangNhap)
        {
            txtTaiKhoan.Text = tenDangNhap;
            txtMatKhau.Clear();
            txtMatKhau.Focus();
        }
        //Thực hiện quá trình đăng nhập
        //Bao gồm: kiểm tra lockout, validate input, xác thực thông tin, xử lý kết quả
        private void PerformLogin()
        {
            try
            {
                // Kiểm tra tài khoản có bị khóa không
                if (IsAccountLocked())
                {
                    return;
                }
                // Reset lockout nếu đã hết hạn
                ResetLockoutIfExpired();
                // Validate dữ liệu đầu vào
                if (!ValidateLoginInput())
                {
                    return;
                }
                // Thực hiện xác thực đăng nhập
                DataTable loginResult = AuthenticateUser();
                // Xử lý kết quả đăng nhập
                ProcessLoginResult(loginResult);
            }
            catch (Exception loi)
            {
                ShowLoginError(loi);
            }
        }
        //Kiểm tra tài khoản có đang bị khóa không
        //<returns>True nếu tài khoản đang bị khóa, False nếu không</returns>
        private bool IsAccountLocked()
        {
            if (_thoiGianKhoaDen.HasValue && DateTime.Now < _thoiGianKhoaDen.Value)
            {
                int soPhutConLai = (int)(_thoiGianKhoaDen.Value - DateTime.Now).TotalMinutes;
                ShowAccountLockedMessage(soPhutConLai);
                return true;
            }
            return false;
        }
        //Hiển thị thông báo tài khoản bị khóa
        //<param name="soPhutConLai">Số phút còn lại trước khi mở khóa</param>
        private void ShowAccountLockedMessage(int soPhutConLai)
        {
            string thongBao = $"Tài khoản đã bị khóa tạm thời do đăng nhập sai quá nhiều lần!\n\n" +
                           $"Vui lòng thử lại sau {soPhutConLai} phút.";

            MessageBox.Show(
                thongBao,
                "Tài khoản bị khóa",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
        }
        //Reset lockout nếu thời gian khóa đã hết hạn
        private void ResetLockoutIfExpired()
        {
            if (_thoiGianKhoaDen.HasValue && DateTime.Now >= _thoiGianKhoaDen.Value)
            {
                _thoiGianKhoaDen = null;
                _soLanDangNhapSai = 0;
            }
        }
        //Validate dữ liệu đầu vào của form đăng nhập
        //<returns>True nếu dữ liệu hợp lệ, False nếu không</returns>
        private bool ValidateLoginInput()
        {
            if (string.IsNullOrWhiteSpace(txtTaiKhoan.Text))
            {
                ShowValidationError("Vui lòng nhập tên đăng nhập!", txtTaiKhoan);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtMatKhau.Text))
            {
                ShowValidationError("Vui lòng nhập mật khẩu!", txtMatKhau);
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
        }
        //Xác thực thông tin đăng nhập với database
        //<returns>DataTable chứa thông tin user nếu đăng nhập thành công, null nếu thất bại</returns>
        private DataTable AuthenticateUser()
        {
            string tenDangNhap = txtTaiKhoan.Text.Trim();
            string matKhau = txtMatKhau.Text;
            string maHoaMatKhau = PasswordHelper.HashPassword(matKhau);

            SqlParameter[] thamSo = new SqlParameter[]
            {
                new SqlParameter("@Username", tenDangNhap),
                new SqlParameter("@PasswordHash", maHoaMatKhau)
            };

            return DatabaseHelper.ExecuteStoredProcedure("sp_UserLogin", thamSo);
        }
        //Xử lý kết quả đăng nhập từ database
        //<param name="ketQuaDangNhap">Kết quả trả về từ stored procedure</param>
        private void ProcessLoginResult(DataTable ketQuaDangNhap)
        {
            if (ketQuaDangNhap == null || ketQuaDangNhap.Rows.Count == 0)
            {
                HandleFailedLogin();
                return;
            }

            DataRow dongNguoiDung = ketQuaDangNhap.Rows[0];

            // Kiểm tra ErrorCode nếu stored procedure trả về
            if (HasErrorCode(dongNguoiDung))
            {
                HandleErrorCodeResponse(dongNguoiDung);
                return;
            }

            // Kiểm tra Id có null không (trường hợp stored procedure cũ)
            if (IsUserIdNull(dongNguoiDung))
            {
                HandleFailedLogin();
                return;
            }

            // Đăng nhập thành công
            HandleSuccessfulLogin(dongNguoiDung);
        }
        //Kiểm tra xem kết quả có chứa ErrorCode không
        //<param name="dongNguoiDung">Dòng dữ liệu từ database</param>
        //<returns>True nếu có ErrorCode, False nếu không</returns>
        private bool HasErrorCode(DataRow dongNguoiDung)
        {
            return dongNguoiDung.Table.Columns.Contains("ErrorCode");
        }
        //Xử lý response có ErrorCode từ stored procedure
        //<param name="dongNguoiDung">Dòng dữ liệu từ database</param>
        private void HandleErrorCodeResponse(DataRow dongNguoiDung)
        {
            object maLoiObj = dongNguoiDung["ErrorCode"];
            if (maLoiObj == null || maLoiObj == DBNull.Value)
            {
                return;
            }

            int maLoi = Convert.ToInt32(maLoiObj);
            if (maLoi == 0)
            {
                return; // Không có lỗi
            }

            string thongBaoLoi = dongNguoiDung["ErrorMessage"]?.ToString() ?? "Đăng nhập thất bại";

            if (maLoi == 2) // Tài khoản bị khóa
            {
                ShowAccountDisabledMessage(thongBaoLoi);
            }
            else
            {
                _soLanDangNhapSai++;
                HandleFailedLogin();
            }
        }
        //Hiển thị thông báo tài khoản bị vô hiệu hóa
        //<param name="thongBaoLoi">Thông báo lỗi từ database</param>
        private void ShowAccountDisabledMessage(string thongBaoLoi)
        {
            string thongBao = $"Tài khoản đã bị khóa hoặc vô hiệu hóa!\n\n{thongBaoLoi}\n\nVui lòng liên hệ quản trị viên.";

            MessageBox.Show(
                thongBao,
                "Tài khoản bị khóa",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
        }
        //Kiểm tra mã người dùng có null không
        //<param name="dongNguoiDung">Dòng dữ liệu từ database</param>
        //<returns>True nếu mã người dùng null, False nếu không</returns>
        private bool IsUserIdNull(DataRow dongNguoiDung)
        {
            return dongNguoiDung["Id"] == null || dongNguoiDung["Id"] == DBNull.Value;
        }
        //Xử lý khi đăng nhập thành công
        //<param name="dongNguoiDung">Dòng dữ liệu chứa thông tin người dùng</param>
        private void HandleSuccessfulLogin(DataRow dongNguoiDung)
        {
            int maNguoiDung = Convert.ToInt32(dongNguoiDung["Id"]);
            string tenDangNhap = txtTaiKhoan.Text.Trim();
            string hoTen = dongNguoiDung["FullName"].ToString();

            // Reset số lần thất bại
            ResetFailedLoginAttempts();

            // Lưu thông tin đăng nhập nếu chọn "Ghi nhớ"
            SaveOrClearRememberedCredentials(tenDangNhap);

            // LƯU THÔNG TIN NGƯỜI DÙNG VÀO CurrentUser ĐỂ CÁC FORM KHÁC CÓ THỂ SỬ DỤNG
            CurrentUser.SetCurrentUser(maNguoiDung, tenDangNhap, hoTen);

            // Hiển thị thông báo thành công
            ShowSuccessMessage(hoTen);

            // Sau này khi có frmMain, sẽ mở frmMain với thông tin này:
            // this.Hide();
            // frmMain mainForm = new frmMain(maNguoiDung, tenDangNhap, hoTen);
            // mainForm.ShowDialog();
            // this.Close();

            // Đóng form
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        //Reset số lần đăng nhập thất bại về 0
        private void ResetFailedLoginAttempts()
        {
            _soLanDangNhapSai = 0;
            _thoiGianKhoaDen = null;
        }
        //Lưu hoặc xóa thông tin đăng nhập tùy theo checkbox "Ghi nhớ"
        //<param name="tenDangNhap">Tên đăng nhập cần lưu</param>
        private void SaveOrClearRememberedCredentials(string tenDangNhap)
        {
            if (chkGhiNhoDangNhap.Checked)
            {
                SaveRememberedCredentials(tenDangNhap);
            }
            else
            {
                ClearRememberedCredentials();
            }
        }
        //Hiển thị thông báo đăng nhập thành công
        //<param name="hoTen">Họ tên của người dùng</param>
        private void ShowSuccessMessage(string hoTen)
        {
            MessageBox.Show(
                $"Đăng nhập thành công!\n\nXin chào, {hoTen}!",
                "Thành công",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }
        //Xử lý khi đăng nhập thất bại
        //Tăng số lần thất bại và hiển thị thông báo phù hợp
        private void HandleFailedLogin()
        {
            _soLanDangNhapSai++;

            string thongBaoLoi = BuildFailedLoginMessage();

            MessageBox.Show(
                thongBaoLoi,
                "Đăng nhập thất bại",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );

            ClearPasswordField();
        }
        //Xây dựng thông báo lỗi khi đăng nhập thất bại
        //<returns>Thông báo lỗi</returns>
        private string BuildFailedLoginMessage()
        {
            string thongBaoCoBan = "Tên đăng nhập hoặc mật khẩu không đúng!\n\nVui lòng kiểm tra lại.";

            // Lấy giá trị từ database
            int soLanDangNhapSaiToiDa = Helpers.SystemSettings.MaxLoginAttempts;
            int thoiGianKhoaPhut = Helpers.SystemSettings.LockoutMinutes;

            if (_soLanDangNhapSai >= soLanDangNhapSaiToiDa)
            {
                _thoiGianKhoaDen = DateTime.Now.AddMinutes(thoiGianKhoaPhut);
                return thongBaoCoBan + $"\n\n⚠️ Cảnh báo: Bạn đã đăng nhập sai {_soLanDangNhapSai} lần.\n" +
                                   $"Tài khoản sẽ bị khóa tạm thời trong {thoiGianKhoaPhut} phút.";
            }
            else
            {
                int soLanConLai = soLanDangNhapSaiToiDa - _soLanDangNhapSai;
                return thongBaoCoBan + $"\n\nCòn lại {soLanConLai} lần thử.";
            }
        }
        //Xóa trường mật khẩu và focus vào đó
        private void ClearPasswordField()
        {
            txtMatKhau.Clear();
            txtMatKhau.Focus();
        }
        //Hiển thị thông báo lỗi khi có exception xảy ra
        //<param name="loi">Exception xảy ra</param>
        private void ShowLoginError(Exception loi)
        {
            MessageBox.Show(
                $"Lỗi khi đăng nhập!\n\nChi tiết: {loi.Message}",
                "Lỗi",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
        //Lưu thông tin đăng nhập vào Settings (chỉ lưu tên đăng nhập, không lưu mật khẩu)
        //<param name="tenDangNhap">Tên đăng nhập cần lưu</param>
        private void SaveRememberedCredentials(string tenDangNhap)
        {
            try
            {
                Properties.Settings.Default.RememberedUsername = tenDangNhap;
                Properties.Settings.Default.Save();
            }
            catch
            {
                // Bỏ qua lỗi khi lưu settings để không làm gián đoạn quá trình đăng nhập
            }
        }
        //Xóa thông tin đăng nhập đã lưu trong Settings
        private void ClearRememberedCredentials()
        {
            try
            {
                Properties.Settings.Default.RememberedUsername = string.Empty;
                Properties.Settings.Default.Save();
            }
            catch
            {
                // Bỏ qua lỗi khi lưu settings
            }
        }
        //Load thông tin đăng nhập đã lưu từ Settings
        //Tự động điền tên đăng nhập và check checkbox "Ghi nhớ" nếu có
        private void LoadRememberedCredentials()
        {
            try
            {
                string tenDangNhapDaNho = Properties.Settings.Default.RememberedUsername;
                if (!string.IsNullOrWhiteSpace(tenDangNhapDaNho))
                {
                    txtTaiKhoan.Text = tenDangNhapDaNho;
                    chkGhiNhoDangNhap.Checked = true;
                    txtMatKhau.Focus();
                }
            }
            catch
            {
                // Bỏ qua lỗi khi load settings
            }
        }
        #endregion
    }
}
