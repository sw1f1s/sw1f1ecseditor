using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sw1f1.Ecs.Editor.Profiler {
    public class SystemsVisualElement : AbstractProfilerVisualElement {
        private readonly EcsProfiler _profiler;
        private readonly List<SystemVisualElement> _elements;
        
        public SystemsVisualElement(EcsProfiler profiler) {
            _profiler = profiler;
            _elements = new List<SystemVisualElement>();
        }
        
        public override void Update(IWorld currentWorld) {
            Clear();
            
            int index = 0;
            foreach (var system in _profiler.GetSystems(currentWorld)) {
                var element = GetOrCreateElement(index);
                element.Update(system);
                Add(element);
                index++;
            }
        }

        private SystemVisualElement GetOrCreateElement(int index) {
            if (_elements.Count <= index) {
                _elements.Add(new SystemVisualElement());
            }
            
            return _elements[index];
        }
    }
}