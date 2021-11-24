using AIDMusicApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace AIDMusicApp.Sql.Adapters
{
    public class ContractsAdapter : BaseAdapter
    {
        [SqlCommandKey] private const string SQL_INSERT = "SQL_Insert";
        [SqlCommandKey] private const string SQL_DELETE = "SQL_Delete";
        [SqlCommandKey] private const string SQL_SELECT_LABELSBYGROUPID = "SQL_Select_LabelsByGroupId";
        [SqlCommandKey] private const string SQL_SELECT_ID_BYGROUPIDANDLABELID = "SQL_Select_Id_ByGroupIdAndLabelId";

        public ContractsAdapter(SqlConnection connection) : base(connection, "SQLCommands\\SQLContracts.aid") { }

        public int Insert(int groupId, int labelId)
        {
            using (var command = new SqlCommand(_sqlComands[SQL_INSERT], _sqlConnection))
            {
                command.Parameters.AddWithValue("@group_id", groupId);
                command.Parameters.AddWithValue("@label_id", labelId);

                return Convert.ToInt32(command.ExecuteScalar());
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

        public void Update(List<Label> groupLabels, IEnumerable<Label> destination, int groupId)
        {
            foreach (var label in destination)
            {
                var index = groupLabels.FindIndex((s) => s.Id == label.Id);

                if (index != -1)
                    groupLabels.RemoveAt(index);
                else
                    Insert(groupId, label.Id);
            }

            foreach (var label in groupLabels)
            {
                var id = GetIdByGroupIdAndLabelId(groupId, label.Id);
                Delete(id);
            }
        }

        public IEnumerable<Label> GetLabelsByGroupId(int groupId)
        {
            using (var command = new SqlCommand(_sqlComands[SQL_SELECT_LABELSBYGROUPID], _sqlConnection))
            {
                command.Parameters.AddWithValue("@id", groupId);

                using (var adapter = new SqlDataAdapter(command))
                {
                    var ds = new DataSet();
                    adapter.Fill(ds);

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        yield return new Label
                        {
                            Id = Convert.ToInt32(row[0]),
                            Name = Convert.ToString(row[1])
                        };
                    }
                }
            }
        }

        public int GetIdByGroupIdAndLabelId(int groupId, int labelId)
        {
            using (var command = new SqlCommand(_sqlComands[SQL_SELECT_ID_BYGROUPIDANDLABELID], _sqlConnection))
            {
                command.Parameters.AddWithValue("@group_id", groupId);
                command.Parameters.AddWithValue("@label_id", labelId);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }
    }
}
