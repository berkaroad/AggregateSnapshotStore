namespace AggregateSnapshotStore.SqlServer
{
    internal class SnapshotData
    {
        public string AggregateRootId { get; set; }

        public string AggregateRootTypeName { get; set; }

        public int Version { get; set; }

        public byte[] Data { get; set; }
    }
}
