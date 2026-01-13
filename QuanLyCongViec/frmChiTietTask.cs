using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using QuanLyCongViec.DataAccess;

namespace QuanLyCongViec
{
    public partial class frmChiTietTask : Form
    {
        private int _taskId;
        private int _userId;
        private List<string> _historyLogs = new List<string>();
        private CultureInfo viVN = new CultureInfo("vi-VN");

        // Constructor mặc định
        public frmChiTietTask()
        {
            try
            {
                InitializeComponent();
                LoadAllData(); // Nạp User, Ưu tiên và Trạng thái
                ConfigDateTime();
                DangKySuKien();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khởi tạo form: {ex.Message}\n\n{ex.StackTrace}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        // Constructor dùng khi mở từ danh sách chính - nhận taskId và userId
        public frmChiTietTask(int taskId, int userId) : this()
        {
            _taskId = taskId;
            _userId = userId;
            LoadTaskFromDatabase();
        }

        private void DangKySuKien()
        {
            // 1. Đổi màu chữ theo Ưu tiên
            cboUuTien.SelectedIndexChanged += (s, e) => {
                if (cboUuTien.Text == "Cao") cboUuTien.ForeColor = Color.Red;
                else if (cboUuTien.Text == "Trung bình") cboUuTien.ForeColor = Color.DarkOrange;
                else cboUuTien.ForeColor = Color.Green;
            };

            // 2. Tự động cập nhật Trạng thái và Màu sắc theo thanh Tiến độ
            trkTienDo.Scroll += (s, e) => {
                int val = trkTienDo.Value;
                lblPhanTram.Text = val + "%";
                lblPhanTram.ForeColor = (val < 40) ? Color.Red : (val < 80 ? Color.Orange : Color.Green);

                if (val == 100) cboTrangThai.SelectedItem = "Hoàn thành";
                else if (val > 0) cboTrangThai.SelectedItem = "Đang làm";
                else cboTrangThai.SelectedItem = "Chưa hoàn thành";
            };

            // 3. Xem Nhật ký lưu (Tiếng Việt + Ghi chú)
            btnLichSu.Click += (s, e) => {
                if (_historyLogs.Count == 0)
                    MessageBox.Show("Chưa có nhật ký lưu nào.", "Thông báo");
                else
                    MessageBox.Show(string.Join("\n" + new string('-', 45) + "\n", _historyLogs), "Nhật ký Task");
            };

            // 4. Lưu dữ liệu vào database
            btnLuu.Click += (s, e) => {
                LuuTaskVaoDatabase();
            };

            btnHuy.Click += (s, e) => this.Close();
        }

        private void ConfigDateTime()
        {
            dtpHan.Format = DateTimePickerFormat.Custom;
            dtpHan.CustomFormat = "dd/MM/yyyy";
        }

        private void LoadAllData()
        {
            LoadUsersFromDatabase();

            // Nạp Mức ưu tiên
            cboUuTien.Items.Clear();
            cboUuTien.Items.AddRange(new object[] { "Cao", "Trung bình", "Thấp" });

            // Nạp Trạng thái
            cboTrangThai.Items.Clear();
            cboTrangThai.Items.AddRange(new object[] { "Chưa hoàn thành", "Đang làm", "Hoàn thành" });
        }

        // Load danh sách users từ database
        private void LoadUsersFromDatabase()
        {
            try
            {
                string query = "SELECT Id, FullName FROM Users WHERE IsActive = 1 ORDER BY FullName";
                DataTable dt = DatabaseHelper.ExecuteQuery(query);
                
                cboNguoiLam.Items.Clear();
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        cboNguoiLam.Items.Add(row["FullName"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách người dùng: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                // Fallback: Không load được thì để trống
            }
        }

        // Load task từ database
        private void LoadTaskFromDatabase()
        {
            try
            {
                if (_taskId <= 0)
                {
                    MessageBox.Show("TaskId không hợp lệ!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                SqlParameter[] parameters = { new SqlParameter("@TaskId", _taskId) };
                DataTable dt = DatabaseHelper.ExecuteStoredProcedure("sp_GetTaskById", parameters);

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    
                    // Load dữ liệu vào form
                    txtMa.Text = "CV" + row["Id"].ToString();
                    txtTen.Text = row["Title"].ToString();
                    txtMoTa.Text = row["Description"] != DBNull.Value ? row["Description"].ToString() : "";
                    
                    // Load DueDate
                    if (row["DueDate"] != DBNull.Value)
                    {
                        dtpHan.Value = Convert.ToDateTime(row["DueDate"]);
                    }

                    // Load Priority (chuyển từ tiếng Anh sang tiếng Việt)
                    string priorityEN = row["Priority"].ToString();
                    string priorityVI = ChuyenDoiPrioritySangTiengViet(priorityEN);
                    if (cboUuTien.Items.Contains(priorityVI))
                        cboUuTien.SelectedItem = priorityVI;

                    // Load Status (chuyển từ tiếng Anh sang tiếng Việt)
                    string statusEN = row["Status"].ToString();
                    string statusVI = ChuyenDoiStatusSangTiengViet(statusEN);
                    if (cboTrangThai.Items.Contains(statusVI))
                        cboTrangThai.SelectedItem = statusVI;

                    // Load User (người phụ trách)
                    string userFullName = row["UserFullName"] != DBNull.Value ? row["UserFullName"].ToString() : "";
                    if (!string.IsNullOrEmpty(userFullName) && cboNguoiLam.Items.Contains(userFullName))
                        cboNguoiLam.SelectedItem = userFullName;

                    // Set progress bar dựa trên status
                    if (statusVI == "Hoàn thành")
                    {
                        trkTienDo.Value = 100;
                    }
                    else if (statusVI == "Đang làm")
                    {
                        trkTienDo.Value = 50;
                    }
                    else
                    {
                        trkTienDo.Value = 0;
                    }
                    lblPhanTram.Text = trkTienDo.Value + "%";
                }
                else
                {
                    MessageBox.Show("Không tìm thấy công việc này!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu công việc: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Lưu task vào database
        private void LuuTaskVaoDatabase()
        {
            try
            {
                if (_taskId <= 0)
                {
                    MessageBox.Show("TaskId không hợp lệ!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtTen.Text))
                {
                    MessageBox.Show("Vui lòng nhập Tên công việc!", "Nhắc nhở",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtTen.Focus();
                    return;
                }

                // Chuyển đổi dữ liệu từ tiếng Việt sang tiếng Anh
                string statusEN = ChuyenDoiStatusSangTiengAnh(cboTrangThai.Text);
                string priorityEN = ChuyenDoiPrioritySangTiengAnh(cboUuTien.Text);
                string description = string.IsNullOrWhiteSpace(txtMoTa.Text) ? null : txtMoTa.Text.Trim();
                string category = "Work"; // Mặc định

                // UserId trong sp_UpdateTask là userId của người sở hữu task (để kiểm tra quyền)
                // Không thay đổi userId, chỉ dùng _userId hiện tại
                SqlParameter[] parameters = 
                {
                    new SqlParameter("@TaskId", _taskId),
                    new SqlParameter("@Title", txtTen.Text.Trim()),
                    new SqlParameter("@Description", (object)description ?? DBNull.Value),
                    new SqlParameter("@UserId", _userId),
                    new SqlParameter("@Priority", priorityEN),
                    new SqlParameter("@Status", statusEN),
                    new SqlParameter("@Category", category),
                    new SqlParameter("@DueDate", dtpHan.Value)
                };

                DatabaseHelper.ExecuteStoredProcedureNonQuery("sp_UpdateTask", parameters);

                // Ghi log vào memory (giữ lại tính năng hiện tại)
                string note = string.IsNullOrWhiteSpace(txtMoTa.Text) ? "Không có ghi chú" : txtMoTa.Text;
                string timeVN = DateTime.Now.ToString("HH:mm:ss - dddd, dd/MM/yyyy", viVN);
                string entry = $"[{timeVN}]\n" +
                               $"| Mã: {txtMa.Text} | Task: {txtTen.Text}\n" +
                               $"   => {cboTrangThai.Text} ({trkTienDo.Value}%) | Sửa bởi: {cboNguoiLam.Text}\n" +
                               $"   => Ghi chú: {note}";
                _historyLogs.Add(entry);

                MessageBox.Show("Cập nhật công việc thành công!", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu công việc: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Chuyển đổi Status từ tiếng Anh (DB) sang tiếng Việt (UI)
        private string ChuyenDoiStatusSangTiengViet(string statusEN)
        {
            switch (statusEN?.Trim())
            {
                case "Todo":
                    return "Chưa hoàn thành";
                case "Doing":
                    return "Đang làm";
                case "Done":
                    return "Hoàn thành";
                default:
                    return "Chưa hoàn thành";
            }
        }

        // Chuyển đổi Status từ tiếng Việt (UI) sang tiếng Anh (DB)
        private string ChuyenDoiStatusSangTiengAnh(string statusVI)
        {
            switch (statusVI?.Trim())
            {
                case "Chưa hoàn thành":
                    return "Todo";
                case "Đang làm":
                    return "Doing";
                case "Hoàn thành":
                    return "Done";
                default:
                    return "Todo";
            }
        }

        // Chuyển đổi Priority từ tiếng Anh (DB) sang tiếng Việt (UI)
        private string ChuyenDoiPrioritySangTiengViet(string priorityEN)
        {
            switch (priorityEN?.Trim())
            {
                case "High":
                    return "Cao";
                case "Medium":
                    return "Trung bình";
                case "Low":
                    return "Thấp";
                default:
                    return "Trung bình";
            }
        }

        // Chuyển đổi Priority từ tiếng Việt (UI) sang tiếng Anh (DB)
        private string ChuyenDoiPrioritySangTiengAnh(string priorityVI)
        {
            switch (priorityVI?.Trim())
            {
                case "Cao":
                    return "High";
                case "Trung bình":
                    return "Medium";
                case "Thấp":
                    return "Low";
                default:
                    return "Medium";
            }
        }
    }
}
