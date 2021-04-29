using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Vampire.Binding
{
    public static class BlackboardPropertyFieldFactory
    {
        private delegate BindableElement CreationDelegate(Type t, object o);
        private static readonly Dictionary<Type, CreationDelegate> creationDictionary = new();
        private static readonly List<Type> creationTypes = new();
        private static bool initialized = false;
        private static string bindKey;
        private static PropertyFieldBinder fieldBinder;

        /// <summary>
        /// Returns all types drawable by the field factory.
        /// </summary>
        public static IReadOnlyCollection<Type> GetDrawableTypes()
        {
            if (!initialized)
            {
                RegisterAll();
                initialized = true;
            }

            return creationTypes;
        }
        
        public static BindableElement Create(Type t, object someObject, PropertyFieldBinder binder, 
            string bindTo = null)
        {
            bindKey = !string.IsNullOrEmpty(bindTo) ? 
                bindTo : PropertyFieldBinder.GenerateUniqueFieldIdentifier(someObject);
            fieldBinder = binder;
            CreationDelegate func = CheckIfRegistered(t);
            return func?.Invoke(t, someObject);
        }

        private static void Register<T>(CreationDelegate creationAction)
        {
            creationTypes.Add(typeof(T));
            creationDictionary.Add(typeof(T), creationAction);
        }

        private static void FastRegister<ArgType, FieldType>()
            where FieldType : BaseField<ArgType>, new()
        {
            Register<ArgType>(ConstructField<ArgType, FieldType>);
        }

        private static FieldType ConstructField<ArgType, FieldType>(Type t, object o)
            where FieldType : BaseField<ArgType>, new()
        {
            var m = new FieldType();
            fieldBinder.BindingResolver<ArgType, FieldType>(m, o, bindKey);
            return m;
        }

        private static void RegisterAll()
        {
            FastRegister<float, FloatField>();
            FastRegister<int, IntegerField>();
            FastRegister<string, TextField>();
            FastRegister<Vector2, Vector2Field>();
            FastRegister<Vector3, Vector3Field>();
            FastRegister<Vector4, Vector4Field>();
            FastRegister<Vector2Int, Vector2IntField>();
            FastRegister<Vector3Int, Vector3IntField>();
            FastRegister<Bounds, BoundsField>();
            FastRegister<Color, ColorField>();
            FastRegister<AnimationCurve, CurveField>();
            FastRegister<Gradient, GradientField>();
            FastRegister<Rect, RectField>();
            FastRegister<bool, Toggle>();
            Register<UnityEngine.Object>((t, o) =>
            {
                var m = new ObjectField {objectType = t, allowSceneObjects = false};
                fieldBinder.BindingResolver<UnityEngine.Object, ObjectField>(m, o, bindKey);
                return m;
            });
        }

        private static CreationDelegate CheckIfRegistered(Type t)
        {
            if (!initialized)
            {
                RegisterAll();
                initialized = true;
            }

            //Check for exact matches
            if (creationDictionary.TryGetValue(t, out var creationFunc))
                return creationFunc;

            //We found no exact match, our fall back is checking the entire inheritance tree.
            return (from item
                    in creationTypes
                where item.IsAssignableFrom(t)
                select creationDictionary[item]).FirstOrDefault();
        }
    }
}