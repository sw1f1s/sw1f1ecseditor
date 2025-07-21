using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sw1f1.Ecs.Editor {
    [ComponentFieldDrawer(3)]
    public class DictionaryComponentFieldDrawer : AbstractComponentFieldDrawer {
        private readonly Dictionary<string, Foldout> _foldouts = new Dictionary<string, Foldout>();
        
        public override bool CanDrawGUI(object component) {
            return component.GetType().IsArray || component is IDictionary;
        }

        public override VisualElement DrawGUI(object component, FieldInfo field, IWorld world) {
            var fieldValue = field.GetValue(component);
            var shortName = GetShortName(field);
            return DrawGUI(shortName, fieldValue, field.FieldType, component, world);
        }

        public override VisualElement DrawGUI(string name, object fieldValue, Type fieldType, object component, IWorld world) {
            var root = new VisualElement();
            if (fieldValue is not IDictionary dictionary) {
                root.Add(new Label("Not a dictionary"));
                return root;
            }

            var genericArgs = fieldType.GetGenericArguments();
            var keyType = genericArgs[0];
            var valueType = genericArgs[1];

            var sessionKey = component.GetType() + "/" + name;
            var foldout = GetFoldout(name, sessionKey);
            foldout.RegisterValueChangedCallback(evt => {
                SessionState.SetBool(sessionKey, evt.newValue);
            });

            root.Add(foldout);
            if (!foldout.value) {
                return root;   
            }
            
            if (dictionary.Count == 0) {
                var emptyLabel = new Label("Empty");
                emptyLabel.style.unityTextAlign = TextAnchor.MiddleLeft;
                emptyLabel.style.marginLeft = 8;
                foldout.Add(emptyLabel);
                return root;
            }
            
            foreach (DictionaryEntry entry in dictionary) {
                var label = entry.Key.ToString();
                var element = ComponentDrawer.DrawTypeField(label, entry.Value, valueType, component, world);
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