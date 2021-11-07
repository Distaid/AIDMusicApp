﻿using AIDMusicApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace AIDMusicApp.Sql.Adapters
{
    public class GroupGenresAdapter : BaseAdapter
    {
        public GroupGenresAdapter(SqlConnection connection) : base(connection, "SQLCommands\\SQLGroupGenres.aid") { }

        public int Insert(int groupId, int genreId)
        {
            using (var command = new SqlCommand(_sqlComands["SQL_Insert"], _sqlConnection))
            {
                command.Parameters.AddWithValue("@group_id", groupId);
                command.Parameters.AddWithValue("@genre_id", genreId);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public void Delete(int id)
        {
            using (var command = new SqlCommand(_sqlComands["SQL_Delete"], _sqlConnection))
            {
                command.Parameters.AddWithValue("@id", id);

                command.ExecuteNonQuery();
            }
        }

        public IEnumerable<Genre> GetGenresByGroupId(int groupId)
        {
            using (var command = new SqlCommand(_sqlComands["SQL_Select_GenresByGroupId"], _sqlConnection))
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
            using (var command = new SqlCommand(_sqlComands["SQL_Select_Id_ByGroupIdAndGenreId"], _sqlConnection))
            {
                command.Parameters.AddWithValue("@group_id", groupId);
                command.Parameters.AddWithValue("@genre_id", genreId);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }
    }
}
