#if !SERVER_DRIVE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BlackAm
{
    public class AutoButton : AbstractUI
    {
        const float speed=40;

        private void OnEnable()
        {
            transform.localEulerAngles = new Vector3(0, 0, 0);
        }

        private void Update()
        {
            transform.Rotate(-Vector3.forward * Time.deltaTime * speed);
        }
    }
}
#endif