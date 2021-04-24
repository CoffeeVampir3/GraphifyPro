using System;
using System.Collections.Generic;
using UnityEditor.GraphToolsFoundation.Overdrive;
using UnityEngine;

namespace Vampire.Graphify.EditorOnly
{
    [Serializable]
    public class InfoCollection<InfoItem, Definition>
    where InfoItem : class
    {
        [SerializeField] 
        private List<string> fieldNames = new();
        [SerializeField] 
        private List<InfoItem> infoList = new();
        public IReadOnlyList<InfoItem> InfoList => infoList;

        public bool TryGetInfo(in IPortModel port, out InfoItem info)
        {
            return TryGetInfo(port.UniqueName, out info);
        }

        public bool TryGetInfo(in string portFieldName, out InfoItem info)
        {
            for (int i = 0; i < fieldNames.Count; i++)
            {
                if (!string.Equals(portFieldName, fieldNames[i], StringComparison.Ordinal)) 
                    continue;
                info = infoList[i];
                return true;
            }
            info = null;
            return false;
        }

        public InfoItem AddOrUpdateInfo(string fieldName, Definition defAttrib, Func<Definition, InfoItem> createInfo)
        {
            var pInfo = createInfo.Invoke(defAttrib);
            for (int i = 0; i < fieldNames.Count; i++)
            {
                if (!string.Equals(fieldName, fieldNames[i], StringComparison.Ordinal)) 
                    continue;
                infoList[i] = pInfo;
                return pInfo;
            }
            
            fieldNames.Add(fieldName);
            infoList.Add(pInfo);
            return pInfo;
        }

        public void RemoveUnused(HashSet<string> usedPortNames)
        {
            for (var i = fieldNames.Count - 1; i >= 0; i--)
            {
                if (usedPortNames.Contains(fieldNames[i])) continue;
                infoList.RemoveAt(i);
                fieldNames.RemoveAt(i);
            }
        }
    }
}