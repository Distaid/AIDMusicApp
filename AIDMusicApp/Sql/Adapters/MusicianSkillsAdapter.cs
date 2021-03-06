using AIDMusicApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace AIDMusicApp.Sql.Adapters
{
    public class MusicianSkillsAdapter : BaseAdapter
    {
        [SqlCommandKey] private const string SQL_INSERT = "SQL_Insert";
        [SqlCommandKey] private const string SQL_DELETE = "SQL_Delete";
        [SqlCommandKey] private const string SQL_SELECT_SKILLSBYMUSICIANID = "SQL_Select_SkillsByMusicianId";
        [SqlCommandKey] private const string SQL_SELECT_ID_BYMUSICIANIDANDSKILLID = "SQL_Select_Id_ByMusicianIdAndSkillId";

        public MusicianSkillsAdapter(SqlConnection connection) : base(connection, "SQLMusicianSkills.aid") { }

        public int Insert(int musicianId, int skillId)
        {
            using (var command = new SqlCommand(_sqlComands[SQL_INSERT], _sqlConnection))
            {
                command.Parameters.AddWithValue("@musician_id", musicianId);
                command.Parameters.AddWithValue("@skill_id", skillId);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public void Update(List<Skill> musicianSkills, IEnumerable<Skill> destination, int musicianId)
        {
            foreach (var skill in destination)
            {
                var index = musicianSkills.FindIndex((s) => s.Id == skill.Id);

                if (index != -1)
                    musicianSkills.RemoveAt(index);
                else
                    Insert(musicianId, skill.Id);
            }

            foreach (var skill in musicianSkills)
            {
                var id = GetIdByMusicianIdAndSkillId(musicianId, skill.Id);
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

        public IEnumerable<Skill> GetSkillsByMusicianId(int musicianId)
        {
            using (var command = new SqlCommand(_sqlComands[SQL_SELECT_SKILLSBYMUSICIANID], _sqlConnection))
            {
                command.Parameters.AddWithValue("@id", musicianId);

                using (var adapter = new SqlDataAdapter(command))
                {
                    var ds = new DataSet();
                    adapter.Fill(ds);

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        yield return new Skill
                        {
                            Id = Convert.ToInt32(row[0]),
                            Name = Convert.ToString(row[1])
                        };
                    }
                }
            }
        }

        public int GetIdByMusicianIdAndSkillId(int musicianId, int skillId)
        {
            using (var command = new SqlCommand(_sqlComands[SQL_SELECT_ID_BYMUSICIANIDANDSKILLID], _sqlConnection))
            {
                command.Parameters.AddWithValue("@musician_id", musicianId);
                command.Parameters.AddWithValue("@skill_id", skillId);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }
    }
}
