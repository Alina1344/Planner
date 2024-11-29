using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Npgsql;

namespace Storage
{
    public class DatabaseRepository<T> : IDatabaseRepository<T>
    {
        private readonly string _connectionString;
        private readonly string _tableName;

        public DatabaseRepository(string connectionString, string tableName)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _tableName = tableName ?? throw new ArgumentNullException(nameof(tableName));
        }


        // Получение записи по условию
        public async Task<T?> GetSingleAsync(string whereClause, object? parameters, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(whereClause)) throw new ArgumentException("Условие не может быть пустым.", nameof(whereClause));

            using var connection = new NpgsqlConnection(_connectionString);
            var query = $"SELECT * FROM {_tableName} WHERE {whereClause}";
            await connection.OpenAsync(cancellationToken);
            return await connection.QueryFirstOrDefaultAsync<T>(query, parameters);
        }

        // Получение списка записей по условию
        public async Task<List<T>> GetListAsync(string whereClause, object? parameters, CancellationToken cancellationToken)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            var query = string.IsNullOrWhiteSpace(whereClause) 
                ? $"SELECT * FROM {_tableName}" 
                : $"SELECT * FROM {_tableName} WHERE {whereClause}";
            
            await connection.OpenAsync(cancellationToken);
            var result = await connection.QueryAsync<T>(query, parameters);
            return result.ToList();
        }

        // Добавление записи
        public async Task AddAsync(T entity, CancellationToken cancellationToken)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            using var connection = new NpgsqlConnection(_connectionString);
            var insertQuery = GenerateInsertQuery();
            await connection.OpenAsync(cancellationToken);
            await connection.ExecuteAsync(insertQuery, entity);
        }

        // Удаление записей по условию
        public async Task DeleteAsync(string whereClause, object? parameters, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(whereClause)) throw new ArgumentException("Условие не может быть пустым.", nameof(whereClause));

            using var connection = new NpgsqlConnection(_connectionString);
            var query = $"DELETE FROM {_tableName} WHERE {whereClause}";
            await connection.OpenAsync(cancellationToken);
            await connection.ExecuteAsync(query, parameters);
        }

        // Обновление записи
        public async Task UpdateAsync(string whereClause, object? parameters, T updatedEntity, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(whereClause)) throw new ArgumentException("Условие не может быть пустым.", nameof(whereClause));
            if (updatedEntity == null) throw new ArgumentNullException(nameof(updatedEntity));

            using var connection = new NpgsqlConnection(_connectionString);
            var updateQuery = GenerateUpdateQuery(whereClause);
            await connection.OpenAsync(cancellationToken);
            await connection.ExecuteAsync(updateQuery, updatedEntity);
        }

        // Генерация SQL-запроса для вставки
        private string GenerateInsertQuery()
        {
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead)
                .Select(p => p.Name);

            var columns = string.Join(", ", properties);
            var values = string.Join(", ", properties.Select(p => $"@{p}"));

            return $"INSERT INTO {_tableName} ({columns}) VALUES ({values})";
        }

        // Генерация SQL-запроса для обновления
        private string GenerateUpdateQuery(string whereClause)
        {
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite)
                .Select(p => $"{p.Name} = @{p.Name}");

            var setClause = string.Join(", ", properties);
            return $"UPDATE {_tableName} SET {setClause} WHERE {whereClause}";
        }
    }
}
