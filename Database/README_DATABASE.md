# 📊 HƯỚNG DẪN DATABASE - QUẢN LÝ CÔNG VIỆC

## 📁 DANH SÁCH FILE SQL

| File | Mô tả | Bắt buộc? |
|------|-------|-----------|
| `CreateDatabase.sql` | Tạo database và cấu trúc cơ bản | ✅ BẮT BUỘC |
| `AdditionalProcedures.sql` | Tạo các stored procedures | ✅ BẮT BUỘC |
| `InsertRandomSampleData.sql` | Tạo dữ liệu mẫu tự động | ⚪ TÙY CHỌN |

---

**💡 XÓA DATABASE (NẾU CẦN):**
- Dùng SSMS: Right click database → Delete
- Hoặc chạy SQL: `DROP DATABASE QuanLyCongViec;`

---

## 🚀 THỨ TỰ THỰC HIỆN (3 BƯỚC)

### ════════════════════════════════════════
### BƯỚC 1: TẠO DATABASE VÀ CẤU TRÚC
### ════════════════════════════════════════

**📄 File:** `CreateDatabase.sql`

**✅ Chức năng:**
- Tạo database `QuanLyCongViec`
- Tạo 3 bảng: `Users`, `Tasks`, `TaskHistory`
- Tạo 5 Views cho báo cáo
- Tạo 4 Stored Procedures cơ bản
- Tạo 2 Triggers tự động

**⏱️ Thời gian:** ~30 giây

**🔧 Cách chạy:**
```sql
1. Mở SQL Server Management Studio (SSMS)
2. Kết nối với SQL Server
3. Mở file CreateDatabase.sql
4. Nhấn F5 (Execute)
5. Kiểm tra: "Database QuanLyCongViec đã được tạo thành công!"
```

**📋 Kết quả sau khi chạy:**
- ✅ Database `QuanLyCongViec` được tạo
- ✅ 3 bảng: Users, Tasks, TaskHistory
- ✅ 5 views: vw_StatusStats, vw_PriorityStats, vw_CategoryStats, vw_TaskOverdueAndDueSoon, vw_UserTaskSummary
- ✅ 4 stored procedures cơ bản
- ✅ 2 triggers: tr_Tasks_Insert, tr_Tasks_Update

---

### ════════════════════════════════════════
### BƯỚC 2: TẠO STORED PROCEDURES
### ════════════════════════════════════════

**📄 File:** `AdditionalProcedures.sql`

**✅ Chức năng:**
- Bổ sung ~20+ stored procedures:
  - **User Management:** Login, Register, Get, Update, Change Password
  - **Task Management:** Get, Create, Update, Delete, Search, Filter
  - **Statistics:** Dashboard Stats, User Stats
  - **Admin & Reporting:** Overdue Tasks, Task History

**⏱️ Thời gian:** ~15 giây

**🔧 Cách chạy:**
```sql
1. Đảm bảo đã chạy CreateDatabase.sql (Bước 1)
2. Mở file AdditionalProcedures.sql
3. Nhấn F5 (Execute)
4. Kiểm tra: "Các Stored Procedures bổ sung đã được tạo thành công!"
```

**📋 Kết quả sau khi chạy:**
- ✅ ~20+ stored procedures được tạo
- ✅ Đầy đủ chức năng: User, Task, Statistics, Admin

---

### ════════════════════════════════════════
### BƯỚC 3: TẠO DỮ LIỆU MẪU (TÙY CHỌN)
### ════════════════════════════════════════

**Có 2 lựa chọn:**

#### 📄 LỰA CHỌN A: InsertRandomSampleData.sql (KHUYẾN NGHỊ)

**✅ Chức năng:**
- Tạo nhiều Users tự động (mặc định: 15 users)
- Tạo nhiều Tasks cho mỗi user (mặc định: 12 tasks/user)
- Tự động tạo tài khoản admin
- Dữ liệu được random với đầy đủ trạng thái, độ ưu tiên

**⏱️ Thời gian:** ~10-30 giây

**🔧 Cách chạy:**
```sql
1. Đảm bảo đã chạy Bước 1 và Bước 2
2. (Tùy chọn) Mở file và thay đổi số lượng:
   DECLARE @SoLuongUsers INT = 15;        -- Thay đổi số này
   DECLARE @SoLuongTasksPerUser INT = 12; -- Thay đổi số này
3. Mở file InsertRandomSampleData.sql
4. Nhấn F5 (Execute)
5. Xem thống kê được in ra
```

**📋 Kết quả:**
- ✅ Tài khoản admin (username: admin, password: 123456)
- ✅ 15 users: user1, user2, ... user15 (password: 123456)
- ✅ ~180 tasks tổng cộng
- ✅ Tasks có đầy đủ: Todo, Doing, Done, Quá hạn, Sắp đến hạn

**📖 Xem chi tiết:** File `HUONG_DAN_TAO_DU_LIEU_MAU.txt`

---

#### 📝 LỰA CHỌN B: Tạo tài khoản thủ công qua ứng dụng

**✅ Chức năng:**
- Tạo Users qua form Đăng ký trong ứng dụng
- Tự tạo Tasks qua form Quản lý công việc

**🔧 Cách làm:**
```
1. Chạy ứng dụng (sau Bước 1 và Bước 2)
2. Click "Đăng ký" trên form Đăng nhập
3. Tạo tài khoản với thông tin của bạn
4. Đăng nhập và sử dụng
```

---

## 📊 TÓM TẮT THỨ TỰ

```
┌─────────────────────────────────────┐
│  BƯỚC 1: CreateDatabase.sql        │
│  ✅ BẮT BUỘC                        │
│  → Tạo database và cấu trúc        │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│  BƯỚC 2: AdditionalProcedures.sql  │
│  ✅ BẮT BUỘC                        │
│  → Tạo stored procedures           │
└──────────────┬──────────────────────┘
               │
               ▼
        ┌──────┴──────┐
        │             │
        ▼             ▼
┌─────────────┐  ┌─────────────┐
│ LỰA CHỌN A │  │ LỰA CHỌN B  │
│ Insert     │  │ Tạo thủ công│
│ Random     │  │ qua ứng dụng│
│ SampleData │  │             │
│ ⚪ TÙY CHỌN│  │ ⚪ TÙY CHỌN │
└─────────────┘  └─────────────┘
```

---

## ✅ KIỂM TRA SAU KHI HOÀN THÀNH

Sau khi chạy xong, kiểm tra bằng các câu lệnh:

```sql
-- 1. Kiểm tra database
USE QuanLyCongViec;
SELECT DB_NAME() AS DatabaseName;

-- 2. Kiểm tra các bảng
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE';
-- Kết quả: TaskHistory, Tasks, Users

-- 3. Kiểm tra stored procedures
SELECT COUNT(*) AS SoLuongSP
FROM INFORMATION_SCHEMA.ROUTINES
WHERE ROUTINE_TYPE = 'PROCEDURE';
-- Kết quả: ~20+

-- 4. Kiểm tra users (nếu đã chạy script mẫu)
SELECT COUNT(*) AS SoLuongUsers FROM Users WHERE IsActive = 1;
-- Nếu chạy script mẫu: 15+

-- 5. Kiểm tra tasks (nếu đã chạy script mẫu)
SELECT COUNT(*) AS SoLuongTasks FROM Tasks WHERE IsDeleted = 0;
-- Nếu chạy script mẫu: 180+
```

---

## 🔐 THÔNG TIN ĐĂNG NHẬP MẶC ĐỊNH

**Sau khi chạy InsertRandomSampleData.sql:**

| Username | Password | Vai trò |
|----------|----------|---------|
| admin | 123456 | Quản trị viên |
| user1 | 123456 | User thường |
| user2 | 123456 | User thường |
| ... | ... | ... |
| user15 | 123456 | User thường |

---

## ⚠️ XỬ LÝ LỖI THƯỜNG GẶP

### ❌ Lỗi: "Database already exists"
→ **KHÔNG SAO!** Script sẽ tiếp tục và tạo lại cấu trúc bên trong

### ❌ Lỗi: "Cannot drop the table 'Users' because it does not exist"
→ **KHÔNG SAO!** Script đang kiểm tra và chỉ xóa nếu tồn tại

### ❌ Lỗi: "Foreign key constraint"
→ Xóa dữ liệu theo thứ tự: TaskHistory → Tasks → Users
→ Hoặc chạy lại CreateDatabase.sql từ đầu

### ❌ Lỗi: "Stored procedure already exists"
→ **KHÔNG SAO!** Script sẽ tự động xóa và tạo lại

### ❌ Lỗi: "Login failed" khi test ứng dụng
→ Kiểm tra connection string trong App.config
→ Kiểm tra SQL Server đang chạy
→ Thử đăng nhập với: admin / 123456

---

## 📚 FILE HƯỚNG DẪN LIÊN QUAN

- `HUONG_DAN_TAO_DU_LIEU_MAU.txt` - Hướng dẫn tạo dữ liệu mẫu
- `SETUP_INSTRUCTIONS.txt` - Hướng dẫn setup toàn bộ project
- `README_SHARE.md` - Hướng dẫn chia sẻ project

---

## 🎯 KỊCH BẢN SỬ DỤNG

### KỊCH BẢN 1: Cài đặt lần đầu với dữ liệu mẫu (KHUYẾN NGHỊ)
```
1. CreateDatabase.sql          → Chạy
2. AdditionalProcedures.sql    → Chạy
3. InsertRandomSampleData.sql  → Chạy
```
✅ Kết quả: Có sẵn dữ liệu mẫu để test ngay

### KỊCH BẢN 2: Cài đặt không có dữ liệu mẫu
```
1. CreateDatabase.sql          → Chạy
2. AdditionalProcedures.sql    → Chạy
```
✅ Kết quả: Database trống, tự tạo dữ liệu qua ứng dụng

### KỊCH BẢN 3: Chỉ cập nhật stored procedures
```
1. AdditionalProcedures.sql    → Chạy
```
✅ Kết quả: Cập nhật stored procedures, giữ nguyên dữ liệu

---

## 📝 LƯU Ý QUAN TRỌNG

1. ⚠️ **Bắt buộc chạy đúng thứ tự:** Bước 1 → Bước 2 → Bước 3
2. ⚠️ **Không được bỏ qua Bước 1 và Bước 2**
3. ⚠️ **Bước 3 chỉ cần chọn MỘT trong hai cách**
4. ✅ Script tự động xóa và tạo lại nếu đã tồn tại
5. ✅ Có thể chạy lại script nhiều lần an toàn

---

**🎉 Chúc bạn setup thành công!**

