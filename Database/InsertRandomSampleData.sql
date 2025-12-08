-- =============================================
-- Script tạo dữ liệu mẫu ngẫu nhiên
-- Tạo nhiều Users và Tasks để test ứng dụng
-- =============================================
-- ⚠️ QUAN TRỌNG: Phải chạy TOÀN BỘ script từ đầu đến cuối
-- Không được chạy từng phần hoặc chọn dòng giữa chừng
-- =============================================

USE QuanLyCongViec;
GO

-- =============================================
-- KHAI BÁO CÁC BIẾN
-- =============================================
-- Hash của "123456" + "QuanLyCongViec_Salt_2024" (SHA256 hex)
-- Đã tính từ PasswordHelper.cs trong ứng dụng C#
DECLARE @PasswordHash NVARCHAR(255) = '8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918';

-- CẤU HÌNH SỐ LƯỢNG - Có thể thay đổi các giá trị này
DECLARE @SoLuongUsers INT = 15;        -- Số lượng users muốn tạo (tối đa 20)
DECLARE @SoLuongTasksPerUser INT = 12; -- Số lượng tasks mỗi user

-- =============================================
-- XÓA DỮ LIỆU CŨ (NẾU MUỐN)
-- =============================================
-- Bỏ comment nếu muốn xóa dữ liệu cũ trước khi chèn mới
-- DELETE FROM TaskHistory;
-- DELETE FROM Tasks;
-- DELETE FROM Users WHERE Username != 'admin'; -- Giữ lại user admin
-- DBCC CHECKIDENT ('Users', RESEED, 0);
-- DBCC CHECKIDENT ('Tasks', RESEED, 0);
-- DBCC CHECKIDENT ('TaskHistory', RESEED, 0);

PRINT '========================================';
PRINT 'BẮT ĐẦU TẠO DỮ LIỆU MẪU';
PRINT '========================================';
PRINT 'Số lượng Users: ' + CAST(@SoLuongUsers AS VARCHAR(10));
PRINT 'Số lượng Tasks mỗi User: ' + CAST(@SoLuongTasksPerUser AS VARCHAR(10));
PRINT '========================================';

-- =============================================
-- TẠO DANH SÁCH TÊN MẪU
-- =============================================
-- Tạo bảng tạm chứa danh sách tên
CREATE TABLE #TempNames (
    FirstName NVARCHAR(50),
    LastName NVARCHAR(50)
);

INSERT INTO #TempNames VALUES
('Nguyễn Văn', 'An'), ('Trần Thị', 'Bình'), ('Lê Văn', 'Cường'), ('Phạm Thị', 'Dung'),
('Hoàng Văn', 'Đức'), ('Ngô Thị', 'Hương'), ('Vũ Văn', 'Hùng'), ('Đỗ Thị', 'Lan'),
('Bùi Văn', 'Minh'), ('Lý Thị', 'Nga'), ('Đinh Văn', 'Phong'), ('Mai Thị', 'Quỳnh'),
('Tạ Văn', 'Sơn'), ('Võ Thị', 'Trang'), ('Phan Văn', 'Tuấn'), ('Hồ Thị', 'Uyên'),
('Dương Văn', 'Việt'), ('Lưu Thị', 'Yến'), ('Chu Văn', 'Bảo'), ('Trịnh Thị', 'Châu');

-- =============================================
-- TẠO DANH SÁCH TÊN CÔNG VIỆC MẪU
-- =============================================
CREATE TABLE #TempTaskTitles (
    Title NVARCHAR(200),
    Category NVARCHAR(20)
);

INSERT INTO #TempTaskTitles VALUES
-- Công việc Work
('Phân tích yêu cầu hệ thống', 'Work'), ('Thiết kế database', 'Work'), ('Lập trình module đăng nhập', 'Work'),
('Lập trình module quản lý user', 'Work'), ('Thiết kế giao diện người dùng', 'Work'), ('Lập trình API backend', 'Work'),
('Viết unit test', 'Work'), ('Code review', 'Work'), ('Tối ưu hóa hiệu suất', 'Work'), ('Deploy lên server', 'Work'),
('Sửa lỗi bug', 'Work'), ('Cập nhật tài liệu', 'Work'), ('Họp với khách hàng', 'Work'), ('Báo cáo tiến độ', 'Work'),
('Đào tạo nhân viên mới', 'Work'), ('Nghiên cứu công nghệ mới', 'Work'), ('Tối ưu database', 'Work'), ('Backup dữ liệu', 'Work'),
-- Công việc Personal
('Mua sắm đồ dùng cá nhân', 'Personal'), ('Đi khám sức khỏe', 'Personal'), ('Gặp bạn bè', 'Personal'),
('Tập thể dục', 'Personal'), ('Đọc sách', 'Personal'), ('Học ngoại ngữ', 'Personal'), ('Dọn dẹp nhà cửa', 'Personal'),
('Nấu ăn', 'Personal'), ('Xem phim', 'Personal'), ('Du lịch', 'Personal'),
-- Công việc Study
('Học lập trình C#', 'Study'), ('Làm bài tập toán', 'Study'), ('Ôn thi cuối kỳ', 'Study'),
('Viết báo cáo đồ án', 'Study'), ('Thuyết trình', 'Study'), ('Nghiên cứu tài liệu', 'Study'),
('Làm project nhóm', 'Study'), ('Học tiếng Anh', 'Study'), ('Chuẩn bị bài mới', 'Study'), ('Làm bài kiểm tra', 'Study');

-- =============================================
-- TẠO USERS
-- =============================================
DECLARE @Counter INT = 1;
DECLARE @Username NVARCHAR(50);
DECLARE @FullName NVARCHAR(100);
DECLARE @Email NVARCHAR(100);
DECLARE @FirstName NVARCHAR(50);
DECLARE @LastName NVARCHAR(50);
DECLARE @UserId INT;

WHILE @Counter <= @SoLuongUsers
BEGIN
    -- Chọn tên ngẫu nhiên
    SELECT TOP 1 @FirstName = FirstName, @LastName = LastName
    FROM #TempNames
    ORDER BY NEWID();
    
    -- Tạo username và email
    SET @Username = 'user' + CAST(@Counter AS VARCHAR(10));
    SET @FullName = @FirstName + ' ' + @LastName;
    SET @Email = @Username + '@example.com';
    
    -- Kiểm tra username đã tồn tại chưa
    IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = @Username)
    BEGIN
        -- Insert user
        INSERT INTO Users (Username, PasswordHash, FullName, Email, CreatedDate, IsActive)
        VALUES (@Username, @PasswordHash, @FullName, @Email, DATEADD(DAY, -RAND() * 30, GETDATE()), 1);
        
        SET @UserId = SCOPE_IDENTITY();
        
        PRINT 'Đã tạo user: ' + @Username + ' - ' + @FullName;
    END
    ELSE
    BEGIN
        -- Lấy UserId nếu đã tồn tại
        SELECT @UserId = Id FROM Users WHERE Username = @Username;
        PRINT 'User đã tồn tại: ' + @Username;
    END
    
    -- Tạo Tasks cho user này
    IF @UserId IS NOT NULL
    BEGIN
        DECLARE @TaskCounter INT = 1;
        DECLARE @TaskTitle NVARCHAR(200);
        DECLARE @TaskCategory NVARCHAR(20);
        DECLARE @TaskPriority NVARCHAR(20);
        DECLARE @TaskStatus NVARCHAR(20);
        DECLARE @TaskDueDate DATETIME;
        DECLARE @TaskCreatedDate DATETIME;
        DECLARE @TaskDescription NVARCHAR(MAX);
        DECLARE @RandomValue FLOAT;
        
        WHILE @TaskCounter <= @SoLuongTasksPerUser
        BEGIN
            -- Chọn tiêu đề công việc ngẫu nhiên
            SELECT TOP 1 @TaskTitle = Title, @TaskCategory = Category
            FROM #TempTaskTitles
            ORDER BY NEWID();
            
            -- Random Priority (đảm bảo random mỗi lần khác nhau)
            SET @RandomValue = RAND(CHECKSUM(NEWID()));
            SET @TaskPriority = CASE 
                WHEN @RandomValue < 0.33 THEN 'High'
                WHEN @RandomValue < 0.66 THEN 'Medium'
                ELSE 'Low'
            END;
            
            -- Random Status
            SET @RandomValue = RAND(CHECKSUM(NEWID()));
            SET @TaskStatus = CASE 
                WHEN @RandomValue < 0.4 THEN 'Todo'
                WHEN @RandomValue < 0.75 THEN 'Doing'
                ELSE 'Done'
            END;
            
            -- Random DueDate (từ 30 ngày trước đến 30 ngày sau)
            SET @RandomValue = RAND(CHECKSUM(NEWID()));
            SET @TaskDueDate = DATEADD(DAY, CAST(@RandomValue * 60 - 30 AS INT), GETDATE());
            
            -- Random CreatedDate (từ 60 ngày trước đến hiện tại)
            SET @RandomValue = RAND(CHECKSUM(NEWID()));
            SET @TaskCreatedDate = DATEADD(DAY, -CAST(@RandomValue * 60 AS INT), GETDATE());
            
            -- Tạo Description
            SET @RandomValue = RAND(CHECKSUM(NEWID()));
            SET @TaskDescription = @TaskTitle + '. Thời gian dự kiến: ' + CAST(CAST(@RandomValue * 40 + 10 AS INT) AS VARCHAR(10)) + ' giờ.';
            
            -- Insert Task
            INSERT INTO Tasks (Title, Description, UserId, Priority, Status, Category, DueDate, CreatedDate, CompletedDate, IsDeleted)
            VALUES (
                @TaskTitle,
                @TaskDescription,
                @UserId,
                @TaskPriority,
                @TaskStatus,
                @TaskCategory,
                @TaskDueDate,
                @TaskCreatedDate,
                CASE WHEN @TaskStatus = 'Done' THEN DATEADD(DAY, CAST(RAND(CHECKSUM(NEWID())) * 10 AS INT), @TaskCreatedDate) ELSE NULL END,
                0
            );
            
            SET @TaskCounter = @TaskCounter + 1;
        END
    END
    
    SET @Counter = @Counter + 1;
END

-- =============================================
-- TẠO TÀI KHOẢN ADMIN (nếu chưa có)
-- =============================================
IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'admin')
BEGIN
    INSERT INTO Users (Username, PasswordHash, FullName, Email, CreatedDate, IsActive)
    VALUES ('admin', @PasswordHash, 'Quản trị viên', 'admin@example.com', GETDATE(), 1);
    
    DECLARE @AdminUserId INT = SCOPE_IDENTITY();
    
    -- Tạo một số tasks cho admin
    DECLARE @AdminTaskCounter INT = 1;
    DECLARE @AdminTaskTitle NVARCHAR(200);
    DECLARE @AdminTaskCategory NVARCHAR(20);
    DECLARE @AdminTaskPriority NVARCHAR(20);
    DECLARE @AdminTaskStatus NVARCHAR(20);
    DECLARE @AdminTaskDueDate DATETIME;
    DECLARE @AdminTaskCreatedDate DATETIME;
    DECLARE @AdminTaskDescription NVARCHAR(MAX);
    DECLARE @AdminRandomValue FLOAT;
    
    WHILE @AdminTaskCounter <= 5
    BEGIN
        -- Chọn tiêu đề công việc ngẫu nhiên từ bảng tạm
        SELECT TOP 1 @AdminTaskTitle = Title, @AdminTaskCategory = Category
        FROM #TempTaskTitles
        ORDER BY NEWID();
        
        SET @AdminRandomValue = RAND(CHECKSUM(NEWID()));
        SET @AdminTaskPriority = CASE WHEN @AdminRandomValue < 0.5 THEN 'High' ELSE 'Medium' END;
        
        SET @AdminRandomValue = RAND(CHECKSUM(NEWID()));
        SET @AdminTaskStatus = CASE WHEN @AdminRandomValue < 0.5 THEN 'Todo' ELSE 'Doing' END;
        
        SET @AdminRandomValue = RAND(CHECKSUM(NEWID()));
        SET @AdminTaskDueDate = DATEADD(DAY, CAST(@AdminRandomValue * 30 AS INT), GETDATE());
        
        SET @AdminRandomValue = RAND(CHECKSUM(NEWID()));
        SET @AdminTaskCreatedDate = DATEADD(DAY, -CAST(@AdminRandomValue * 15 AS INT), GETDATE());
        
        SET @AdminTaskDescription = @AdminTaskTitle + '. Công việc quản trị.';
        
        INSERT INTO Tasks (Title, Description, UserId, Priority, Status, Category, DueDate, CreatedDate, IsDeleted)
        VALUES (@AdminTaskTitle, @AdminTaskDescription, @AdminUserId, @AdminTaskPriority, @AdminTaskStatus, @AdminTaskCategory, @AdminTaskDueDate, @AdminTaskCreatedDate, 0);
        
        SET @AdminTaskCounter = @AdminTaskCounter + 1;
    END
    
    PRINT 'Đã tạo tài khoản admin';
END

-- =============================================
-- DỌN DẸP BẢNG TẠM
-- =============================================
DROP TABLE #TempNames;
DROP TABLE #TempTaskTitles;

PRINT '========================================';
PRINT 'HOÀN THÀNH TẠO DỮ LIỆU MẪU';
PRINT '========================================';

-- Hiển thị thống kê
SELECT 
    'Tổng số Users' AS ThongKe,
    COUNT(*) AS SoLuong
FROM Users
WHERE IsActive = 1

UNION ALL

SELECT 
    'Tổng số Tasks' AS ThongKe,
    COUNT(*) AS SoLuong
FROM Tasks
WHERE IsDeleted = 0

UNION ALL

SELECT 
    'Số Tasks Todo' AS ThongKe,
    COUNT(*) AS SoLuong
FROM Tasks
WHERE Status = 'Todo' AND IsDeleted = 0

UNION ALL

SELECT 
    'Số Tasks Doing' AS ThongKe,
    COUNT(*) AS SoLuong
FROM Tasks
WHERE Status = 'Doing' AND IsDeleted = 0

UNION ALL

SELECT 
    'Số Tasks Done' AS ThongKe,
    COUNT(*) AS SoLuong
FROM Tasks
WHERE Status = 'Done' AND IsDeleted = 0

UNION ALL

SELECT 
    'Số Tasks Quá Hạn' AS ThongKe,
    COUNT(*) AS SoLuong
FROM Tasks
WHERE Status != 'Done' AND DueDate < CAST(GETDATE() AS DATE) AND IsDeleted = 0;

GO

PRINT '';
PRINT '========================================';
PRINT 'THÔNG TIN ĐĂNG NHẬP';
PRINT '========================================';
PRINT 'Username: admin (hoặc user1, user2, ...)';
PRINT 'Password: 123456';
PRINT '========================================';
GO

