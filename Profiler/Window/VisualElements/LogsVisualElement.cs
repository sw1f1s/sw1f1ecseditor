using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sw1f1.Ecs.Editor.Profiler {
    public class LogsVisualElement : AbstractProfilerVisualElement {
        private readonly EcsProfiler _profiler;
        private readonly ListView _listView;
        private readonly VisualElement _headerContainer;
        private readonly TextField _entitySearchField;
        private readonly Button _menuButton;
        private readonly Label _messageLabel;
        private string _logSearchQuery = "";
        
        private IWorld _currentWorld;
        private List<ComponentChangeLog> _cachedLogs;
        
        public LogsVisualElement(EcsProfiler profiler) {
            _profiler = profiler;
            _headerContainer = new VisualElement();
            _headerContainer.style.flexDirection = FlexDirection.Row;
            _headerContainer.style.minHeight = 30;
            
            _listView = new ListView(_cachedLogs);
            _listView.selectionType = SelectionType.Single;
            _listView.fixedItemHeight = 25;
            _listView.style.flexGrow = 1;
            _listView.virtualizationMethod = CollectionVirtualizationMethod.FixedHeight;
            _listView.makeItem = () => new LogVisualElement();
            _listView.bindItem = (element, i) => {
                (element as LogVisualElement).Setup(_cachedLogs[i].ToString());
            }; 
            
            _entitySearchField = new TextField("Search:");
            _entitySearchField.style.flexGrow = 1;
            _entitySearchField.style.paddingTop = 5;
            _entitySearchField.style.paddingBottom = 5;
            _entitySearchField.RegisterValueChangedCallback(evt => {
                _logSearchQuery = evt.newValue.ToLowerInvariant();
                Rebuild();
            });
            
            _menuButton = new Button(() => ShowMenu(_menuButton)) {
                text = "⋮"
            };
            
            _menuButton.style.paddingTop = 5;
            _menuButton.style.paddingBottom = 5;
            _menuButton.style.alignSelf = Align.FlexEnd;
            _menuButton.tooltip = "Action Menu";
            
            _messageLabel = new Label("");
            _messageLabel.style.alignSelf = Align.Center;
            
            _headerContainer.Add(_entitySearchField);
            _headerContainer.Add(_menuButton);

            _profiler.OnAddComponentChangeLog += AddComponentChangeLog;
            _profiler.OnClearComponentChangeLog += ClearLogs;
        }
        
        public override void Setup(IWorld currentWorld) {
            _currentWorld = currentWorld;
            Rebuild();
        }

        private void AddComponentChangeLog(IWorld world, ComponentChangeLog changeLog) {
            if (_currentWorld != world) {
                return;
            }

            if (!changeLog.ToString().ToLowerInvariant().Contains(_logSearchQuery)) {
                return;
            }
            
            _cachedLogs.Add(changeLog);
            _listView.RefreshItem(_cachedLogs.Count - 1);
        }

        private void ClearLogs(IWorld world) {
            if (_currentWorld != world) {
                return;
            }
            
            _cachedLogs.Clear();
            _listView.RefreshItems();
        }

        private void Rebuild() {
            Clear();
            Add(_headerContainer);
            if (_profiler.IsLogPause) {
                _messageLabel.text = "Is Pause";
                Add(_messageLabel);
            }

            _cachedLogs = _profiler.GetComponentChangeLogs(_currentWorld).Where(x=> x.ToString().ToLowerInvariant().Contains(_logSearchQuery)).ToList();
            _listView.itemsSource = _cachedLogs;
            _listView.Rebuild();
            Add(_listView);
        }

        private void ShowMenu(VisualElement anchor) {
            var menu = new GenericMenu();

            menu.AddItem(new GUIContent("Clear log"), false, () => {
                _profiler.ClearLog(_currentWorld);
            });

            menu.AddItem(new GUIContent("Export to file"), false, () => {
                ProfilerUtilities.SaveLog(_profiler.GetFullComponentChangeLogs(_currentWorld).ToString());
            });

            menu.AddItem(new GUIContent("Pause"), _profiler.IsLogPause, () => {
                _profiler.IsLogPause = !_profiler.IsLogPause;
                Rebuild();
            });
            
            var worldPos = anchor.worldBound.position;
            menu.DropDown(new Rect(worldPos, Vector2.zero));
        }
    }
}