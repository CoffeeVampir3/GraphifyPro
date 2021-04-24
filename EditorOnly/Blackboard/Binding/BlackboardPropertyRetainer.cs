using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;

namespace Vampire.Binding
{
    [CreateAssetMenu(menuName = "Blackboard")]
    public class BlackboardPropertyRetainer : SerializedScriptableObject
    {
        [OdinSerialize] 
        private object[] serializedObjects;
        [SerializeField] 
        private string[] serializedKeys;
        //then we handle all the edge cases object mysteriously doesn't work on ~,~
        [SerializeField] 
        private UnityEngine.Object[] unityObjects;
        [SerializeField] 
        private string[] serializedUnityKeys;
        [SerializeField] 
        private AnimationCurve[] serializedCurves;
        [SerializeField] 
        private string[] serializedCurveKeys;
        [SerializeField]
        private Gradient[] serializedGradients;
        [SerializeField] 
        private string[] serializedGradientKeys;
        [SerializeField]
        private string[] serializedStrings;
        [SerializeField] 
        private string[] serializedStringKeys;

        public void Serialize(Dictionary<string, object> data)
        {
            var objects = new List<object>(16);
            var objectKeys = new List<string>(16);
            var strings = new List<string>(16);
            var stringKeys = new List<string>(16);
            var unityObjs = new List<UnityEngine.Object>(16);
            var unityKeys = new List<string>(16);
            var curves = new List<AnimationCurve>(8);
            var curveKeys = new List<string>(8);
            var gradients = new List<Gradient>(8);
            var gradientKeys = new List<string>(8);
            var hashedKeys = new HashSet<string>();

            foreach (var item in data)
            {
                if (hashedKeys.Contains(item.Key))
                {
                    Debug.LogWarning("Attempted to serialize multiple of the same key named: " + item.Key);
                    continue;
                }
                hashedKeys.Add(item.Key);
                switch (item.Value)
                {
                    case UnityEngine.Object uObj:
                        unityObjs.Add(uObj);
                        unityKeys.Add(item.Key);
                        continue;
                    case Gradient grad:
                        gradients.Add(grad);
                        gradientKeys.Add(item.Key);
                        continue;
                    case AnimationCurve curve:
                        curves.Add(curve);
                        curveKeys.Add(item.Key);
                        continue;
                    case string str:
                        strings.Add(str);
                        stringKeys.Add(item.Key);
                        continue;
                    default:
                        Debug.Log(item.Key + " " + item.Value);
                        objects.Add(item.Value);
                        objectKeys.Add(item.Key);
                        continue;
                }
            }
            
            EditorUtility.SetDirty(this);

            serializedObjects = objects.ToArray();
            serializedKeys = objectKeys.ToArray();
            unityObjects = unityObjs.ToArray();
            serializedUnityKeys = unityKeys.ToArray();
            serializedGradients = gradients.ToArray();
            serializedGradientKeys = gradientKeys.ToArray();
            serializedCurves = curves.ToArray();
            serializedCurveKeys = curveKeys.ToArray();
            serializedStrings = strings.ToArray();
            serializedStringKeys = stringKeys.ToArray();
            
            EditorUtility.SetDirty(this);
        }

        public Dictionary<string, object> Deserialize()
        {
            Dictionary<string, object> data = new();
            for (var i = 0; i < serializedObjects.Length; i++)
            {
                var item = serializedObjects[i];
                var key = serializedKeys[i];
                data.Add(key, item);
            }

            for (var i = 0; i < serializedStrings.Length; i++)
            {
                var item = serializedStrings[i];
                var key = serializedStringKeys[i];
                data.Add(key, item);
            }

            for (var i = 0; i < unityObjects.Length; i++)
            {
                var item = unityObjects[i];
                var key = serializedUnityKeys[i];
                data.Add(key, item);
            }
            
            for (var i = 0; i < serializedCurves.Length; i++)
            {
                var item = serializedCurves[i];
                var key = serializedCurveKeys[i];
                data.Add(key, item);
            }
            
            for (var i = 0; i < serializedGradients.Length; i++)
            {
                var item = serializedGradients[i];
                var key = serializedGradientKeys[i];
                data.Add(key, item);
            }

            return data;
        }
    }
}