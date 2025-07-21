using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Sw1f1.Ecs.Editor.Profiler {
    public class EntitiesVisualElement : AbstractProfilerVisualElement {
        private readonly EcsProfiler _profiler;
        private readonly List<EntityVisualElement> _elements;
        private readonly TextField _entitySearchField;
        private string _entitySearchQuery = "";
        
        public EntitiesVisualElement(EcsProfiler profiler) {
            _profiler = profiler;
            _elements = new List<EntityVisualElement>();
            ComponentDrawer.OnClickEntity += ClickEntity;
            
            _entitySearchField = new TextField("Search:");
            _entitySearchField.style.flexGrow = 1;
            _entitySearchField.style.paddingTop = 5;
            _entitySearchField.style.paddingBottom = 5;
            _entitySearchField.RegisterValueChangedCallback(evt => {
                _entitySearchQuery = evt.newValue.ToLowerInvariant();
            });
        }
        
        public override void Update(IWorld currentWorld) {
            Clear();
            Add(_entitySearchField);

            int index = 0;
            foreach (var entity in currentWorld.AllEntities()) {
                string displayName = entity.ToString();
                if (!string.IsNullOrEmpty(_entitySearchQuery) && !displayName.ToLowerInvariant().Contains(_entitySearchQuery)) {
                    continue;
                }

                var element = GetOrCreateElement(index);
                element.Update(entity);
                Add(element);
                index++;
            }
        }
        
        private EntityVisualElement GetOrCreateElement(int index) {
            if (_elements.Count <= index) {
                _elements.Add(new EntityVisualElement(index.ToString()));
            }
            
            return _elements[index];
        }

        private void ClickEntity(Entity entity) {
            _entitySearchQuery = entity.ToString().ToLowerInvariant();
            _entitySearchField.value = _entitySearchQuery;
        }
    }
}