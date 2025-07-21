using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.UIElements;

namespace Sw1f1.Ecs.Editor {
    [ComponentFieldDrawer(4)]
    public class EntityComponentFieldDrawer : AbstractComponentFieldDrawer {
        private Dictionary<string, Button> _buttons = new Dictionary<string, Button>();
        public override bool CanDrawGUI(object component) {
            return component is Entity;
        }

        public override VisualElement DrawGUI(object component, FieldInfo field, IWorld world) {
            var fieldValue = field.GetValue(component);
            var shortName = GetShortName(field);
            return DrawGUI(shortName, fieldValue, field.FieldType, component, world);
        }

        public override VisualElement DrawGUI(string name, object fieldValue, Type fieldType, object component, IWorld world) {
            var entity = (Entity)fieldValue;
            
            var propRow = new VisualElement();
            propRow.style.flexDirection = FlexDirection.Row;
            propRow.style.justifyContent = Justify.SpaceBetween;
            propRow.style.paddingLeft = 10;
            propRow.style.paddingRight = 10;
            
            var valLabel = new Label(name);
            valLabel.style.flexGrow = 1;

            string path = $"{component.GetType().Name}/{fieldType.Name}/ {name}";
            var button = GetButton(path, entity);
            button.style.flexGrow = 1;
            
            propRow.Add(valLabel);
            propRow.Add(button);

            return propRow;
        }
        
        private Button GetButton(string path, Entity entity) {
            if (!_buttons.ContainsKey(path)) {
                var newButton = new Button(() => {
                    ComponentDrawer.OnClickEntity?.Invoke(entity);
                }) {
                    text = entity.ToString() 
                };
                _buttons.Add(path, newButton);
            }
            
            var b = _buttons[path];
            b.Clear();
            return b;
        }
    }
}