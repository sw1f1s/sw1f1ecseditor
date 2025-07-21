using System;
using System.Reflection;
using UnityEngine.UIElements;

namespace Sw1f1.Ecs.Editor {
    [ComponentFieldDrawer(Int32.MaxValue - 1)]
    public class DefaultComponentFieldDrawer : AbstractComponentFieldDrawer {
        public override bool CanDrawGUI(object component) {
            return true;
        }

        public override VisualElement DrawGUI(object component, FieldInfo field, IWorld world) {
            var fieldValue = field.GetValue(component);
            var shortName = GetShortName(field);
            return DrawGUI(shortName, fieldValue, field.FieldType, component, world);
        }

        public override VisualElement DrawGUI(string name, object fieldValue, Type fieldType, object component, IWorld world) {
            var valLabel = new TextField(name);
            valLabel.value = fieldValue.ToString();
            valLabel.style.flexDirection = FlexDirection.Row;
            valLabel.style.justifyContent = Justify.SpaceBetween;
            valLabel.style.flexGrow = 1;
            return valLabel;
        }
    }
}