using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Tooltip : MonoBehaviour
{
    public TextMeshProUGUI descriptionUI;
    public Image toolImageUI;

    private RectTransform descriptionRect;
    private CanvasGroup canvasGroup;

    public Sprite brushSprite;
    public Sprite chiselSprite;

    void Awake()
    {
        descriptionRect = descriptionUI.GetComponent<RectTransform>();

        canvasGroup = GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetDescription(Tool tool, string description, float width = 105)
    {
        canvasGroup.DOFade(1f, 1f);
        
        if (tool == Tool.Brush) toolImageUI.sprite = brushSprite;
        else if (tool == Tool.Chisel) toolImageUI.sprite = chiselSprite;

        descriptionRect.sizeDelta = new Vector2(width, descriptionRect.sizeDelta.y);
        descriptionUI.text = description;

    }

    public void Hide(float seconds = 1f)
    {
        canvasGroup.DOFade(0f, 1f);
    }
}
