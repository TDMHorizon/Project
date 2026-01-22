using QuanLyCongViec.DataAccess;
using QuanLyCongViec.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyCongViec
{
    public partial class frmProfile : Form
    {

        #region 1. KHAI BÁO BIẾN & THUỘC TÍNH
        private int _currentUserId;
        private string _currentPasswordHashInDb; // Lưu hash pass cũ để kiểm tra

        // Dùng để trả tên mới về FormMain
        public string NewFullName { get; private set; }

        private string _originalFullName;  // Lưu tên gốc để so sánh thay đổi
        private string _originalEmail; // Lưu email gốc để so sánh thay đổi

        #endregion

        #region 2. CONSTRUCTOR

        public frmProfile(int userId, string username, string fullName)
        {
            InitializeComponent();
            _currentUserId = userId;
            NewFullName = string.Empty; // Mặc định rỗng
            // Khi user sửa Họ tên hoặc Email → kiểm tra thay đổi
            txt_Hoten.TextChanged += (s, e) => KiemTraThayDoiThongTin();
            txt_Email.TextChanged += (s, e) => KiemTraThayDoiThongTin();
        }

        #endregion

        #region Tải Thông Tin User

        // ⭐ Hiển thị dữ liệu user lên form

        private void HienThiDuLieuLenForm(DataRow row)
        {
            // Tab Thông tin
            txt_Username.Text = row["Username"].ToString();
            txt_Hoten.Text = row["FullName"].ToString();
            txt_Email.Text = row["Email"].ToString();

            // Lưu hash mật khẩu để đối chiếu sau này
            _currentPasswordHashInDb = row["PasswordHash"].ToString();

            // Hiển thị ngày tạo tài khoản
            if (row["CreatedAt"] != DBNull.Value)
            {
                txt_Ngaytao.Text = Convert.ToDateTime(row["CreatedAt"]).ToString("dd/MM/yyyy");
            }
        }

        // ⭐ Xóa trắng các ô mật khẩu sau khi đổi
        private void ResetONhapMatKhau()
        {
            txt_MatKhauCu.Clear();
            txt_MatKhauMoi.Clear();
            txt_XacNhanMatKhauMoi.Clear();
        }

        // ⭐ Lấy thông tin user từ DB
        private void TaiThongTinUser()
        {
            try
            {
                SqlParameter[] parameters =
                {
                    new SqlParameter("@UserId", _currentUserId)
                };

                DataTable dt = DatabaseHelper.ExecuteStoredProcedure(
                    "sp_GetUserById",
                    parameters
                );

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    txt_Username.Text = row["Username"].ToString();
                    txt_Hoten.Text = row["FullName"].ToString();
                    txt_Email.Text = row["Email"].ToString();
                    txt_Ngaytao.Text = Convert.ToDateTime(row["CreatedDate"])
                        .ToString("dd/MM/yyyy");
                }

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];

                    txt_Username.Text = row["Username"].ToString();
                    txt_Hoten.Text = row["FullName"].ToString();
                    txt_Email.Text = row["Email"].ToString();
                    txt_Ngaytao.Text = Convert.ToDateTime(row["CreatedDate"])
                        .ToString("dd/MM/yyyy");

                    // Lưu giá trị gốc ban đầu để kiểm tra thay đổi
                    _originalFullName = txt_Hoten.Text;
                    _originalEmail = txt_Email.Text;

                    // 🔹 Ban đầu chưa thay đổi → disable nút và màu xanh dương
                    btn_capnhat.Enabled = false;
                    btn_capnhat.BackColor = ColorTranslator.FromHtml("#3498db"); // Xanh dương
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải thông tin tài khoản: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region KIỂM TRA THAY ĐỔI

        // ⭐ Kiểm tra xem Họ tên / Email có thay đổi không
        private void KiemTraThayDoiThongTin()
        {
            bool daThayDoi =
                txt_Hoten.Text.Trim() != _originalFullName ||
                txt_Email.Text.Trim() != _originalEmail;

            btn_capnhat.Enabled = daThayDoi; // Enable nút nếu có thay đổi
            
            // Đổi màu nút khi có thay đổi
            if (daThayDoi)
            {
                btn_capnhat.BackColor = ColorTranslator.FromHtml("#27ae60"); // Xanh lá
            }
            else
            {
                btn_capnhat.BackColor = ColorTranslator.FromHtml("#3498db"); // Xanh dương
            }
        }

        #endregion

        #region VALIDATION

        // ⭐ Kiểm tra định dạng email hợp lệ
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Regex pattern cho email hợp lệ
                string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region XỬ LÝ CẬP NHẬT THÔNG TIN

        private void XuLyCapNhatThongTin()
        {
            // 1. Validate
            string hoTen = txt_Hoten.Text.Trim();
            string email = txt_Email.Text.Trim();

            // Cập nhật lại giá trị gốc
            _originalFullName = txt_Hoten.Text.Trim();
            _originalEmail = txt_Email.Text.Trim();

            // Disable lại nút và đổi màu về xanh dương
            btn_capnhat.Enabled = false;
            btn_capnhat.BackColor = ColorTranslator.FromHtml("#3498db"); // Xanh dương

            // Kiểm tra họ tên trống
            if (string.IsNullOrEmpty(hoTen))
            {
                MessageBox.Show("Họ tên không được để trống.", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txt_Hoten.Focus();
                return;
            }

            // Kiểm tra email không được để trống
            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Email không được để trống.", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txt_Email.Focus();
                return;
            }

            // Kiểm tra định dạng email
            if (!IsValidEmail(email))
            {
                MessageBox.Show("Email không hợp lệ! Vui lòng nhập đúng định dạng email.\nVí dụ: example@gmail.com", 
                    "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txt_Email.Focus();
                return;
            }

            // 2. Gọi Stored Procedure qua DatabaseHelper
            try
            {
                SqlParameter[] parameters =
                {
                    new SqlParameter("@UserId", _currentUserId),
                    new SqlParameter("@FullName", hoTen),
                    new SqlParameter("@Email", email)
                };

                // Giả định ExecuteStoredProcedure trả về DataTable, 
                // hoặc bạn có thể dùng ExecuteNonQuery nếu Helper hỗ trợ.
                // Ở đây dùng ExecuteStoredProcedure (trả về bảng rỗng nếu update thành công không select gì)
                DatabaseHelper.ExecuteStoredProcedure("sp_UpdateUser", parameters);

                MessageBox.Show("Cập nhật thông tin thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Gán giá trị vào biến public để FormMain cập nhật lại Label Xin chào
                NewFullName = hoTen;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Đổi Mật Khẩu

        private void XuLyDoiMatKhau()
        {
            string matKhauCu = txt_MatKhauCu.Text.Trim();
            string matKhauMoi = txt_MatKhauMoi.Text.Trim();
            string xacNhan = txt_XacNhanMatKhauMoi.Text.Trim();

            // 1. Validate rỗng
            if (string.IsNullOrEmpty(matKhauCu) ||
                string.IsNullOrEmpty(matKhauMoi) ||
                string.IsNullOrEmpty(xacNhan))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ mật khẩu",
                    "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Kiểm tra xác nhận mật khẩu
            if (matKhauMoi != xacNhan)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!",
                    "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 3. Kiểm tra độ dài mật khẩu
            if (matKhauMoi.Length < 6)
            {
                MessageBox.Show("Mật khẩu mới phải có ít nhất 6 ký tự!",
                    "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 4. Không hash mật khẩu, lưu dạng plain text
                SqlParameter[] parameters =
        {
        new SqlParameter("@UserId", _currentUserId),
        new SqlParameter("@OldPassword", matKhauCu),
        new SqlParameter("@NewPassword", matKhauMoi)
    };

                // 5. Gọi stored procedure
                int rowsAffected = DatabaseHelper.ExecuteScalarStoredProcedure(
                "sp_ChangePassword",
                parameters
            );

                // 6. Kiểm tra kết quả 
                if (rowsAffected == 0)
                {
                    // Có dòng được update -> Thành công
                    MessageBox.Show("Đổi mật khẩu thành công!",
                        "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    txt_MatKhauCu.Clear();
                    txt_MatKhauMoi.Clear();
                    txt_XacNhanMatKhauMoi.Clear();
                }
                else
                {
                    // Không có dòng nào được update -> Mật khẩu cũ sai (hoặc ID không tồn tại)
                    MessageBox.Show("Mật khẩu cũ không đúng! Vui lòng kiểm tra lại.",
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txt_MatKhauCu.Focus(); // Đưa con trỏ về ô nhập lại
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hệ thống: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        // ⭐ Nút đóng form
        private void btn_Dong_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // ⭐ Khi form load → tải thông tin user
        private void frmProfile_Load(object sender, EventArgs e)
        {
            TaiThongTinUser();
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        // ⭐ Nút cập nhật thông tin
        private void btn_capnhat_Click(object sender, EventArgs e)
        {
            XuLyCapNhatThongTin();
        }

        // ⭐ Nút đổi mật khẩu
        private void btn_Doi_Click(object sender, EventArgs e)
        {
            XuLyDoiMatKhau();
        }
    }
}
