using System;
using System.IO;
using UnityEngine;

namespace Sw1f1.Ecs.Editor.Profiler {
    public static class ProfilerUtilities {
        private const string LOG_FILE_NAME = "ecs_profiler_log.txt";
        public static string GetCleanGenericTypeName(Type type) {
            if (!type.IsGenericType) {
                return type.Name;
            }
            var constraints = "";
            foreach (var constraint in type.GetGenericArguments ()) {
                constraints += constraints.Length > 0 ? $", {GetCleanGenericTypeName (constraint)}" : constraint.Name;
            }
            var genericIndex = type.Name.LastIndexOf ("`", StringComparison.Ordinal);
            var typeName = genericIndex == -1
                ? type.Name
                : type.Name.Substring (0, genericIndex);
            return $"{typeName}<{constraints}>";
        }

        public static void SaveLog(string log) {
            string tempPath = Path.Combine(Path.GetTempPath(), LOG_FILE_NAME);
            File.WriteAllText(tempPath, log);
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            string url = "file://" + tempPath.Replace(" ", "%20");
            Application.OpenURL(url);
#else
            Application.OpenURL(tempPath);
#endif
        }
    }
}