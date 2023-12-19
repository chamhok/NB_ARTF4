using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Scripts.UI;
using TMPro;
using UnityEngine;

public class Game_UI : UI_Base
{
    #region Fields

    private enum Texts
    {
        Timer,
        TryCount
    }

    private int _tryCount = 1;
    private float _timer;

    #endregion


    private void Start()
    {
        Initialized();
    }

    protected override bool Initialized()
    {
        if (!base.Initialized()) return false;
        SetText(typeof(Texts));
        UpdateDeathCountUI();
        Main.UI.OnCloseDeathPanel += IncreaseDeathCount;
        return true;
    }

    private void FixedUpdate()
    {
        _timer += Time.deltaTime;
        GetText((int)Texts.Timer).text = _timer.ToString("N2");
    }

    private void IncreaseDeathCount()
    {
        _tryCount++;
        UpdateDeathCountUI();
    }

    private void UpdateDeathCountUI()
    {
        GetText((int)Texts.TryCount).text = $"Try : {_tryCount} 회";
    }
}
