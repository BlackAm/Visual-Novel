#if !SERVER_DRIVE
using System;
using System.Collections;
using System.Collections.Generic;
using k514;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//ServerConnectUI.cs
public class MenuController : KeyCodeTouchEventSenderBase
{
    private Transform touchBase;

    public override void OnSpawning()
    {
        base.OnSpawning();
        touchBase = transform.Find("LoginButton/Button");
    }

    private void Awake()
    {
        throw new NotImplementedException();
    }


    protected override void OnKeyCodeEventPointerDown(PointerEventData p_EventData)
    {
        base.OnKeyCodeEventPointerDown(p_EventData);
        
    }
}

#endif