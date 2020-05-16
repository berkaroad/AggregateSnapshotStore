using System.Collections.Generic;
using Xunit;

namespace AggregateSnapshotStore.SqlServer.Tests
{
    public class SqlServerAggregateSnapshotStoreWithShardingTests
    {
        private SqlServerAggregateSnapshotStore _store;

        public SqlServerAggregateSnapshotStoreWithShardingTests()
        {
            _store = new SqlServerAggregateSnapshotStore();
            _store.Initialize("Server=localhost;Database=SnapshotStore;UID=demo;PWD=123456", tableCount: 2);
        }

        [Fact]
        public void BatchInsertMustSuccess()
        {
            var datas = new List<AggregateSnapshotData>();
            for (var i = 1; i < 100; i++)
            {
                datas.Add(new AggregateSnapshotData($"B100{i}", "StockBox", 1, new byte[] { (byte)i, 1, 2, 3 }));
            }
            _store.BatchSaveAsync(datas)
                .ConfigureAwait(false).GetAwaiter().GetResult();
        }

        [Fact]
        public void BatchUpdateMustSuccess()
        {
            var datas = new List<AggregateSnapshotData>();
            for (var i = 1; i < 100; i++)
            {
                datas.Add(new AggregateSnapshotData($"B200{i}", "StockBox", 1, new byte[] { (byte)i, 1, 2, 3 }));
            }
            _store.BatchSaveAsync(datas)
                .ConfigureAwait(false).GetAwaiter().GetResult();

            datas = new List<AggregateSnapshotData>();
            for (var i = 1; i < 100; i++)
            {
                datas.Add(new AggregateSnapshotData($"B200{i}", "StockBox", 2, new byte[] { (byte)i, 1, 2, 3, 4 }));
            }
            _store.BatchSaveAsync(datas)
                .ConfigureAwait(false).GetAwaiter().GetResult();
        }

        [Fact]
        public void FindLatestHeaderMustNotNull()
        {
            var datas = new AggregateSnapshotData[]{
                 new AggregateSnapshotData("B3001", "StockBox", 1, new byte[]{1,2,3,4})
            };
            _store.BatchSaveAsync(datas)
                .ConfigureAwait(false).GetAwaiter().GetResult();
            var header = _store.FindLatestHeaderAsync("B3001", "StockBox")
                .ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.NotNull(header);
            Assert.Equal("B3001", header.AggregateRootId);
            Assert.Equal("StockBox", header.AggregateRootTypeName);
            Assert.Equal(1, header.Version);
        }

        [Fact]
        public void FindLatestHeaderMustNull()
        {
            var datas = new AggregateSnapshotData[]{
                 new AggregateSnapshotData("B4001", "StockBox", 1, new byte[]{1,2,3,4})
            };
            _store.BatchSaveAsync(datas)
                .ConfigureAwait(false).GetAwaiter().GetResult();
            var header = _store.FindLatestHeaderAsync("B4001", "DownGoodsBill")
                .ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.Null(header);
        }

        [Fact]
        public void FindLatestMustNotNull()
        {
            var datas = new AggregateSnapshotData[]{
                 new AggregateSnapshotData("B5001", "StockBox", 1, new byte[]{1,2,3,4})
            };
            _store.BatchSaveAsync(datas)
                .ConfigureAwait(false).GetAwaiter().GetResult();
            var data = _store.FindLatestAsync("B5001", "StockBox")
                .ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.NotNull(data);
            Assert.Equal("B5001", data.AggregateRootId);
            Assert.Equal("StockBox", data.AggregateRootTypeName);
            Assert.Equal(1, data.Version);
            Assert.Equal(4, data.Data.Length);
        }

        [Fact]
        public void FindLatestMustNull()
        {
            var datas = new AggregateSnapshotData[]{
                 new AggregateSnapshotData("B6001", "StockBox", 1, new byte[]{1,2,3,4})
            };
            _store.BatchSaveAsync(datas)
                .ConfigureAwait(false).GetAwaiter().GetResult();
            var data = _store.FindLatestAsync("B6001", "DownGoodsBill")
                .ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.Null(data);
        }
    }
}
