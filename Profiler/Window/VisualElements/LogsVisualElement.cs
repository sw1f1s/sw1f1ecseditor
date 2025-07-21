using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sw1f1.Ecs.Editor.Profiler {
    public class LogsVisualElement : AbstractProfilerVisualElement {
        private const string LOG_FILE_NAME = "ecs_profiler_log.txt";
        
        private readonly EcsProfiler _profiler;
        private readonly List<LogVisualElement> _elements;
        private readonly VisualElement _headerContainer;
        private readonly TextField _entitySearchField;
        private readonly Button _menuButton;
        private readonly Label _messageLabel;
        private string _entitySearchQuery = "";
        
        private IWorld _currentWorld;
        
        public LogsVisualElement(EcsProfiler profiler) {
            _profiler = profiler;
            _elements = new List<LogVisualElement>();
            _headerContainer = new VisualElement();
            _headerContainer.style.flexDirection = FlexDirection.Row;
            
            _entitySearchField = new TextField("Search:");
            _entitySearchField.style.flexGrow = 1;
            _entitySearchField.style.paddingTop = 5;
            _entitySearchField.style.paddingBottom = 5;
            _entitySearchField.RegisterValueChangedCallback(evt => {
                _entitySearchQuery = evt.newValue.ToLowerInvariant();
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
        }
        
        public override void Update(IWorld currentWorld) {
            _currentWorld = currentWorld;
            Clear();
            Add(_headerContainer);
            if (_profiler.IsLogPause) {
                _messageLabel.text = "Is Pause";
                Add(_messageLabel);
            }
            int index = 0;
            foreach (var componentLog in _profiler.GetComponentChangeLogs(currentWorld)) {
                var log = componentLog.ToString();
                if (!string.IsNullOrEmpty(_entitySearchQuery) && !log.ToLowerInvariant().Contains(_entitySearchQuery)) {
                    continue;
                }
                
                var element = GetOrCreateElement(index);
                element.Update(log);
                Add(element);
                index++;
            }
        }

        private void ShowMenu(VisualElement anchor) {
            var menu = new GenericMenu();

            menu.AddItem(new GUIContent("Clear log"), false, () => {
                _profiler.ClearLog(_currentWorld);
            });

            menu.AddItem(new GUIContent("Export to file"), false, () => {
                string tempPath = Path.Combine(Path.GetTempPath(), LOG_FILE_NAME);
                File.WriteAllText(tempPath, _profiler.GetFullComponentChangeLogs(_currentWorld).ToString());
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
                string url = "file://" + tempPath.Replace(" ", "%20");
                Application.OpenURL(url);
#else
                Application.OpenURL(tempPath);
#endif
            });

            menu.AddItem(new GUIContent("Pause"), _profiler.IsLogPause, () => {
                _profiler.IsLogPause = !_profiler.IsLogPause;
            });
            
            var worldPos = anchor.worldBound.position;
            menu.DropDown(new Rect(worldPos, Vector2.zero));
        }
        
        private LogVisualElement GetOrCreateElement(int index) {
            if (_elements.Count <= index) {
                _elements.Add(new LogVisualElement());
            }
            
            return _elements[index];
        }
    }
}