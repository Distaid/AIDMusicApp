using AIDMusicApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace AIDMusicApp.Sql.Adapters
{
    public class AlbumFormatsAdapter : BaseAdapter
    {
        [SqlCommandKey] private const string SQL_INSERT = "SQL_Insert";
        [SqlCommandKey] private const string SQL_DELETE = "SQL_Delete";
        [SqlCommandKey] private const string SQL_SELECT_FORMATSBYALBUMID = "SQL_Select_FormatsByAlbumId";
        [SqlCommandKey] private const string SQL_SELECT_ID_BYALBUMIDANDFORMATID = "SQL_Select_Id_ByAlbumIdAndFormatId";

        public AlbumFormatsAdapter(SqlConnection connection) : base(connection, "SQLCommands\\SQLAlbumFormats.aid") { }

        public int Insert(int albumId, int formatId)
        {
            using (var command = new SqlCommand(_sqlComands[SQL_INSERT], _sqlConnection))
            {
                command.Parameters.AddWithValue("@album_id", albumId);
                command.Parameters.AddWithValue("@format_id", formatId);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public void Update(List<Format> albumFormats, IEnumerable<Format> destination, int albumId)
        {
            foreach (var format in destination)
            {
                var index = albumFormats.FindIndex((s) => s.Id == format.Id);

                if (index != -1)
                    albumFormats.RemoveAt(index);
                else
                    Insert(albumId, format.Id);
            }

            foreach (var format in albumFormats)
            {
                var id = GetIdByAlbumIdAndFormatId(albumId, format.Id);
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

        public IEnumerable<Format> GetFormatsByGroupId(int albumId)
        {
            using (var command = new SqlCommand(_sqlComands[SQL_SELECT_FORMATSBYALBUMID], _sqlConnection))
            {
                command.Parameters.AddWithValue("@id", albumId);

                using (var adapter = new SqlDataAdapter(command))
                {
                    var ds = new DataSet();
                    adapter.Fill(ds);

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        yield return new Format
                        {
                            Id = Convert.ToInt32(row[0]),
                            Name = Convert.ToString(row[1])
                        };
                    }
                }
            }
        }

        public int GetIdByAlbumIdAndFormatId(int albumId, int formatId)
        {
            using (var command = new SqlCommand(_sqlComands[SQL_SELECT_ID_BYALBUMIDANDFORMATID], _sqlConnection))
            {
                command.Parameters.AddWithValue("@album_id", albumId);
                command.Parameters.AddWithValue("@format_id", formatId);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }
    }
}
