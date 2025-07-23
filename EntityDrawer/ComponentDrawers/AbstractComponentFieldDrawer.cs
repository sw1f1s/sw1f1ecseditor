using System;
using System.Reflection;
using UnityEngine.UIElements;

namespace Sw1f1.Ecs.Editor {
    public abstract class AbstractComponentFieldDrawer {
        public abstract bool CanDrawGUI(object component);
        public abstract VisualElement DrawGUI(EntityVisualElement entityVisualElement, object component, FieldInfo field, IWorld world);
        public abstract VisualElement DrawGUI(EntityVisualElement entityVisualElement, string name, object fieldValue, Type fieldType, object component, IWorld world);

        protected string GetShortName(FieldInfo field) {
            var shortName = field.Name;
            if (shortName.Contains("k__BackingField")) {
                int start = shortName.IndexOf('<') + 1;
                int end = shortName.IndexOf('>');
                if (start > 0 && end > start)
                    shortName = shortName.Substring(start, end - start);
            }
            
            return shortName;
        }
    }
}