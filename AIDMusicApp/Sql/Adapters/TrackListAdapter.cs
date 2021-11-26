using AIDMusicApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;

namespace AIDMusicApp.Sql.Adapters
{
    public class TrackListAdapter : BaseAdapter
    {
        [SqlCommandKey] private const string SQL_INSERT = "SQL_Insert";
        [SqlCommandKey] private const string SQL_DELETE = "SQL_Delete";
        [SqlCommandKey] private const string SQL_SELECT_SONGSBYALBUMID = "SQL_Select_SongsByAlbumId";
        [SqlCommandKey] private const string SQL_SELECT_ID_BYSONGIDANDALBUMID = "SQL_Select_Id_BySongIdAndAlbumId";

        public TrackListAdapter(SqlConnection connection) : base(connection, "SQLTrackLists.aid") { }

        public int Insert(int songId, int albumId)
        {
            using (var command = new SqlCommand(_sqlComands[SQL_INSERT], _sqlConnection))
            {
                command.Parameters.AddWithValue("@song_id", songId);
                command.Parameters.AddWithValue("@album_id", albumId);

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

        public IEnumerable<Song> GetSongsByAlbumId(int albumId)
        {
            using (var command = new SqlCommand(_sqlComands[SQL_SELECT_SONGSBYALBUMID], _sqlConnection))
            {
                command.Parameters.AddWithValue("@id", albumId);

                using (var adapter = new SqlDataAdapter(command))
                {
                    var ds = new DataSet();
                    adapter.Fill(ds);

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        var song = new Song
                        {
                            Id = Convert.ToInt32(row[0]),
                            Name = Convert.ToString(row[1]),
                            Time = Convert.ToString(row[2]),
                            Guests = new ObservableCollection<Musician>()
                        };

                        foreach (var musician in SqlDatabase.Instance.FeatsAdapter.GetMusiciansBySongId(song.Id))
                            song.Guests.Add(musician);

                        yield return song;
                    }
                }
            }
        }

        public int GetIdByAlbumIdAndGroupId(int songId, int albumId)
        {
            using (var command = new SqlCommand(_sqlComands[SQL_SELECT_ID_BYSONGIDANDALBUMID], _sqlConnection))
            {
                command.Parameters.AddWithValue("@song_id", songId);
                command.Parameters.AddWithValue("@album_id", albumId);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }
    }
}
