using System;
using System.Data;
using System.IO;
using OpenNet.Orm.Sql;
#if XAMARIN
using SQLiteConnection = Microsoft.Data.Sqlite.SqliteConnection;
#else
using SQLiteConnection = System.Data.SQLite.SQLiteConnection;
#endif

namespace OpenNet.Orm.Sqlite
{
    public class SqliteDbEngine : IDbEngine
    {
        private string _datasource;
        private string _connectionString;

        public SqliteDbEngine(string datasource, string password)
        {
            _datasource = datasource;
            _connectionString = string.Format("Data Source={0};", _datasource);

            if (!string.IsNullOrEmpty(password))
            {
                _connectionString += string.Format("Password={0};", password);
            }
        }

        public bool DatabaseExists
        {
            get { return File.Exists(_datasource); }
        }

        public string Name { get; }

        public void DeleteDatabase()
        {
            if (!DatabaseExists)
                return;

            File.Delete(_datasource);
        }

        public void CreateDatabase()
        {
            //Managed in Open method
        }

        public IDbConnection GetNewConnection()
        {
            return new SQLiteConnection(_connectionString);
        }

        public void Compact()
        {
            throw new NotImplementedException();
        }

        public void RepairDb()
        {
            throw new NotImplementedException();
        }
    }
}