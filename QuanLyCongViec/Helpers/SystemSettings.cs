using System;
using System.Data;
using System.Collections.Generic;
using QuanLyCongViec.DataAccess;
using System.Data.SqlClient;

namespace QuanLyCongViec.Helpers
{
    /// <summary>
    /// Class lấy các cấu hình hệ thống từ database
    /// Không cần hardcode constants, tự động lấy từ database
    /// </summary>
    public static class SystemSettings
    {
        // Cache để tránh query database nhiều lần
        private static Dictionary<string, string> _cache = new Dictionary<string, string>();
        private static bool _isLoaded = false;

        /// <summary>
        /// Lấy số lần đăng nhập sai tối đa cho phép
        /// </summary>
        public static int MaxLoginAttempts
        {
            get { return GetIntSetting("MAX_LOGIN_ATTEMPTS", 5); }
        }

        /// <summary>
        /// Lấy thời gian khóa tài khoản (tính bằng phút)
        /// </summary>
        public static int LockoutMinutes
        {
            get { return GetIntSetting("LOCKOUT_MINUTES", 15); }
        }

        /// <summary>
        /// Lấy mã lỗi: Username đã tồn tại
        /// </summary>
        public static int ErrorUsernameExists
        {
            get { return GetIntSetting("ERROR_USERNAME_EXISTS", -1); }
        }

        /// <summary>
        /// Lấy mã lỗi: Email đã tồn tại
        /// </summary>
        public static int ErrorEmailExists
        {
            get { return GetIntSetting("ERROR_EMAIL_EXISTS", -2); }
        }

        /// <summary>
        /// Lấy giá trị cấu hình dạng số nguyên từ database
        /// </summary>
        private static int GetIntSetting(string settingKey, int defaultValue)
        {
            string value = GetSetting(settingKey);
            if (!string.IsNullOrEmpty(value) && int.TryParse(value, out int result))
            {
                return result;
            }
            return defaultValue; // Fallback nếu không lấy được
        }

        /// <summary>
        /// Lấy giá trị cấu hình từ cache hoặc database
        /// </summary>
        private static string GetSetting(string settingKey)
        {
            // Load từ database lần đầu tiên
            if (!_isLoaded)
            {
                LoadSettingsFromDatabase();
            }

            // Lấy từ cache
            if (_cache.ContainsKey(settingKey))
            {
                return _cache[settingKey];
            }

            // Nếu không có trong cache, trả về null
            return null;
        }

        /// <summary>
        /// Load tất cả các cấu hình từ database thông qua stored procedure
        /// </summary>
        private static void LoadSettingsFromDatabase()
        {
            try
            {
                // Gọi stored procedure để lấy tất cả settings
                DataTable bangDuLieu = DatabaseHelper.ExecuteStoredProcedure("sp_GetAllSystemSettings");

                // Lưu vào cache
                foreach (DataRow dong in bangDuLieu.Rows)
                {
                    string key = dong["SettingKey"].ToString();
                    string value = dong["SettingValue"].ToString();
                    _cache[key] = value;
                }

                _isLoaded = true;
            }
            catch
            {
                // Nếu lỗi, sử dụng giá trị mặc định
                // Không throw exception để không làm gián đoạn ứng dụng
                _isLoaded = true; // Đánh dấu đã load (dù thất bại) để không retry liên tục
            }
        }

        /// <summary>
        /// Lấy giá trị cấu hình cụ thể từ database (không cache)
        /// </summary>
        public static string GetSettingValue(string settingKey)
        {
            try
            {
                SqlParameter[] thamSo = new SqlParameter[]
                {
                    new SqlParameter("@SettingKey", settingKey)
                };

                DataTable bangDuLieu = DatabaseHelper.ExecuteStoredProcedure("sp_GetSystemSetting", thamSo);

                if (bangDuLieu.Rows.Count > 0)
                {
                    return bangDuLieu.Rows[0]["SettingValue"].ToString();
                }
            }
            catch
            {
                // Bỏ qua lỗi
            }

            return null;
        }

        /// <summary>
        /// Reset cache để load lại từ database (dùng khi cần refresh)
        /// </summary>
        public static void Reset()
        {
            _cache.Clear();
            _isLoaded = false;
        }
    }
}

