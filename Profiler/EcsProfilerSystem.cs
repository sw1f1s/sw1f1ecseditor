using System;
using System.Diagnostics;

namespace Sw1f1.Ecs.Editor.Profiler {
    public class EcsProfilerSystem : IDisposable {
        public readonly ISystem System;
        public string Name => System.GetType().Name;
        
        public double ExecutionTimeMs {get; private set;}
        public long Allocations {get; private set;}
        
        private Stopwatch _stopwatch;
        private long _memoryBefore;

        public EcsProfilerSystem(ISystem system) {
            System = system;
        }

        public void StartRecord() {
            _stopwatch = Stopwatch.StartNew();
            _memoryBefore = GC.GetTotalMemory(false);
        }
        
        public void StopRecord() {
            _stopwatch?.Stop();
            ExecutionTimeMs = _stopwatch?.Elapsed.TotalMilliseconds ?? 0;
            long memoryAfter = GC.GetTotalMemory(false);
            Allocations = memoryAfter - _memoryBefore;
        }

        public void Dispose() {
            
        }
    }
}