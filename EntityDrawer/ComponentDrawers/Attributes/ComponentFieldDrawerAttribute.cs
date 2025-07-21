using System;

namespace Sw1f1.Ecs.Editor {
    [AttributeUsage (AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ComponentFieldDrawerAttribute : Attribute {
        public int Order { get; set; }

        public ComponentFieldDrawerAttribute(int order) {
            Order = order;
        }
    }
}