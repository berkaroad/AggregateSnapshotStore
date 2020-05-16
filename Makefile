all: pack

pack: build
	mkdir -p `pwd`/packages
	dotnet pack -c Release `pwd`/src/AggregateSnapshotStore/
	mv `pwd`/src/AggregateSnapshotStore/bin/Release/*.nupkg `pwd`/packages/
	dotnet pack -c Release `pwd`/src/AggregateSnapshotStore.SqlServer/
	mv `pwd`/src/AggregateSnapshotStore.SqlServer/bin/Release/*.nupkg `pwd`/packages/
	dotnet pack -c Release `pwd`/src/AggregateSnapshotStore.MySQL/
	mv `pwd`/src/AggregateSnapshotStore.MySQL/bin/Release/*.nupkg `pwd`/packages/

test:
	dotnet test `pwd`/src/AggregateSnapshotStore.MySQL.Tests
	#dotnet test `pwd`/src/AggregateSnapshotStore.SqlServer.Tests

build:
	dotnet build -c Release `pwd`/src/AggregateSnapshotStore/
	dotnet build -c Release `pwd`/src/AggregateSnapshotStore.SqlServer/
	dotnet build -c Release `pwd`/src/AggregateSnapshotStore.MySQL/
