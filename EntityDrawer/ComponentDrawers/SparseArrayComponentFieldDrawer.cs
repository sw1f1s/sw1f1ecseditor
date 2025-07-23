using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Sw1f1.Ecs.Collections;

namespace Sw1f1.Ecs.Editor {
    public abstract class SparseArrayComponentFieldDrawer<T> : AbstractComponentFieldDrawer {
        private readonly Dictionary<string, Foldout> _foldouts = new Dictionary<string, Foldout>();
        
        public override bool CanDrawGUI(object component) {
            return component.GetType().IsArray || component is SparseArray<T>;
        }

        public override VisualElement DrawGUI(EntityVisualElement entityVisualElement, object component, FieldInfo field, IWorld world) {
            var fieldValue = field.GetValue(component);
            var shortName = GetShortName(field);
            return DrawGUI(entityVisualElement, shortName, fieldValue, field.FieldType, component, world);
        }

        public override VisualElement DrawGUI(EntityVisualElement entityVisualElement, string name, object fieldValue, Type fieldType, object component, IWorld world) {
            var root = new VisualElement();
            if (fieldValue is not SparseArray<T> list) {
                root.Add(new Label($"Not a SparseArray<{nameof(T)}>"));
                return root;
            }

            var elementType = typeof(T);

            var sessionKey = component.GetType() + "/" + name;
            var foldout = GetFoldout(name, sessionKey);
            foldout.RegisterValueChangedCallback(evt => {
                SessionState.SetBool(sessionKey, evt.newValue);
            });

            root.Add(foldout);
            if (!foldout.value) {
                return root;   
            }
            
            if (list.Count == 0) {
                var emptyLabel = new Label("Empty");
                emptyLabel.style.unityTextAlign = TextAnchor.MiddleLeft;
                emptyLabel.style.marginLeft = 8;
                foldout.Add(emptyLabel);
                return root;
            }

            int index = 0;
            foreach (var entry in list) {
                var label = $"Element {index}";
                var element = ComponentDrawer.DrawTypeField(entityVisualElement, label, entry, elementType, component, world);
                root.Add(element);
            }

            return root;
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
    }
}