using System.Collections.Generic;
using System.Threading.Tasks;

namespace AggregateSnapshotStore.Tests
{
    public class MockupAggregateSnapshotStore : IAggregateSnapshotStore
    {
        private Dictionary<string, AggregateSnapshotData> _coll = new Dictionary<string, AggregateSnapshotData>();

        public Task BatchSaveAsync(IEnumerable<AggregateSnapshotData> snapshotDatas)
        {
            foreach (var snapshotData in snapshotDatas)
            {
                if (_coll.ContainsKey(snapshotData.AggregateRootId))
                {
                    if (_coll[snapshotData.AggregateRootId].Version < snapshotData.Version)
                    {
                        _coll[snapshotData.AggregateRootId] = snapshotData;
                    }
                }
                else
                {
                    _coll.Add(snapshotData.AggregateRootId, snapshotData);
                }
            }
            return Task.CompletedTask;
        }

        public Task<AggregateSnapshotData> FindLatestAsync(string aggregateRootId, string aggregateRootTypeName)
        {
            if (_coll.ContainsKey(aggregateRootId))
            {
                return Task.FromResult(_coll[aggregateRootId]);
            }
            return Task.FromResult((AggregateSnapshotData)null);
        }

        public Task<AggregateSnapshotHeader> FindLatestHeaderAsync(string aggregateRootId, string aggregateRootTypeName)
        {
            if (_coll.ContainsKey(aggregateRootId))
            {
                return Task.FromResult((AggregateSnapshotHeader)_coll[aggregateRootId]);
            }
            return Task.FromResult((AggregateSnapshotHeader)null);
        }

        public IEnumerable<AggregateSnapshotData> GetDatas()
        {
            return _coll.Values;
        }
    }
}