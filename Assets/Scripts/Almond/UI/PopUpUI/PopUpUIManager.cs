using System;
using System.Collections;
using k514;
using UnityEngine;
using UnityEngine.UI;

#if !SERVER_DRIVE
namespace UI2020
{
    public class PopUpUIManager : AbstractUI
    {
        private static PopUpUIManager _instance;
        public static PopUpUIManager Instance => _instance;

        public GameObject touchLock;
        
        public void Init()
        {
            if (_instance == null) _instance = this;

            touchLock = Find("TouchLock").gameObject;
            touchLock.SetActive(false);

            SetActive(true);
        }
    }
}
#endif