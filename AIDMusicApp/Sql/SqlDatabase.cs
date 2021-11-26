using AIDMusicApp.Sql.Adapters;
using System;
using System.Data.SqlClient;
using System.IO;

namespace AIDMusicApp.Sql
{
    public class SqlDatabase
    {
        public static SqlDatabase Instance { get; private set; }

        private SqlConnection _sqlConnection = null;

        public AccessAdapter AccessAdapter { get; private set; }
        public CountriesListAdapter CountriesListAdapter { get; private set; }
        public GenresListAdapter GenresListAdapter { get; private set; }
        public LabelsListAdapter LabelsListAdapter { get; private set; }
        public SkillsListAdapter SkillsListAdapter { get; private set; }
        public FormatsListAdapter FormatsListAdapter { get; private set; }

        public UsersAdapter UsersAdapter { get; private set; }
        public MusiciansAdapter MusiciansAdapter { get; private set; }
        public GroupsAdapter GroupsAdapter { get; private set; }
        public AlbumsAdapter AlbumsAdapter { get; private set; }
        public SongsAdapter SongsAdapter { get; private set; }

        public MusicianSkillsAdapter MusicianSkillsAdapter { get; private set; }
        public GroupGenresAdapter GroupGenresAdapter { get; private set; }
        public AlbumGenresAdapter AlbumGenresAdapter { get; private set; }
        public AlbumFormatsAdapter AlbumFormatsAdapter { get; private set; }
        public DiscographyAdapter DiscographyAdapter { get; private set; }
        public ContractsAdapter ContractsAdapter { get; private set; }
        public MembersAdapter MembersAdapter { get; private set; }
        public TrackListAdapter TrackListAdapter { get; private set; }
        public FeatsAdapter FeatsAdapter { get; private set; }

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
                SongsAdapter = new SongsAdapter(_sqlConnection);

                MusicianSkillsAdapter = new MusicianSkillsAdapter(_sqlConnection);
                GroupGenresAdapter = new GroupGenresAdapter(_sqlConnection);
                AlbumGenresAdapter = new AlbumGenresAdapter(_sqlConnection);
                AlbumFormatsAdapter = new AlbumFormatsAdapter(_sqlConnection);
                DiscographyAdapter = new DiscographyAdapter(_sqlConnection);
                ContractsAdapter = new ContractsAdapter(_sqlConnection);
                MembersAdapter = new MembersAdapter(_sqlConnection);
                TrackListAdapter = new TrackListAdapter(_sqlConnection);
                FeatsAdapter = new FeatsAdapter(_sqlConnection);
            }
            catch
            {
                throw;
            }
        }
    }
}
