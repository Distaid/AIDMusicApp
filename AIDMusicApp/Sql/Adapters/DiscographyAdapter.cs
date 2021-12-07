using AIDMusicApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;

namespace AIDMusicApp.Sql.Adapters
{
    public class DiscographyAdapter : BaseAdapter
    {
        [SqlCommandKey] private const string SQL_INSERT = "SQL_Insert";
        [SqlCommandKey] private const string SQL_DELETE = "SQL_Delete";
        [SqlCommandKey] private const string SQL_SELECT_ALBUMSBYGROUPID = "SQL_Select_AlbumsByGroupId";
        [SqlCommandKey] private const string SQL_SELECT_ID_BYALBUMIDANDGROUPID = "SQL_Select_Id_ByAlbumIdAndGroupId";
        [SqlCommandKey] private const string SQL_SELECT_SHORT_ALBUMSBYGROUPID = "SQL_Select_Short_AlbumsByGroupId";
        [SqlCommandKey] private const string SQL_SELECT_GROUPNAMEBYALBUMID = "SQL_Select_GroupNameByAlbumId";
        [SqlCommandKey] private const string SQL_SELECT_ALBUMSBYGROUPNAME = "SQL_Select_AlbumsByGroupName";
        [SqlCommandKey] private const string SQL_SELECT_COUNT = "SQL_Select_Count";

        public DiscographyAdapter(SqlConnection connection) : base(connection, "SQLDiscography.aid") { }

        public int Insert(int albumId, int groupId)
        {
            using (var command = new SqlCommand(_sqlComands[SQL_INSERT], _sqlConnection))
            {
                command.Parameters.AddWithValue("@album_id", albumId);
                command.Parameters.AddWithValue("@group_id", groupId);

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

        public IEnumerable<Album> GetAlbumsByGroupId(int groupId)
        {
            using (var command = new SqlCommand(_sqlComands[SQL_SELECT_ALBUMSBYGROUPID], _sqlConnection))
            {
                command.Parameters.AddWithValue("@id", groupId);

                using (var adapter = new SqlDataAdapter(command))
                {
                    var ds = new DataSet();
                    adapter.Fill(ds);

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        var album = new Album
                        {
                            Id = Convert.ToInt32(row[0]),
                            Name = Convert.ToString(row[1]),
                            Description = Convert.ToString(row[2]),
                            Year = Convert.ToInt16(row[3]),
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
        }

        public int GetIdByAlbumIdAndGroupId(int albumId, int groupId)
        {
            using (var command = new SqlCommand(_sqlComands[SQL_SELECT_ID_BYALBUMIDANDGROUPID], _sqlConnection))
            {
                command.Parameters.AddWithValue("@album_id", albumId);
                command.Parameters.AddWithValue("@group_id", groupId);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public IEnumerable<Album> GetShortAlbumsByGroupId(int groupId)
        {
            using (var command = new SqlCommand(_sqlComands[SQL_SELECT_SHORT_ALBUMSBYGROUPID], _sqlConnection))
            {
                command.Parameters.AddWithValue("@id", groupId);

                using (var adapter = new SqlDataAdapter(command))
                {
                    var ds = new DataSet();
                    adapter.Fill(ds);

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        yield return new Album
                        {
                            Id = Convert.ToInt32(row[0]),
                            Name = Convert.ToString(row[1]),
                            Description = Convert.ToString(row[2]),
                            Year = Convert.ToInt16(row[3]),
                            Cover = (byte[])row[4]
                        };
                    }
                }
            }
        }

        public string GetGroupNameByAlbumId(int albumId)
        {
            using (var command = new SqlCommand(_sqlComands[SQL_SELECT_GROUPNAMEBYALBUMID], _sqlConnection))
            {
                command.Parameters.AddWithValue("@id", albumId);

                using (var adapter = new SqlDataAdapter(command))
                {
                    var ds = new DataSet();
                    adapter.Fill(ds);

                    var row = ds.Tables[0].Rows[0];

                    return Convert.ToString(row[0]);
                }
            }
        }

        public IEnumerable<Tuple<Album, string>> GetAlbumsByGroupName(string name)
        {
            using (var command = new SqlCommand(_sqlComands[SQL_SELECT_ALBUMSBYGROUPNAME], _sqlConnection))
            {
                command.Parameters.AddWithValue("@name", $"%{name}%");

                using (var adapter = new SqlDataAdapter(command))
                {
                    var ds = new DataSet();
                    adapter.Fill(ds);

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        var album = new Album
                        {
                            Id = Convert.ToInt32(row[0]),
                            Name = Convert.ToString(row[1]),
                            Description = Convert.ToString(row[2]),
                            Year = Convert.ToInt16(row[3]),
                            Cover = (byte[])row[4]
                        };

                        yield return new Tuple<Album, string>(album, Convert.ToString(row[5]));
                    }
                }
            }
        }

        public int GetCount(int groupId)
        {
            using (var command = new SqlCommand(_sqlComands[SQL_SELECT_COUNT], _sqlConnection))
            {
                command.Parameters.AddWithValue("@id", groupId);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }
    }
}
