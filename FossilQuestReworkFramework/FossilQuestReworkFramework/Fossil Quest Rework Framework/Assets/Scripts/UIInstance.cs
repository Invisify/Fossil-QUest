using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInstance : MonoBehaviour
{
    public Toggle brushToggle;
    public Toggle chiselToggle;

    public Toggle liveFishToggle;
    public Toggle fishFossilToggle;

    public RectTransform artifactsLayout;

    public Image fossilUI;
    public List<Image> artifactUI;

    public RectTransform fishModelPos;
    public RectTransform fishUIRect;

    public RectTransform screenRect;

    public FishUI fishUI;

    public Image congratulationsUI;

    public Tooltip toolTipUI;

    public TouchFeedback touchFeedback;

    void Awake()
    {
        screenRect = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
