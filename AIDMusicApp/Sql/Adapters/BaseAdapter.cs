using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;

namespace AIDMusicApp.Sql.Adapters
{
    public abstract class BaseAdapter
    {
        protected Dictionary<string, string> _sqlComands;

        protected SqlConnection _sqlConnection;

        protected BaseAdapter(SqlConnection connection, string file)
        {
            _sqlComands = new Dictionary<string, string>();
            _sqlConnection = connection;

            if (!File.Exists(file))
                throw new Exception($"Файл {Path.GetFileName(file)} не найден!\nРабота приложения будет прекращена!");

            foreach (var pair in FileLoader.GetFileData(file))
                _sqlComands.Add(pair.Key, pair.Value);

            CheckKeys(file);
        }

        private void CheckKeys(string file)
        {
            var type = GetType();
            var fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            foreach (var field in fields)
            {
                var attr = Attribute.GetCustomAttribute(field, typeof(SqlCommandKeyAttribute), true) as SqlCommandKeyAttribute;
                if (attr != null)
                    if (!_sqlComands.ContainsKey(field.GetValue(this).ToString()))
                        throw new Exception($"Ключ {field.GetValue(this)} не содержится в исходном файле {Path.GetFileName(file)}!\nВероятно, файл был поврежден.");
            }
        }
    }
}
