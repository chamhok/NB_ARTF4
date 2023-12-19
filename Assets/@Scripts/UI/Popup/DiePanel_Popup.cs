using System.Collections.Generic;
using Scripts.Event.UI;
using Scripts.UI.Popup;
using Scripts.Utility;
using UnityEngine;
using UnityEngine.EventSystems;

public class DiePanel_Popup : Popup
{
    #region Fields

    private enum Buttons
    {
        ContinueBtn,
        ExitBtn
    }
    #endregion


    #region Initialized

    private void Start()
    {
        Initialized();
    }

    protected override bool Initialized()
    {
        if (!base.Initialized()) return false;
        SetButton(typeof(Buttons));
        GetButton((int)Buttons.ContinueBtn).gameObject.SetEvent(UIEventType.Click, ReStartGame);
        GetButton((int)Buttons.ExitBtn).gameObject.SetEvent(UIEventType.Click, ReturnIntroScene);
        return true;
    }

    private void ReStartGame(PointerEventData obj)
    {
        // TODO : Restart인데 씬을 다시 불러와야 하는건 비 효율적 일 것 같아요
        Main.UI.CloseDeathPanel();
        DescribeEventTypes(); 
        // SceneUtility.LoadScene("GameScene"); 
    }

    private void ReturnIntroScene(PointerEventData obj)
    {
        DescribeEventTypes();
        SceneUtility.LoadScene("IntroScene");
    }

    private void DescribeEventTypes()
    {
        List<UIEventType> eventTypes = new List<UIEventType> { UIEventType.Click };
        Main.UI.ClosePopUp(this, eventTypes);
    }

    #endregion
}

