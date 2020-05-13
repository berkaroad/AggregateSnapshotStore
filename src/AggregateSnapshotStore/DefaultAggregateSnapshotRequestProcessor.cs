using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AggregateSnapshotStore
{
    /// <summary>
    /// 快照请求处理器
    /// </summary>
    public class DefaultAggregateSnapshotRequestProcessor : IAggregateSnapshotRequestQueue
    {
        private Timer _timer = null;
        private ConcurrentDictionary<string, AggregateSnapshotHeader> _waitForProcessDict = new ConcurrentDictionary<string, AggregateSnapshotHeader>();
        private IAggregateSnapshotRequestFilter _filter;
        private IAggregateSnapshotSaver _saver;
        private TimeSpan _interval;
        private volatile int _isRunning;

        /// <summary>
        /// 快照请求处理器
        /// </summary>
        public DefaultAggregateSnapshotRequestProcessor()
        {
            _timer = new Timer(BackendExecute, null, Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="filter"></param>
        /// <param name="saver"></param>
        public void Initialize(TimeSpan interval, IAggregateSnapshotRequestFilter filter, IAggregateSnapshotSaver saver)
        {
            _interval = interval;
            _filter = filter;
            _saver = saver;
        }

        /// <summary>
        /// 启动
        /// </summary>
        public void Start()
        {
            if (Interlocked.CompareExchange(ref _isRunning, 1, 0) == 0)
            {
                _timer.Change(TimeSpan.FromSeconds(1), _interval);
            }
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            Interlocked.CompareExchange(ref _isRunning, 0, 1);
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        /// 是否正在运行
        /// </summary>
        /// <returns></returns>
        public bool IsRunning()
        {
            return _isRunning == 1;
        }

        /// <summary>
        /// 入队
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public void Enqueue(AggregateSnapshotHeader data)
        {
            _waitForProcessDict.AddOrUpdate(data.AggregateRootId, data, (k, origin) => origin.Version > data.Version ? origin : data);
        }

        private void BackendExecute(object status)
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
            if (_waitForProcessDict.Count > 0)
            {
                try
                {
                    var dict = new Dictionary<string, AggregateSnapshotHeader>();
                    foreach (var key in _waitForProcessDict.Keys.ToArray())
                    {
                        _waitForProcessDict.TryRemove(key, out AggregateSnapshotHeader val);
                        if (val != null)
                        {
                            dict.Add(key, val);
                        }
                    }
                    IEnumerable<AggregateSnapshotHeader> snapshotHeaders = dict.Values;

                    if (_filter != null)
                    {
                        snapshotHeaders = _filter.Filter(dict.Values);
                    }
                    _saver?.SaveAsync(snapshotHeaders);
                }
                catch { }
            }
            _timer.Change(_interval, _interval);
        }
    }
}
