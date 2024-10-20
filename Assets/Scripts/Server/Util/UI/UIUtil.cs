using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Almond.UI
{
    public class UIUtil
    {

        static private Dictionary<string, string> m_widgetToFullName = new Dictionary<string, string>();

        private static void AddWigetToFullNameData(string widgetName, string fullName)
        {
            if (m_widgetToFullName.ContainsKey(widgetName))
            {
                if (Application.isEditor)
                    Debug.LogError("Same Key In the WidgetName to FullName Dict ,Now Replace It! : " + widgetName);
            }
            m_widgetToFullName[widgetName] = fullName;
        }
        private static void DelWigetFullNameData(string widgetName)
        {
            if (m_widgetToFullName.ContainsKey(widgetName))
            {
                m_widgetToFullName.Remove(widgetName);
            }
        }
        public static Transform GetUI(string uiName, Transform uiObject)
        {
            string uiFullName = GetFullName(uiName);
            if (string.IsNullOrEmpty(uiFullName)) return null;
            return uiObject.Find(uiFullName);
        }
        private static string GetFullName(Transform currentTransform, Transform rootTransform)
        {
            string fullName = "";

            while (currentTransform != rootTransform)
            {
                fullName = currentTransform.name + fullName;

                if (currentTransform.parent != rootTransform)
                {
                    fullName = "/" + fullName;
                }

                currentTransform = currentTransform.parent;
            }

            return fullName;
        }
        public static void FillFullNameData(Transform rootTransform)
        {
            FillFullNameData(rootTransform, rootTransform);
        }
        public static void FillFullNameData(Transform rootTransform, Transform thisTransform, bool delete = false)
        {
            for (int i = 0; i < rootTransform.childCount; ++i)
            {
                if (!delete) AddWigetToFullNameData(rootTransform.GetChild(i).name, GetFullName(rootTransform.GetChild(i), thisTransform));
                else DelWigetFullNameData(rootTransform.GetChild(i).name);
                FillFullNameData(rootTransform.GetChild(i), thisTransform, delete);
            }
        }
        public static void DeleteFullNameData(Transform rootTransform)
        {
            DeleteFullNameData(rootTransform, rootTransform);
        }
        public static void DeleteFullNameData(Transform rootTransform, Transform thisTransform)
        {
            FillFullNameData(rootTransform, thisTransform, true);
        }
        public static string GetFullName(string name)
        {
            if (!m_widgetToFullName.ContainsKey(name))
            {
                if (Application.isEditor)
                    Debug.LogError(string.Concat(name, " can not found a fullName !"));

                return "";
            }
            else
            {
                return m_widgetToFullName[name];
            }
        }
        public static void Clear()
        {
            m_widgetToFullName.Clear();
        }
    }

}
