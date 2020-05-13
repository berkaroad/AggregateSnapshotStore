using System.Collections.Generic;

namespace AggregateSnapshotStore
{
    /// <summary>
    /// 快照请求筛选器，用于筛选出需要建快照的快照头信息
    /// </summary>
    public interface IAggregateSnapshotRequestFilter
    {
        /// <summary>
        /// 筛选
        /// </summary>
        /// <param name="snapshotHeaders"></param>
        /// <returns></returns>
        IEnumerable<AggregateSnapshotHeader> Filter(IEnumerable<AggregateSnapshotHeader> snapshotHeaders);
    }
}
