using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sw1f1.Ecs.Editor {
    public static class ComponentDrawer {
        private static readonly Dictionary<Type, FieldInfo[]> ComponentFieldsCache = new Dictionary<Type, FieldInfo[]>();
        private static List<AbstractComponentFieldDrawer> _componentFieldDrawers;

        public static FieldInfo[] GetFields(Type type) {
            if (!ComponentFieldsCache.ContainsKey(type)) {
                ComponentFieldsCache.Add(type, type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
            }
            
            return ComponentFieldsCache[type];
        }
        
        public static VisualElement DrawTypeField(EntityVisualElement entityVisualElement, object component, FieldInfo field, IWorld world) {
            foreach (var drawer in GetComponentDrawers()) {
                try {
                    if (drawer.CanDrawGUI(field.GetValue(component))) {
                        return drawer.DrawGUI(entityVisualElement, component, field, world);
                    }
                } catch (Exception e) {
                    return new Label(e.Message);
                }
            }

            return new Label($"Not found drawer for {field.Name}");
        }
        
        public static VisualElement DrawTypeField(EntityVisualElement entityVisualElement, string name, object fieldValue, Type fieldType, object component, IWorld world) {
            foreach (var drawer in GetComponentDrawers()) {
                try {
                    if (drawer.CanDrawGUI(fieldValue)) {
                        return drawer.DrawGUI(entityVisualElement, name, fieldValue, fieldType, component, world);
                    }
                } catch (Exception e) {
                    return new Label(e.Message);
                }
            }

            return new Label($"Not found drawer for {fieldType.Name}");
        }

        private static List<AbstractComponentFieldDrawer> GetComponentDrawers() {
            if (_componentFieldDrawers == null || _componentFieldDrawers.Count == 0) {
                _componentFieldDrawers = new List<AbstractComponentFieldDrawer>();
                
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                    Type[] types;
                    try {
                        types = assembly.GetTypes();
                    }catch (ReflectionTypeLoadException ex) {
                        types = ex.Types.Where(t => t != null).ToArray();
                    }

                    foreach (var type in types) {
                        if (type == null || !type.IsClass || type.IsAbstract) {
                            continue;
                        }

                        if (type.GetCustomAttributes(typeof(ComponentFieldDrawerAttribute), inherit: false).Any()) {
                            try {
                                if (Activator.CreateInstance(type) is AbstractComponentFieldDrawer instance) {
                                    _componentFieldDrawers.Add(instance);
                                }
                            }
                            catch (Exception e) {
                                Debug.LogError($"Failed to instantiate {type.FullName}: {e.Message}");
                            }
                        }
                    }
                }
                
                _componentFieldDrawers = _componentFieldDrawers.OrderBy(x=> x.GetType().GetCustomAttribute<ComponentFieldDrawerAttribute>().Order).ToList();
            }

            return _componentFieldDrawers;
        }
    }
}