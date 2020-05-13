all: pack

pack: build
	mkdir -p `pwd`/packages
	dotnet pack -c Release `pwd`/src/AggregateSnapshotStore/
	mv `pwd`/src/AggregateSnapshotStore/bin/Release/*.nupkg `pwd`/packages/
	dotnet pack -c Release `pwd`/src/AggregateSnapshotStore.SqlServer/
	mv `pwd`/src/AggregateSnapshotStore.SqlServer/bin/Release/*.nupkg `pwd`/packages/

build:
	dotnet build -c Release `pwd`/src/AggregateSnapshotStore/
	dotnet build -c Release `pwd`/src/AggregateSnapshotStore.SqlServer/
