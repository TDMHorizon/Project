using QuanLyCongViec.DataAccess;
using QuanLyCongViec.Helpers;
using QuanLyCongViec.Controls;
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
    public partial class frrmMain : Form
    {

        #region 1. KHAI BÁO BIẾN & THUỘC TÍNH (Fields)

        private int _currentUserId;
        private string _username;
        private string _fullName;
        private Timer _timer; // Timer dùng để cập nhật thời gian liên tục
        private frmThoiKhoaBieu _calendarForm; // Form calendar riêng bên phải
        private Panel _userMenuPanel; // Panel chứa menu dropdown
        private bool _isMenuVisible = false; // Trạng thái hiển thị menu

        #endregion

        #region 2. CONSTRUCTOR & KHỞI TẠO

        public frrmMain(int userId, string username, string fullName)
        {
            // Constructor nhận thông tin user từ form đăng nhập truyền sang
            InitializeComponent();
            _currentUserId = userId;
            _username = username;
            _fullName = fullName;

            // Khởi tạo giao diện & dữ liệu ban đầu
            HienThiThongTinUser();   // Hiển thị tên, username
            CapNhatDashboard();      // Load số liệu Dashboard
            KhoiTaoTimer();          // Khởi tạo đồng hồ thời gian

            // Thiết lập màu sắc cho các panel
            panel_Tong.BackColor = ColorTranslator.FromHtml("#4C84FF");
            panel_Todo.BackColor = ColorTranslator.FromHtml("#6AA9FF");
            panel_Doing.BackColor = ColorTranslator.FromHtml("#FFC94D");
            panel_Done.BackColor = ColorTranslator.FromHtml("#69D16F");
            panel_QuaHan.BackColor = ColorTranslator.FromHtml("#FF6B6B");

            // Khởi tạo menu dropdown
            KhoiTaoUserMenu();
        }

        // Khi đóng form → dừng timer để tránh rò rỉ tài nguyên
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _timer?.Stop();
            _timer?.Dispose();
            base.OnFormClosing(e);
        }

        #endregion

        #region 3. CÁC HÀM XỬ LÝ GIAO DIỆN & THỜI GIAN

        // Hiển thị tên
        private void HienThiThongTinUser()
        {
            lbl_Ten.Text = $"Xin chào, {_fullName}";
            CapNhatThoiGian();
        }

        // Cập nhật ngày tháng hiện tại
        private void CapNhatThoiGian()
        {
            DateTime ngayHienTai = DateTime.Now;
            lbl_NgayThang.Text = ngayHienTai.ToString("'hôm nay:' dd/MM/yyyy");
        }

        // Khởi tạo timer → mỗi 1 giây sẽ cập nhật thời gian trên giao diện
        private void KhoiTaoTimer()
        {
            _timer = new Timer();
            _timer.Interval = 1000; // 1 giây
            _timer.Tick += (s, e) => CapNhatThoiGian();
            _timer.Start();
        }

        /// <summary>
        /// Khởi tạo menu dropdown cho user
        /// </summary>
        private void KhoiTaoUserMenu()
        {
            // Tạo panel menu (chỉ còn 2 items)
            _userMenuPanel = new Panel
            {
                Size = new Size(200, 80),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false
            };

            // Tính vị trí menu (bên dưới nút đăng xuất)
            _userMenuPanel.Location = new Point(
                btn_DangXuat.Right - _userMenuPanel.Width,
                btn_DangXuat.Bottom + 5
            );

            // Menu Item 1: Thông tin tài khoản
            Button btnProfile = TaoMenuItem("👤 Thông tin tài khoản", 0, () =>
            {
                frmProfile profileForm = new frmProfile(_currentUserId, _username, _fullName);
                profileForm.ShowDialog();
                if (!string.IsNullOrEmpty(profileForm.NewFullName))
                {
                    _fullName = profileForm.NewFullName;
                    HienThiThongTinUser();
                }
                AnMenu();
            });

            // Menu Item 2: Đăng xuất
            Button btnLogout = TaoMenuItem("🚪 Đăng xuất", 40, () =>
            {
                if (MessageBox.Show("Bạn có chắc muốn đăng xuất?", "Xác nhận",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _timer?.Stop();
                    this.Hide();
                    frmDangNhap loginForm = new frmDangNhap();
                    loginForm.ShowDialog();
                    this.Close();
                }
                AnMenu();
            });

            // Thêm các menu items vào panel
            _userMenuPanel.Controls.Add(btnProfile);
            _userMenuPanel.Controls.Add(btnLogout);

            // Thêm panel vào form (trên cùng)
            this.Controls.Add(_userMenuPanel);
            _userMenuPanel.BringToFront();

            // Gắn sự kiện click vào nút "Tài khoản"
            btn_DangXuat.Click -= btn_DangXuat_Click; // Xóa event cũ
            btn_DangXuat.Click += (s, e) => ToggleMenu();
            // Click ra ngoài để đóng menu
            this.Click += (s, e) => AnMenu();
            groupBox1.Click += (s, e) => AnMenu();
            groupBox3.Click += (s, e) => AnMenu();
        }

        /// <summary>
        /// Tạo một menu item button
        /// </summary>
        private Button TaoMenuItem(string text, int yPosition, Action clickAction)
        {
            Button btn = new Button
            {
                Text = text,
                Location = new Point(0, yPosition),
                Size = new Size(198, 40),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Regular),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(52, 73, 94),
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0),
                Cursor = Cursors.Hand
            };

            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(236, 240, 241);

            btn.Click += (s, e) => clickAction();

            return btn;
        }

        /// <summary>
        /// Toggle hiển thị/ẩn menu
        /// </summary>
        private void ToggleMenu()
        {
            if (_isMenuVisible)
            {
                AnMenu();
            }
            else
            {
                HienMenu();
            }
        }

        /// <summary>
        /// Hiển thị menu
        /// </summary>
        private void HienMenu()
        {
            _userMenuPanel.Visible = true;
            _userMenuPanel.BringToFront();
            _isMenuVisible = true;
        }

        /// <summary>
        /// Ẩn menu
        /// </summary>
        private void AnMenu()
        {
            _userMenuPanel.Visible = false;
            _isMenuVisible = false;
        }

        #endregion

        #region 4. CÁC HÀM XỬ LÝ DỮ LIỆU (DATA LOGIC)

        // Hàm gọi Stored Procedure để lấy thống kê công việc
        private void CapNhatDashboard()
        {
            try
            {
                SqlParameter[] parameters =
                {
                    new SqlParameter("@UserId", _currentUserId) // chỉ lấy dữ liệu của user hiện tại
                };

                DataTable dt = DatabaseHelper.ExecuteStoredProcedure(
                    "sp_GetDashboardStats",
                    parameters
                );

                if (dt.Rows.Count > 0)
                {
                    // Đổ dữ liệu vào label Dashboard
                    DataRow row = dt.Rows[0];
                    lbl_TongCongViec.Text = $"Tổng công việc: {row["TotalTasks"]}";
                    lbl_Todo.Text = $"Cần làm (To-do): {row["TodoCount"]}";
                    lbl_Doing.Text = $"Đang làm (Doing): {row["DoingCount"]}";
                    lbl_Done.Text = $"Hoàn thành (Done): {row["DoneCount"]}";
                    lbl_quahan.Text = $"Quá hạn: {row["OverdueCount"]}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dashboard: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region 5. XỬ LÝ SỰ KIỆN CỦA CONTROLS (Events)

        // Mở form báo cáo
        private void btn_BaoCao_Click(object sender, EventArgs e)
        {
            frmBaoCao reportForm = new frmBaoCao(_currentUserId);
            reportForm.ShowDialog();
        }

        // Mở form lịch sử
        private void btn_LichSu_Click(object sender, EventArgs e)
        {
            frmLichSu historyForm = new frmLichSu();
            historyForm.ShowDialog();
        }

        // Mở form quản lý công việc
        private void btn_QuanLyCongViec_Click(object sender, EventArgs e)
        {
            frmThemSuaTask taskForm = new frmThemSuaTask(_currentUserId);
            taskForm.ShowDialog();
            CapNhatDashboard(); // Refresh sau khi đóng
        }

        // Mở form thông báo
        private void btn_ThongBao_Click(object sender, EventArgs e)
        {
            frmThongBao thongBaoForm = new frmThongBao(_currentUserId);
            thongBaoForm.ShowDialog();
        }

        // Mở form hướng dẫn
        private void btn_Help_Click(object sender, EventArgs e)
        {
            frmHuongDan huongDanForm = new frmHuongDan();
            huongDanForm.ShowDialog();
        }

        // Xử lý đăng xuất
        private void btn_DangXuat_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc muốn đăng xuất?", "Xác nhận",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _timer?.Stop();
                this.Hide();
                frmDangNhap loginForm = new frmDangNhap();
                loginForm.ShowDialog();
                this.Close();
            }
        }

        // Sự kiện không có logic nghiệp vụ, có thể bỏ qua hoặc giữ lại
        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            // Không làm gì
        }

        #endregion

        // Khi form load → khóa resize + mở form calendar bên phải
        private void frrmMain_Load(object sender, EventArgs e)
        {
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            // Mở form Calendar bên phải
            MoCalendarBenPhai();
        }

        /// <summary>
        /// Mở form Calendar bên phải form Main
        /// </summary>
        private void MoCalendarBenPhai()
        {
            // Truyền UserId của user hiện tại vào Calendar
            _calendarForm = new frmThoiKhoaBieu(_currentUserId);
            
            // Tính toán vị trí bên phải form Main
            int leftPos = this.Right + 10; // Cách form Main 10px
            int topPos = this.Top;

            _calendarForm.StartPosition = FormStartPosition.Manual;
            _calendarForm.Location = new Point(leftPos, topPos);
            
            // Hiển thị form không modal (có thể tương tác cả 2 form)
            _calendarForm.Show();
            
            // Event khi đóng form Calendar
            _calendarForm.FormClosed += (s, e) =>
            {
                _calendarForm = null;
            };
        }

        /// <summary>
        /// Khi đóng form Main → đóng form Calendar luôn
        /// </summary>
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (_calendarForm != null && !_calendarForm.IsDisposed)
            {
                _calendarForm.Close();
            }
            base.OnFormClosed(e);
        }

        /// <summary>
        /// Khi di chuyển form Main → di chuyển Calendar theo
        /// </summary>
        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);
            
            if (_calendarForm != null && !_calendarForm.IsDisposed)
            {
                int leftPos = this.Right + 10;
                int topPos = this.Top;
                _calendarForm.Location = new Point(leftPos, topPos);
            }
        }
    }
}
