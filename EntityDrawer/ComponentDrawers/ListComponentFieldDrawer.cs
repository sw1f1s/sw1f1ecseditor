using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sw1f1.Ecs.Editor {
    [ComponentFieldDrawer(2)]
    public class ListComponentFieldDrawer : AbstractComponentFieldDrawer {
        private readonly Dictionary<string, Foldout> _foldouts = new Dictionary<string, Foldout>();
        
        public override bool CanDrawGUI(object component) {
            return component.GetType().IsArray || component is IList;
        }

        public override VisualElement DrawGUI(EntityVisualElement entityVisualElement, object component, FieldInfo field, IWorld world) {
            var fieldValue = field.GetValue(component);
            var shortName = GetShortName(field);
            return DrawGUI(entityVisualElement, shortName, fieldValue, field.FieldType, component, world);
        }

        public override VisualElement DrawGUI(EntityVisualElement entityVisualElement, string name, object fieldValue, Type fieldType, object component, IWorld world) {
            var root = new VisualElement();
            if (fieldValue is not IList list) {
                root.Add(new Label("Not a list"));
                return root;
            }

            var elementType = fieldType.IsArray
                ? fieldType.GetElementType()
                : fieldType.GetGenericArguments()[0];

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
            
            for (int i = 0; i < list.Count; i++) {
                var label = $"Element {i}";
                var element = ComponentDrawer.DrawTypeField(entityVisualElement, label, list[i], elementType, component, world);
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