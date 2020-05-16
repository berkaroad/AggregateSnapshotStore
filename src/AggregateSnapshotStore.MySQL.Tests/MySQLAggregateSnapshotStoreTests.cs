using Xunit;

namespace AggregateSnapshotStore.MySQL.Tests
{
    public class MySQLAggregateSnapshotStoreTests
    {
        private MySQLAggregateSnapshotStore _store;
        
        public MySQLAggregateSnapshotStoreTests()
        {
            _store = new MySQLAggregateSnapshotStore();
            _store.Initialize("Server=localhost;Database=SnapshotStore;UID=demo;PWD=123456");
         }

        [Fact]
        public void BatchInsertMustSuccess()
        {
            var datas = new AggregateSnapshotData[]{
                 new AggregateSnapshotData("B001", "StockBox", 1, new byte[]{1,2,3,4}),
                 new AggregateSnapshotData("B002", "StockBox", 1, new byte[]{2,2,3,4})
            };
            _store.BatchSaveAsync(datas)
                .ConfigureAwait(false).GetAwaiter().GetResult();
        }

        [Fact]
        public void BatchUpdateMustSuccess()
        {
            var datas = new AggregateSnapshotData[]{
                 new AggregateSnapshotData("B003", "StockBox", 1, new byte[]{1,2,3,4}),
                 new AggregateSnapshotData("B004", "StockBox", 1, new byte[]{2,2,3,4})
            };
            _store.BatchSaveAsync(datas)
                .ConfigureAwait(false).GetAwaiter().GetResult();

            datas = new AggregateSnapshotData[]{
                 new AggregateSnapshotData("B003", "StockBox", 2, new byte[]{1,2,3,4,5}),
                 new AggregateSnapshotData("B004", "StockBox", 2, new byte[]{2,2,3,4,5})
            };
            _store.BatchSaveAsync(datas)
                .ConfigureAwait(false).GetAwaiter().GetResult();
        }

        [Fact]
        public void FindLatestHeaderMustNotNull()
        {
            var datas = new AggregateSnapshotData[]{
                 new AggregateSnapshotData("B005", "StockBox", 1, new byte[]{1,2,3,4})
            };
            _store.BatchSaveAsync(datas)
                .ConfigureAwait(false).GetAwaiter().GetResult();
            var header = _store.FindLatestHeaderAsync("B005", "StockBox")
                .ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.NotNull(header);
            Assert.Equal("B005", header.AggregateRootId);
            Assert.Equal("StockBox", header.AggregateRootTypeName);
            Assert.Equal(1, header.Version);
        }

        [Fact]
        public void FindLatestHeaderMustNull()
        {
            var datas = new AggregateSnapshotData[]{
                 new AggregateSnapshotData("B006", "StockBox", 1, new byte[]{1,2,3,4})
            };
            _store.BatchSaveAsync(datas)
                .ConfigureAwait(false).GetAwaiter().GetResult();
            var header = _store.FindLatestHeaderAsync("B006", "DownGoodsBill")
                .ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.Null(header);
        }

        [Fact]
        public void FindLatestMustNotNull()
        {
            var datas = new AggregateSnapshotData[]{
                 new AggregateSnapshotData("B007", "StockBox", 1, new byte[]{1,2,3,4})
            };
            _store.BatchSaveAsync(datas)
                .ConfigureAwait(false).GetAwaiter().GetResult();
            var data = _store.FindLatestAsync("B007", "StockBox")
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
            var datas = new AggregateSnapshotData[]{
                 new AggregateSnapshotData("B008", "StockBox", 1, new byte[]{1,2,3,4})
            };
            _store.BatchSaveAsync(datas)
                .ConfigureAwait(false).GetAwaiter().GetResult();
            var data = _store.FindLatestAsync("B008", "DownGoodsBill")
                .ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.Null(data);
        }
    }
}
