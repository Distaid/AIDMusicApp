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
        [SqlCommandKey] private const string SQL_SELECT = "SQL_Select";
        [SqlCommandKey] private const string SQL_INSERT = "SQL_Insert";
        [SqlCommandKey] private const string SQL_UPDATE = "SQL_Update";
        [SqlCommandKey] private const string SQL_DELETE = "SQL_Delete";
        [SqlCommandKey] private const string SQL_SELECT_ID_NAME = "SQL_Select_Id_Name";
        [SqlCommandKey] private const string SQL_SELECT_SHORT = "SQL_Select_Short";
        [SqlCommandKey] private const string SQL_SELECT_TOP10 = "SQL_Select_Top10";
        [SqlCommandKey] private const string SQL_SELECT_BYNAME = "SQL_Select_ByName";
        [SqlCommandKey] private const string SQL_SELECT_COUNT = "SQL_Select_Count";

        public GroupsAdapter(SqlConnection connection) : base(connection, "SQLGroups.aid") { }

        public IEnumerable<Group> GetAll()
        {
            using (var adapter = new SqlDataAdapter(_sqlComands[SQL_SELECT], _sqlConnection))
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
                        Logo = (byte[])row[6],
                        Albums = new ObservableCollection<Album>(),
                        Genres = new ObservableCollection<Genre>(),
                        Labels = new ObservableCollection<Label>(),
                        Members = new ObservableCollection<Musician>()
                    };

                    foreach (var genre in SqlDatabase.Instance.GroupGenresAdapter.GetGenresByGroupId(group.Id))
                        group.Genres.Add(genre);

                    foreach (var label in SqlDatabase.Instance.ContractsAdapter.GetLabelsByGroupId(group.Id))
                        group.Labels.Add(label);

                    foreach (var musician in SqlDatabase.Instance.MembersAdapter.GetMusiciansByGroupId(group.Id))
                        group.Members.Add(musician);

                    foreach (var album in SqlDatabase.Instance.DiscographyAdapter.GetAlbumsByGroupId(group.Id))
                        group.Albums.Add(album);

                    yield return group;
                }

            }
        }

        public Group Insert(string name, string description, short? yearOfCreation, short? yearOfBreakup, int countryId, byte[] logo)
        {
            using (var command = new SqlCommand(_sqlComands[SQL_INSERT], _sqlConnection))
            {
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@description", !string.IsNullOrWhiteSpace(description) ? description : DBNull.Value);
                command.Parameters.AddWithValue("@creation", yearOfCreation.HasValue ? yearOfCreation.Value : DBNull.Value);
                command.Parameters.AddWithValue("@breakup", yearOfBreakup.HasValue ? yearOfBreakup.Value : DBNull.Value);
                command.Parameters.AddWithValue("@country_id", countryId);
                command.Parameters.AddWithValue("@logo", logo);

                return new Group
                {
                    Id = Convert.ToInt32(command.ExecuteScalar()),
                    Name = name,
                    Description = description,
                    YearOfCreation = yearOfCreation,
                    YearOfBreakup = yearOfBreakup,
                    CountryId = SqlDatabase.Instance.CountriesListAdapter.GetById(countryId),
                    Logo = logo,
                    Albums = new ObservableCollection<Album>(),
                    Genres = new ObservableCollection<Genre>(),
                    Labels = new ObservableCollection<Label>(),
                    Members = new ObservableCollection<Musician>()
                };
            }
        }

        public void Update(int id, string name, string description, short? yearOfCreation, short? yearOfBreakup, int countryId, byte[] logo)
        {
            using (var command = new SqlCommand(_sqlComands[SQL_UPDATE], _sqlConnection))
            {
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@description", !string.IsNullOrWhiteSpace(description) ? description : DBNull.Value);
                command.Parameters.AddWithValue("@creation", yearOfCreation.HasValue ? yearOfCreation.Value : DBNull.Value);
                command.Parameters.AddWithValue("@breakup", yearOfBreakup.HasValue ? yearOfBreakup.Value : DBNull.Value);
                command.Parameters.AddWithValue("@country_id", countryId);
                command.Parameters.AddWithValue("@logo", logo);

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

        public IEnumerable<Group> GetAllIdName()
        {
            using (var adapter = new SqlDataAdapter(_sqlComands[SQL_SELECT_ID_NAME], _sqlConnection))
            {
                var ds = new DataSet();
                adapter.Fill(ds);

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    yield return new Group
                    {
                        Id = Convert.ToInt32(row[0]),
                        Name = Convert.ToString(row[1])
                    };
                }

            }
        }

        public IEnumerable<Group> GetAllShort()
        {
            using (var adapter = new SqlDataAdapter(_sqlComands[SQL_SELECT_SHORT], _sqlConnection))
            {
                var ds = new DataSet();
                adapter.Fill(ds);

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    yield return new Group
                    {
                        Id = Convert.ToInt32(row[0]),
                        Name = Convert.ToString(row[1]),
                        Description = Convert.ToString(row[2]),
                        YearOfCreation = row[3].Equals(DBNull.Value) ? null : Convert.ToInt16(row[3]),
                        YearOfBreakup = row[4].Equals(DBNull.Value) ? null : Convert.ToInt16(row[4]),
                        CountryId = SqlDatabase.Instance.CountriesListAdapter.GetById(Convert.ToInt32(row[5])),
                        Logo = (byte[])row[6]
                    };
                }

            }
        }

        public IEnumerable<Group> GetTop10()
        {
            using (var adapter = new SqlDataAdapter(_sqlComands[SQL_SELECT_TOP10], _sqlConnection))
            {
                var ds = new DataSet();
                adapter.Fill(ds);

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    yield return new Group
                    {
                        Id = Convert.ToInt32(row[0]),
                        Name = Convert.ToString(row[1]),
                        Description = Convert.ToString(row[2]),
                        YearOfCreation = row[3].Equals(DBNull.Value) ? null : Convert.ToInt16(row[3]),
                        YearOfBreakup = row[4].Equals(DBNull.Value) ? null : Convert.ToInt16(row[4]),
                        CountryId = SqlDatabase.Instance.CountriesListAdapter.GetById(Convert.ToInt32(row[5])),
                        Logo = (byte[])row[6]
                    };
                }
            }
        }

        public IEnumerable<Group> GetAllByName(string name)
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
                        yield return new Group
                        {
                            Id = Convert.ToInt32(row[0]),
                            Name = Convert.ToString(row[1]),
                            Description = Convert.ToString(row[2]),
                            YearOfCreation = row[3].Equals(DBNull.Value) ? null : Convert.ToInt16(row[3]),
                            YearOfBreakup = row[4].Equals(DBNull.Value) ? null : Convert.ToInt16(row[4]),
                            CountryId = SqlDatabase.Instance.CountriesListAdapter.GetById(Convert.ToInt32(row[5])),
                            Logo = (byte[])row[6]
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
