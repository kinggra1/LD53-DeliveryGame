using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FadeInAfterSeconds : MonoBehaviour
{
    public enum ElementType { TEXT, IMAGE }
    public ElementType elementType = ElementType.TEXT;
    public float delaySeconds = 1f;
    public float fadeInTime = 1f;

    private RectTransform thisRect;
    private TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        thisRect = GetComponent<RectTransform>();
        text = GetComponent<TextMeshProUGUI>();
        Debug.Log(text);

        switch (elementType) {
            case ElementType.TEXT:
                Color32 targetColor = text.faceColor;
                Color32 startingColor = targetColor;
                startingColor.a = 0;
                text.faceColor = startingColor;
                LeanTween.value(this.gameObject, UpdateColorCallback, startingColor, targetColor, fadeInTime).setDelay(delaySeconds);
                LeanTween.textAlpha(thisRect, 0f, 1f);
                break;
            case ElementType.IMAGE:
                break;
        }
    }

    void UpdateColorCallback(Color val) {
        text.faceColor = val;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
