# AggregateSnapshotStore.SqlServer
基于 SqlServer 的聚合根快照存储实现，支持按聚合根ID作为ShardKey，进行单库水平拆分多表。

- 分片Hash算法采用Crc16。

- 分片的表名后缀为`"_<hash-index>"`,  `hash-index` 范围为 0 ～ tableCount-1

如tableCount=2，则表名分别为 AggregateSnapshot_0、AggregateSnapshot_1

```
dotnet add package AggregateSnapshotStore.SqlServer
```

## 发布历史
### 1.0.3
1）修复CRC16算法

### 1.0.2
1）优化水平拆分表下，不同表并行保存，以提高性能

### 1.0.1
1）修改水平拆分表的拆分Hash算法，改为crc16

### 1.0.0
1）初版