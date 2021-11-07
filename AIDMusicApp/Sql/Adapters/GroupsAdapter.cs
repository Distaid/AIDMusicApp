using AIDMusicApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;

namespace AIDMusicApp.Sql.Adapters
{
    public class GroupsAdapter : BaseAdapter
    {
        public GroupsAdapter(SqlConnection connection) : base(connection, "SQLCommands\\SQLGroups.aid") { }

        public IEnumerable<Group> GetAll()
        {
            using (var adapter = new SqlDataAdapter(_sqlComands["SQL_Select"], _sqlConnection))
            {
                var ds = new DataSet();
                adapter.Fill(ds);

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    var group = new Group
                    {
                        Id = Convert.ToInt32(row[0]),
                        Name = Convert.ToString(row[1]),
                        Description = Convert.ToString(row[2]),
                        YearOfCreation = row[3].Equals(DBNull.Value) ? null : Convert.ToInt16(row[3]),
                        YearOfBreakup = row[4].Equals(DBNull.Value) ? null : Convert.ToInt16(row[4]),
                        CountryId = SqlDatabase.Instance.CountriesListAdapter.GetById(Convert.ToInt32(row[5])),
                        Albums = new ObservableCollection<Album>(),
                        Genres = new ObservableCollection<Genre>(),
                        Labels = new ObservableCollection<Label>(),
                        Members = new ObservableCollection<Member>()
                    };

                    foreach (var genre in SqlDatabase.Instance.GroupGenresAdapter.GetGenresByGroupId(group.Id))
                        group.Genres.Add(genre);

                    yield return group;
                }

            }
        }

        public Group Insert(string name, string description, short? yearOfCreation, short? yearOfBreakup, int countryId)
        {
            using (var command = new SqlCommand(_sqlComands["SQL_Insert"], _sqlConnection))
            {
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@description", !string.IsNullOrWhiteSpace(description) ? description : DBNull.Value);
                command.Parameters.AddWithValue("@creation", yearOfCreation.HasValue ? yearOfCreation.Value : DBNull.Value);
                command.Parameters.AddWithValue("@breakup", yearOfBreakup.HasValue ? yearOfBreakup.Value : DBNull.Value);
                command.Parameters.AddWithValue("@country_id", countryId);

                return new Group
                {
                    Id = Convert.ToInt32(command.ExecuteScalar()),
                    Name = name,
                    Description = description,
                    YearOfCreation = yearOfCreation,
                    YearOfBreakup = yearOfBreakup,
                    CountryId = SqlDatabase.Instance.CountriesListAdapter.GetById(countryId),
                    Albums = new ObservableCollection<Album>(),
                    Genres = new ObservableCollection<Genre>(),
                    Labels = new ObservableCollection<Label>(),
                    Members = new ObservableCollection<Member>()
                };
            }
        }

        public void Update(int id, string name, string description, short? yearOfCreation, short? yearOfBreakup, int countryId)
        {
            using (var command = new SqlCommand(_sqlComands["SQL_Update"], _sqlConnection))
            {
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@description", !string.IsNullOrWhiteSpace(description) ? description : DBNull.Value);
                command.Parameters.AddWithValue("@creation", yearOfCreation.HasValue ? yearOfCreation.Value : DBNull.Value);
                command.Parameters.AddWithValue("@breakup", yearOfBreakup.HasValue ? yearOfBreakup.Value : DBNull.Value);
                command.Parameters.AddWithValue("@country_id", countryId);

                command.ExecuteNonQuery();
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
    }
}
