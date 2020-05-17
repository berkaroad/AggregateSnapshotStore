using Xunit;

namespace AggregateSnapshotStore.MySQL.Tests
{
    public class MySQLAggregateSnapshotStoreTests
    {
        [Fact]
        public void BatchInsertMustSuccess()
        {
            var store = new MySQLAggregateSnapshotStore();
            store.Initialize("Server=localhost;Database=SnapshotStore;UID=demo;PWD=123456");
            var datas = new AggregateSnapshotData[]{
                 new AggregateSnapshotData("B001", "StockBox", 1, new byte[]{1,2,3,4}),
                 new AggregateSnapshotData("B002", "StockBox", 1, new byte[]{2,2,3,4})
            };
            store.BatchSaveAsync(datas)
                .ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.Equal(2, store.GetSuccessCount());
        }

        [Fact]
        public void BatchUpdateMustSuccess()
        {
            var store = new MySQLAggregateSnapshotStore();
            store.Initialize("Server=localhost;Database=SnapshotStore;UID=demo;PWD=123456");
            var datas = new AggregateSnapshotData[]{
                 new AggregateSnapshotData("B003", "StockBox", 1, new byte[]{1,2,3,4}),
                 new AggregateSnapshotData("B004", "StockBox", 1, new byte[]{2,2,3,4})
            };
            store.BatchSaveAsync(datas)
                .ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.Equal(2, store.GetSuccessCount());

            datas = new AggregateSnapshotData[]{
                 new AggregateSnapshotData("B003", "StockBox", 2, new byte[]{1,2,3,4,5}),
                 new AggregateSnapshotData("B004", "StockBox", 2, new byte[]{2,2,3,4,5})
            };
            store.BatchSaveAsync(datas)
                .ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.Equal(2, store.GetSuccessCount());
        }

        [Fact]
        public void FindLatestHeaderMustNotNull()
        {
            var store = new MySQLAggregateSnapshotStore();
            store.Initialize("Server=localhost;Database=SnapshotStore;UID=demo;PWD=123456");

            var datas = new AggregateSnapshotData[]{
                 new AggregateSnapshotData("B005", "StockBox", 1, new byte[]{1,2,3,4})
            };
            store.BatchSaveAsync(datas)
                .ConfigureAwait(false).GetAwaiter().GetResult();
            var header = store.FindLatestHeaderAsync("B005", "StockBox")
                .ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.NotNull(header);
            Assert.Equal("B005", header.AggregateRootId);
            Assert.Equal("StockBox", header.AggregateRootTypeName);
            Assert.Equal(1, header.Version);
        }

        [Fact]
        public void FindLatestHeaderMustNull()
        {
            var store = new MySQLAggregateSnapshotStore();
            store.Initialize("Server=localhost;Database=SnapshotStore;UID=demo;PWD=123456");

            var datas = new AggregateSnapshotData[]{
                 new AggregateSnapshotData("B006", "StockBox", 1, new byte[]{1,2,3,4})
            };
            store.BatchSaveAsync(datas)
                .ConfigureAwait(false).GetAwaiter().GetResult();
            var header = store.FindLatestHeaderAsync("B006", "DownGoodsBill")
                .ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.Null(header);
        }

        [Fact]
        public void FindLatestMustNotNull()
        {
            var store = new MySQLAggregateSnapshotStore();
            store.Initialize("Server=localhost;Database=SnapshotStore;UID=demo;PWD=123456");

            var datas = new AggregateSnapshotData[]{
                 new AggregateSnapshotData("B007", "StockBox", 1, new byte[]{1,2,3,4})
            };
            store.BatchSaveAsync(datas)
                .ConfigureAwait(false).GetAwaiter().GetResult();
            var data = store.FindLatestAsync("B007", "StockBox")
                .ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.NotNull(data);
            Assert.Equal("B007", data.AggregateRootId);
            Assert.Equal("StockBox", data.AggregateRootTypeName);
            Assert.Equal(1, data.Version);
            Assert.Equal(4, data.Data.Length);
        }

        [Fact]
        public void FindLatestMustNull()
        {
            var store = new MySQLAggregateSnapshotStore();
            store.Initialize("Server=localhost;Database=SnapshotStore;UID=demo;PWD=123456");

            var datas = new AggregateSnapshotData[]{
                 new AggregateSnapshotData("B008", "StockBox", 1, new byte[]{1,2,3,4})
            };
            store.BatchSaveAsync(datas)
                .ConfigureAwait(false).GetAwaiter().GetResult();
            var data = store.FindLatestAsync("B008", "DownGoodsBill")
                .ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.Null(data);
        }
    }
}
