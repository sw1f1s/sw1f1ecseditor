using System;
using System.Reflection;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Sw1f1.Ecs.Editor {
    [ComponentFieldDrawer(0)]
    public class ObjectComponentFieldDrawer : AbstractComponentFieldDrawer {
        public override bool CanDrawGUI(object component) {
            return component.GetType() == typeof(Object) || component.GetType().IsSubclassOf(typeof(Object));
        }

        public override VisualElement DrawGUI(object component, FieldInfo field, IWorld world) {
            var fieldValue = field.GetValue(component);
            var shortName = GetShortName(field);
            return DrawGUI(shortName, fieldValue, field.FieldType, component, world);
        }

        public override VisualElement DrawGUI(string name, object fieldValue, Type fieldType, object component, IWorld world) {
            var propRow = new VisualElement();
            propRow.style.flexDirection = FlexDirection.Row;
            propRow.style.justifyContent = Justify.SpaceBetween;
            propRow.style.paddingLeft = 10;
            propRow.style.paddingRight = 10;

            var objField = new ObjectField(name);
            objField.objectType = fieldType;
            objField.allowSceneObjects = true;
            objField.style.flexGrow = 1;
            objField.value = fieldValue as Object;
            propRow.Add(objField);

            return propRow;
        }
    }
}