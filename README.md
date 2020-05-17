# AggregateSnapshotStore
聚合根快照存储接口，用于使用事件源的CQRS，提高C端聚合根实例加载速度。

## 安装

```
dotnet add package AggregateSnapshotStore
```

## 使用流程

- 将需要建快照的聚合根，在订阅领域事件时，将快照头信息入快照请求队列 `IAggregateSnapshotRequestQueue`

- 由 `IAggregateSnapshotRequestFilter` 进行筛选，只保留需要建快照的快照头信息

- 最后通过 `IAggregateSnapshotSaver` 将最近一次快照 + 中间产生的事件流，通过事件重放，产生新的快照，保存到 `IAggregateSnapshotStore`。

## 存储实现

只要实现接口 `IAggregateSnapshotStore`即可。

### AggregateSnapshotStore.SqlServer
基于 SqlServer 的聚合根快照存储实现，支持按聚合根ID作为ShardKey，进行单库水平拆分多表。

- 分片Hash算法采用Crc16。

- 分片的表名后缀为`"_<hash-index>"`,  `hash-index` 范围为 0 ～ tableCount-1

如tableCount=2，则表名分别为 AggregateSnapshot_0、AggregateSnapshot_1

```
dotnet add package AggregateSnapshotStore.SqlServer
```
### AggregateSnapshotStore.MySQL
基于 MySQL 的聚合根快照存储实现，支持按聚合根ID作为ShardKey，进行单库水平拆分多表。

- 分片Hash算法采用Crc16。

- 分片的表名后缀为`"_<hash-index>"`,  `hash-index` 范围为 0 ～ tableCount-1

如tableCount=2，则表名分别为 AggregateSnapshot_0、AggregateSnapshot_1

```
dotnet add package AggregateSnapshotStore.MySQL
```

## 发布历史

### AggregateSnapshotStore

#### 1.0.1（2020/5/16）

- 优化 `DefaultAggregateSnapshotRequestProcessor`： 未运行时阻止入队；停止时，再次处理队列数据，直至清空

#### 1.0.0（2020/5/13）

初版


### AggregateSnapshotStore.SqlServer

#### 1.0.2

- 优化水平拆分表下，不同表并行保存，以提高性能

#### 1.0.1

- 修改水平拆分表的拆分Hash算法，改为crc16

#### 1.0.0

初版

### AggregateSnapshotStore.MySQL

#### 1.0.2

- 优化水平拆分表下，不同表并行保存，以提高性能

#### 1.0.1

- 修改水平拆分表的拆分Hash算法，改为crc16

#### 1.0.0

初版