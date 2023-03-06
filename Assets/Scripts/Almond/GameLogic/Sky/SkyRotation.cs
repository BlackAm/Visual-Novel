using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using k514;

public class SkyRotation : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed = 0;
    private float dir;
#if !UNITY_EDITOR
    private void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", dir += Time.deltaTime * speed);
    }
#endif
    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }
}
