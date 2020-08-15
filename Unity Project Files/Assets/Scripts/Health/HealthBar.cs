using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class HealthBar : MonoBehaviour
{
    public static float currentPercentHP = 1;
    float startY = 0;
    float endY = -2f;
    float currentY = 0f;

    float startScale = 6.9f;
    float endScale = 0f;
    float currentScale = 6.9f;

    void Update()
    {
        if (currentPercentHP > 1)
            currentPercentHP = 1;

        if (currentPercentHP < 0)
            currentPercentHP = 0;

        resizeRemove(1 - currentPercentHP);
    }


    void resizeRemove(float p)
    {    
        float dif1 = 2f;    // startY - endY;
        float dif2 = 6.9f;  // startScale - endScale;

        currentY = startY - (dif1 * p);

        currentScale = startScale - (dif2 * p);

        this.gameObject.transform.localScale = new Vector3(1f, currentScale, 1f);
        this.gameObject.transform.position = new Vector3(14.5f, currentY, 0f);
    }

}
