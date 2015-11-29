using UnityEngine;
using System.Collections;
using System;

public class PaperResize : MonoBehaviour
{
    Transform code;
    Transform bottom;
    TextMesh textMesh;
    BoxCollider boxCollider;

    // Use this for initialization
    void Start()
    {
        code = transform.FindChild("Code");
        bottom = transform.FindChild("Paper").FindChild("BottomPaper");
        textMesh = code.GetComponent<TextMesh>();
        boxCollider = GetComponent<BoxCollider>();
        SetText(textMesh.text);
    }
    
    public void SetText(string text)
    {
        textMesh.text = text;
        int newLines = 0;
        foreach (char c in textMesh.text) if (c == '\n') newLines++;
        newLines -= 8;
        newLines = Math.Max(0, newLines);
        Vector3 scale = bottom.localScale;
        scale.z = newLines / 57f;
        bottom.localScale = scale;

        Vector3 bcSize = boxCollider.size;
        Vector3 bcCenter = boxCollider.center;
        bcSize.y = newLines * 0.042f + 0.4f;
        bcCenter.y = 2.8f - (bcSize.y / 2);
        boxCollider.size = bcSize;
        boxCollider.center = bcCenter;
    }
}
