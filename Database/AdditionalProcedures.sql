-- =============================================
-- Script bổ sung các Stored Procedures cần thiết
-- Chạy sau khi chạy CreateDatabase.sql
-- =============================================

USE QuanLyCongViec;
GO

-- =============================================
-- USER MANAGEMENT PROCEDURES
-- =============================================

-- SP: Đăng nhập người dùng
IF OBJECT_ID('sp_UserLogin', 'P') IS NOT NULL
    DROP PROCEDURE sp_UserLogin;
GO

CREATE PROCEDURE sp_UserLogin
    @Username NVARCHAR(50),
    @PasswordHash NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @UserId INT;
    
    SELECT 
        @UserId = Id
    FROM Users
    WHERE Username = @Username 
        AND PasswordHash = @PasswordHash
        AND IsActive = 1;
    
    IF @UserId IS NOT NULL
    BEGIN
        -- Cập nhật LastLoginDate
        UPDATE Users SET LastLoginDate = GETDATE() WHERE Id = @UserId;
        
        SELECT 
            Id,
            Username,
            FullName,
            Email,
            CreatedDate
        FROM Users
        WHERE Id = @UserId;
        
        RETURN 1;
    END
    ELSE
    BEGIN
        RETURN 0;
    END
END;
GO

-- SP: Đăng ký người dùng mới
IF OBJECT_ID('sp_UserRegister', 'P') IS NOT NULL
    DROP PROCEDURE sp_UserRegister;
GO

CREATE PROCEDURE sp_UserRegister
    @Username NVARCHAR(50),
    @PasswordHash NVARCHAR(255),
    @FullName NVARCHAR(100),
    @Email NVARCHAR(100),
    @UserId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    IF EXISTS (SELECT 1 FROM Users WHERE Username = @Username)
    BEGIN
        SET @UserId = -1;
        RETURN;
    END
    
    IF EXISTS (SELECT 1 FROM Users WHERE Email = @Email)
    BEGIN
        SET @UserId = -2;
        RETURN;
    END
    
    INSERT INTO Users (Username, PasswordHash, FullName, Email, CreatedDate)
    VALUES (@Username, @PasswordHash, @FullName, @Email, GETDATE());
    
    SET @UserId = SCOPE_IDENTITY();
END;
GO

-- SP: Lấy thông tin người dùng theo ID
IF OBJECT_ID('sp_GetUserById', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetUserById;
GO

CREATE PROCEDURE sp_GetUserById
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Username,
        FullName,
        Email,
        CreatedDate
    FROM Users
    WHERE Id = @UserId AND IsActive = 1;
END;
GO

-- SP: Cập nhật thông tin người dùng
IF OBJECT_ID('sp_UpdateUser', 'P') IS NOT NULL
    DROP PROCEDURE sp_UpdateUser;
GO

CREATE PROCEDURE sp_UpdateUser
    @UserId INT,
    @FullName NVARCHAR(100),
    @Email NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    IF EXISTS (SELECT 1 FROM Users WHERE Email = @Email AND Id != @UserId)
    BEGIN
        RAISERROR('Email đã được sử dụng bởi người dùng khác', 16, 1);
        RETURN;
    END
    
    UPDATE Users
    SET FullName = @FullName, Email = @Email
    WHERE Id = @UserId;
END;
GO

-- SP: Đổi mật khẩu
IF OBJECT_ID('sp_ChangePassword', 'P') IS NOT NULL
    DROP PROCEDURE sp_ChangePassword;
GO

CREATE PROCEDURE sp_ChangePassword
    @UserId INT,
    @OldPasswordHash NVARCHAR(255),
    @NewPasswordHash NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    
    IF NOT EXISTS (SELECT 1 FROM Users WHERE Id = @UserId AND PasswordHash = @OldPasswordHash)
    BEGIN
        RAISERROR('Mật khẩu cũ không đúng', 16, 1);
        RETURN;
    END
    
    UPDATE Users SET PasswordHash = @NewPasswordHash WHERE Id = @UserId;
END;
GO

-- =============================================
-- TASK MANAGEMENT PROCEDURES
-- =============================================

-- SP: Lấy tasks theo bộ lọc
IF OBJECT_ID('sp_GetTasksByFilter', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetTasksByFilter;
GO

CREATE PROCEDURE sp_GetTasksByFilter
    @UserId INT = NULL,
    @Status NVARCHAR(20) = NULL,
    @Priority NVARCHAR(20) = NULL,
    @Category NVARCHAR(20) = NULL,
    @SearchTitle NVARCHAR(200) = NULL,
    @IsOverdue BIT = NULL,
    @IsDueSoon BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        t.*,
        u.FullName AS UserFullName
    FROM Tasks t
    INNER JOIN Users u ON t.UserId = u.Id
    WHERE 
        t.IsDeleted = 0
        AND (@UserId IS NULL OR t.UserId = @UserId)
        AND (@Status IS NULL OR t.Status = @Status)
        AND (@Priority IS NULL OR t.Priority = @Priority)
        AND (@Category IS NULL OR t.Category = @Category)
        AND (@SearchTitle IS NULL OR t.Title LIKE '%' + @SearchTitle + '%')
        AND (@IsOverdue IS NULL OR 
             (@IsOverdue = 1 AND t.Status != 'Done' AND t.DueDate < CAST(GETDATE() AS DATE)) OR
             (@IsOverdue = 0 AND NOT (t.Status != 'Done' AND t.DueDate < CAST(GETDATE() AS DATE))))
        AND (@IsDueSoon IS NULL OR 
             (@IsDueSoon = 1 AND t.Status != 'Done' AND t.DueDate <= DATEADD(DAY, 3, CAST(GETDATE() AS DATE))) OR
             (@IsDueSoon = 0 AND NOT (t.Status != 'Done' AND t.DueDate <= DATEADD(DAY, 3, CAST(GETDATE() AS DATE)))))
    ORDER BY 
        CASE WHEN t.Priority = 'High' THEN 1 
             WHEN t.Priority = 'Medium' THEN 2 
             ELSE 3 END,
        t.DueDate ASC;
END;
GO

-- SP: Lấy task theo ID
IF OBJECT_ID('sp_GetTaskById', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetTaskById;
GO

CREATE PROCEDURE sp_GetTaskById
    @TaskId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        t.*,
        u.FullName AS UserFullName,
        u.Email AS UserEmail
    FROM Tasks t
    INNER JOIN Users u ON t.UserId = u.Id
    WHERE t.Id = @TaskId AND t.IsDeleted = 0;
END;
GO

-- SP: Lấy lịch sử thay đổi của task
IF OBJECT_ID('sp_GetTaskHistory', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetTaskHistory;
GO

CREATE PROCEDURE sp_GetTaskHistory
    @TaskId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        th.*,
        u.FullName AS UserFullName
    FROM TaskHistory th
    INNER JOIN Users u ON th.UserId = u.Id
    WHERE th.TaskId = @TaskId
    ORDER BY th.ActionDate DESC;
END;
GO

-- SP: Xóa task (soft delete)
IF OBJECT_ID('sp_DeleteTask', 'P') IS NOT NULL
    DROP PROCEDURE sp_DeleteTask;
GO

CREATE PROCEDURE sp_DeleteTask
    @TaskId INT,
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    IF NOT EXISTS (SELECT 1 FROM Tasks WHERE Id = @TaskId AND UserId = @UserId)
    BEGIN
        RAISERROR('Bạn không có quyền xóa công việc này', 16, 1);
        RETURN;
    END
    
    UPDATE Tasks
    SET IsDeleted = 1, DeletedDate = GETDATE()
    WHERE Id = @TaskId;
    
    INSERT INTO TaskHistory (TaskId, Action, OldStatus, NewStatus, Notes, UserId)
    SELECT Id, 'Deleted', Status, NULL, 'Công việc đã bị xóa', @UserId
    FROM Tasks WHERE Id = @TaskId;
END;
GO

-- SP: Tìm kiếm tasks
IF OBJECT_ID('sp_SearchTasks', 'P') IS NOT NULL
    DROP PROCEDURE sp_SearchTasks;
GO

CREATE PROCEDURE sp_SearchTasks
    @UserId INT,
    @SearchTerm NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        t.*,
        u.FullName AS UserFullName
    FROM Tasks t
    INNER JOIN Users u ON t.UserId = u.Id
    WHERE t.UserId = @UserId
        AND t.IsDeleted = 0
        AND (t.Title LIKE '%' + @SearchTerm + '%' OR t.Description LIKE '%' + @SearchTerm + '%')
    ORDER BY 
        CASE WHEN t.Priority = 'High' THEN 1 
             WHEN t.Priority = 'Medium' THEN 2 
             ELSE 3 END,
        t.DueDate ASC;
END;
GO

-- SP: Tạo công việc mới
IF OBJECT_ID('sp_CreateTask', 'P') IS NOT NULL
    DROP PROCEDURE sp_CreateTask;
GO

CREATE PROCEDURE sp_CreateTask
    @Title NVARCHAR(200),
    @Description NVARCHAR(MAX) = NULL,
    @UserId INT,
    @Priority NVARCHAR(20) = 'Medium',
    @Status NVARCHAR(20) = 'Todo',
    @Category NVARCHAR(20) = 'Work',
    @DueDate DATETIME,
    @TaskId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO Tasks (Title, Description, UserId, Priority, Status, Category, DueDate, CreatedDate)
    VALUES (@Title, @Description, @UserId, @Priority, @Status, @Category, @DueDate, GETDATE());
    
    SET @TaskId = SCOPE_IDENTITY();
END;
GO

-- SP: Cập nhật công việc
IF OBJECT_ID('sp_UpdateTask', 'P') IS NOT NULL
    DROP PROCEDURE sp_UpdateTask;
GO

CREATE PROCEDURE sp_UpdateTask
    @TaskId INT,
    @Title NVARCHAR(200),
    @Description NVARCHAR(MAX) = NULL,
    @UserId INT,
    @Priority NVARCHAR(20),
    @Status NVARCHAR(20),
    @Category NVARCHAR(20),
    @DueDate DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    
    IF NOT EXISTS (SELECT 1 FROM Tasks WHERE Id = @TaskId AND UserId = @UserId AND IsDeleted = 0)
    BEGIN
        RAISERROR('Bạn không có quyền sửa công việc này', 16, 1);
        RETURN;
    END
    
    UPDATE Tasks
    SET Title = @Title,
        Description = @Description,
        Priority = @Priority,
        Status = @Status,
        Category = @Category,
        DueDate = @DueDate
    WHERE Id = @TaskId;
END;
GO

-- =============================================
-- STATISTICS PROCEDURES
-- =============================================

-- SP: Lấy thống kê tổng quan cho dashboard
IF OBJECT_ID('sp_GetDashboardStats', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetDashboardStats;
GO

CREATE PROCEDURE sp_GetDashboardStats
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        COUNT(*) AS TotalTasks,
        SUM(CASE WHEN Status = 'Todo' THEN 1 ELSE 0 END) AS TodoCount,
        SUM(CASE WHEN Status = 'Doing' THEN 1 ELSE 0 END) AS DoingCount,
        SUM(CASE WHEN Status = 'Done' THEN 1 ELSE 0 END) AS DoneCount,
        SUM(CASE WHEN Status != 'Done' AND DueDate < CAST(GETDATE() AS DATE) THEN 1 ELSE 0 END) AS OverdueCount,
        SUM(CASE WHEN Status != 'Done' AND DueDate <= DATEADD(DAY, 3, CAST(GETDATE() AS DATE)) THEN 1 ELSE 0 END) AS DueSoonCount,
        CASE 
            WHEN COUNT(*) > 0 
            THEN CAST(SUM(CASE WHEN Status = 'Done' THEN 1 ELSE 0 END) * 100.0 / COUNT(*) AS DECIMAL(5,2))
            ELSE 0 
        END AS CompletionRate
    FROM Tasks
    WHERE UserId = @UserId AND IsDeleted = 0;
END;
GO

-- SP: Lấy UserId đầu tiên (dùng cho các form)
IF OBJECT_ID('sp_GetFirstUserId', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetFirstUserId;
GO

CREATE PROCEDURE sp_GetFirstUserId
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT TOP 1 Id 
    FROM Users 
    WHERE IsActive = 1 
    ORDER BY Id;
END;
GO

-- SP: Lấy thống kê người phụ trách
IF OBJECT_ID('sp_GetUserStats', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetUserStats;
GO

CREATE PROCEDURE sp_GetUserStats
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Trả về cả 2 giá trị trong 1 resultset
    SELECT 
        (SELECT COUNT(DISTINCT UserId) FROM Tasks WHERE IsDeleted = 0) AS TotalUsersWithTasks,
        (SELECT COUNT(DISTINCT UserId) FROM Tasks WHERE IsDeleted = 0 AND Status != 'Done' AND DueDate < CAST(GETDATE() AS DATE)) AS UsersWithOverdueTasks;
END;
GO

-- SP: Lấy danh sách công việc quá hạn
IF OBJECT_ID('sp_GetOverdueTasks', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetOverdueTasks;
GO

CREATE PROCEDURE sp_GetOverdueTasks
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        t.Id,
        'CV' + CAST(t.Id AS VARCHAR(10)) AS MaCV,
        t.Title AS TenCV,
        u.FullName AS NguoiPhuTrach,
        CAST(t.CreatedDate AS DATE) AS NgayBatDau,
        CAST(t.DueDate AS DATE) AS NgayKetThuc,
        CASE 
            WHEN t.Status = 'Todo' THEN 'Chưa bắt đầu'
            WHEN t.Status = 'Doing' THEN 'Đang thực hiện'
            WHEN t.Status = 'Done' THEN 'Hoàn thành'
            ELSE t.Status
        END AS TrangThai,
        CASE 
            WHEN t.Priority = 'High' THEN 'Cao'
            WHEN t.Priority = 'Medium' THEN 'Trung bình'
            WHEN t.Priority = 'Low' THEN 'Thấp'
            ELSE t.Priority
        END AS DoUuTien
    FROM Tasks t
    INNER JOIN Users u ON t.UserId = u.Id
    WHERE t.IsDeleted = 0
    AND t.Status != 'Done'
    AND t.DueDate < CAST(GETDATE() AS DATE)
    ORDER BY t.DueDate ASC;
END;
GO

-- SP: Lấy danh sách người có công việc quá hạn
IF OBJECT_ID('sp_GetUsersWithOverdueTasks', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetUsersWithOverdueTasks;
GO

CREATE PROCEDURE sp_GetUsersWithOverdueTasks
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        u.FullName AS NguoiPhuTrach,
        COUNT(t.Id) AS SoLuongQuaHan
    FROM Tasks t
    INNER JOIN Users u ON t.UserId = u.Id
    WHERE t.IsDeleted = 0
    AND t.Status != 'Done'
    AND t.DueDate < CAST(GETDATE() AS DATE)
    GROUP BY u.FullName, t.UserId
    ORDER BY COUNT(t.Id) DESC;
END;
GO

-- SP: Lấy tất cả lịch sử thay đổi công việc
IF OBJECT_ID('sp_GetAllTaskHistory', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetAllTaskHistory;
GO

CREATE PROCEDURE sp_GetAllTaskHistory
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        th.Id AS MaLichSu,
        u.FullName AS NguoiThaoTac,
        th.TaskId,
        'CV' + CAST(th.TaskId AS VARCHAR(10)) AS MaCongViec,
        CAST(th.ActionDate AS DATE) AS NgayTao,
        CASE 
            WHEN th.Action = 'Created' THEN 'Thêm'
            WHEN th.Action = 'Updated' THEN 'Sửa'
            WHEN th.Action = 'StatusChanged' THEN 'Sửa'
            WHEN th.Action = 'Completed' THEN 'Hoàn thành'
            WHEN th.Action = 'Deleted' THEN 'Xóa'
            ELSE th.Action
        END AS HanhDong,
        t.Priority,
        CASE 
            WHEN t.Priority = 'High' THEN 'Cao'
            WHEN t.Priority = 'Medium' THEN 'Trung bình'
            WHEN t.Priority = 'Low' THEN 'Thấp'
            ELSE t.Priority
        END AS DoUuTien,
        ISNULL(th.Notes, '') AS ChiTiet
    FROM TaskHistory th
    INNER JOIN Users u ON th.UserId = u.Id
    LEFT JOIN Tasks t ON th.TaskId = t.Id
    ORDER BY th.ActionDate DESC;
END;
GO

-- SP: Tìm kiếm lịch sử theo người thao tác hoặc mã công việc
IF OBJECT_ID('sp_SearchTaskHistory', 'P') IS NOT NULL
    DROP PROCEDURE sp_SearchTaskHistory;
GO

CREATE PROCEDURE sp_SearchTaskHistory
    @SearchTerm NVARCHAR(200),
    @SearchType NVARCHAR(20)  -- 'User' hoặc 'Task'
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @SearchType = 'User'
    BEGIN
        -- Tìm theo người thao tác
        SELECT 
            th.Id AS MaLichSu,
            u.FullName AS NguoiThaoTac,
            th.TaskId,
            'CV' + CAST(th.TaskId AS VARCHAR(10)) AS MaCongViec,
            CAST(th.ActionDate AS DATE) AS NgayTao,
            CASE 
                WHEN th.Action = 'Created' THEN 'Thêm'
                WHEN th.Action = 'Updated' THEN 'Sửa'
                WHEN th.Action = 'StatusChanged' THEN 'Sửa'
                WHEN th.Action = 'Completed' THEN 'Hoàn thành'
                WHEN th.Action = 'Deleted' THEN 'Xóa'
                ELSE th.Action
            END AS HanhDong,
            t.Priority,
            CASE 
                WHEN t.Priority = 'High' THEN 'Cao'
                WHEN t.Priority = 'Medium' THEN 'Trung bình'
                WHEN t.Priority = 'Low' THEN 'Thấp'
                ELSE t.Priority
            END AS DoUuTien,
            ISNULL(th.Notes, '') AS ChiTiet
        FROM TaskHistory th
        INNER JOIN Users u ON th.UserId = u.Id
        LEFT JOIN Tasks t ON th.TaskId = t.Id
        WHERE u.FullName LIKE '%' + @SearchTerm + '%'
        ORDER BY th.ActionDate DESC;
    END
    ELSE
    BEGIN
        -- Tìm theo mã công việc
        SELECT 
            th.Id AS MaLichSu,
            u.FullName AS NguoiThaoTac,
            th.TaskId,
            'CV' + CAST(th.TaskId AS VARCHAR(10)) AS MaCongViec,
            CAST(th.ActionDate AS DATE) AS NgayTao,
            CASE 
                WHEN th.Action = 'Created' THEN 'Thêm'
                WHEN th.Action = 'Updated' THEN 'Sửa'
                WHEN th.Action = 'StatusChanged' THEN 'Sửa'
                WHEN th.Action = 'Completed' THEN 'Hoàn thành'
                WHEN th.Action = 'Deleted' THEN 'Xóa'
                ELSE th.Action
            END AS HanhDong,
            t.Priority,
            CASE 
                WHEN t.Priority = 'High' THEN 'Cao'
                WHEN t.Priority = 'Medium' THEN 'Trung bình'
                WHEN t.Priority = 'Low' THEN 'Thấp'
                ELSE t.Priority
            END AS DoUuTien,
            ISNULL(th.Notes, '') AS ChiTiet
        FROM TaskHistory th
        INNER JOIN Users u ON th.UserId = u.Id
        LEFT JOIN Tasks t ON th.TaskId = t.Id
        WHERE CAST(th.TaskId AS VARCHAR(10)) LIKE '%' + @SearchTerm + '%'
        ORDER BY th.ActionDate DESC;
    END
END;
GO

-- =============================================
-- SP: Lấy các giới hạn validation từ database metadata
-- =============================================
IF OBJECT_ID('sp_GetValidationLimits', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetValidationLimits;
GO

CREATE PROCEDURE sp_GetValidationLimits
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Lấy thông tin từ metadata của bảng Users
    SELECT 
        'Username' AS TenTruong,
        -- Lấy độ dài tối đa từ CHARACTER_MAXIMUM_LENGTH
        CAST(CHARACTER_MAXIMUM_LENGTH AS INT) AS DoDaiToiDa,
        -- Lấy độ dài tối thiểu từ CHECK constraint (LEN(Username) >= 3)
        3 AS DoDaiToiThieu
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'Users' AND COLUMN_NAME = 'Username'
    
    UNION ALL
    
    SELECT 
        'PasswordHash' AS TenTruong,
        100 AS DoDaiToiDa,  -- Độ dài tối đa của mật khẩu gốc (trước khi hash)
        6 AS DoDaiToiThieu  -- Mật khẩu tối thiểu 6 ký tự
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'Users' AND COLUMN_NAME = 'PasswordHash'
    
    UNION ALL
    
    SELECT 
        'Email' AS TenTruong,
        CAST(CHARACTER_MAXIMUM_LENGTH AS INT) AS DoDaiToiDa,
        5 AS DoDaiToiThieu  -- Email tối thiểu: a@b.c (5 ký tự)
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'Users' AND COLUMN_NAME = 'Email'
    
    UNION ALL
    
    SELECT 
        'FullName' AS TenTruong,
        CAST(CHARACTER_MAXIMUM_LENGTH AS INT) AS DoDaiToiDa,
        1 AS DoDaiToiThieu  -- Họ tên tối thiểu 1 ký tự
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'Users' AND COLUMN_NAME = 'FullName';
END;
GO

-- =============================================
-- SP: Lấy giá trị cấu hình hệ thống
-- =============================================
IF OBJECT_ID('sp_GetSystemSetting', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetSystemSetting;
GO

CREATE PROCEDURE sp_GetSystemSetting
    @SettingKey NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT SettingValue, Description
    FROM Settings
    WHERE SettingKey = @SettingKey;
END;
GO

-- =============================================
-- SP: Lấy tất cả các cấu hình hệ thống
-- =============================================
IF OBJECT_ID('sp_GetAllSystemSettings', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetAllSystemSettings;
GO

CREATE PROCEDURE sp_GetAllSystemSettings
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT SettingKey, SettingValue, Description
    FROM Settings
    ORDER BY SettingKey;
END;
GO

-- =============================================
-- SP: Cập nhật giá trị cấu hình hệ thống
-- =============================================
IF OBJECT_ID('sp_UpdateSystemSetting', 'P') IS NOT NULL
    DROP PROCEDURE sp_UpdateSystemSetting;
GO

CREATE PROCEDURE sp_UpdateSystemSetting
    @SettingKey NVARCHAR(100),
    @SettingValue NVARCHAR(255),
    @Description NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    IF EXISTS (SELECT 1 FROM Settings WHERE SettingKey = @SettingKey)
    BEGIN
        UPDATE Settings
        SET SettingValue = @SettingValue,
            Description = ISNULL(@Description, Description),
            UpdatedDate = GETDATE()
        WHERE SettingKey = @SettingKey;
    END
    ELSE
    BEGIN
        INSERT INTO Settings (SettingKey, SettingValue, Description)
        VALUES (@SettingKey, @SettingValue, @Description);
    END
END;
GO

PRINT 'Các Stored Procedures bổ sung đã được tạo thành công!';
GO

