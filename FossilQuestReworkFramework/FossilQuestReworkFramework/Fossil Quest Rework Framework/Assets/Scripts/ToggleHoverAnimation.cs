using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class ToggleHoverAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Toggle toggle;

    [Header("Text Change")]
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color hoverColor;
    [SerializeField] private bool isPressedColorAllow;
    [SerializeField] private Color pressedColor;

    [Space(10)]
    [SerializeField] private bool enableFontChange = false;
    [SerializeField] private TMP_FontAsset defaultFont;
    [SerializeField] private TMP_FontAsset hoverFont;
    [SerializeField] private bool isPressedFontAllow;
    [SerializeField] private TMP_FontAsset pressedFont;

    [Header("Icon Change")]
    [SerializeField] private Image icon;
    [SerializeField] private Sprite defaultIcon;
    [SerializeField] private Sprite hoverIcon;
    [SerializeField] private bool isPressedIconAllow;
    [SerializeField] private Sprite pressedIcon;

    [Header("DOTween Animation")]
    [SerializeField] private Image DOHoverIcon;
    [SerializeField] private float idleOpacity = 0;
    [SerializeField] private float hoveredOpacity = 0.6f;
    [SerializeField] private float aniSpeed = 0.2f;
    [SerializeField] private bool isPressedDOIconAllow;
    [SerializeField] private float pressedOpacity = 1f;

    [Header("Toggle Icon BackGround")]
    [Tooltip("Assign the background & Use it if you need to hide the checkmark background when is on, otherwise leave it empty")]
    [SerializeField] private Image checkmarkBackground;

    void Awake()
    {
        toggle = this.GetComponent<Toggle>();

        toggle.onValueChanged.AddListener((isOn) =>
        {
            if (isOn)
            {
                if (isPressedColorAllow)
                    text.color = pressedColor;

                if (isPressedIconAllow)
                    icon.sprite = pressedIcon;

                if (isPressedDOIconAllow)
                    DOHoverIcon.color = new Color(1, 1, 1, pressedOpacity);

                if (enableFontChange && isPressedFontAllow)
                    text.font = pressedFont;
            }
            else
            {
                if (isPressedColorAllow)
                    text.color = defaultColor;

                if (isPressedIconAllow)
                    icon.sprite = defaultIcon;

                if (isPressedDOIconAllow)
                    DOHoverIcon.color = new Color(1, 1, 1, idleOpacity);

                if (enableFontChange && isPressedFontAllow)
                    text.font = defaultFont;
            }

            if (checkmarkBackground != null)
                checkmarkBackground.enabled = !isOn;

            Debug.Log("toggle selected and changed color or icon");
        });
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!toggle.isOn)
        {
            if (text != null)
            {
                text.color = hoverColor;

                if (enableFontChange)
                    text.font = hoverFont;
            }
            if (icon != null)
            {
                icon.sprite = hoverIcon;
            }
            if (DOHoverIcon != null)
            {
                DOHoverIcon.DOFade(hoveredOpacity, aniSpeed);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!toggle.isOn)
        {
            if (text != null)
            {
                text.color = defaultColor;

                if (enableFontChange)
                    text.font = defaultFont;
            }
            if (icon != null)
            {
                icon.sprite = defaultIcon;
            }
            if (DOHoverIcon != null)
            {
                DOHoverIcon.DOFade(idleOpacity, aniSpeed);
            }
        }
    }
}
