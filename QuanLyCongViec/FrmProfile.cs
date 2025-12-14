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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyCongViec
{
    public partial class FrmProfile : Form
    {

        #region 1. KHAI BÁO BIẾN & THUỘC TÍNH
        private int _currentUserId;
        private string _currentPasswordHashInDb; // Lưu hash pass cũ để kiểm tra

        // Thuộc tính công khai để FormMain lấy tên mới cập nhật (nếu có)
        public string NewFullName { get; private set; }

        private string _originalFullName;
        private string _originalEmail;

        #endregion

        #region 2. CONSTRUCTOR

        public FrmProfile(int userId, string username, string fullName)
        {
            InitializeComponent();
            _currentUserId = userId;
            NewFullName = string.Empty; // Mặc định rỗng

            txt_Hoten.TextChanged += (s, e) => KiemTraThayDoiThongTin();
            txt_Email.TextChanged += (s, e) => KiemTraThayDoiThongTin();

            // Cấu hình giao diện ban đầu (nếu cần)

        }

        #endregion

        #region Tải Thông Tin User

        // Phương thức tải thông tin người dùng từ DB

        private void HienThiDuLieuLenForm(DataRow row)
        {
            // Tab Thông tin
            txt_Username.Text = row["Username"].ToString();
            txt_Hoten.Text = row["FullName"].ToString();
            txt_Email.Text = row["Email"].ToString();

            // Lưu hash mật khẩu để đối chiếu sau này
            _currentPasswordHashInDb = row["PasswordHash"].ToString();

            // Hiển thị ngày tháng
            if (row["CreatedAt"] != DBNull.Value)
            {
                txt_Ngaytao.Text = Convert.ToDateTime(row["CreatedAt"]).ToString("dd/MM/yyyy");
            }
        }

        private void ResetONhapMatKhau()
        {
            txt_MatKhauCu.Clear();
            txt_MatKhauMoi.Clear();
            txt_XacNhanMatKhauMoi.Clear();
        }

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

                    // 🔹 LƯU GIÁ TRỊ GỐC
                    _originalFullName = txt_Hoten.Text;
                    _originalEmail = txt_Email.Text;

                    // 🔹 Ban đầu chưa thay đổi → disable nút
                    btn_capnhat.Enabled = false;
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
        private void KiemTraThayDoiThongTin()
        {
            bool daThayDoi =
                txt_Hoten.Text.Trim() != _originalFullName ||
                txt_Email.Text.Trim() != _originalEmail;

            btn_capnhat.Enabled = daThayDoi;
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

            // Disable lại nút
            btn_capnhat.Enabled = false;


            if (string.IsNullOrEmpty(hoTen))
            {
                MessageBox.Show("Họ tên không được để trống.", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!",
                    "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Validate xác nhận mật khẩu
            if (matKhauMoi != xacNhan)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!",
                    "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 3. Validate độ dài mật khẩu
            if (matKhauMoi.Length < 6)
            {
                MessageBox.Show("Mật khẩu mới phải có ít nhất 6 ký tự!",
                    "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 4. Hash mật khẩu
                string hashedOldPassword = PasswordHelper.HashPassword(matKhauCu);
                string hashedNewPassword = PasswordHelper.HashPassword(matKhauMoi);

                SqlParameter[] parameters =
        {
        new SqlParameter("@UserId", _currentUserId),
        new SqlParameter("@OldPasswordHash", hashedOldPassword),
        new SqlParameter("@NewPasswordHash", hashedNewPassword)
    };

                // 5. Gọi stored procedure
                int rowsAffected = DatabaseHelper.ExecuteScalarStoredProcedure(
                "sp_ChangePassword",
                parameters
            );

                // 6. Kiểm tra kết quả (SỬA LẠI ĐOẠN NÀY)
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

        private void btn_Dong_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void FrmProfile_Load(object sender, EventArgs e)
        {
            TaiThongTinUser();
        }

        private void btn_capnhat_Click(object sender, EventArgs e)
        {
            XuLyCapNhatThongTin();
        }

        private void btn_Doi_Click(object sender, EventArgs e)
        {
            XuLyDoiMatKhau();
        }
    }
}
