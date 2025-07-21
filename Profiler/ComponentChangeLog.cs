using System;

namespace Sw1f1.Ecs.Editor.Profiler {
    public readonly struct ComponentChangeLog {
        private readonly Entity _entity;
        private readonly string _systemName;
        private readonly string _componentName;
        private readonly DateTime _timeStamp;
        private readonly string _action;

        private ComponentChangeLog(Entity entity, string systemName, string componentName, DateTime timeStamp, string action) {
            _entity = entity;
            _systemName = systemName;
            _componentName = componentName;
            _timeStamp = timeStamp;
            _action = action;
        }

        public static ComponentChangeLog AddComponent(Entity entity, string systemName, string componentName, DateTime timeStamp) {
            return new ComponentChangeLog(entity, systemName, componentName, timeStamp, "Add");
        }
        
        public static ComponentChangeLog RemoveComponent(Entity entity, string systemName, string componentName, DateTime timeStamp) {
            return new ComponentChangeLog(entity, systemName, componentName, timeStamp, "Remove");
        }
        
        public static ComponentChangeLog CreateEntity(Entity entity, string systemName, DateTime timeStamp) {
            return new ComponentChangeLog(entity, systemName, "", timeStamp, "Create");
        }
        
        public static ComponentChangeLog CloneEntity(Entity entity, string systemName, DateTime timeStamp) {
            return new ComponentChangeLog(entity, systemName, "", timeStamp, "Clone");
        }
        
        public static ComponentChangeLog DestroyEntity(Entity entity, string systemName, DateTime timeStamp) {
            return new ComponentChangeLog(entity, systemName, "", timeStamp, "Destroy");
        }

        public override string ToString() {
            if (string.IsNullOrEmpty(_systemName) && string.IsNullOrEmpty(_componentName)) {
                return $"[{_timeStamp:HH:mm:ss}] {_action} {_entity}";
            }
            
            if (string.IsNullOrEmpty(_systemName)) {
                return $"[{_timeStamp:HH:mm:ss}] {_action} {_componentName} in {_entity}";
            }
            
            if (string.IsNullOrEmpty(_componentName)) {
                return $"[{_timeStamp:HH:mm:ss}] {_systemName} {_action} in {_entity}";
            }
            
            return $"[{_timeStamp:HH:mm:ss}] {_systemName} {_action} {_componentName} in {_entity}";
        }
    }
}