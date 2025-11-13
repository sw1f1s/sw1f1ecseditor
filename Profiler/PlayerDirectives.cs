using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Sw1f1.Ecs.Editor.Profiler {
    public class PlayerDirectives {
        private readonly List<string> _directives;
        private readonly BuildTargetGroup _targetGroup;
        private readonly bool _isLogEnabled;

        public PlayerDirectives(BuildTargetGroup targetGroup, bool isLogEnabled = true) {
            _targetGroup = targetGroup;
            _isLogEnabled = isLogEnabled;
            string directives = PlayerSettings.GetScriptingDefineSymbolsForGroup(_targetGroup);
            _directives = directives.Split(";")
                .ToList();
        }

        public PlayerDirectives SetDirective(string directive, bool value) {
            if (value && !_directives.Contains(directive)) {
                _directives.Add(directive);
                Log($"==> Add define {directive}");
            } else if (!value && _directives.Contains(directive)) {
                _directives.Remove(directive);
                Log($"==> Remove define {directive}");
            }

            return this;
        }

        public PlayerDirectives ToggleDirective(string directive) {
            bool nextValue = !HasDirective(directive);
            return SetDirective(directive, nextValue);
        }

        public bool HasDirective(string directive) => 
            _directives.Contains(directive);

        public void Save() {
            string directives = string.Join(";", _directives);
            Log($"==> Result Defines: {directives}");
            PlayerSettings.SetScriptingDefineSymbolsForGroup(_targetGroup, directives);
        }

        private void Log(string value) {
            if (_isLogEnabled) {
                Debug.Log(value);
            }
        }
    }
}