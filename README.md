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
基于SqlServer的聚合根快照存储实现。

```
dotnet add package AggregateSnapshotStore.SqlServer
```

## 发布历史

### 1.0.0
- 初始版本