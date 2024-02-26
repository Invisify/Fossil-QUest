using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Fossil : Artifact
{
    public MaskCamera secondLayerBrushing;

    public SpriteRenderer fossilSecondLayer;

    public GameObject ObjectToChisel;
    public SpriteRenderer FinalObject;

    public GameObject ChiselParent;

    public event System.Action OnChiselComplete;

    //TEMPORARY FIELD
    private bool complete = false;

    // Start is called before the first frame update

    private void Awake()
    {
        //InitialiseFragments;
        ObjectToChisel.gameObject.SetActive(true);

        foreach (Explodable e in GetComponentsInChildren<Explodable>()) e.fragmentInEditor();

        ObjectToChisel.gameObject.SetActive(false);

        artifactDigMaskCamera.OnObjectUnmasked += () =>
        {
            fossilSecondLayer.maskInteraction = SpriteMaskInteraction.None;
            artifactDigMaskCamera.Dust.GetComponent<SpriteRenderer>().enabled = false;

            artifactDigMaskCamera.ObjectToUnmask.transform.GetChild(0).gameObject.SetActive(true);

            fossilSecondLayer.transform.DOScale(1f, 2f).onComplete += ()=> 
            {
                ObjectToChisel.SetActive(true);

                artifactDigMaskCamera.ObjectToUnmask.transform.GetChild(0).gameObject.SetActive(false);
                //artifactDigMaskCamera.ObjectToUnmask.transform.GetChild(0).GetComponent<SpriteRenderer>().DOFade(0f, .5f);


                /*foreach (Transform t in ObjectToChisel.transform)
                {
                    SpriteRenderer r = t.GetComponent<SpriteRenderer>();

                    if (r)
                    {
                        r.DOFade(0f, 0f);
                        r.DOFade(1f, 0.5f);
                    }
                }*/

                artifactDigMaskCamera.ObjectToUnmask.GetComponent<SpriteRenderer>().DOFade(0f, 1f).onComplete += () => 
                { 
                    artifactDigMaskCamera.ObjectToUnmask.SetActive(false); 
                    //ObjectToChisel.SetActive(true);
                };

                

                //artifactDigMaskCamera.ObjectToUnmask.SetActive(false);

                secondLayerBrushing.gameObject.SetActive(true);
            };

        };       
    }
    void Start()
    {            
      
    }

    // Update is called once per frame
    void Update()
    {
        if (!complete)
        {
            if (ChiselParent.transform.childCount == 0)
            {
                complete = true;
                if (OnChiselComplete != null) OnChiselComplete();
            }
        }
        
    }
}
