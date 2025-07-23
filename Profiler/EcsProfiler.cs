using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sw1f1.Ecs.Editor.Profiler {
    public class EcsProfiler : IDisposable {
        private const int LOG_LENGTH = 999;
        private readonly Dictionary<IWorld, Dictionary<ISystem, EcsProfilerSystem>> _worldMap;
        private readonly Dictionary<IWorld, EcsProfilerSystem> _currentSystem;
        private readonly Dictionary<IWorld, List<ComponentChangeLog>> _componentChangeLogs;
        private readonly Dictionary<IWorld, StringBuilder> _componentChangeLogBuilder;

        public event Action<IWorld> OnChangeEntities;
        public event Action<IWorld, ComponentChangeLog> OnAddComponentChangeLog;
        public event Action<IWorld> OnClearComponentChangeLog;
        
        public IEnumerable<string> WorldNames => _worldMap.Keys.Select(x => $"World {x.Id}");
        public bool IsLogPause { get; set; }
        
        public EcsProfiler() {
            _worldMap = new Dictionary<IWorld, Dictionary<ISystem, EcsProfilerSystem>>();
            _currentSystem = new Dictionary<IWorld, EcsProfilerSystem>();
            _componentChangeLogs = new Dictionary<IWorld, List<ComponentChangeLog>>();
            _componentChangeLogBuilder = new Dictionary<IWorld, StringBuilder>();
            
            foreach (var world in WorldBuilder.Worlds) {
                RegisterWorld(world);
            }
            
            foreach (var systems in Systems.SystemsMap) {
                RegisterSystems(systems.Key, systems.Value);
            }

            WorldBuilder.OnWorldCreated += RegisterWorld;
            WorldBuilder.OnWorldDestroyed += UnregisterWorld;
            Systems.OnAddSystem += RegisterSystem;
            Systems.OnStartSystemExecute += StartProfilerSystem;
            Systems.OnEndSystemExecute += EndProfilerSystem;
        }
        
        public IWorld GetWorldByName(string name) {
            return _worldMap.Keys.FirstOrDefault(x => $"World {x.Id}".Equals(name));
        }
        
        public IEnumerable<EcsProfilerSystem> GetSystems(IWorld world) {
            return _worldMap[world].Values;
        }
        
        public IEnumerable<ComponentChangeLog> GetComponentChangeLogs(IWorld world) {
            return _componentChangeLogs[world];
        }
        
        public StringBuilder GetFullComponentChangeLogs(IWorld world) {
            return _componentChangeLogBuilder[world];
        }


        public void ClearLog(IWorld world) {
            if (_componentChangeLogs.TryGetValue(world, out var logs)) {
                logs.Clear();   
            }
            
            if (_componentChangeLogBuilder.TryGetValue(world, out var stringBuilder)) {
                stringBuilder.Clear();   
            }
            
            OnClearComponentChangeLog?.Invoke(world);
        }
        
        private void RegisterWorld(IWorld world) {
            if (!_worldMap.ContainsKey(world)) {
                _worldMap[world] = new Dictionary<ISystem, EcsProfilerSystem>();
                _componentChangeLogs[world] = new List<ComponentChangeLog>();
                _componentChangeLogBuilder[world] = new StringBuilder();
                _currentSystem[world] = null;
                
                world.OnCreateEntity += CreateEntity;
                world.OnCopyEntity += CopyEntity;
                world.OnDestroyEntity += DestroyEntity;
                
                world.OnAddComponent += AddComponent;
                world.OnRemoveComponent += RemoveComponent;
            }
        }

        private void RegisterSystems(IWorld world, ISystems systems) {
            RegisterWorld(world);
            foreach (var system in systems.AllSystems) {
                RegisterSystem(world, system);
            }
        }

        private void RegisterSystem(IWorld world, ISystem system) {
            RegisterWorld(world);
            _worldMap[world].Add(system, new EcsProfilerSystem(system));
        }

        private void StartProfilerSystem(IWorld world, ISystem system) {
            UnityEngine.Profiling.Profiler.BeginSample(system.GetType().Name);
            _currentSystem[world] = _worldMap[world][system];
            _worldMap[world][system].StartRecord();
        }
        
        private void EndProfilerSystem(IWorld world, ISystem system) {
            _currentSystem[world] = null;
            _worldMap[world][system].StopRecord();
            UnityEngine.Profiling.Profiler.EndSample();
        }
        private void CreateEntity(IWorld world, Entity entity) {
            string systemName = "";
            if (_currentSystem.TryGetValue(world, out var value)) {
                systemName = value?.Name ?? "";
            }

            if (!IsLogPause) {
                var log = ComponentChangeLog.CreateEntity(entity, systemName, DateTime.Now);
                _componentChangeLogs[world].Insert(0, log);
                _componentChangeLogBuilder[world].Append("\n");
                _componentChangeLogBuilder[world].Append(log.ToString()); 
                ClearLogIfLimit(world);
                OnAddComponentChangeLog?.Invoke(world, log);
            }
            
            OnChangeEntities?.Invoke(world);
        }
        
        private void CopyEntity(IWorld world, Entity entity) {
            string systemName = "";
            if (_currentSystem.TryGetValue(world, out var value)) {
                systemName = value?.Name ?? "";
            }

            if (!IsLogPause) {
                var log = ComponentChangeLog.CloneEntity(entity, systemName, DateTime.Now);
                _componentChangeLogs[world].Insert(0, log);
                _componentChangeLogBuilder[world].Append("\n");
                _componentChangeLogBuilder[world].Append(log.ToString());
                ClearLogIfLimit(world);
                OnAddComponentChangeLog?.Invoke(world, log);
            }
            
            OnChangeEntities?.Invoke(world);
        }
        
        private void DestroyEntity(IWorld world, Entity entity) {
            string systemName = "";
            if (_currentSystem.TryGetValue(world, out var value)) {
                systemName = value?.Name ?? "";
            }

            if (!IsLogPause) {
                var log = ComponentChangeLog.DestroyEntity(entity, systemName, DateTime.Now);
                _componentChangeLogs[world].Insert(0, log);
                _componentChangeLogBuilder[world].Append("\n");
                _componentChangeLogBuilder[world].Append(log.ToString());
                ClearLogIfLimit(world);
                OnAddComponentChangeLog?.Invoke(world, log);
            }
            
            OnChangeEntities?.Invoke(world);
        }
        
        private void AddComponent(IWorld world, Entity entity, Type componentType) {
            string systemName = "";
            if (_currentSystem.TryGetValue(world, out var value)) {
                systemName = value?.Name ?? "";
            }

            if (!IsLogPause) {
                var log = ComponentChangeLog.AddComponent(entity, systemName, componentType.Name, DateTime.Now);
                _componentChangeLogs[world].Insert(0, log);
                _componentChangeLogBuilder[world].Append("\n");
                _componentChangeLogBuilder[world].Append(log.ToString());
                ClearLogIfLimit(world);
                OnAddComponentChangeLog?.Invoke(world, log);
            }
        }
        
        private void RemoveComponent(IWorld world, Entity entity, Type componentType) {
            string systemName = "";
            if (_currentSystem.TryGetValue(world, out var value)) {
                systemName = value?.Name ?? "";
            }

            if (!IsLogPause) {
                var log = ComponentChangeLog.RemoveComponent(entity, systemName, componentType.Name, DateTime.Now);
                _componentChangeLogs[world].Insert(0, log);
                _componentChangeLogBuilder[world].Append("\n");
                _componentChangeLogBuilder[world].Append(log.ToString());
                ClearLogIfLimit(world);
                OnAddComponentChangeLog?.Invoke(world, log);
            }
        }

        private void ClearLogIfLimit(IWorld world) {
            if (_componentChangeLogs[world].Count > LOG_LENGTH) {
                _componentChangeLogs[world].RemoveAt(_componentChangeLogs[world].Count - 1);
            }
        }

        private void UnregisterWorld(IWorld world) {
            if (!_worldMap.TryGetValue(world, out var value)) {
                return;
            }
            
            foreach (var system in value) {
                system.Value.Dispose();
            }
            
            world.OnCreateEntity -= CreateEntity;
            world.OnCopyEntity -= CopyEntity;
            world.OnDestroyEntity -= DestroyEntity;
                
            world.OnAddComponent -= AddComponent;
            world.OnRemoveComponent -= RemoveComponent;
            
            _componentChangeLogBuilder[world].Clear();
            _componentChangeLogs[world].Clear();
            _worldMap[world].Clear();

            _currentSystem.Remove(world);
            _componentChangeLogs.Remove(world);
            _componentChangeLogBuilder.Remove(world);
            _worldMap.Remove(world);
        }

        public void Dispose() {
            WorldBuilder.OnWorldCreated -= RegisterWorld;
            WorldBuilder.OnWorldDestroyed -= UnregisterWorld;
            Systems.OnAddSystem -= RegisterSystem;
            Systems.OnStartSystemExecute -= StartProfilerSystem;
            Systems.OnEndSystemExecute -= EndProfilerSystem;
            
            foreach (var world in _worldMap) {
                foreach (var system in world.Value) {
                    system.Value.Dispose();
                }
                
                world.Key.OnCreateEntity += CreateEntity;
                world.Key.OnCopyEntity += CopyEntity;
                world.Key.OnDestroyEntity += DestroyEntity;
                
                world.Key.OnAddComponent += AddComponent;
                world.Key.OnRemoveComponent += RemoveComponent;
            }
            
            _worldMap.Clear();
            _componentChangeLogs.Clear();
            _componentChangeLogBuilder.Clear();
            _currentSystem.Clear();
        }
    }
}
