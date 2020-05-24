using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using log4net;

namespace AggregateSnapshotStore.Redis
{
    /// <summary>
    /// Redis 聚合快照存储，用于快照获取和生成
    /// </summary>
    public class RedisAggregateSnapshotStore : IAggregateSnapshotStore
    {
        private string _keyPrefix;
        private StackExchange.Redis.IConnectionMultiplexer _connection;
        private StackExchange.Redis.RedisValue _typeField = new StackExchange.Redis.RedisValue("type");
        private StackExchange.Redis.RedisValue _verField = new StackExchange.Redis.RedisValue("version");
        private StackExchange.Redis.RedisValue _dataField = new StackExchange.Redis.RedisValue("data");
        private ILog _logger;
        private volatile int _successCount;

        /// <summary>
        /// 获取成功次数，仅用于测试
        /// </summary>
        /// <returns></returns>
        public int GetSuccessCount()
        {
            return _successCount;
        }

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="redisConfiguration"></param>
        /// <param name="keyPrefix"></param>
        public void Initialize(string redisConfiguration, string keyPrefix)
        {
            _keyPrefix = keyPrefix;
            _connection = StackExchange.Redis.ConnectionMultiplexer.Connect(redisConfiguration);
            _logger = LogManager.GetLogger(GetType());
        }

        /// <summary>
        /// 查找最近一次快照头信息
        /// </summary>
        /// <param name="aggregateRootId"></param>
        /// <param name="aggregateRootTypeName"></param>
        /// <returns></returns>
        public async Task<AggregateSnapshotHeader> FindLatestHeaderAsync(string aggregateRootId, string aggregateRootTypeName)
        {
            var db = _connection.GetDatabase();
            var typeVal = await db.HashGetAsync(GetSnapshotKey(aggregateRootId), _typeField);
            if (typeVal != aggregateRootTypeName)
            {
                return null;
            }
            var verVal = await db.HashGetAsync(GetSnapshotKey(aggregateRootId), _verField);
            var version = (int)verVal;
            if (version == 0)
            {
                return null;
            }
            return new AggregateSnapshotHeader(aggregateRootId, aggregateRootTypeName, version);
        }

        /// <summary>
        /// 查找最近一次快照信息
        /// </summary>
        /// <param name="aggregateRootId"></param>
        /// <param name="aggregateRootTypeName"></param>
        /// <returns></returns>
        public async Task<AggregateSnapshotData> FindLatestAsync(string aggregateRootId, string aggregateRootTypeName)
        {
            var db = _connection.GetDatabase();
            var item = await db.HashGetAllAsync(GetSnapshotKey(aggregateRootId));
            if (item == null || item.Length < 3
                || item.First(f => f.Name == _typeField).Value.ToString() != aggregateRootTypeName)
            {
                return null;
            }
            var version = (int)item.First(f => f.Name == _verField).Value;
            var data = (byte[])item.First(f => f.Name == _dataField).Value;
            if (version == 0 || data == null || data.Length == 0)
            {
                return null;
            }
            return new AggregateSnapshotData(aggregateRootId, aggregateRootTypeName, version, data);
        }

        /// <summary>
        /// 批量保存
        /// </summary>
        /// <param name="snapshotDatas"></param>
        /// <returns></returns>
        public async Task BatchSaveAsync(IEnumerable<AggregateSnapshotData> snapshotDatas)
        {
            const string LUA_SCRIPT = @"
local ver = redis.call('HGET', @key, @versionField);
local type = redis.call('HGET', @key, @typeField);
if not ver then
    redis.call('HSET', @key, @typeField, @typeVal);
    redis.call('HSET', @key, @versionField, @verVal);
    redis.call('HSET', @key, @dataField, @dataVal);
    return 1;
elseif type==@typeVal then
    redis.call('HSET', @key, @versionField, @verVal);
    redis.call('HSET', @key, @dataField, @dataVal);
    return 1;
else
    return 0;
end
";

            Interlocked.Exchange(ref _successCount, 0);
            if (snapshotDatas == null || !snapshotDatas.Any()) return;
            var db = _connection.GetDatabase();
            foreach (var snapshotData in snapshotDatas)
            {
                try
                {
                    var result = await db.ScriptEvaluateAsync(StackExchange.Redis.LuaScript.Prepare(LUA_SCRIPT), new
                    {
                        key = GetSnapshotKey(snapshotData.AggregateRootId),
                        typeField = _typeField,
                        typeVal = snapshotData.AggregateRootTypeName,
                        versionField = _verField,
                        verVal = snapshotData.Version,
                        dataField = _dataField,
                        dataVal = snapshotData.Data
                    });
                    if ((int)result == 0)
                    {
                        _logger.Error($"Save snapshot fail:different aggregate root type. AggregateRootId={snapshotData.AggregateRootId},AggregateRootTypeName={snapshotData.AggregateRootTypeName}");
                        continue;
                    }
                    Interlocked.Increment(ref _successCount);
                }
                catch (Exception ex)
                {
                    _logger.Error($"Save snapshot fail:{ex.Message}. AggregateRootId={snapshotData.AggregateRootId},AggregateRootTypeName={snapshotData.AggregateRootTypeName}", ex);
                }
            }
        }

        /// <summary>
        /// Remove key(just for test)
        /// </summary>
        /// <param name="aggregateRootId"></param>
        /// <returns></returns>
        public async Task RemoveKeyAsync(string aggregateRootId)
        {
            var db = _connection.GetDatabase();
            await db.KeyDeleteAsync(GetSnapshotKey(aggregateRootId));
        }

        private StackExchange.Redis.RedisKey GetSnapshotKey(string aggregateRootId)
        {
            return new StackExchange.Redis.RedisKey($"{_keyPrefix}:ass:{aggregateRootId}");
        }
    }
}