using AIDMusicApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace AIDMusicApp.Sql.Adapters
{
    public class FormatsListAdapter : BaseAdapter
    {
        [SqlCommandKey] private const string SQL_SELECT = "SQL_Select";
        [SqlCommandKey] private const string SQL_INSERT = "SQL_Insert";
        [SqlCommandKey] private const string SQL_UPDATE = "SQL_Update";
        [SqlCommandKey] private const string SQL_DELETE = "SQL_Delete";
        [SqlCommandKey] private const string SQL_CHECK_NAME = "SQL_Check_Name";
        [SqlCommandKey] private const string SQL_SELECT_COUNT = "SQL_Select_Count";

        public FormatsListAdapter(SqlConnection connection) : base(connection, "SQLFormatsList.aid") { }

        public IEnumerable<Format> GetAll()
        {
            using (var adapter = new SqlDataAdapter(_sqlComands[SQL_SELECT], _sqlConnection))
            {
                var ds = new DataSet();
                adapter.Fill(ds);

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    yield return new Format
                    {
                        Id = Convert.ToInt32(row[0]),
                        Name = Convert.ToString(row[1])
                    };
                }
            }
        }

        public Format Insert(string name)
        {
            using (var command = new SqlCommand(_sqlComands[SQL_INSERT], _sqlConnection))
            {
                command.Parameters.AddWithValue("@name", name);

                return new Format
                {
                    Id = Convert.ToInt32(command.ExecuteScalar()),
                    Name = name
                };
            }
        }

        public void Update(int id, string name)
        {
            using (var command = new SqlCommand(_sqlComands[SQL_UPDATE], _sqlConnection))
            {
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@name", name);

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

        public bool ContainsName(string name)
        {
            using (var command = new SqlCommand(_sqlComands[SQL_CHECK_NAME], _sqlConnection))
            {
                command.Parameters.AddWithValue("@name", name);

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
