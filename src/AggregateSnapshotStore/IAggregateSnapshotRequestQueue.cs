namespace AggregateSnapshotStore
{
    /// <summary>
    /// 快照请求队列，将需要建快照的聚合根，在订阅领域事件后加入进来
    /// </summary>
    public interface IAggregateSnapshotRequestQueue
    {
        /// <summary>
        /// 入队
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        void Enqueue(AggregateSnapshotHeader data);
    }
}
