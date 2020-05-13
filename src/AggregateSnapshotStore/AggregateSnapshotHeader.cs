using System;

namespace AggregateSnapshotStore
{
    /// <summary>
    /// 快照头信息
    /// </summary>
    public class AggregateSnapshotHeader
    {
        /// <summary>
        /// 快照头信息
        /// </summary>
        /// <param name="aggregateRootId">聚合根ID</param>
        /// <param name="aggregateRootTypeName">聚合根类型名</param>
        /// <param name="version">版本号</param>
        public AggregateSnapshotHeader(string aggregateRootId, string aggregateRootTypeName, int version)
        {
            if (string.IsNullOrEmpty(aggregateRootId))
            {
                throw new ArgumentNullException(nameof(aggregateRootId));
            }
            if (string.IsNullOrEmpty(aggregateRootTypeName))
            {
                throw new ArgumentNullException(nameof(aggregateRootTypeName));
            }
            if (version <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(version));
            }
            AggregateRootId = aggregateRootId;
            AggregateRootTypeName = aggregateRootTypeName;
            Version = version;
        }

        /// <summary>
        /// 聚合根ID
        /// </summary>
        public string AggregateRootId { get; private set; }

        /// <summary>
        /// 聚合根类型名
        /// </summary>
        public string AggregateRootTypeName { get; private set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public int Version { get; private set; }

        /// <summary>
        /// 字符串形式
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"[AggregateRootId={AggregateRootId},AggregateRootTypeName={AggregateRootTypeName},Version={Version}]";
        }

        /// <summary>
        /// 相等比较
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return obj != null && obj.ToString() == ToString();
        }

        /// <summary>
        /// 获取哈希码
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}
