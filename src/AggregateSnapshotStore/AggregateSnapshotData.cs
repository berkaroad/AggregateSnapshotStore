using System;

namespace AggregateSnapshotStore
{
    /// <summary>
    /// 快照数据
    /// </summary>
    [Serializable]
    public class AggregateSnapshotData : AggregateSnapshotHeader
    {
        /// <summary>
        /// 快照数据
        /// </summary>
        /// <param name="aggregateRootId"></param>
        /// <param name="aggregateRootTypeName"></param>
        /// <param name="version"></param>
        /// <param name="data"></param>
        public AggregateSnapshotData(string aggregateRootId, string aggregateRootTypeName, int version, byte[] data)
            : base(aggregateRootId, aggregateRootTypeName, version)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            Data = data;
        }

        /// <summary>
        /// 数据
        /// </summary>
        /// <value></value>
        public byte[] Data { get; private set; }

        /// <summary>
        /// 字符串形式
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"[AggregateRootId={AggregateRootId},AggregateRootTypeName={AggregateRootTypeName},Version={Version},DataLength={Data?.Length}]";
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
