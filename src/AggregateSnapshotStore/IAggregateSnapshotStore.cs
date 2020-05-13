using System.Collections.Generic;
using System.Threading.Tasks;

namespace AggregateSnapshotStore
{
    /// <summary>
    /// 快照存储
    /// </summary>
    public interface IAggregateSnapshotStore
    {
        /// <summary>
        /// 查找最近一次快照头信息
        /// </summary>
        /// <param name="aggregateRootId">聚合根ID</param>
        /// <param name="aggregateRootTypeName">聚合根类型名</param>
        /// <returns></returns>
        Task<AggregateSnapshotHeader> FindLatestHeaderAsync(string aggregateRootId, string aggregateRootTypeName);

        /// <summary>
        /// 查找最近一次快照信息
        /// </summary>
        /// <param name="aggregateRootId">聚合根ID</param>
        /// <param name="aggregateRootTypeName">聚合根类型名</param>
        /// <returns></returns>
        Task<AggregateSnapshotData> FindLatestAsync(string aggregateRootId, string aggregateRootTypeName);

        /// <summary>
        /// 批量保存
        /// </summary>
        /// <param name="snapshotDatas">快照数据</param>
        /// <returns></returns>
        Task BatchSaveAsync(IEnumerable<AggregateSnapshotData> snapshotDatas);
    }
}
