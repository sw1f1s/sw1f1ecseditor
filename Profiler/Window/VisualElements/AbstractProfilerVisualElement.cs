using UnityEngine.UIElements;

namespace Sw1f1.Ecs.Editor.Profiler {
    public abstract class AbstractProfilerVisualElement : VisualElement {
        public abstract void Update(IWorld currentWorld);
    }
}