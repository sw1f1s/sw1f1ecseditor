using UnityEngine;
using UnityEngine.UIElements;

namespace Sw1f1.Ecs.Editor.Profiler {
    public class LogVisualElement : VisualElement {
        private readonly Label _infoLabel;
        
        public LogVisualElement() {
            style.flexDirection = FlexDirection.Row;
            style.justifyContent = Justify.SpaceBetween;
            style.marginBottom = 2;
            style.paddingRight = 8;
            style.paddingLeft = 4;
            style.paddingTop = 2;
            style.paddingBottom = 2;
            style.backgroundColor = new Color(0.15f, 0.15f, 0.15f);
                
            _infoLabel = new Label("");
            _infoLabel.style.unityTextAlign = TextAnchor.MiddleLeft;
            _infoLabel.style.width = 140;
            _infoLabel.style.flexShrink = 0;
                
            Add(_infoLabel);
        }

        public void Update(string log) {
            _infoLabel.text = log;
        }
    }
}