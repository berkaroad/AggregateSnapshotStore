using System.Collections.Generic;

namespace AggregateSnapshotStore
{
    /// <summary>
    /// 默认快照请求过滤器
    /// </summary>
    public class DefaultAggregateSnapshotRequestFilter : IAggregateSnapshotRequestFilter
    {
        private IAggregateSnapshotStore _snapshotStore;
        private int _minVersionDiffNum;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="minVersionDiffNum">触发快照的最小版本差异数</param>
        /// <param name="snapshotStore">快照存储</param>
        public void Initialize(int minVersionDiffNum, IAggregateSnapshotStore snapshotStore)
        {
            _snapshotStore = snapshotStore;
            _minVersionDiffNum = 20;
            _minVersionDiffNum = minVersionDiffNum;
        }

        /// <summary>
        /// 筛选
        /// </summary>
        /// <param name="snapshotHeaders">快照头信息</param>
        /// <returns></returns>
        public IEnumerable<AggregateSnapshotHeader> Filter(IEnumerable<AggregateSnapshotHeader> snapshotHeaders)
        {
            foreach (var snapshotHeader in snapshotHeaders)
            {
                var lastSnapshot = _snapshotStore.FindLatestHeaderAsync(snapshotHeader.AggregateRootId, snapshotHeader.AggregateRootTypeName).ConfigureAwait(false).GetAwaiter().GetResult();
                var lastVersion = lastSnapshot == null ? 0 : lastSnapshot.Version;
                if (snapshotHeader.Version - lastVersion >= _minVersionDiffNum)
                {
                    yield return snapshotHeader;
                }
            }
        }
    }
}
