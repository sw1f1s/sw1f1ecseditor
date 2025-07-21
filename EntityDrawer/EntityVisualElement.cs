using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sw1f1.Ecs.Editor {
    public class EntityVisualElement : VisualElement {
        private readonly Foldout _foldout;
        private readonly Dictionary<string, Foldout> _foldouts = new Dictionary<string, Foldout>();

        public EntityVisualElement(string path) {
            _foldout = new Foldout() { text = "", value = SessionState.GetBool(path, false) };
            _foldout.RegisterValueChangedCallback(evt => {
                SessionState.SetBool(path, evt.newValue);
            });
            _foldout.style.marginBottom = 4;
            _foldout.style.paddingLeft = 4;
            _foldout.style.backgroundColor = new Color(0.15f, 0.15f, 0.15f);
            _foldout.style.borderBottomColor = Color.black;
            _foldout.style.borderBottomWidth = 1;
            Add(_foldout);
        }
        
        public void Update(Entity entity) {
            string displayName = entity.ToString();
            _foldout.text = displayName;
            foreach (var component in entity.Components) {
                var type = component.GetType();
                var path = entity + "/" + type.FullName;
                var typeName = GetCleanGenericTypeName(type);
                var componentFoldout = GetFoldout(typeName, path);
                componentFoldout.style.paddingLeft = 6;
                componentFoldout.style.marginBottom = 2;
                componentFoldout.style.backgroundColor = new Color(0.10f, 0.10f, 0.10f);

                var fields = ComponentDrawer.GetFields(type);
                if(fields.Length > 0) {
                    foreach (var fieldInfo in fields) {
                        var element = ComponentDrawer.DrawTypeField(component, fieldInfo, WorldBuilder.GetWorld(entity.WorldId));
                        componentFoldout.Add(element);
                    }
                }

                _foldout.Add(componentFoldout);
            }
        }
        
        private Foldout GetFoldout(string text, string path) {
            if (!_foldouts.ContainsKey(path)) {
                var f = new Foldout() { text = text, value = SessionState.GetBool(path, false) };
                f.RegisterValueChangedCallback(evt => {
                    SessionState.SetBool(path, evt.newValue);
                });
                _foldouts.Add(path, f);
            }
            
            var foldout = _foldouts[path];
            foldout.Clear();
            return foldout;
        }
        
        private static string GetCleanGenericTypeName(Type type) {
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
    }
}