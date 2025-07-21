using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sw1f1.Ecs.Editor.Profiler {
    public class EcsProfilerWindow : EditorWindow {
        private VisualElement _root;
        private DropdownField _worldDropdown;
        private Button _systemsButton;
        private Button _entitiesButton;
        private Button _logButton;
        private VisualElement _contentPanel;
        private EcsProfiler _profiler;
        
        private EcsProfilerPanelType _panelType;
        private Dictionary<EcsProfilerPanelType, AbstractProfilerVisualElement> _panels;
        private IWorld _currentWorld;
        private float _updateDelta;
        
        [MenuItem("Tools/Ecs/Profiler")]
        public static void ShowWindow() {
            var window = GetWindow<EcsProfilerWindow>();
            window.titleContent = new GUIContent("ECS Profiler");
        }
        
        private void OnEnable() {
            _profiler = new EcsProfiler();
            CreateUI();
            EditorApplication.update += UpdateProfiler;
        }

        private void OnDisable() {
            _profiler?.Dispose();
            EditorApplication.update -= UpdateProfiler;
        }
        
        private void CreateUI() {
            _root = rootVisualElement;
            _root.style.flexDirection = FlexDirection.Column;
            _root.style.paddingTop = 4;
            _root.style.paddingBottom = 4;
            _root.style.paddingLeft = 4;
            _root.style.paddingRight = 4;
            
            var toolbar = new VisualElement();
            toolbar.style.flexDirection = FlexDirection.Row;
            toolbar.style.marginBottom = 4;
            toolbar.style.height = 24;
            
            _worldDropdown = new DropdownField("World", _profiler.WorldNames.ToList(), 0);
            _worldDropdown.style.width = 180;
            _worldDropdown.labelElement.style.minWidth = 40;
            _worldDropdown.RegisterValueChangedCallback(evt => {
                RefreshContent();
            });
            
            _systemsButton = new Button(() => {
                _panelType = EcsProfilerPanelType.Systems;
                RefreshContent();
            }) { text = "Systems" };
            
            _entitiesButton = new Button(() => {
                _panelType = EcsProfilerPanelType.Entities;
                RefreshContent();
            }) { text = "Entities" };
            
            _logButton = new Button(() => {
                _panelType = EcsProfilerPanelType.Log;
                RefreshContent();
            }) { text = "Logs" };

            _systemsButton.style.flexGrow = 1;
            _entitiesButton.style.flexGrow = 1;
            _logButton.style.flexGrow = 1;
            _systemsButton.style.marginLeft = 4;
            _entitiesButton.style.marginLeft = 4;
            _logButton.style.marginLeft = 4;
            
            toolbar.Add(_worldDropdown);
            toolbar.Add(_systemsButton);
            toolbar.Add(_entitiesButton);
            toolbar.Add(_logButton);

            _root.Add(toolbar);
            
            _contentPanel = new VisualElement();
            _contentPanel.style.flexGrow = 1;
            _root.Add(_contentPanel);
            
            _currentWorld = _profiler.GetWorldByName(_worldDropdown.value);
            _panels = new Dictionary<EcsProfilerPanelType, AbstractProfilerVisualElement>() {
                {EcsProfilerPanelType.Systems, new SystemsVisualElement(_profiler)},
                {EcsProfilerPanelType.Entities, new EntitiesVisualElement(_profiler)},
                {EcsProfilerPanelType.Log, new LogsVisualElement(_profiler)}
            };
        }
        
        private void UpdateProfiler() {
            if (_currentWorld != null && !_currentWorld.IsAlive()) {
                _currentWorld = null;
                RefreshContent();
            }
            
            _worldDropdown.choices = new List<string>(_profiler.WorldNames);
            if (_currentWorld == null && _worldDropdown.choices.Count > 0) {
                _worldDropdown.value = _worldDropdown.choices[0];
                RefreshContent();
            }

            _updateDelta += Time.smoothDeltaTime;
            if (_updateDelta >= 0.5f) {
                _panels[_panelType].Update();
                _updateDelta = 0;
            }
        }

        private void RefreshContent() {
            _currentWorld = _profiler.GetWorldByName(_worldDropdown.value);
            _contentPanel.Clear();
            if (_currentWorld == null) {
                var l = new Label($"No World Selected");
                l.style.alignSelf = Align.Center;
                _contentPanel.Add(l);
                return;
            }

            if (!_currentWorld.IsAlive()) {
                var l = new Label($"World {_currentWorld.Id} is not Alive");
                l.style.alignSelf = Align.Center;
                _contentPanel.Add(l);
                return;
            }
            
            var currentPanel = _panels[_panelType];
            currentPanel.Setup(_currentWorld);
            _contentPanel.Add(currentPanel);
            _root.MarkDirtyRepaint();
        }
    }
}