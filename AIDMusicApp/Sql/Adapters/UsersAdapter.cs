using AIDMusicApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace AIDMusicApp.Sql.Adapters
{
    public class UsersAdapter : BaseAdapter
    {
        [SqlCommandKey] private const string SQL_SELECT = "SQL_Select";
        [SqlCommandKey] private const string SQL_SELECT_BYLOGIN = "SQL_Select_ByLogin";
        [SqlCommandKey] private const string SQL_INSERT = "SQL_Insert";
        [SqlCommandKey] private const string SQL_UPDATE = "SQL_Update";
        [SqlCommandKey] private const string SQL_DELETE = "SQL_Delete";
        [SqlCommandKey] private const string SQL_CHECK_LOGIN = "SQL_Check_Login";
        [SqlCommandKey] private const string SQL_CHECK_PHONE = "SQL_Check_Phone";
        [SqlCommandKey] private const string SQL_CHECK_EMAIL = "SQL_Check_Email";
        [SqlCommandKey] private const string SQL_SELECT_COUNT = "SQL_Select_Count";

        public UsersAdapter(SqlConnection connection) : base(connection, "SQLUsers.aid") { }

        public IEnumerable<User> GetAll()
        {
            using (var adapter = new SqlDataAdapter(_sqlComands[SQL_SELECT], _sqlConnection))
            {
                var ds = new DataSet();
                adapter.Fill(ds);

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    yield return new User
                    {
                        Id = Convert.ToInt32(row[0]),
                        Login = Convert.ToString(row[1]),
                        Password = Convert.ToString(row[2]),
                        Phone = Convert.ToString(row[3]),
                        Email = Convert.ToString(row[4]),
                        AccessId = SqlDatabase.Instance.AccessAdapter.GetById(Convert.ToInt32(row[5])),
                        Avatar = (byte[])row[6]
                    };
                }
            }
        }

        public User GetByLogin(string login)
        {
            using (var command = new SqlCommand(_sqlComands[SQL_SELECT_BYLOGIN], _sqlConnection))
            {
                command.Parameters.AddWithValue("@login", login);

                using (var adapter = new SqlDataAdapter(command))
                {
                    var ds = new DataSet();
                    adapter.Fill(ds);

                    var row = ds.Tables[0].Rows[0];

                    return new User
                    {
                        Id = Convert.ToInt32(row[0]),
                        Login = Convert.ToString(row[1]),
                        Password = Convert.ToString(row[2]),
                        Phone = Convert.ToString(row[3]),
                        Email = Convert.ToString(row[4]),
                        AccessId = SqlDatabase.Instance.AccessAdapter.GetById(Convert.ToInt32(row[5])),
                        Avatar = (byte[])row[6]
                    };
                }
            }
        }

        public User Insert(string login, string password, string phone, string email, int accessId, byte[] avatar)
        {
            using (var command = new SqlCommand(_sqlComands[SQL_INSERT], _sqlConnection))
            {
                command.Parameters.AddWithValue("@login", login);
                command.Parameters.AddWithValue("@password", password);
                command.Parameters.AddWithValue("@phone", !string.IsNullOrWhiteSpace(phone) ? phone : SqlString.Null);
                command.Parameters.AddWithValue("@email", !string.IsNullOrWhiteSpace(email) ? email : SqlString.Null);
                command.Parameters.AddWithValue("@access_id", accessId);
                command.Parameters.AddWithValue("@avatar", avatar);

                return new User
                {
                    Id = Convert.ToInt32(command.ExecuteScalar()),
                    Login = login,
                    Password = password,
                    Phone = !string.IsNullOrWhiteSpace(phone) ? phone : "",
                    Email = !string.IsNullOrWhiteSpace(email) ? email : "",
                    AccessId = SqlDatabase.Instance.AccessAdapter.GetById(accessId),
                    Avatar = avatar
                };
            }
        }

        public void Update(int id, string login, string password, string phone, string email, int access_id, byte[] avatar)
        {
            using (var command = new SqlCommand(_sqlComands[SQL_UPDATE], _sqlConnection))
            {
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@login", login);
                command.Parameters.AddWithValue("@password", password);
                command.Parameters.AddWithValue("@phone", !string.IsNullOrWhiteSpace(phone) ? phone : SqlString.Null);
                command.Parameters.AddWithValue("@email", !string.IsNullOrWhiteSpace(email) ? email : SqlString.Null);
                command.Parameters.AddWithValue("@access_id", access_id);
                command.Parameters.AddWithValue("@avatar", avatar);

                command.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            using (var command = new SqlCommand(_sqlComands[SQL_DELETE], _sqlConnection))
            {
                command.Parameters.AddWithValue("@id", id);

                command.ExecuteNonQuery();
            }
        }

        public bool ContainsLogin(string login)
        {
            using (var command = new SqlCommand(_sqlComands[SQL_CHECK_LOGIN], _sqlConnection))
            {
                command.Parameters.AddWithValue("@login", login);

                var count = Convert.ToInt32(command.ExecuteScalar());
                return count != 0;
            }
        }

        public bool ContainsPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return false;

            using (var command = new SqlCommand(_sqlComands[SQL_CHECK_PHONE], _sqlConnection))
            {
                command.Parameters.AddWithValue("@phone", phone);

                var count = Convert.ToInt32(command.ExecuteScalar());
                return count != 0;
            }
        }

        public bool ContainsEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            using (var command = new SqlCommand(_sqlComands[SQL_CHECK_EMAIL], _sqlConnection))
            {
                command.Parameters.AddWithValue("@email", email);

                var count = Convert.ToInt32(command.ExecuteScalar());
                return count != 0;
            }
        }

        public int GetCount()
        {
            using (var command = new SqlCommand(_sqlComands[SQL_SELECT_COUNT], _sqlConnection))
            {
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }
    }
}
