using Base.Application;
using Npgsql;
using System.Data;

namespace User.Infrastructure.Configuration.DataAccess
{
    public class SqlServerConnectionFactory : ISqlConnectionFactory, IDisposable
    {
        private readonly string _connectionString;
        private IDbConnection _connection;

        public SqlServerConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection GetOpenConnection()
        {
            if (_connection == null || _connection.State != ConnectionState.Open)
            {
                _connection = new NpgsqlConnection(_connectionString);
                _connection.Open();
            }

            return _connection;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_connection != null)
                {
                    if (_connection.State == ConnectionState.Open || _connection.State != ConnectionState.Connecting)
                    {
                        _connection.Close();
                    }
                    _connection.Dispose();
                }
            }

        }
    }
}
