using System.Collections.Generic;
using Xunit;

namespace AggregateSnapshotStore.MySQL.Tests
{
    public class MySQLAggregateSnapshotStoreWithShardingTests
    {
        [Fact]
        public void BatchInsertMustSuccess()
        {
            var store = new MySQLAggregateSnapshotStore();
            store.Initialize("Server=localhost;Database=SnapshotStore;UID=demo;PWD=123456", tableCount: 2);
            var datas = new List<AggregateSnapshotData>();
            for (var i = 1; i <= 100; i++)
            {
                datas.Add(new AggregateSnapshotData($"B100{i}", "StockBox", 1, new byte[] { (byte)i, 1, 2, 3 }));
            }
            store.BatchSaveAsync(datas)
                .ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.Equal(100, store.GetSuccessCount());
        }

        [Fact]
        public void BatchUpdateMustSuccess()
        {
            var store = new MySQLAggregateSnapshotStore();
            store.Initialize("Server=localhost;Database=SnapshotStore;UID=demo;PWD=123456", tableCount: 2);
            var datas = new List<AggregateSnapshotData>();
            for (var i = 1; i <= 100; i++)
            {
                datas.Add(new AggregateSnapshotData($"B200{i}", "StockBox", 1, new byte[] { (byte)i, 1, 2, 3 }));
            }
            store.BatchSaveAsync(datas)
                .ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.Equal(100, store.GetSuccessCount());

            datas = new List<AggregateSnapshotData>();
            for (var i = 1; i <= 100; i++)
            {
                datas.Add(new AggregateSnapshotData($"B200{i}", "StockBox", 2, new byte[] { (byte)i, 1, 2, 3, 4 }));
            }
            store.BatchSaveAsync(datas)
                .ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.Equal(100, store.GetSuccessCount());
        }

        [Fact]
        public void FindLatestHeaderMustNotNull()
        {
            var store = new MySQLAggregateSnapshotStore();
            store.Initialize("Server=localhost;Database=SnapshotStore;UID=demo;PWD=123456", tableCount: 2);

            var datas = new AggregateSnapshotData[]{
                 new AggregateSnapshotData("B3001", "StockBox", 1, new byte[]{1,2,3,4})
            };
            store.BatchSaveAsync(datas)
                .ConfigureAwait(false).GetAwaiter().GetResult();
            var header = store.FindLatestHeaderAsync("B3001", "StockBox")
                .ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.NotNull(header);
            Assert.Equal("B3001", header.AggregateRootId);
            Assert.Equal("StockBox", header.AggregateRootTypeName);
            Assert.Equal(1, header.Version);
        }

        [Fact]
        public void FindLatestHeaderMustNull()
        {
            var store = new MySQLAggregateSnapshotStore();
            store.Initialize("Server=localhost;Database=SnapshotStore;UID=demo;PWD=123456", tableCount: 2);

            var datas = new AggregateSnapshotData[]{
                 new AggregateSnapshotData("B4001", "StockBox", 1, new byte[]{1,2,3,4})
            };
            store.BatchSaveAsync(datas)
                .ConfigureAwait(false).GetAwaiter().GetResult();
            var header = store.FindLatestHeaderAsync("B4001", "DownGoodsBill")
                .ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.Null(header);
        }

        [Fact]
        public void FindLatestMustNotNull()
        {
            var store = new MySQLAggregateSnapshotStore();
            store.Initialize("Server=localhost;Database=SnapshotStore;UID=demo;PWD=123456", tableCount: 2);

            var datas = new AggregateSnapshotData[]{
                 new AggregateSnapshotData("B5001", "StockBox", 1, new byte[]{1,2,3,4})
            };
            store.BatchSaveAsync(datas)
                .ConfigureAwait(false).GetAwaiter().GetResult();
            var data = store.FindLatestAsync("B5001", "StockBox")
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
            var store = new MySQLAggregateSnapshotStore();
            store.Initialize("Server=localhost;Database=SnapshotStore;UID=demo;PWD=123456", tableCount: 2);

            var datas = new AggregateSnapshotData[]{
                 new AggregateSnapshotData("B6001", "StockBox", 1, new byte[]{1,2,3,4})
            };
            store.BatchSaveAsync(datas)
                .ConfigureAwait(false).GetAwaiter().GetResult();
            var data = store.FindLatestAsync("B6001", "DownGoodsBill")
                .ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.Null(data);
        }
    }
}
