using System.Collections.Generic;
using System.Threading.Tasks;

namespace AggregateSnapshotStore
{
    /// <summary>
    /// 快照保存器，将经过筛选后的快照头信息，结合现有最新快照和事件流产生新的快照，然后保存
    /// </summary>
    public interface IAggregateSnapshotSaver
    {
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="snapshotHeaders">快照头信息</param>
        /// <returns></returns>
        Task SaveAsync(IEnumerable<AggregateSnapshotHeader> snapshotHeaders);
    }
}
