using UnityEngine.UIElements;

namespace Sw1f1.Ecs.Editor.Profiler {
    public abstract class AbstractProfilerVisualElement : VisualElement {
        public abstract void Setup(IWorld currentWorld);
        public virtual void Update() {}
    }
}