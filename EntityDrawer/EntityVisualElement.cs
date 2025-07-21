using System;
using System.Collections.Generic;
using Sw1f1.Ecs.Editor.Profiler;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sw1f1.Ecs.Editor {
    public class EntityVisualElement : VisualElement {
        private readonly Foldout _foldout;
        private readonly Dictionary<string, Foldout> _foldouts = new Dictionary<string, Foldout>();
        private Entity _entity;

        public EntityVisualElement() {
            _foldout = new Foldout() { text = "" };
            _foldout.style.marginBottom = 4;
            _foldout.style.paddingLeft = 4;
            _foldout.style.backgroundColor = new Color(0.15f, 0.15f, 0.15f);
            _foldout.style.borderBottomColor = Color.black;
            _foldout.style.borderBottomWidth = 1;
            _foldout.RegisterValueChangedCallback(RegisterFoldoutChangedCallback);
            
            Add(_foldout);
            RegisterCallback<AttachToPanelEvent>(OnAttached);
            RegisterCallback<DetachFromPanelEvent>(OnDetached);
        }
        
        public void Setup(Entity entity) {
            _entity = entity;
            string displayName = entity.ToString();
            _foldout.text = displayName;
            _foldout.value = SessionState.GetBool(entity.ToString(), false);

            RebuildComponents();
        }
        
        private void OnAttached(AttachToPanelEvent evt) {
            EditorApplication.update += RebuildComponents;
        }

        private void OnDetached(DetachFromPanelEvent evt) {
            EditorApplication.update -= RebuildComponents;
        }

        private void RegisterFoldoutChangedCallback(ChangeEvent<bool> evt) {
            SessionState.SetBool(_entity.ToString(), evt.newValue);
            RebuildComponents();
        }
        
        private void RebuildComponents() {
            if (!_foldout.value) {
                return;
            }
            
            _foldout.Clear();
            foreach (var component in _entity.Components) {
                var type = component.GetType();
                var path = _entity + "/" + type.FullName;
                var typeName = ProfilerUtilities.GetCleanGenericTypeName(type);
                var componentFoldout = GetComponentFoldout(typeName, path);
                componentFoldout.style.paddingLeft = 6;
                componentFoldout.style.marginBottom = 2;
                componentFoldout.style.backgroundColor = new Color(0.10f, 0.10f, 0.10f);

                var fields = ComponentDrawer.GetFields(type);
                if(fields.Length > 0) {
                    foreach (var fieldInfo in fields) {
                        var element = ComponentDrawer.DrawTypeField(component, fieldInfo, WorldBuilder.GetWorld(_entity.WorldId));
                        componentFoldout.Add(element);
                    }
                }

                _foldout.Add(componentFoldout);
            }
        }
        
        private Foldout GetComponentFoldout(string text, string path) {
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