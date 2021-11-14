using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Threading;

namespace AIDMusicVerify
{
    public class Program
    {
        private static List<string> _tables = new List<string>
        {
            "AccessRights",
            "CountriesList",
            "GenresList",
            "LabelsList",
            "SkillsList",
            "AlbumFormatsList",
            "Users",
            "Groups",
            "Albums",
            "Songs",
            "Musicians",
            "GroupGenres",
            "AlbumGenres",
            "SongGenres",
            "Contracts",
            "Members",
            "MusicianSkills",
            "Discography",
            "AlbumFormats",
            "TrackLists",
            "Feats",
            "Covers"
        };

        private static Dictionary<string, string> _comands = new Dictionary<string, string>();

        private static string _connString = "Data Source={0};Initial Catalog={1};Integrated Security=True";

        private static SqlConnection _connection = null;

        public static void Main(string[] args)
        {
            Init();
            var initialSourse = GetInitialSource();
            Connect(initialSourse);

            var dbResult = CheckDB();
            if (dbResult)
            {
                Console.Clear();
                Console.WriteLine("Database already exists. Do you want to check tables? (Y/N)");
                var key = Console.ReadKey(true).KeyChar;

                if (key == 'n' || key == 'N')
                {
                    Console.WriteLine("\nApplication will be close.");
                    Console.ReadKey(true);
                    return;
                }
            }

            SwapDatabase(initialSourse);
            try
            {
                CreateTables();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Close();
                return;
            }

            CreateAdmin();

            Close();
        }

        private static IEnumerable<KeyValuePair<string, string>> GetFileData(byte[] sqlFile)
        {
            using (var file = new BinaryReader(new MemoryStream(sqlFile)))
            {
                var codeSize = file.ReadInt32();
                var codeData = file.ReadBytes(codeSize);

                var textSize = file.ReadInt32();
                var data = file.ReadBytes(textSize);

                var jpart = JObject.Parse(Crypto.Encrypt(data));
                var dataParts = JArray.Parse(jpart["Data"].ToString());
                foreach (var part in dataParts)
                {
                    var key = part["SQL_Name"].ToString();
                    var value = part["SQL_Command"].ToString();
                    yield return new KeyValuePair<string, string>(key, value);
                }
            }
        }

        private static void Init()
        {
            Console.WriteLine("Resource initializating...");
            foreach (var pair in GetFileData(Resource.SQLVerifyCheck))
                _comands.Add(pair.Key, pair.Value);
            foreach (var pair in GetFileData(Resource.SQLVerifyCreate))
                _comands.Add(pair.Key, pair.Value);
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("Resource initializating Success!");
        }

        private static string GetInitialSource()
        {
            Console.SetCursorPosition(0, 3);
            Console.WriteLine("Oyherwise, please, install: https://aka.ms/ssmsfullsetup");
            Console.SetCursorPosition(0, 2);
            Console.Write("If MSQ SQL Management Studio already installed, enter Initial Source: ");
            return Console.ReadLine();
        }

        private static void Connect(string source)
        {
            Console.SetCursorPosition(0, 5);
            Console.WriteLine("Connecting to MS SQL...");

            _connection = new SqlConnection(string.Format(_connString, source, "master"));
            _connection.Open();

            Console.SetCursorPosition(0, 5);
            Console.WriteLine("Connecting to MS SQL Success!");
            Thread.Sleep(2000);
        }

        private static bool CheckDB()
        {
            Console.Clear();
            Console.WriteLine("Checking and creating Database:\n");

            Console.Write(" Checking for existence: ");
            var result = ContainsDB();
            if (result)
            {
                Console.Write("( ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(result.ToString());
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(" |       )\n");

                Thread.Sleep(1000);
                return true;
            }
            else
            {
                Console.Write("(      | ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(result.ToString());
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(" )\n\n");

                Console.Write(" Creating: ");
                CreateDB();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Success");
                Console.ForegroundColor = ConsoleColor.Gray;
                Thread.Sleep(2000);
                return false;
            }
        }

        private static bool ContainsDB()
        {
            using (var command = new SqlCommand(_comands["SQL_Check_Database_MusicDB"], _connection))
            {
                var count = Convert.ToInt32(command.ExecuteScalar());
                if (count == 0) return false;
            }

            return true;
        }

        private static void CreateDB()
        {
            using (var command = new SqlCommand(_comands["SQL_Create_Database_MusicDB"], _connection))
                command.ExecuteNonQuery();
        }

        private static void SwapDatabase(string source)
        {
            _connection.Close();
            _connection = new SqlConnection(string.Format(_connString, source, "MusicDB"));
            _connection.Open();
        }

        private static void CreateTables()
        {
            Console.Clear();
            Console.WriteLine("Checking and creating tables:\n");

            foreach (var table in _tables)
            {
                Console.WriteLine($" {table} table:");
                Console.Write(" \tChecking for existence: ");
                Thread.Sleep(250);
                var result = ContainsTable(table);

                if (result)
                {
                    Console.Write("( ");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(result.ToString());
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write(" |       )\n");
                }
                else
                {
                    Console.Write("(      | ");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(result.ToString());
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write(" )\n");

                    Console.Write(" \tCreating: ");
                    CreateTable(table);
                    Thread.Sleep(250);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Success");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            }

            Console.WriteLine("\nChecking complete!\n");
        }

        private static bool ContainsTable(string table)
        {
            using (var command = new SqlCommand(_comands[$"SQL_Check_Table_{table}"], _connection))
            {
                var count = Convert.ToInt32(command.ExecuteScalar());
                if (count == 0) return false;
            }

            return true;
        }

        private static void CreateTable(string table)
        {
            using (var command = new SqlCommand(_comands[$"SQL_Create_Table_{table}"], _connection))
                command.ExecuteNonQuery();
        }

        private static void CreateAdmin()
        {
            Console.WriteLine("\nDo you want to create admin user? (Y/N)");
            var key = Console.ReadKey(true).KeyChar;

            if (key == 'n' || key == 'N')
                return;

            CreateAdminUser();
            Console.WriteLine("\nLogin:    root");
            Console.WriteLine("Password: 12345");
        }

        private static void CreateAdminUser()
        {
            using (var command = new SqlCommand(_comands["SQL_Insert_Table_Admin"], _connection))
            {
                command.Parameters.AddWithValue("@avatar", Resource.person_default);

                command.ExecuteNonQuery();
            }
        }

        private static void Close()
        {
            _connection.Close();
            Console.WriteLine("\n\nVerify complete!");
            Console.ReadKey(true);
        }
    }
}
