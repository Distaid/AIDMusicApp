using AIDMusicApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;

namespace AIDMusicApp.Sql.Adapters
{
    public class MusiciansAdapter : BaseAdapter
    {
        [SqlCommandKey] private const string SQL_SELECT = "SQL_Select";
        [SqlCommandKey] private const string SQL_INSERT = "SQL_Insert";
        [SqlCommandKey] private const string SQL_UPDATE = "SQL_Update";
        [SqlCommandKey] private const string SQL_DELETE = "SQL_Delete";
        [SqlCommandKey] private const string SQL_SELECT_TOP10 = "SQL_Select_Top10";
        [SqlCommandKey] private const string SQL_SELECT_BYNAME = "SQL_Select_ByName";
        [SqlCommandKey] private const string SQL_SELECT_COUNT = "SQL_Select_Count";

        public MusiciansAdapter(SqlConnection connection) : base(connection, "SQLMusicians.aid") { }

        public IEnumerable<Musician> GetAll()
        {
            using (var adapter = new SqlDataAdapter(_sqlComands[SQL_SELECT], _sqlConnection))
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

        public Musician Insert(string name, byte age, int countryId, bool isDead)
        {
            using (var command = new SqlCommand(_sqlComands[SQL_INSERT], _sqlConnection))
            {
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@age", age);
                command.Parameters.AddWithValue("@country_id", countryId);
                command.Parameters.AddWithValue("@is_dead", isDead);

                return new Musician
                {
                    Id = Convert.ToInt32(command.ExecuteScalar()),
                    Name = name,
                    Age = age,
                    CountryId = SqlDatabase.Instance.CountriesListAdapter.GetById(countryId),
                    IsDead = isDead,
                    Skills = new ObservableCollection<Skill>()
                };
            }
        }

        public void Update(int id, string name, byte age, int countryId, bool isDead)
        {
            using (var command = new SqlCommand(_sqlComands[SQL_UPDATE], _sqlConnection))
            {
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@age", age);
                command.Parameters.AddWithValue("@country_id", countryId);
                command.Parameters.AddWithValue("@is_dead", isDead);

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

        public IEnumerable<Musician> GetTop10()
        {
            using (var adapter = new SqlDataAdapter(_sqlComands[SQL_SELECT_TOP10], _sqlConnection))
            {
                var ds = new DataSet();
                adapter.Fill(ds);

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    yield return new Musician
                    {
                        Id = Convert.ToInt32(row[0]),
                        Name = Convert.ToString(row[1]),
                        Age = Convert.ToByte(row[2]),
                        CountryId = SqlDatabase.Instance.CountriesListAdapter.GetById(Convert.ToInt32(row[3])),
                        IsDead = Convert.ToBoolean(row[4])
                    };
                }
            }
        }

        public IEnumerable<Musician> GetAllByName(string name)
        {
            using (var command = new SqlCommand(_sqlComands[SQL_SELECT_BYNAME], _sqlConnection))
            {
                command.Parameters.AddWithValue("@name", $"%{name}%");

                using (var adapter = new SqlDataAdapter(command))
                {
                    var ds = new DataSet();
                    adapter.Fill(ds);

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        yield return new Musician
                        {
                            Id = Convert.ToInt32(row[0]),
                            Name = Convert.ToString(row[1]),
                            Age = Convert.ToByte(row[2]),
                            CountryId = SqlDatabase.Instance.CountriesListAdapter.GetById(Convert.ToInt32(row[3])),
                            IsDead = Convert.ToBoolean(row[4])
                        };
                    }
                }
            }
        }

        public int GetCount()
        {
            using (var command = new SqlCommand(_sqlComands[SQL_SELECT_COUNT], _sqlConnection))
            {
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }
    }
}
