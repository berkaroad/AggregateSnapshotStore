using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AggregateSnapshotStore.Tests
{
    public class DefaultAggregateSnapshotRequestProcessorTests
    {
        [Fact]
        public void Enqueue()
        {
            var store = new MockupAggregateSnapshotStore();
            var filter = new DefaultAggregateSnapshotRequestFilter();
            filter.Initialize(5, store);
            var saver = new MockupAggregateSnapshotSaver();
            saver.Initialize(store);
            var processor = new DefaultAggregateSnapshotRequestProcessor();
            processor.Initialize(TimeSpan.FromSeconds(1), filter, saver);
            processor.Start();

            var taskList = new List<Task>();
            var aggregateCount = 100;
            var versionCount = 100;
            var random = new Random();
            for (var i = 1; i <= aggregateCount; i++)
            {
                for (var j = 1; j <= versionCount; j++)
                {
                    var task = new Task((state) =>
                    {
                        var aggregateRootId = $"B100{((dynamic)state).i}";
                        var version = (int)((dynamic)state).j;
                        processor.Enqueue(new AggregateSnapshotHeader(aggregateRootId, "StockBox", version));
                        // Console.WriteLine($"aggregateRootId={aggregateRootId},version={version}");
                    }, new { i, j });

                    // 模拟一个聚合根实例每隔一段时间发布事件
                    taskList.Add(Task.Run(async () =>
                    {
                        await Task.Delay(random.Next(900, 9000)).ContinueWith(lastTask =>
                        {
                            task.Start();
                        });
                    }));
                }
            }
            Console.WriteLine("wait task...");
            Task.WaitAll(taskList.ToArray());
            Thread.Sleep(1000);
            Console.WriteLine("stopping...");
            processor.Stop();
            Assert.Equal(aggregateCount, store.GetDatas().Count());
        }
    }
}
