using AIDMusicApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace AIDMusicApp.Sql.Adapters
{
    public class AlbumGenresAdapter : BaseAdapter
    {
        [SqlCommandKey] private const string SQL_INSERT = "SQL_Insert";
        [SqlCommandKey] private const string SQL_DELETE = "SQL_Delete";
        [SqlCommandKey] private const string SQL_SELECT_GENRESBYALBUMID = "SQL_Select_GenresByAlbumId";
        [SqlCommandKey] private const string SQL_SELECT_ID_BYALBUMIDANDGENREID = "SQL_Select_Id_ByAlbumIdAndGenreId";

        public AlbumGenresAdapter(SqlConnection connection) : base(connection, "SQLCommands\\SQLAlbumGenres.aid") { }

        public int Insert(int albumId, int genreId)
        {
            using (var command = new SqlCommand(_sqlComands[SQL_INSERT], _sqlConnection))
            {
                command.Parameters.AddWithValue("@album_id", albumId);
                command.Parameters.AddWithValue("@genre_id", genreId);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public void Update(List<Genre> albumGenres, IEnumerable<Genre> destination, int albumId)
        {
            foreach (var genre in destination)
            {
                var index = albumGenres.FindIndex((s) => s.Id == genre.Id);

                if (index != -1)
                    albumGenres.RemoveAt(index);
                else
                    Insert(albumId, genre.Id);
            }

            foreach (var genre in albumGenres)
            {
                var id = GetIdByAlbumIdAndGenreId(albumId, genre.Id);
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

        public IEnumerable<Genre> GetGenresByGroupId(int albumId)
        {
            using (var command = new SqlCommand(_sqlComands[SQL_SELECT_GENRESBYALBUMID], _sqlConnection))
            {
                command.Parameters.AddWithValue("@id", albumId);

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

        public int GetIdByAlbumIdAndGenreId(int albumId, int genreId)
        {
            using (var command = new SqlCommand(_sqlComands[SQL_SELECT_ID_BYALBUMIDANDGENREID], _sqlConnection))
            {
                command.Parameters.AddWithValue("@album_id", albumId);
                command.Parameters.AddWithValue("@genre_id", genreId);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }
    }
}
