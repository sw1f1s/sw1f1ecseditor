using System;
using System.Reflection;
using UnityEngine.UIElements;

namespace Sw1f1.Ecs.Editor {
    [ComponentFieldDrawer(1)]
    public class EnumComponentFieldDrawer : AbstractComponentFieldDrawer {
        public override bool CanDrawGUI(object component) {
            return component.GetType().IsEnum;
        }

        public override VisualElement DrawGUI(EntityVisualElement entityVisualElement, object component, FieldInfo field, IWorld world) {
            var fieldValue = field.GetValue(component);
            var fieldType = field.FieldType;
            var shortName = GetShortName(field);
            return DrawGUI(entityVisualElement, shortName, fieldValue, fieldType, component, world);
        }

        public override VisualElement DrawGUI(EntityVisualElement entityVisualElement, string name, object fieldValue, Type fieldType, object component, IWorld world) {
            var enumValue = (Enum) fieldValue;
            var isFlags = Attribute.IsDefined (fieldType, typeof(FlagsAttribute));

            if (!isFlags) {
                var enumField = new EnumField(name, enumValue);
                return enumField;
            }
            
            var container = new Foldout { text = name, value = false };
            Array values = Enum.GetValues(fieldType);
            long currentBits = Convert.ToInt64(name);
            foreach (Enum option in values) {
                long bit = Convert.ToInt64(option);
                var toggle = new Toggle(option.ToString()) {
                    value = (currentBits & bit) != 0
                };
                container.Add(toggle);
            }

            return container;
        }
    }
}