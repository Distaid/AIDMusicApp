using AIDMusicApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;

namespace AIDMusicApp.Sql.Adapters
{
    public class FeatsAdapter : BaseAdapter
    {
        [SqlCommandKey] private const string SQL_INSERT = "SQL_Insert";
        [SqlCommandKey] private const string SQL_DELETE = "SQL_Delete";
        [SqlCommandKey] private const string SQL_SELECT_MUSICIANSBYSONGID = "SQL_Select_MusiciansBySongId";
        [SqlCommandKey] private const string SQL_SELECT_ID_BYSONGIDANDMUSICIANID = "SQL_Select_Id_BySongIdAndMusicianId";

        public FeatsAdapter(SqlConnection connection) : base(connection, "SQLFeats.aid") { }

        public int Insert(int songId, int musicianId)
        {
            using (var command = new SqlCommand(_sqlComands[SQL_INSERT], _sqlConnection))
            {
                command.Parameters.AddWithValue("@song_id", songId);
                command.Parameters.AddWithValue("@musician_id", musicianId);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public void Update(List<Musician> songGuests, IEnumerable<Musician> destination, int songId)
        {
            foreach (var musician in destination)
            {
                var index = songGuests.FindIndex((s) => s.Id == musician.Id);

                if (index != -1)
                    songGuests.RemoveAt(index);
                else
                    Insert(songId, musician.Id);
            }

            foreach (var guest in songGuests)
            {
                var id = GetIdBySongIdAndMusicianId(songId, guest.Id);
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

        public IEnumerable<Musician> GetMusiciansBySongId(int songId)
        {
            using (var command = new SqlCommand(_sqlComands[SQL_SELECT_MUSICIANSBYSONGID], _sqlConnection))
            {
                command.Parameters.AddWithValue("@id", songId);

                using (var adapter = new SqlDataAdapter(command))
                {
                    var ds = new DataSet();
                    adapter.Fill(ds);

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        var musician = new Musician
                        {
                            Id = Convert.ToInt32(row[0]),
                            Name = Convert.ToString(row[1]),
                            Age = Convert.ToByte(row[2]),
                            CountryId = SqlDatabase.Instance.CountriesListAdapter.GetById(Convert.ToInt32(row[3])),
                            IsDead = Convert.ToBoolean(row[4]),
                            Skills = new ObservableCollection<Skill>()
                        };

                        foreach (var skill in SqlDatabase.Instance.MusicianSkillsAdapter.GetSkillsByMusicianId(musician.Id))
                            musician.Skills.Add(skill);

                        yield return musician;
                    }
                }
            }
        }

        public int GetIdBySongIdAndMusicianId(int songId, int musicianId)
        {
            using (var command = new SqlCommand(_sqlComands[SQL_SELECT_ID_BYSONGIDANDMUSICIANID], _sqlConnection))
            {
                command.Parameters.AddWithValue("@song_id", songId);
                command.Parameters.AddWithValue("@musician_id", musicianId);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }
    }
}
