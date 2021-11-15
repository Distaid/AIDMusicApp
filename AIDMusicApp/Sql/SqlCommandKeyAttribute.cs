using System;

namespace AIDMusicApp.Sql
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class SqlCommandKeyAttribute : Attribute
    {
        public SqlCommandKeyAttribute() { }
    }
}
