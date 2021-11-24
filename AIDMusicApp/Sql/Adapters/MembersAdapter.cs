using AIDMusicApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;

namespace AIDMusicApp.Sql.Adapters
{
    public class MembersAdapter : BaseAdapter
    {
        [SqlCommandKey] private const string SQL_INSERT = "SQL_Insert";
        [SqlCommandKey] private const string SQL_DELETE = "SQL_Delete";
        [SqlCommandKey] private const string SQL_UPDATE = "SQL_Update";
        [SqlCommandKey] private const string SQL_SELECT_MUSICIANSBYGROUPID = "SQL_Select_MusiciansByGroupId";
        [SqlCommandKey] private const string SQL_SELECT_ID_BYMUSICIANIDANDGROUPID = "SQL_Select_Id_ByMusicianIdAndGroupId";

        public MembersAdapter(SqlConnection connection) : base(connection, "SQLCommands\\SQLMembers.aid") { }

        public int Insert(int musicianId, int groupId, bool isFormer)
        {
            using (var command = new SqlCommand(_sqlComands[SQL_INSERT], _sqlConnection))
            {
                command.Parameters.AddWithValue("@musician_id", musicianId);
                command.Parameters.AddWithValue("@group_id", groupId);
                command.Parameters.AddWithValue("@is_former", isFormer);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public void Update(List<Musician> groupMembers, IEnumerable<Musician> destination, int groupId)
        {
            foreach (var musician in destination)
            {
                var index = groupMembers.FindIndex((s) => s.Id == musician.Id);

                if (index != -1)
                {
                    Update(musician.Id, groupId, musician.IsFormer);
                    groupMembers.RemoveAt(index);
                }
                else
                    Insert(musician.Id, groupId, musician.IsFormer);
            }

            foreach (var member in groupMembers)
            {
                var id = GetIdByMusicianIdAndGroupId(member.Id, groupId);
                Delete(id);
            }
        }

        private void Update(int musicianId, int groupId, bool isFormer)
        {
            using (var command = new SqlCommand(_sqlComands[SQL_UPDATE], _sqlConnection))
            {
                command.Parameters.AddWithValue("@musician_id", musicianId);
                command.Parameters.AddWithValue("@group_id", groupId);
                command.Parameters.AddWithValue("@is_former", isFormer);

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

        public IEnumerable<Musician> GetMusiciansByGroupId(int groupId)
        {
            using (var command = new SqlCommand(_sqlComands[SQL_SELECT_MUSICIANSBYGROUPID], _sqlConnection))
            {
                command.Parameters.AddWithValue("@id", groupId);

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
                            IsFormer = Convert.ToBoolean(row[5]),
                            Skills = new ObservableCollection<Skill>()
                        };

                        foreach (var skill in SqlDatabase.Instance.MusicianSkillsAdapter.GetSkillsByMusicianId(musician.Id))
                            musician.Skills.Add(skill);

                        yield return musician;
                    }
                }
            }
        }

        public int GetIdByMusicianIdAndGroupId(int musicianId, int groupId)
        {
            using (var command = new SqlCommand(_sqlComands[SQL_SELECT_ID_BYMUSICIANIDANDGROUPID], _sqlConnection))
            {
                command.Parameters.AddWithValue("@musician_id", musicianId);
                command.Parameters.AddWithValue("@group_id", groupId);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }
    }
}
