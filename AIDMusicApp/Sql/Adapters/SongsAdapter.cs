using AIDMusicApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;

namespace AIDMusicApp.Sql.Adapters
{
    public class SongsAdapter : BaseAdapter
    {
        [SqlCommandKey] private const string SQL_SELECT = "SQL_Select";
        [SqlCommandKey] private const string SQL_INSERT = "SQL_Insert";
        [SqlCommandKey] private const string SQL_UPDATE = "SQL_Update";
        [SqlCommandKey] private const string SQL_DELETE = "SQL_Delete";

        public SongsAdapter(SqlConnection connection) : base(connection, "SQLSongs.aid") { }

        public IEnumerable<Song> GetAll()
        {
            using (var adapter = new SqlDataAdapter(_sqlComands[SQL_SELECT], _sqlConnection))
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

        public Song Insert(string name, string time)
        {
            using (var command = new SqlCommand(_sqlComands[SQL_INSERT], _sqlConnection))
            {
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@time", time);

                return new Song
                {
                    Id = Convert.ToInt32(command.ExecuteScalar()),
                    Name = name,
                    Time = time,
                    Guests = new ObservableCollection<Musician>()
                };
            }
        }

        public void Update(int id, string name, string time)
        {
            using (var command = new SqlCommand(_sqlComands[SQL_UPDATE], _sqlConnection))
            {
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@time", time);

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
