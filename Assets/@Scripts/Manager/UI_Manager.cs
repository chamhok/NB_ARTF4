using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Scene;
using Scripts.UI;
using Scripts.UI.Popup;
using Scripts.Utility;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    #region Fields

    private int _sortByOrderLayer = 1;
    private Stack<Popup> _popupOrder = new();

    #endregion

    private GameObject UIBase
    {
        get
        {
            GameObject uiBase = GameObject.Find("@UI_Base");
            if (uiBase == null)
            {
                uiBase = new GameObject { name = "@UI_Base" };
            }
            return uiBase;
        }
    }

    public T OpenPopup<T>(string objectName = null) where T : Popup
    {
        string objName = NameOfUI<T>(objectName);
        // TODO : 리소스 매니져 생성 전 강제 인스턴스 화
        T popup = SetUI<T>(objName, UIBase.transform);
        popup.name = $"{objName}";
        _popupOrder.Push(popup);
        SetTimeScale();
        return popup;
    }

    public void ClosePopUp(Popup popup)
    {
        if (_popupOrder.Count == 0) return;
        if (_popupOrder.Peek() != popup) return;
        Popup openPopup = _popupOrder.Pop();
        _sortByOrderLayer--;
        Destroy(openPopup.gameObject);
        SetTimeScale();
    }

    public void SetSceneUI<T>() where T : UI_Base
    {
        string sceneTypeName = typeof(T).Name;
        SetUI<T>(sceneTypeName, UIBase.transform);
    }

    private static string NameOfUI<T>(string name)
    {
        return string.IsNullOrEmpty(name) ? typeof(T).Name : name;
    }

    private T SetUI<T>(string uiName, Transform parent = null) where T : Component
    {
        GameObject uiObject = Main.Resource.InstantiatePrefab($"{uiName}.prefab", parent);
        T ui = SceneUtility.GetAddComponent<T>(uiObject);
        ui.transform.SetParent(UIBase.transform);
        return ui;
    }

    public void OrderLayerToCanvas(GameObject uiObject, bool sort = true)
    {
        Canvas canvas = SceneUtility.GetAddComponent<Canvas>(uiObject);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;
        SortingOder(canvas, sort);
        CanvasScaler scales = SceneUtility.GetAddComponent<CanvasScaler>(canvas.gameObject);
        scales.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scales.referenceResolution = new Vector2(1920, 1080);
        canvas.referencePixelsPerUnit = 100;
    }

    private void SortingOder(Canvas canvas, bool sort)
    {
        if (sort)
        {
            canvas.sortingOrder = _sortByOrderLayer;
            _sortByOrderLayer++;
        }
        else
        {
            canvas.sortingOrder = 0;
        }
    }

    private void SetTimeScale()
    {
        Debug.Log(Main.Scene.CurrentScene.ToString());
        if (Main.Scene.CurrentScene != Label.GameScene)
        {
            Time.timeScale = 1;
            return;
        }
        Time.timeScale = _popupOrder.Count > 0 ? 0 : 1;
    }
}
