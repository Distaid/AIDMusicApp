using AIDMusicApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace AIDMusicApp.Sql.Adapters
{
    public class AlbumsAdapter : BaseAdapter
    {
        [SqlCommandKey] private const string SQL_SELECT = "SQL_Select";
        [SqlCommandKey] private const string SQL_INSERT = "SQL_Insert";
        [SqlCommandKey] private const string SQL_UPDATE = "SQL_Update";
        [SqlCommandKey] private const string SQL_DELETE = "SQL_Delete";

        public AlbumsAdapter(SqlConnection connection) : base(connection, "SQLAlbums.aid") { }

        public IEnumerable<Album> GetAll()
        {
            using (var adapter = new SqlDataAdapter(_sqlComands[SQL_SELECT], _sqlConnection))
            {
                var ds = new DataSet();
                adapter.Fill(ds);

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    var album = new Album
                    {
                        Id = Convert.ToInt32(row[0]),
                        Name = Convert.ToString(row[1]),
                        Year = Convert.ToInt16(row[2]),
                        Description = Convert.ToString(row[3]),
                        Cover = (byte[])row[4],
                        Genres = new ObservableCollection<Genre>(),
                        Formats = new ObservableCollection<Format>(),
                        Songs = new ObservableCollection<Song>()
                    };

                    foreach (var genre in SqlDatabase.Instance.AlbumGenresAdapter.GetGenresByGroupId(album.Id))
                        album.Genres.Add(genre);

                    foreach (var format in SqlDatabase.Instance.AlbumFormatsAdapter.GetFormatsByGroupId(album.Id))
                        album.Formats.Add(format);

                    foreach (var song in SqlDatabase.Instance.TrackListAdapter.GetSongsByAlbumId(album.Id))
                        album.Songs.Add(song);

                    yield return album;
                }
            }
        }

        public Album Insert(string name, short year, string description, byte[] cover)
        {
            using (var command = new SqlCommand(_sqlComands[SQL_INSERT], _sqlConnection))
            {
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@year", year);
                command.Parameters.AddWithValue("@description", !string.IsNullOrWhiteSpace(description) ? description : SqlString.Null);
                command.Parameters.AddWithValue("@cover", cover);

                return new Album
                {
                    Id = Convert.ToInt32(command.ExecuteScalar()),
                    Name = name,
                    Year = year,
                    Description = !string.IsNullOrWhiteSpace(description) ? description : "",
                    Cover = cover,
                    Genres = new ObservableCollection<Genre>(),
                    Formats = new ObservableCollection<Format>(),
                    Songs = new ObservableCollection<Song>()
                };
            }
        }

        public void Update(int id, string name, short year, string description, byte[] cover)
        {
            using (var command = new SqlCommand(_sqlComands[SQL_UPDATE], _sqlConnection))
            {
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@year", year);
                command.Parameters.AddWithValue("@description", !string.IsNullOrWhiteSpace(description) ? description : SqlString.Null);
                command.Parameters.AddWithValue("@cover", cover);

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
    }
}
