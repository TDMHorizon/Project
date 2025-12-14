using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace QuanLyCongViec.DataAccess
{
    /// <summary>
    /// Helper class để kết nối và làm việc với Database
    /// Tuân thủ Clean Code principles
    /// </summary>
    public class DatabaseHelper
    {
        private static string _connectionString;

        /// <summary>
        /// Lấy connection string từ App.config
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                {
                    _connectionString = ConfigurationManager.ConnectionStrings["QuanLyCongViecConnection"]?.ConnectionString;
                    
                    if (string.IsNullOrEmpty(_connectionString))
                    {
                        throw new Exception("Không tìm thấy connection string 'QuanLyCongViecConnection' trong App.config");
                    }
                }
                
                return _connectionString;
            }
        }

        /// <summary>
        /// Kiểm tra kết nối database
        /// </summary>
        /// <returns>True nếu kết nối thành công</returns>
        public static bool TestConnection()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi kết nối database: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Tạo SqlConnection mới
        /// </summary>
        /// <returns>SqlConnection instance</returns>
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        /// <summary>
        /// Thực thi query không trả về kết quả (INSERT, UPDATE, DELETE)
        /// </summary>
        /// <param name="query">SQL query</param>
        /// <param name="parameters">SqlParameters</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        public static int ExecuteNonQuery(string query, params SqlParameter[] parameters)
        {
            using (SqlConnection connection = GetConnection())
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    if (parameters != null && parameters.Length > 0)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    
                    return command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Thực thi query trả về một giá trị (Scalar)
        /// </summary>
        /// <param name="query">SQL query</param>
        /// <param name="parameters">SqlParameters</param>
        /// <returns>Giá trị trả về</returns>
        public static object ExecuteScalar(string query, params SqlParameter[] parameters)
        {
            using (SqlConnection connection = GetConnection())
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    if (parameters != null && parameters.Length > 0)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    
                    return command.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// Thực thi query trả về DataTable
        /// </summary>
        /// <param name="query">SQL query</param>
        /// <param name="parameters">SqlParameters</param>
        /// <returns>DataTable</returns>
        public static DataTable ExecuteQuery(string query, params SqlParameter[] parameters)
        {
            using (SqlConnection connection = GetConnection())
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    if (parameters != null && parameters.Length > 0)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
        }

        /// <summary>
        /// Thực thi Stored Procedure trả về DataTable
        /// </summary>
        /// <param name="procedureName">Tên stored procedure</param>
        /// <param name="parameters">SqlParameters</param>
        /// <returns>DataTable</returns>
        public static DataTable ExecuteStoredProcedure(string procedureName, params SqlParameter[] parameters)
        {
            using (SqlConnection connection = GetConnection())
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(procedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    
                    if (parameters != null && parameters.Length > 0)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
        }

        /// <summary>
        /// Thực thi Stored Procedure không trả về kết quả
        /// </summary>
        /// <param name="procedureName">Tên stored procedure</param>
        /// <param name="parameters">SqlParameters</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        public static int ExecuteStoredProcedureNonQuery(string procedureName, params SqlParameter[] parameters)
        {
            using (SqlConnection connection = GetConnection())
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(procedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    
                    if (parameters != null && parameters.Length > 0)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    
                    return command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Thực thi Stored Procedure trả về một giá trị vô hướng (scalar value).
        /// </summary>
        /// <param name="storedProcedureName">Tên stored procedure.</param>
        /// <param name="parameters">Mảng các SqlParameters.</param>
        /// <returns>Giá trị Integer được trả về bởi stored procedure (ví dụ: mã lỗi, mã thành công).</returns>
        public static int ExecuteScalarStoredProcedure(string storedProcedureName, SqlParameter[] parameters = null)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(storedProcedureName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                conn.Open();
                object result = cmd.ExecuteScalar();
                // Trả về 0 nếu kết quả null hoặc không phải số, nếu không thì trả về giá trị số
                return (result != null && result != DBNull.Value) ? Convert.ToInt32(result) : 0;
            }
        }
    }
}

