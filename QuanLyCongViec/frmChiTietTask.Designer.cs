namespace QuanLyCongViec
{
    partial class frmChiTietTask
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblMa, lblTen, lblHan, lblUuTien, lblTrangThai, lblMoTa, lblTienDo, lblPhanTram, lblNguoiLam;
        private System.Windows.Forms.TextBox txtMa, txtTen, txtMoTa;
        private System.Windows.Forms.DateTimePicker dtpHan;
        private System.Windows.Forms.ComboBox cboUuTien, cboTrangThai, cboNguoiLam;
        private System.Windows.Forms.TrackBar trkTienDo;
        private System.Windows.Forms.Button btnLuu, btnHuy, btnLichSu;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblMa = new System.Windows.Forms.Label();
            this.txtMa = new System.Windows.Forms.TextBox();
            this.lblTen = new System.Windows.Forms.Label();
            this.txtTen = new System.Windows.Forms.TextBox();
            this.lblNguoiLam = new System.Windows.Forms.Label();
            this.cboNguoiLam = new System.Windows.Forms.ComboBox();
            this.lblUuTien = new System.Windows.Forms.Label();
            this.cboUuTien = new System.Windows.Forms.ComboBox();
            this.lblHan = new System.Windows.Forms.Label();
            this.dtpHan = new System.Windows.Forms.DateTimePicker();
            this.lblTrangThai = new System.Windows.Forms.Label();
            this.cboTrangThai = new System.Windows.Forms.ComboBox();
            this.lblTienDo = new System.Windows.Forms.Label();
            this.trkTienDo = new System.Windows.Forms.TrackBar();
            this.lblPhanTram = new System.Windows.Forms.Label();
            this.lblMoTa = new System.Windows.Forms.Label();
            this.txtMoTa = new System.Windows.Forms.TextBox();
            this.btnLuu = new System.Windows.Forms.Button();
            this.btnHuy = new System.Windows.Forms.Button();
            this.btnLichSu = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.trkTienDo)).BeginInit();
            this.SuspendLayout();

            // Form Background & Style
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(400, 480);
            this.Text = "Chi Tiết Task Chuyên Sâu";
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmChiTietTask";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;

            // Cấu hình các Control (Vị trí chuẩn theo ảnh của bạn)
            this.lblMa.AutoSize = true;
            this.lblMa.Location = new System.Drawing.Point(20, 20);
            this.lblMa.Text = "Mã công việc:";
            this.txtMa.Location = new System.Drawing.Point(140, 17);
            this.txtMa.Size = new System.Drawing.Size(230, 25);

            this.lblTen.AutoSize = true;
            this.lblTen.Location = new System.Drawing.Point(20, 55);
            this.lblTen.Text = "Tên công việc:";
            this.txtTen.Location = new System.Drawing.Point(140, 52);
            this.txtTen.Size = new System.Drawing.Size(230, 25);
            this.txtTen.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);

            this.lblNguoiLam.AutoSize = true;
            this.lblNguoiLam.Location = new System.Drawing.Point(20, 90);
            this.lblNguoiLam.Text = "Người thực hiện:";
            this.cboNguoiLam.Location = new System.Drawing.Point(140, 87);
            this.cboNguoiLam.Size = new System.Drawing.Size(230, 25);
            this.cboNguoiLam.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

            this.lblUuTien.AutoSize = true;
            this.lblUuTien.Location = new System.Drawing.Point(20, 125);
            this.lblUuTien.Text = "Mức ưu tiên:";
            this.cboUuTien.Location = new System.Drawing.Point(140, 122);
            this.cboUuTien.Size = new System.Drawing.Size(230, 25);

            this.lblHan.AutoSize = true;
            this.lblHan.Location = new System.Drawing.Point(20, 160);
            this.lblHan.Text = "Hạn chót:";
            this.dtpHan.Location = new System.Drawing.Point(140, 157);
            this.dtpHan.Size = new System.Drawing.Size(230, 25);

            this.lblTrangThai.AutoSize = true;
            this.lblTrangThai.Location = new System.Drawing.Point(20, 195);
            this.lblTrangThai.Text = "Trạng thái:";
            this.cboTrangThai.Location = new System.Drawing.Point(140, 192);
            this.cboTrangThai.Size = new System.Drawing.Size(230, 25);

            this.lblTienDo.AutoSize = true;
            this.lblTienDo.Location = new System.Drawing.Point(20, 230);
            this.lblTienDo.Text = "Tiến độ:";
            this.trkTienDo.Location = new System.Drawing.Point(140, 225);
            this.trkTienDo.Size = new System.Drawing.Size(180, 45);
            this.trkTienDo.Maximum = 100;

            this.lblPhanTram.AutoSize = true;
            this.lblPhanTram.Location = new System.Drawing.Point(325, 230);
            this.lblPhanTram.Text = "0%";
            this.lblPhanTram.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);

            this.lblMoTa.AutoSize = true;
            this.lblMoTa.Location = new System.Drawing.Point(20, 275);
            this.lblMoTa.Text = "Ghi chú/Chi tiết:";
            this.txtMoTa.Location = new System.Drawing.Point(20, 300);
            this.txtMoTa.Size = new System.Drawing.Size(350, 80);
            this.txtMoTa.Multiline = true;

            // Nút bấm
            this.btnLichSu.Location = new System.Drawing.Point(20, 410);
            this.btnLichSu.Size = new System.Drawing.Size(100, 40);
            this.btnLichSu.Text = "📜 Lịch sử";
            this.btnLichSu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;

            this.btnLuu.Location = new System.Drawing.Point(155, 410);
            this.btnLuu.Size = new System.Drawing.Size(100, 40);
            this.btnLuu.Text = "Lưu lại";
            this.btnLuu.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            this.btnLuu.ForeColor = System.Drawing.Color.White;
            this.btnLuu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;

            this.btnHuy.Location = new System.Drawing.Point(265, 410);
            this.btnHuy.Size = new System.Drawing.Size(100, 40);
            this.btnHuy.Text = "Đóng";
            this.btnHuy.FlatStyle = System.Windows.Forms.FlatStyle.Flat;

            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblMa, this.txtMa, this.lblTen, this.txtTen, this.lblNguoiLam, this.cboNguoiLam,
                this.lblUuTien, this.cboUuTien, this.lblHan, this.dtpHan, this.lblTrangThai, this.cboTrangThai,
                this.lblTienDo, this.trkTienDo, this.lblPhanTram, this.lblMoTa, this.txtMoTa,
                this.btnLuu, this.btnHuy, this.btnLichSu
            });
            ((System.ComponentModel.ISupportInitialize)(this.trkTienDo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}