using System;
using System.Collections.Generic;
using Scripts.Event.UI;
using Scripts.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;
using Object = UnityEngine.Object;

namespace Scripts.UI
{
    public class UI_Base : MonoBehaviour
    {
        #region Fields

        private readonly Dictionary<Type, Object[]> _objects = new();
        private bool _initialized;


        #endregion

        #region Bind

        private void BindUIComponents<T>(Type enumType) where T : Object
        {
            SetBinder.Binding<T>(gameObject, enumType, _objects);
        }

        protected void SetButton(Type type) => BindUIComponents<Button>(type);
        protected void SetImage(Type type) => BindUIComponents<Image>(type);
        protected void SetText(Type type) => BindUIComponents<TextMeshProUGUI>(type);
        protected void SetObject(Type type) => BindUIComponents<GameObject>(type);

        #endregion

        #region Getter

        private T GetUIComponents<T>(int componentIndex) where T : Object
        {
            return SetBinder.Getter<T>(componentIndex, _objects);
        }

        protected Button GetButton(int componentIndex)
        {
            return GetUIComponents<Button>(componentIndex);
        }

        protected Image GetImage(int componentIndex)
        {
            return GetUIComponents<Image>(componentIndex);
        }

        protected TextMeshProUGUI GetText(int componentIndex)
        {
            return GetUIComponents<TextMeshProUGUI>(componentIndex);
        }

        protected GameObject GetObject(int componentsIndex)
        {
            return GetUIComponents<GameObject>(componentsIndex);
        }

        #endregion

        #region Initialized

        protected virtual bool Initialized()
        {
            if (_initialized) return false;
            _initialized = true;
            return _initialized;
        }

        #endregion
    }
}