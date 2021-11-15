using AIDMusicApp.Sql.Adapters;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIDMusicApp.Sql
{
    public class SqlDatabase
    {
        public static SqlDatabase Instance = null;

        private SqlConnection _sqlConnection = null;

        public AccessAdapter AccessAdapter = null;
        public CountriesListAdapter CountriesListAdapter = null;
        public GenresListAdapter GenresListAdapter = null;
        public LabelsListAdapter LabelsListAdapter = null;
        public SkillsListAdapter SkillsListAdapter = null;
        public FormatsListAdapter FormatsListAdapter = null;

        public UsersAdapter UsersAdapter = null;
        public MusiciansAdapter MusiciansAdapter = null;
        public GroupsAdapter GroupsAdapter = null;
        public AlbumsAdapter AlbumsAdapter = null;

        public MusicianSkillsAdapter MusicianSkillsAdapter = null;
        public GroupGenresAdapter GroupGenresAdapter = null;
        public AlbumGenresAdapter AlbumGenresAdapter = null;
        public AlbumFormatsAdapter AlbumFormatsAdapter = null;
        public DiscographyAdapter DiscographyAdapter = null;

        protected SqlDatabase()
        {
            try
            {
                _sqlConnection = new SqlConnection(FileLoader.LoadSqlConnectionString());
                _sqlConnection.Open();
            }
            catch (SqlException)
            {
                throw new Exception("База данных не обнаружена!\nРабота приложения будет прекращена!");
            }
            catch
            {
                throw;
            }
        }

        public static void Initialize()
        {
            if (Instance == null)
            {
                Instance = new SqlDatabase();
            }
        }

        public void CloseConnection()
        {
            _sqlConnection.Close();
        }

        public void LoadComands()
        {
            if (!Directory.Exists("SQLCommands"))
                throw new Exception("Папка с файлами команд не обнаружена!\nРабота приложения будет прекращена!");

            try
            {
                AccessAdapter = new AccessAdapter(_sqlConnection);

                CountriesListAdapter = new CountriesListAdapter(_sqlConnection);
                GenresListAdapter = new GenresListAdapter(_sqlConnection);
                LabelsListAdapter = new LabelsListAdapter(_sqlConnection);
                SkillsListAdapter = new SkillsListAdapter(_sqlConnection);
                FormatsListAdapter = new FormatsListAdapter(_sqlConnection);

                UsersAdapter = new UsersAdapter(_sqlConnection);
                MusiciansAdapter = new MusiciansAdapter(_sqlConnection);
                GroupsAdapter = new GroupsAdapter(_sqlConnection);
                AlbumsAdapter = new AlbumsAdapter(_sqlConnection);

                MusicianSkillsAdapter = new MusicianSkillsAdapter(_sqlConnection);
                GroupGenresAdapter = new GroupGenresAdapter(_sqlConnection);
                AlbumGenresAdapter = new AlbumGenresAdapter(_sqlConnection);
                AlbumFormatsAdapter = new AlbumFormatsAdapter(_sqlConnection);
                DiscographyAdapter = new DiscographyAdapter(_sqlConnection);
            }
            catch
            {
                throw;
            }
        }
    }
}
