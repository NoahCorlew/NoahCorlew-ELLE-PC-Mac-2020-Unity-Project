using System;
using UnityEngine;
public class TextMeshSharpener : MonoBehaviour
{
    /*
    Makes TextMesh look sharp regardless of camera size/resolution
    Do NOT change character size or font size; use scale only
    */
    // Properties
    private float lastPixelHeight = -1;
    private TextMesh textMesh;
    void Start()
    {
        textMesh = GetComponent<TextMesh>();
        resize();
        textMesh.fontSize = 40;
    }
    void Update()
    {
        // Always resize in the editor, or when playing the game, only when the resolution changes
        if (Camera.main.pixelHeight != lastPixelHeight || (Application.isEditor && !Application.isPlaying)) resize();
    }
    private void resize()
    {
        float ph = Camera.main.pixelHeight;
        float ch = Camera.main.orthographicSize;
        float pixelRatio = (ch * 2.0f) / ph;
        float targetRes = 128f;
        textMesh.characterSize = pixelRatio * Camera.main.orthographicSize / Math.Max(transform.localScale.x, transform.localScale.y);
        textMesh.fontSize = (int)Math.Round(targetRes / textMesh.characterSize);
        lastPixelHeight = ph;
    }
}

// https://answers.unity.com/questions/20633/textmesh-looking-fuzzy.html