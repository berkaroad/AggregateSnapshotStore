using Xunit;

namespace AggregateSnapshotStore.Redis.Tests
{
    public class RedisAggregateSnapshotStoreTests
    {
        private string _redisConfiguration = "127.0.0.1:6379,syncTimeout=3000,defaultDatabase=0,name=demo,allowAdmin=false";

        [Fact]
        public void BatchInsertMustSuccess()
        {
            var store = new RedisAggregateSnapshotStore();
            store.Initialize(_redisConfiguration, "demo");
            var datas = new AggregateSnapshotData[]{
                new AggregateSnapshotData("B001", "StockBox", 1, new byte[]{1,2,3,4}),
                new AggregateSnapshotData("B002", "StockBox", 1, new byte[]{2,2,3,4})
            };
            try
            {
                store.BatchSaveAsync(datas)
                    .ConfigureAwait(false).GetAwaiter().GetResult();
                Assert.Equal(2, store.GetSuccessCount());
            }
            finally
            {
                store.RemoveKeyAsync("B001").Wait();
                store.RemoveKeyAsync("B002").Wait();
            }
        }

        [Fact]
        public void BatchUpdateMustSuccess()
        {
            var store = new RedisAggregateSnapshotStore();
            store.Initialize(_redisConfiguration, "demo");
            var datas = new AggregateSnapshotData[]{
                new AggregateSnapshotData("B003", "StockBox", 1, new byte[]{1,2,3,4}),
                new AggregateSnapshotData("B004", "StockBox", 1, new byte[]{2,2,3,4})
            };
            try
            {
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
            finally
            {
                store.RemoveKeyAsync("B003").Wait();
                store.RemoveKeyAsync("B004").Wait();
            }
        }

        [Fact]
        public void FindLatestHeaderMustNotNull()
        {
            var store = new RedisAggregateSnapshotStore();
            store.Initialize(_redisConfiguration, "demo");

            var datas = new AggregateSnapshotData[]{
                 new AggregateSnapshotData("B005", "StockBox", 1, new byte[]{1,2,3,4})
            };
            try
            {
                store.BatchSaveAsync(datas)
                    .ConfigureAwait(false).GetAwaiter().GetResult();
                var header = store.FindLatestHeaderAsync("B005", "StockBox")
                    .ConfigureAwait(false).GetAwaiter().GetResult();
                Assert.NotNull(header);
                Assert.Equal("B005", header.AggregateRootId);
                Assert.Equal("StockBox", header.AggregateRootTypeName);
                Assert.Equal(1, header.Version);
            }
            finally
            {
                store.RemoveKeyAsync("B005").Wait();
            }
        }

        [Fact]
        public void FindLatestHeaderMustNull()
        {
            var store = new RedisAggregateSnapshotStore();
            store.Initialize(_redisConfiguration, "demo");

            var datas = new AggregateSnapshotData[]{
                 new AggregateSnapshotData("B006", "StockBox", 1, new byte[]{1,2,3,4})
            };
            try
            {
                store.BatchSaveAsync(datas)
                    .ConfigureAwait(false).GetAwaiter().GetResult();
                var header = store.FindLatestHeaderAsync("B006", "DownGoodsBill")
                    .ConfigureAwait(false).GetAwaiter().GetResult();
                Assert.Null(header);
            }
            finally
            {
                store.RemoveKeyAsync("B006").Wait();
            }
        }

        [Fact]
        public void FindLatestMustNotNull()
        {
            var store = new RedisAggregateSnapshotStore();
            store.Initialize(_redisConfiguration, "demo");

            var datas = new AggregateSnapshotData[]{
                 new AggregateSnapshotData("B007", "StockBox", 1, new byte[]{1,2,3,4})
            };
            try
            {
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
            finally
            {
                store.RemoveKeyAsync("B007").Wait();
            }
        }

        [Fact]
        public void FindLatestMustNull()
        {
            var store = new RedisAggregateSnapshotStore();
            store.Initialize(_redisConfiguration, "demo");

            var datas = new AggregateSnapshotData[]{
                 new AggregateSnapshotData("B008", "StockBox", 1, new byte[]{1,2,3,4})
            };
            try
            {
                store.BatchSaveAsync(datas)
                    .ConfigureAwait(false).GetAwaiter().GetResult();
                var data = store.FindLatestAsync("B008", "DownGoodsBill")
                    .ConfigureAwait(false).GetAwaiter().GetResult();
                Assert.Null(data);
            }
            finally
            {
                store.RemoveKeyAsync("B008").Wait();
            }
        }
    }
}
