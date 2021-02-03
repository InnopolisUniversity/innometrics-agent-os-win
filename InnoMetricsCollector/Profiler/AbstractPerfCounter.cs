using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace InnoMetricsCollector.Profiler
{
    public abstract class AbstractPerfCounter<TCounterType> : IDisposable
    {
        private readonly Dictionary<TCounterType, PerformanceCounter> _counters =
            new Dictionary<TCounterType, PerformanceCounter>();

        protected AbstractPerfCounter(string category)
        {
            Category = category;
        }

        public string Category { get; }

        public void Dispose()
        {
            var toDispose = _counters.Values.ToList();
            _counters.Clear();
            toDispose.ForEach(c => c.Dispose());
        }

        protected void Initialize(string instance, params TCounterType[] types)
        {
            Dispose(); // Dispose any previous counters
            foreach (var type in types)
                _counters[type] = new PerformanceCounter(Category, CounterTypeToString(type), instance, true);
        }

        // Converts counter type to counterName under Category
        protected abstract string CounterTypeToString(TCounterType type);

        public float Pop(TCounterType type)
        {
            try
            {
                if (!_counters.ContainsKey(type))
                {
                    if (_counters.Count > 0)
                        // Can obtain pName from a neighboring counter
                        _counters[type] = new PerformanceCounter(Category, CounterTypeToString(type),
                            _counters.First().Value.InstanceName);
                    else
                        throw new InvalidOperationException("Counter must be initialized");
                }
                _counters[type].NextValue();
                return _counters[type].NextValue();
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}