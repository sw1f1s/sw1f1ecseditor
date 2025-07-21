using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace Sw1f1.Ecs.Editor.Profiler {
    public class SystemsVisualElement : AbstractProfilerVisualElement {
        private readonly EcsProfiler _profiler;
        private readonly ListView _listView;
        private List<EcsProfilerSystem> _cachedSystems;
        private IWorld _currentWorld;
        
        public SystemsVisualElement(EcsProfiler profiler) {
            _profiler = profiler;
            _listView = new ListView();
            _listView.selectionType = SelectionType.None;
            _listView.fixedItemHeight = 25;
            _listView.style.flexGrow = 1;
            _listView.virtualizationMethod = CollectionVirtualizationMethod.FixedHeight;
            _listView.reorderable = true;
            _listView.makeItem = () => new SystemVisualElement();
            _listView.bindItem = (element, i) => {
                (element as SystemVisualElement).Setup(_cachedSystems[i]);
            }; 
        }
        
        public override void Setup(IWorld currentWorld) {
            _currentWorld = currentWorld;
            Rebuild();
        }

        public override void Update() {
            if (_cachedSystems == null) {
                return;
            }
            
            _cachedSystems.Sort((a, b) => {
                if (b.ExecutionTimeMs.Equals(a.ExecutionTimeMs)) {
                    return String.Compare(b.Name, a.Name, StringComparison.Ordinal);
                }
                return b.ExecutionTimeMs.CompareTo(a.ExecutionTimeMs);
            });
            _listView.RefreshItems();
        }

        private void Rebuild() {
            Clear();
            Add(_listView);
            _cachedSystems = _profiler.GetSystems(_currentWorld).ToList();
            _listView.itemsSource = _cachedSystems;
            _listView.Rebuild();
        }
    }
}