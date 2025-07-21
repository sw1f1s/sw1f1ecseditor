using UnityEngine;
using UnityEngine.UIElements;

namespace Sw1f1.Ecs.Editor.Profiler {
    public class SystemVisualElement : VisualElement {
        private readonly Label _name;
        private readonly Label _info;
        private EcsProfilerSystem _system;
        
        public SystemVisualElement() {
            style.flexDirection = FlexDirection.Row;
            style.justifyContent = Justify.SpaceBetween;
            style.marginBottom = 2;
            style.paddingRight = 8;
            style.paddingLeft = 4;
            style.paddingTop = 2;
            style.paddingBottom = 2;
            style.backgroundColor = new Color(0.15f, 0.15f, 0.15f);
                
            _name = new Label();
            _name.style.flexGrow = 1;
            _name.style.unityTextAlign = TextAnchor.MiddleLeft;
                
            _info = new Label();
            _info.style.unityTextAlign = TextAnchor.MiddleRight;
            _info.style.width = 140;
            _info.style.flexShrink = 0;

            Add(_name);
            Add(_info);
        }

        public void Setup(EcsProfilerSystem system) {
            if (_system != null) {
                _system.OnUpdate -= Update;
            }
            
            _system = system;
            _name.text = _system.Name;
            _system.OnUpdate += Update;
            Update();
        }

        private void Update() {
            _info.text = $"{_system.ExecutionTimeMs:F2} ms   {FormatBytes(_system.Allocations)} alloc";
        }
        
        private static string FormatBytes(long bytes) {
            if (bytes < 1024) return $"{bytes}B";
            if (bytes < 1024 * 1024) return $"{(bytes / 1024f):F2}KB";
            return $"{(bytes / 1024f / 1024f):F2}MB";
        }
    }
}