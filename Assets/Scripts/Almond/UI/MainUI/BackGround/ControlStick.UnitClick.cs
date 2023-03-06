#if !SERVER_DRIVE
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

namespace UI2020
{
    public partial class ControlStick :IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.dragging) return;

            OnUnitClickEventPointerUp(eventData);
        }
    }
}


#endif