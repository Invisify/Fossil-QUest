using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Linq;

public class FishUI : MonoBehaviour
{
    RectTransform rectTransform;
    public Image backGround;
    public CanvasGroup canvasGroup;

    public Button closeButton;

    public ToggleGroup hotSpotToggleGroup;

    public Toggle[] hotSpotToggles;
    public Transform[] arrows;

    //page1
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI titleText2;

    public TextMeshProUGUI descriptionText;

    public TextMeshProUGUI habitatText;
    public TextMeshProUGUI sizeText;
    public TextMeshProUGUI distributionText;
    public TextMeshProUGUI classificationText;
    //

    public Image[] timePeriodBars;

    public Toggle liveFishToggle;
    public Toggle fishFossilToggle;

    public RectTransform pagesParent;

    public Image[] backGroundSlices;

    private int currPage;
    private int activePeriod;

    public event System.Action<bool> OnShow;
    public event System.Action<int> OnPageChanged;

    // Start is called before the first frame update

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        //hotSpotToggles = 
        //hotSpotToggles = hotSpotToggleGroup.ActiveToggles().ToArray();

        hotSpotToggles = hotSpotToggleGroup.GetComponentsInChildren<Toggle>();
    }
    void Start()
    {     
        closeButton.onClick.AddListener(() => { Show(false); });

        arrows = new Transform[hotSpotToggles.Length];

        for (int i = 0; i < hotSpotToggles.Length; i++)
        {
            int index = i;

            Toggle t = hotSpotToggles[index];
            t.onValueChanged.AddListener((val) =>
            {
                if (val)
                {               
                    ChangePage(index);
                }

                //TEMP METHODS
                SetToggle(t, !val);
                
                //t.gameObject.SetActive(!val);

                //WITHOUT ANIMATIONS
                if (index < pagesParent.transform.childCount)
                    pagesParent.transform.GetChild(index).gameObject.SetActive(val);
                
            });

            Transform arrow = hotSpotToggles[i].transform.GetChild(1).transform;
            arrows[i] = arrow;
        }

        SetToggle(hotSpotToggles[0], false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Rotate()
    {

    }

    public void Show(bool show)
    {
        gameObject.SetActive(show);

        if (show)
        {
            titleText.alpha = 0f;
            titleText2.alpha = 0f ;
            descriptionText.alpha = 0f;
            //backGround.transform.localScale = Vector3.zero;
            //backGround.DOFade(0f, 0f);
            canvasGroup.alpha = 0f;
        
            Sequence seq = DOTween.Sequence();
            //seq.Append(backGround.transform.DOScale(1f, 1f));

            seq.Append(canvasGroup.DOFade(1f, 1f));
            seq.AppendCallback(() =>
            {
                titleText.DOFade(1f, 1f).SetEase(Ease.OutQuad);
                titleText2.DOFade(1f, 1f).SetEase(Ease.OutQuad);
            });
            seq.AppendInterval(0.2f);
            seq.AppendCallback(() =>
            {
                descriptionText.DOFade(1f, 1f).SetEase(Ease.OutQuad);
            });

            if (OnShow != null) OnShow(show);
        }
        else
        {
            if (OnShow != null) OnShow(show);
        }
    }

    void SetToggle(Toggle t, bool on)
    {
        t.interactable = on;
        t.transform.GetChild(0).gameObject.SetActive(on);
        t.transform.GetChild(1).gameObject.SetActive(on);
    }
    public void ChangePage(int page)
    {
        if (pagesParent.transform.childCount - 1 > page) return;

        currPage = page;
        
    }
}
