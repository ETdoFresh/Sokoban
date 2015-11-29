using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Status : MonoBehaviour
{
    private static GameObject _instance;
    private static Text _text;

    void Start()
    {
        if (_instance != null)
            Destroy(_instance);

        _instance = gameObject;
        _text = transform.FindChild("Canvas").FindChild("Text").GetComponent<Text>();
    }

    public static void SetText(string text)
    {
        if (_text != null)
            _text.text = text;
    }
}
