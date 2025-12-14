namespace QuanLyCongViec
{
    partial class FormMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lbl_Ten = new System.Windows.Forms.Label();
            this.btn_DangXuat = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panel5 = new System.Windows.Forms.Panel();
            this.llb_Username = new System.Windows.Forms.LinkLabel();
            this.lbl_NgayThang = new System.Windows.Forms.Label();
            this.lbl_quahan = new System.Windows.Forms.Label();
            this.lbl_Done = new System.Windows.Forms.Label();
            this.lbl_Doing = new System.Windows.Forms.Label();
            this.lbl_Todo = new System.Windows.Forms.Label();
            this.btn_LichSu = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btn_QuanLyCongViec = new System.Windows.Forms.Button();
            this.btn_BaoCao = new System.Windows.Forms.Button();
            this.lbl_TongCongViec = new System.Windows.Forms.Label();
            this.panel_Tong = new System.Windows.Forms.Panel();
            this.panel_Doing = new System.Windows.Forms.Panel();
            this.panel_Done = new System.Windows.Forms.Panel();
            this.panel_QuaHan = new System.Windows.Forms.Panel();
            this.panel_Todo = new System.Windows.Forms.Panel();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.panel_Tong.SuspendLayout();
            this.panel_Doing.SuspendLayout();
            this.panel_Done.SuspendLayout();
            this.panel_QuaHan.SuspendLayout();
            this.panel_Todo.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbl_Ten
            // 
            this.lbl_Ten.AutoSize = true;
            this.lbl_Ten.Location = new System.Drawing.Point(10, 18);
            this.lbl_Ten.Name = "lbl_Ten";
            this.lbl_Ten.Size = new System.Drawing.Size(165, 16);
            this.lbl_Ten.TabIndex = 0;
            this.lbl_Ten.Text = "Xin chào, [Tên người dùng]";
            // 
            // btn_DangXuat
            // 
            this.btn_DangXuat.Location = new System.Drawing.Point(223, 46);
            this.btn_DangXuat.Name = "btn_DangXuat";
            this.btn_DangXuat.Size = new System.Drawing.Size(95, 24);
            this.btn_DangXuat.TabIndex = 1;
            this.btn_DangXuat.Text = "Đăng Xuất";
            this.btn_DangXuat.UseVisualStyleBackColor = true;
            this.btn_DangXuat.Click += new System.EventHandler(this.btn_DangXuat_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.panel5);
            this.groupBox1.Controls.Add(this.llb_Username);
            this.groupBox1.Controls.Add(this.lbl_NgayThang);
            this.groupBox1.Controls.Add(this.lbl_Ten);
            this.groupBox1.Controls.Add(this.btn_DangXuat);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(480, 89);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            // 
            // panel5
            // 
            this.panel5.Location = new System.Drawing.Point(164, 95);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(310, 51);
            this.panel5.TabIndex = 11;
            // 
            // llb_Username
            // 
            this.llb_Username.AutoSize = true;
            this.llb_Username.Location = new System.Drawing.Point(220, 18);
            this.llb_Username.Name = "llb_Username";
            this.llb_Username.Size = new System.Drawing.Size(78, 16);
            this.llb_Username.TabIndex = 4;
            this.llb_Username.TabStop = true;
            this.llb_Username.Text = "[Username]";
            this.llb_Username.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llb_Username_LinkClicked);
            // 
            // lbl_NgayThang
            // 
            this.lbl_NgayThang.AutoSize = true;
            this.lbl_NgayThang.Location = new System.Drawing.Point(13, 54);
            this.lbl_NgayThang.Name = "lbl_NgayThang";
            this.lbl_NgayThang.Size = new System.Drawing.Size(64, 16);
            this.lbl_NgayThang.TabIndex = 3;
            this.lbl_NgayThang.Text = "Hôm nay:";
            // 
            // lbl_quahan
            // 
            this.lbl_quahan.AutoSize = true;
            this.lbl_quahan.Location = new System.Drawing.Point(3, 11);
            this.lbl_quahan.Name = "lbl_quahan";
            this.lbl_quahan.Size = new System.Drawing.Size(57, 16);
            this.lbl_quahan.TabIndex = 0;
            this.lbl_quahan.Text = "Qúa hạn";
            // 
            // lbl_Done
            // 
            this.lbl_Done.AutoSize = true;
            this.lbl_Done.Location = new System.Drawing.Point(4, 9);
            this.lbl_Done.Name = "lbl_Done";
            this.lbl_Done.Size = new System.Drawing.Size(40, 16);
            this.lbl_Done.TabIndex = 0;
            this.lbl_Done.Text = "Done";
            // 
            // lbl_Doing
            // 
            this.lbl_Doing.AutoSize = true;
            this.lbl_Doing.Location = new System.Drawing.Point(10, 10);
            this.lbl_Doing.Name = "lbl_Doing";
            this.lbl_Doing.Size = new System.Drawing.Size(43, 16);
            this.lbl_Doing.TabIndex = 0;
            this.lbl_Doing.Text = "Doing";
            // 
            // lbl_Todo
            // 
            this.lbl_Todo.AutoSize = true;
            this.lbl_Todo.Location = new System.Drawing.Point(11, 10);
            this.lbl_Todo.Name = "lbl_Todo";
            this.lbl_Todo.Size = new System.Drawing.Size(42, 16);
            this.lbl_Todo.TabIndex = 0;
            this.lbl_Todo.Text = "ToDo";
            // 
            // btn_LichSu
            // 
            this.btn_LichSu.Location = new System.Drawing.Point(13, 21);
            this.btn_LichSu.Name = "btn_LichSu";
            this.btn_LichSu.Size = new System.Drawing.Size(126, 74);
            this.btn_LichSu.TabIndex = 4;
            this.btn_LichSu.Text = "Lịch Sử";
            this.btn_LichSu.UseVisualStyleBackColor = true;
            this.btn_LichSu.Click += new System.EventHandler(this.btn_LichSu_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Location = new System.Drawing.Point(235, 317);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(8, 8);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "groupBox2";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btn_QuanLyCongViec);
            this.groupBox3.Controls.Add(this.btn_BaoCao);
            this.groupBox3.Controls.Add(this.btn_LichSu);
            this.groupBox3.Location = new System.Drawing.Point(12, 338);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(480, 100);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Menu";
            // 
            // btn_QuanLyCongViec
            // 
            this.btn_QuanLyCongViec.Location = new System.Drawing.Point(156, 21);
            this.btn_QuanLyCongViec.Name = "btn_QuanLyCongViec";
            this.btn_QuanLyCongViec.Size = new System.Drawing.Size(177, 74);
            this.btn_QuanLyCongViec.TabIndex = 4;
            this.btn_QuanLyCongViec.Text = "Quản lý công việc";
            this.btn_QuanLyCongViec.UseVisualStyleBackColor = true;
            this.btn_QuanLyCongViec.Click += new System.EventHandler(this.btn_QuanLyCongViec_Click);
            // 
            // btn_BaoCao
            // 
            this.btn_BaoCao.Location = new System.Drawing.Point(354, 20);
            this.btn_BaoCao.Name = "btn_BaoCao";
            this.btn_BaoCao.Size = new System.Drawing.Size(120, 74);
            this.btn_BaoCao.TabIndex = 4;
            this.btn_BaoCao.Text = "Báo Cáo";
            this.btn_BaoCao.UseVisualStyleBackColor = true;
            this.btn_BaoCao.Click += new System.EventHandler(this.btn_BaoCao_Click);
            // 
            // lbl_TongCongViec
            // 
            this.lbl_TongCongViec.AutoSize = true;
            this.lbl_TongCongViec.Location = new System.Drawing.Point(3, 10);
            this.lbl_TongCongViec.Name = "lbl_TongCongViec";
            this.lbl_TongCongViec.Size = new System.Drawing.Size(98, 16);
            this.lbl_TongCongViec.TabIndex = 0;
            this.lbl_TongCongViec.Text = "TongCongViec";
            // 
            // panel_Tong
            // 
            this.panel_Tong.Controls.Add(this.lbl_TongCongViec);
            this.panel_Tong.Location = new System.Drawing.Point(12, 107);
            this.panel_Tong.Name = "panel_Tong";
            this.panel_Tong.Size = new System.Drawing.Size(144, 51);
            this.panel_Tong.TabIndex = 7;
            // 
            // panel_Doing
            // 
            this.panel_Doing.Controls.Add(this.lbl_Doing);
            this.panel_Doing.Location = new System.Drawing.Point(12, 164);
            this.panel_Doing.Name = "panel_Doing";
            this.panel_Doing.Size = new System.Drawing.Size(471, 50);
            this.panel_Doing.TabIndex = 8;
            this.panel_Doing.Paint += new System.Windows.Forms.PaintEventHandler(this.panel2_Paint);
            // 
            // panel_Done
            // 
            this.panel_Done.Controls.Add(this.lbl_Done);
            this.panel_Done.Location = new System.Drawing.Point(12, 220);
            this.panel_Done.Name = "panel_Done";
            this.panel_Done.Size = new System.Drawing.Size(471, 52);
            this.panel_Done.TabIndex = 9;
            // 
            // panel_QuaHan
            // 
            this.panel_QuaHan.Controls.Add(this.lbl_quahan);
            this.panel_QuaHan.Location = new System.Drawing.Point(12, 278);
            this.panel_QuaHan.Name = "panel_QuaHan";
            this.panel_QuaHan.Size = new System.Drawing.Size(471, 60);
            this.panel_QuaHan.TabIndex = 10;
            // 
            // panel_Todo
            // 
            this.panel_Todo.Controls.Add(this.lbl_Todo);
            this.panel_Todo.Location = new System.Drawing.Point(162, 107);
            this.panel_Todo.Name = "panel_Todo";
            this.panel_Todo.Size = new System.Drawing.Size(321, 51);
            this.panel_Todo.TabIndex = 11;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(496, 450);
            this.Controls.Add(this.panel_Todo);
            this.Controls.Add(this.panel_QuaHan);
            this.Controls.Add(this.panel_Done);
            this.Controls.Add(this.panel_Doing);
            this.Controls.Add(this.panel_Tong);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "FormMain";
            this.Text = "Hệ Thống Ứng dụng quản lý công việc";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.panel_Tong.ResumeLayout(false);
            this.panel_Tong.PerformLayout();
            this.panel_Doing.ResumeLayout(false);
            this.panel_Doing.PerformLayout();
            this.panel_Done.ResumeLayout(false);
            this.panel_Done.PerformLayout();
            this.panel_QuaHan.ResumeLayout(false);
            this.panel_QuaHan.PerformLayout();
            this.panel_Todo.ResumeLayout(false);
            this.panel_Todo.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lbl_Ten;
        private System.Windows.Forms.Button btn_DangXuat;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btn_LichSu;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label lbl_NgayThang;
        private System.Windows.Forms.Button btn_QuanLyCongViec;
        private System.Windows.Forms.Button btn_BaoCao;
        private System.Windows.Forms.LinkLabel llb_Username;
        private System.Windows.Forms.Label lbl_quahan;
        private System.Windows.Forms.Label lbl_Done;
        private System.Windows.Forms.Label lbl_Doing;
        private System.Windows.Forms.Label lbl_Todo;
        private System.Windows.Forms.Label lbl_TongCongViec;
        private System.Windows.Forms.Panel panel_Tong;
        private System.Windows.Forms.Panel panel_Doing;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel_Done;
        private System.Windows.Forms.Panel panel_QuaHan;
        private System.Windows.Forms.Panel panel_Todo;
    }
}