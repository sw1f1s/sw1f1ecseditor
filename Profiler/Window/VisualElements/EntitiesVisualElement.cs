using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace Sw1f1.Ecs.Editor.Profiler {
    public class EntitiesVisualElement : AbstractProfilerVisualElement {
        private readonly EcsProfiler _profiler;
        private readonly ListView _listView;
        private readonly TextField _entitySearchField;

        private IWorld _currentWorld;
        private List<Entity> _cachedEntities;
        private string _entitySearchQuery = "";
        
        public EntitiesVisualElement(EcsProfiler profiler) {
            _profiler = profiler;
            _profiler.OnChangeEntities += ChangeEntity;
            ComponentDrawer.OnClickEntity += ClickEntity;

            _listView = new ListView();
            _listView.selectionType = SelectionType.None;
            _listView.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
            _listView.style.flexGrow = 1;
            _listView.makeItem = () => new EntityVisualElement();
            
            _entitySearchField = new TextField("Search:");
            _entitySearchField.style.minHeight = 30;
            _entitySearchField.style.paddingTop = 5;
            _entitySearchField.style.paddingBottom = 5;
            _entitySearchField.RegisterValueChangedCallback(evt => {
                _entitySearchQuery = evt.newValue.ToLowerInvariant();
                Rebuild();
            });
        }
        
        public override void Setup(IWorld currentWorld) {
            _currentWorld = currentWorld;
            Clear();
            Add(_entitySearchField);
            Add(_listView);
            Rebuild();
        }

        private void ChangeEntity(IWorld world) {
            if (world != _currentWorld) {
                return;
            }

            Rebuild();
        }

        private void Rebuild() {
            _cachedEntities = _currentWorld.AllEntities().Where(x=> x.ToString().ToLowerInvariant().Contains(_entitySearchQuery)).ToList();
            _listView.itemsSource = _cachedEntities;
            _listView.bindItem = (element, i) => {
                (element as EntityVisualElement).Setup(_cachedEntities[i]);
            }; 
            
            _listView.Rebuild();
            _listView.RefreshItems();
        }

        private void ClickEntity(Entity entity) {
            _entitySearchQuery = entity.ToString().ToLowerInvariant();
            _entitySearchField.value = _entitySearchQuery;
        }
    }
}