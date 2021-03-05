using DbUp;

using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace ConsoleApp1fw.Database
{
    public class SqlDatabase
    {
        private string _connectionString;

        public SqlDatabase(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection OpenConnection()
        {
            var conn = new SqlConnection(_connectionString);
            conn.Open();
            if (conn.State != ConnectionState.Open)
            {
                throw new Exception("Failed to establish connection with database.");
            }
            return conn;
        }

        public T ExecuteInTransaction<T>(Func<IDbConnection, IDbTransaction, T> func)
        {
            using (var conn = OpenConnection())
            {
                var trans = conn.BeginTransaction();

                try
                {
                    var result = func.Invoke(conn, trans);
                    trans.Commit();
                    return result;
                }
                catch (Exception)
                {
                    trans.Rollback();
                    throw;
                }
            }
        }

        public void ExecuteInTransaction(Action<IDbConnection, IDbTransaction> func)
        {
            using (var conn = OpenConnection())
            {
                var trans = conn.BeginTransaction();

                try
                {
                    func.Invoke(conn, trans);
                    trans.Commit();
                }
                catch (Exception)
                {
                    trans.Rollback();
                    throw;
                }
            }
        }

        public void SyncDatabaseSchema()
        {
            EnsureDatabase.For
              .SqlDatabase(_connectionString);

            DeployChanges.To
                .SqlDatabase(_connectionString)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                .Build()
                .PerformUpgrade();
        }
    }
}
