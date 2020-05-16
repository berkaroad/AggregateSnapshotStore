using Dapper;
using log4net;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AggregateSnapshotStore.MySQL
{
    /// <summary>
    /// 聚合仓储基类，用于快照获取和生成
    /// </summary>
    public sealed class MySQLAggregateSnapshotStore : IAggregateSnapshotStore
    {
        private string _connectionString;
        private string _tableName;
        private int _tableCount;
        private ILog _logger;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <param name="tableName">表名</param>
        /// <param name="tableCount">分表数量</param>
        public void Initialize(string connectionString,
            string tableName = "AggregateSnapshot",
            int tableCount = 1)
        {
            _connectionString = connectionString;
            _tableName = tableName;
            _tableCount = tableCount;
            _logger = LogManager.GetLogger(GetType());
        }

        /// <summary>
        /// 查找最近一次快照头信息
        /// </summary>
        /// <param name="aggregateRootId">聚合根ID</param>
        /// <param name="aggregateRootTypeName">聚合根类型名</param>
        /// <returns></returns>
        public async Task<AggregateSnapshotHeader> FindLatestHeaderAsync(string aggregateRootId, string aggregateRootTypeName)
        {
            string SQL = $@"
SELECT AggregateRootId,AggregateRootTypeName,Version
FROM {GetTableName(aggregateRootId)}
WHERE AggregateRootId=@AggregateRootId";
            try
            {
                SnapshotData snapshotData;
                using (var connect = CreateConnection())
                {
                    snapshotData = (await connect.QueryFirstOrDefaultAsync<SnapshotData>(SQL, new { AggregateRootId = aggregateRootId }));
                }
                if (snapshotData == null || snapshotData.AggregateRootTypeName != aggregateRootTypeName)
                {
                    return null;
                }
                return new AggregateSnapshotHeader(snapshotData.AggregateRootId, snapshotData.AggregateRootTypeName, snapshotData.Version);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 查找最近一次快照信息
        /// </summary>
        /// <param name="aggregateRootId">聚合根ID</param>
        /// <param name="aggregateRootTypeName">聚合根类型名</param>
        /// <returns></returns>
        public async Task<AggregateSnapshotData> FindLatestAsync(string aggregateRootId, string aggregateRootTypeName)
        {
            string SQL = $@"
SELECT AggregateRootId,AggregateRootTypeName,Version,`Data`
FROM {GetTableName(aggregateRootId)}
WHERE AggregateRootId=@AggregateRootId";
            try
            {
                SnapshotData snapshotData;
                using (var connect = CreateConnection())
                {
                    snapshotData = (await connect.QueryFirstOrDefaultAsync<SnapshotData>(SQL, new { AggregateRootId = aggregateRootId }));
                }
                if (snapshotData == null || snapshotData.AggregateRootTypeName != aggregateRootTypeName
                    || snapshotData.Data == null || snapshotData.Data.Length == 0)
                {
                    return null;
                }
                return new AggregateSnapshotData(snapshotData.AggregateRootId, snapshotData.AggregateRootTypeName, snapshotData.Version, snapshotData.Data);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 批量保存
        /// </summary>
        /// <param name="snapshotDatas">快照数据</param>
        /// <returns></returns>
        public async Task BatchSaveAsync(IEnumerable<AggregateSnapshotData> snapshotDatas)
        {
            const string SQL_FORMAT = @"
INSERT INTO {0} (AggregateRootId,AggregateRootTypeName,Version,`Data`)
VALUES (?AggregateRootId,?AggregateRootTypeName,?Version,?Data)
ON DUPLICATE KEY UPDATE Version=?Version,`Data`=?Data;";

            using (var connect = CreateConnection())
            {
                foreach (var snapshotData in snapshotDatas)
                {
                    try
                    {
                        var tableName = GetTableName(snapshotData.AggregateRootId);
                        await connect.ExecuteAsync(string.Format(SQL_FORMAT, tableName), new
                        {
                            AggregateRootId = snapshotData.AggregateRootId,
                            AggregateRootTypeName = snapshotData.AggregateRootTypeName,
                            Version = snapshotData.Version,
                            Data = snapshotData.Data,
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"Save snapshot fail:{ex.Message}. AggregateRootId={snapshotData.AggregateRootId},AggregateRootTypeName={snapshotData.AggregateRootTypeName}", ex);
                    }
                }
            }
        }

        private string GetTableName(string aggregateRootId)
        {
            if (_tableCount <= 1)
            {
                return $"`{_tableName}`";
            }

            var tableIndex = Crc16.GetHashCode(aggregateRootId) % _tableCount;
            return $"`{_tableName}_{tableIndex}`";
        }

        private MySqlConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}
