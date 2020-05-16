using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AggregateSnapshotStore.Tests
{
    public class MockupAggregateSnapshotSaver : IAggregateSnapshotSaver
    {
        private IAggregateSnapshotStore _store;

        public MockupAggregateSnapshotSaver() { }

        public void Initialize(IAggregateSnapshotStore store)
        {
            _store = store;
        }

        public Task SaveAsync(IEnumerable<AggregateSnapshotHeader> snapshotHeaders)
        {
            Console.WriteLine($"Save count: {snapshotHeaders.Count()}.");
            var snapshotDataList = new List<AggregateSnapshotData>();
            foreach (var snapshotHeader in snapshotHeaders)
            {
                var snapshotData = new AggregateSnapshotData(snapshotHeader.AggregateRootId, snapshotHeader.AggregateRootTypeName, snapshotHeader.Version, new byte[] { 1 });
                snapshotDataList.Add(snapshotData);
            }
            return _store.BatchSaveAsync(snapshotDataList);
        }
    }
}