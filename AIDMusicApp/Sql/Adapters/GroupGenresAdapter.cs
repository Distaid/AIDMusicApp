using AIDMusicApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace AIDMusicApp.Sql.Adapters
{
    public class GroupGenresAdapter : BaseAdapter
    {
        [SqlCommandKey] private const string SQL_INSERT = "SQL_Insert";
        [SqlCommandKey] private const string SQL_DELETE = "SQL_Delete";
        [SqlCommandKey] private const string SQL_SELECT_GENRESBYGROUPID = "SQL_Select_GenresByGroupId";
        [SqlCommandKey] private const string SQL_SELECT_ID_BYGROUPIDANDGENREID = "SQL_Select_Id_ByGroupIdAndGenreId";

        public GroupGenresAdapter(SqlConnection connection) : base(connection, "SQLCommands\\SQLGroupGenres.aid") { }

        public int Insert(int groupId, int genreId)
        {
            using (var command = new SqlCommand(_sqlComands[SQL_INSERT], _sqlConnection))
            {
                command.Parameters.AddWithValue("@group_id", groupId);
                command.Parameters.AddWithValue("@genre_id", genreId);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public void Update(List<Genre> groupGenres, IEnumerable<Genre> destination, int groupId)
        {
            foreach (var genre in destination)
            {
                var index = groupGenres.FindIndex((s) => s.Id == genre.Id);

                if (index != -1)
                    groupGenres.RemoveAt(index);
                else
                    Insert(groupId, genre.Id);
            }

            foreach (var genre in groupGenres)
            {
                var id = GetIdByGroupIdAndGenreId(groupId, genre.Id);
                Delete(id);
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

        public IEnumerable<Genre> GetGenresByGroupId(int groupId)
        {
            using (var command = new SqlCommand(_sqlComands[SQL_SELECT_GENRESBYGROUPID], _sqlConnection))
            {
                command.Parameters.AddWithValue("@id", groupId);

                using (var adapter = new SqlDataAdapter(command))
                {
                    var ds = new DataSet();
                    adapter.Fill(ds);

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        yield return new Genre
                        {
                            Id = Convert.ToInt32(row[0]),
                            Name = Convert.ToString(row[1])
                        };
                    }
                }
            }
        }

        public int GetIdByGroupIdAndGenreId(int groupId, int genreId)
        {
            using (var command = new SqlCommand(_sqlComands[SQL_SELECT_ID_BYGROUPIDANDGENREID], _sqlConnection))
            {
                command.Parameters.AddWithValue("@group_id", groupId);
                command.Parameters.AddWithValue("@genre_id", genreId);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }
    }
}
