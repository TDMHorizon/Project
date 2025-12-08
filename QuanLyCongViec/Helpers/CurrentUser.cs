using System;

namespace QuanLyCongViec.Helpers
{
    /// <summary>
    /// Class lưu trữ thông tin user hiện tại đang đăng nhập
    /// Giúp truyền userId giữa các form
    /// </summary>
    public static class CurrentUser
    {
        private static int _userId = 0;
        private static string _username = string.Empty;
        private static string _fullName = string.Empty;

        /// <summary>
        /// Lưu thông tin user hiện tại
        /// </summary>
        /// <param name="userId">ID của user</param>
        /// <param name="username">Tên đăng nhập</param>
        /// <param name="fullName">Họ tên đầy đủ</param>
        public static void SetCurrentUser(int userId, string username, string fullName)
        {
            _userId = userId;
            _username = username;
            _fullName = fullName;
        }

        /// <summary>
        /// Lấy UserId của user hiện tại
        /// </summary>
        /// <returns>UserId, trả về 0 nếu chưa đăng nhập</returns>
        public static int GetUserId()
        {
            return _userId;
        }

        /// <summary>
        /// Lấy Username của user hiện tại
        /// </summary>
        /// <returns>Username, trả về chuỗi rỗng nếu chưa đăng nhập</returns>
        public static string GetUsername()
        {
            return _username;
        }

        /// <summary>
        /// Lấy FullName của user hiện tại
        /// </summary>
        /// <returns>FullName, trả về chuỗi rỗng nếu chưa đăng nhập</returns>
        public static string GetFullName()
        {
            return _fullName;
        }

        /// <summary>
        /// Kiểm tra đã đăng nhập chưa
        /// </summary>
        /// <returns>True nếu đã đăng nhập (userId > 0), False nếu chưa</returns>
        public static bool IsLoggedIn()
        {
            return _userId > 0;
        }

        /// <summary>
        /// Xóa thông tin user (khi đăng xuất)
        /// </summary>
        public static void Clear()
        {
            _userId = 0;
            _username = string.Empty;
            _fullName = string.Empty;
        }
    }
}

