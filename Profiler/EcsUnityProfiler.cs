using UnityEditor;
using UnityEngine;

namespace Sw1f1.Ecs.Editor.Profiler {
    [InitializeOnLoad]
    public static class EcsUnityProfiler {
        private const string ENABLE_UNITY_PROFILER = "Tools/Ecs/Active System Unity Profiler";
        private const string ECS_UNITY_PROFILER = "ECS_UNITY_PROFILER";
        
        private static readonly PlayerDirectives _playerDirectives = new(EditorUserBuildSettings.selectedBuildTargetGroup);
        
        static EcsUnityProfiler() {
            Systems.OnStartSystemExecute += StartProfilerSystem;
            Systems.OnEndSystemExecute += EndProfilerSystem;
        }
        
        [MenuItem(ENABLE_UNITY_PROFILER, priority = 1)]
        public static void ToggleProfiler() => 
            _playerDirectives.ToggleDirective(ECS_UNITY_PROFILER)
                .Save();

        [MenuItem(ENABLE_UNITY_PROFILER, true)]
        public static bool EnableProfiler() {
            bool hasDirective = _playerDirectives.HasDirective(ECS_UNITY_PROFILER);
            Menu.SetChecked(ENABLE_UNITY_PROFILER, hasDirective);
            
            if (Application.isPlaying) {
                return false;
            }
            
            return true;
        }
        
        private static void StartProfilerSystem(IWorld world, ISystem system) {
#if ECS_UNITY_PROFILER && DEBUG
        UnityEngine.Profiling.Profiler.BeginSample(ProfilerUtilities.GetCleanGenericTypeName(system.GetType()));      
#endif
        }
        
        private static void EndProfilerSystem(IWorld world, ISystem system) {
#if ECS_UNITY_PROFILER && DEBUG
            UnityEngine.Profiling.Profiler.EndSample();
#endif
        }
    }
}