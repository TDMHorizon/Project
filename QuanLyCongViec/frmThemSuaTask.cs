using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using QuanLyCongViec.DataAccess;
using QuanLyCongViec.Helpers;

namespace QuanLyCongViec
{
    public partial class frmThemSuaTask : Form
    {
        private int _userId;
        private int _selectedTaskId = 0;

        public frmThemSuaTask(int userId = 0)
        {
            InitializeComponent();
            _userId = userId;
            Helpers.FontHelper.SetUnicodeFont(this);
            Helpers.FontHelper.SetUnicodeFontForDataGridView(dgvCongViec);
            
            // Cấu hình DataGridView để cho phép xuống dòng
            dgvCongViec.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvCongViec.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            
            // Lấy UserId đầu tiên nếu không được truyền vào
            if (_userId == 0)
            {
                try
                {
                    DataTable dt = DatabaseHelper.ExecuteStoredProcedure("sp_GetFirstUserId");
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        _userId = Convert.ToInt32(dt.Rows[0]["UserId"]);
                    }
                }
                catch
                {
                    // Nếu không lấy được, để mặc định là 0
                }
            }
            
            HienThiDanhSach();
        }

        private void HienThiDanhSach()
        {
            try
            {
                SqlParameter[] parameters = { new SqlParameter("@UserId", _userId) };
                DataTable dt = DatabaseHelper.ExecuteStoredProcedure("sp_GetTasksByFilter", parameters);
                
                if (dt != null && dt.Rows.Count > 0)
                {
                    // Tạo DataTable mới với các cột cần hiển thị
                    DataTable dtDisplay = new DataTable();
                    dtDisplay.Columns.Add("Id", typeof(int));
                    dtDisplay.Columns.Add("MaCongViec", typeof(string));
                    dtDisplay.Columns.Add("TenCongViec", typeof(string));
                    dtDisplay.Columns.Add("TrangThai", typeof(string));
                    dtDisplay.Columns.Add("NguoiPhuTrach", typeof(string));
                    dtDisplay.Columns.Add("NgayBatDau", typeof(DateTime));
                    dtDisplay.Columns.Add("NgayKetThuc", typeof(DateTime));
                    dtDisplay.Columns.Add("DoUuTien", typeof(string));
                    dtDisplay.Columns.Add("Category", typeof(string));

                    foreach (DataRow row in dt.Rows)
                    {
                        DataRow newRow = dtDisplay.NewRow();
                        newRow["Id"] = row["Id"];
                        newRow["MaCongViec"] = "CV" + row["Id"].ToString();
                        newRow["TenCongViec"] = row["Title"];
                        newRow["TrangThai"] = ConvertStatus(row["Status"].ToString());
                        newRow["NguoiPhuTrach"] = row["UserFullName"];
                        newRow["NgayBatDau"] = row["CreatedDate"];  // Sử dụng CreatedDate thay vì StartDate (không tồn tại trong DB)
                        newRow["NgayKetThuc"] = row["DueDate"];
                        newRow["DoUuTien"] = ConvertPriority(row["Priority"].ToString());
                        newRow["Category"] = row["Category"];
                        dtDisplay.Rows.Add(newRow);
                    }

                    dgvCongViec.DataSource = dtDisplay;
                    
                    // Ẩn cột Id
                    if (dgvCongViec.Columns["Id"] != null)
                        dgvCongViec.Columns["Id"].Visible = false;
                    
                    // Định dạng cột ngày tháng
                    if (dgvCongViec.Columns["NgayBatDau"] != null)
                        dgvCongViec.Columns["NgayBatDau"].DefaultCellStyle.Format = "dd/MM/yyyy";
                    if (dgvCongViec.Columns["NgayKetThuc"] != null)
                        dgvCongViec.Columns["NgayKetThuc"].DefaultCellStyle.Format = "dd/MM/yyyy";
                    
                    // Đặt header text
                    if (dgvCongViec.Columns["MaCongViec"] != null)
                        dgvCongViec.Columns["MaCongViec"].HeaderText = "Mã CV";
                    if (dgvCongViec.Columns["TenCongViec"] != null)
                        dgvCongViec.Columns["TenCongViec"].HeaderText = "Tên CV";
                    if (dgvCongViec.Columns["TrangThai"] != null)
                        dgvCongViec.Columns["TrangThai"].HeaderText = "Trạng Thái";
                    if (dgvCongViec.Columns["NguoiPhuTrach"] != null)
                        dgvCongViec.Columns["NguoiPhuTrach"].HeaderText = "Người Phụ Trách";
                    if (dgvCongViec.Columns["NgayBatDau"] != null)
                        dgvCongViec.Columns["NgayBatDau"].HeaderText = "Ngày Bắt Đầu";
                    if (dgvCongViec.Columns["NgayKetThuc"] != null)
                        dgvCongViec.Columns["NgayKetThuc"].HeaderText = "Ngày Kết Thúc";
                    if (dgvCongViec.Columns["DoUuTien"] != null)
                        dgvCongViec.Columns["DoUuTien"].HeaderText = "Độ Ưu Tiên";
                }
                else
                {
                    dgvCongViec.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách công việc: " + ex.Message, "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string ConvertStatus(string status)
        {
            switch (status)
            {
                case "Todo": return "Chưa bắt đầu";
                case "Doing": return "Đang thực hiện";
                case "Done": return "Hoàn thành";
                default: return status;
            }
        }

        private string ConvertPriority(string priority)
        {
            switch (priority)
            {
                case "High": return "Cao";
                case "Medium": return "Trung bình";
                case "Low": return "Thấp";
                default: return priority;
            }
        }

        private string ConvertStatusToEnglish(string status)
        {
            // Trim và chuyển về lowercase để so sánh
            string normalized = status?.Trim().ToLower();
            switch (normalized)
            {
                case "chưa bắt đầu": return "Todo";
                case "đang thực hiện": return "Doing";
                case "hoàn thành": return "Done";
                case "todo": return "Todo";
                case "doing": return "Doing";
                case "done": return "Done";
                default: return "Todo"; // Mặc định nếu không hợp lệ
            }
        }

        private string ConvertPriorityToEnglish(string priority)
        {
            // Trim và chuyển về lowercase để so sánh
            string normalized = priority?.Trim().ToLower();
            switch (normalized)
            {
                case "cao": return "High";
                case "trung bình": return "Medium";
                case "thấp": return "Low";
                case "high": return "High";
                case "medium": return "Medium";
                case "low": return "Low";
                default: return "Medium"; // Mặc định nếu không hợp lệ
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenCongViec.Text))
            {
                MessageBox.Show("Vui lòng nhập Tên Công Việc!", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string status = string.IsNullOrWhiteSpace(txtTrangThai.Text) ? "Todo" : ConvertStatusToEnglish(txtTrangThai.Text);
                string priority = string.IsNullOrWhiteSpace(txtDoUuTien.Text) ? "Medium" : ConvertPriorityToEnglish(txtDoUuTien.Text);
                string category = "Work"; // Mặc định
                string description = "";

                SqlParameter taskIdParam = new SqlParameter("@TaskId", SqlDbType.Int) 
                { 
                    Direction = ParameterDirection.Output 
                };

                // Kiểm tra StartDate <= DueDate
                if (dtpNgayBatDau.Value > dtpNgayKetThuc.Value)
                {
                    MessageBox.Show("Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                SqlParameter[] parameters = 
                {
                    new SqlParameter("@Title", txtTenCongViec.Text.Trim()),
                    new SqlParameter("@Description", (object)description ?? DBNull.Value),
                    new SqlParameter("@UserId", _userId),
                    new SqlParameter("@Priority", priority),
                    new SqlParameter("@Status", status),
                    new SqlParameter("@Category", category),
                    new SqlParameter("@DueDate", dtpNgayKetThuc.Value),
                    taskIdParam
                };

                DatabaseHelper.ExecuteStoredProcedureNonQuery("sp_CreateTask", parameters);
                
                int taskId = taskIdParam.Value != DBNull.Value ? Convert.ToInt32(taskIdParam.Value) : 0;
                if (taskId > 0)
                {
                    HienThiDanhSach();
                    LamRongForm();
                    MessageBox.Show("Thêm công việc thành công!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm công việc: " + ex.Message, "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (_selectedTaskId == 0)
            {
                MessageBox.Show("Vui lòng chọn một công việc để sửa!", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtTenCongViec.Text))
            {
                MessageBox.Show("Vui lòng nhập Tên Công Việc!", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string status = string.IsNullOrWhiteSpace(txtTrangThai.Text) ? "Todo" : ConvertStatusToEnglish(txtTrangThai.Text);
                string priority = string.IsNullOrWhiteSpace(txtDoUuTien.Text) ? "Medium" : ConvertPriorityToEnglish(txtDoUuTien.Text);
                string category = "Work"; // Mặc định
                string description = "";

                // Kiểm tra StartDate <= DueDate
                if (dtpNgayBatDau.Value > dtpNgayKetThuc.Value)
                {
                    MessageBox.Show("Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                SqlParameter[] parameters = 
                {
                    new SqlParameter("@TaskId", _selectedTaskId),
                    new SqlParameter("@Title", txtTenCongViec.Text.Trim()),
                    new SqlParameter("@Description", (object)description ?? DBNull.Value),
                    new SqlParameter("@UserId", _userId),
                    new SqlParameter("@Priority", priority),
                    new SqlParameter("@Status", status),
                    new SqlParameter("@Category", category),
                    new SqlParameter("@DueDate", dtpNgayKetThuc.Value)
                };

                DatabaseHelper.ExecuteStoredProcedureNonQuery("sp_UpdateTask", parameters);
                
                HienThiDanhSach();
                MessageBox.Show("Cập nhật công việc thành công!", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật công việc: " + ex.Message, "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (_selectedTaskId == 0)
            {
                MessageBox.Show("Vui lòng chọn một công việc để xóa!", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Bạn có chắc muốn xóa công việc này?", "Xác nhận xóa", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    SqlParameter[] parameters = 
                    {
                        new SqlParameter("@TaskId", _selectedTaskId),
                        new SqlParameter("@UserId", _userId)
                    };

                    DatabaseHelper.ExecuteStoredProcedureNonQuery("sp_DeleteTask", parameters);
                    
                    HienThiDanhSach();
                    LamRongForm();
                    MessageBox.Show("Xóa công việc thành công!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xóa công việc: " + ex.Message, "Lỗi", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string tuKhoa = txtTimKiem.Text.Trim();
            if (string.IsNullOrEmpty(tuKhoa))
            {
                HienThiDanhSach();
                return;
            }

            try
            {
                SqlParameter[] parameters = 
                {
                    new SqlParameter("@UserId", _userId),
                    new SqlParameter("@SearchTerm", tuKhoa)
                };

                DataTable dt = DatabaseHelper.ExecuteStoredProcedure("sp_SearchTasks", parameters);
                
                if (dt != null && dt.Rows.Count > 0)
                {
                    // Tạo DataTable mới với các cột cần hiển thị
                    DataTable dtDisplay = new DataTable();
                    dtDisplay.Columns.Add("Id", typeof(int));
                    dtDisplay.Columns.Add("MaCongViec", typeof(string));
                    dtDisplay.Columns.Add("TenCongViec", typeof(string));
                    dtDisplay.Columns.Add("TrangThai", typeof(string));
                    dtDisplay.Columns.Add("NguoiPhuTrach", typeof(string));
                    dtDisplay.Columns.Add("NgayBatDau", typeof(DateTime));
                    dtDisplay.Columns.Add("NgayKetThuc", typeof(DateTime));
                    dtDisplay.Columns.Add("DoUuTien", typeof(string));

                    foreach (DataRow row in dt.Rows)
                    {
                        DataRow newRow = dtDisplay.NewRow();
                        newRow["Id"] = row["Id"];
                        newRow["MaCongViec"] = "CV" + row["Id"].ToString();
                        newRow["TenCongViec"] = row["Title"];
                        newRow["TrangThai"] = ConvertStatus(row["Status"].ToString());
                        newRow["NguoiPhuTrach"] = row["UserFullName"];
                        newRow["NgayBatDau"] = row["CreatedDate"];
                        newRow["NgayKetThuc"] = row["DueDate"];
                        newRow["DoUuTien"] = ConvertPriority(row["Priority"].ToString());
                        dtDisplay.Rows.Add(newRow);
                    }

                    dgvCongViec.DataSource = dtDisplay;
                    
                    // Ẩn cột Id
                    if (dgvCongViec.Columns["Id"] != null)
                        dgvCongViec.Columns["Id"].Visible = false;
                    
                    // Định dạng cột ngày tháng
                    if (dgvCongViec.Columns["NgayBatDau"] != null)
                        dgvCongViec.Columns["NgayBatDau"].DefaultCellStyle.Format = "dd/MM/yyyy";
                    if (dgvCongViec.Columns["NgayKetThuc"] != null)
                        dgvCongViec.Columns["NgayKetThuc"].DefaultCellStyle.Format = "dd/MM/yyyy";
                }
                else
                {
                    dgvCongViec.DataSource = null;
                    MessageBox.Show("Không tìm thấy kết quả nào.", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tìm kiếm: " + ex.Message, "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnHienThi_Click(object sender, EventArgs e)
        {
            HienThiDanhSach();
            LamRongForm();
        }

        private void dgvCongViec_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCongViec.SelectedRows.Count > 0)
            {
                var row = dgvCongViec.SelectedRows[0];
                
                // Lấy Id từ cột ẩn hoặc từ MaCongViec
                if (row.Cells["Id"] != null && row.Cells["Id"].Value != null)
                {
                    _selectedTaskId = Convert.ToInt32(row.Cells["Id"].Value);
                }
                else if (row.Cells["MaCongViec"] != null)
                {
                    string maCV = row.Cells["MaCongViec"].Value?.ToString() ?? "";
                    if (maCV.StartsWith("CV"))
                    {
                        int.TryParse(maCV.Substring(2), out _selectedTaskId);
                    }
                }
                
                txtMaCongViec.Text = row.Cells["MaCongViec"]?.Value?.ToString() ?? "";
                txtTenCongViec.Text = row.Cells["TenCongViec"]?.Value?.ToString() ?? "";
                txtTrangThai.Text = row.Cells["TrangThai"]?.Value?.ToString() ?? "";
                txtNguoiPhuTrach.Text = row.Cells["NguoiPhuTrach"]?.Value?.ToString() ?? "";
                
                // Xử lý ngày tháng
                if (row.Cells["NgayBatDau"]?.Value != null)
                {
                    if (row.Cells["NgayBatDau"].Value is DateTime dt1)
                    {
                        dtpNgayBatDau.Value = dt1;
                    }
                    else if (DateTime.TryParse(row.Cells["NgayBatDau"].Value.ToString(), out DateTime parsedDate1))
                    {
                        dtpNgayBatDau.Value = parsedDate1;
                    }
                }
                
                if (row.Cells["NgayKetThuc"]?.Value != null)
                {
                    if (row.Cells["NgayKetThuc"].Value is DateTime dt2)
                    {
                        dtpNgayKetThuc.Value = dt2;
                    }
                    else if (DateTime.TryParse(row.Cells["NgayKetThuc"].Value.ToString(), out DateTime parsedDate2))
                    {
                        dtpNgayKetThuc.Value = parsedDate2;
                    }
                }
                
                txtDoUuTien.Text = row.Cells["DoUuTien"]?.Value?.ToString() ?? "";
            }
        }

        private void dgvCongViec_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvCongViec.Rows[e.RowIndex];
                
                // Lấy TaskId từ cột Id (ẩn)
                int taskId = 0;
                if (row.Cells["Id"]?.Value != null)
                {
                    taskId = Convert.ToInt32(row.Cells["Id"].Value);
                }
                else if (row.Cells["MaCongViec"]?.Value != null)
                {
                    // Fallback: Lấy từ MaCongViec (format: "CV123")
                    string maCV = row.Cells["MaCongViec"].Value.ToString();
                    if (maCV.StartsWith("CV"))
                    {
                        int.TryParse(maCV.Substring(2), out taskId);
                    }
                }

                if (taskId > 0)
                {
                    frmChiTietTask chiTietForm = new frmChiTietTask(taskId, _userId);
                    chiTietForm.ShowDialog(this);
                    // Refresh danh sách sau khi đóng form để cập nhật dữ liệu
                    HienThiDanhSach();
                }
                else
                {
                    MessageBox.Show("Không thể xác định công việc được chọn!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void LamRongForm()
        {
            _selectedTaskId = 0;
            txtMaCongViec.Clear();
            txtTenCongViec.Clear();
            txtTrangThai.Clear();
            txtNguoiPhuTrach.Clear();
            dtpNgayBatDau.Value = DateTime.Now;
            dtpNgayKetThuc.Value = DateTime.Now;
            txtDoUuTien.Clear();
            txtThoiGianDuKien.Clear();
            txtTenCongViec.Focus();
        }
    }
}
