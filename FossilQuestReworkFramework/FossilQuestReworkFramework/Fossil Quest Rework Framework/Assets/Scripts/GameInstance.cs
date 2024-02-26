using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameState 
{ 
    SplashScreen = 0,
    Started
}

public enum Tool
{
    Chisel = 0,
    Brush
}

public class GameInstance
{
    public GameState gameState;
    public UIInstance uiInstance;
    public DustParticleController dustParticles;

    public Fossil fossil;
    public Artifact[] artifacts;

    public Fish fish;

    public event System.Action OnGameStarted;
    public event System.Action OnGameEnded;

    private Vector3[] testDist;

    private List<GameObject> cubeTest = new List<GameObject>();

    private Tool currentTool;

    public Tool CurrentTool { get { return currentTool; } }

    public GameInstance() { }

    public GameInstance( UIInstance uiInstance, DustParticleController dustParticles, Fossil fossil, Artifact[] artifacts, Fish fish)
    {
        this.uiInstance = uiInstance;
        this.dustParticles = dustParticles;
        this.fossil = fossil;
        this.artifacts = artifacts;
        this.fish = fish;

        Initialise();
    }

    void Initialise()
    {

        Vector3 angle = uiInstance.fossilUI.transform.rotation.eulerAngles;
        fish.transform.rotation = Quaternion.Euler(angle);

        if (uiInstance.liveFishToggle != null)
        {
            uiInstance.liveFishToggle.onValueChanged.AddListener((selected) =>
            {
                //fish.fishModel.gameObject.SetActive(selected);

                if (selected)
                {
                    fish.SwitchModel(true);
                }
            });

        }
        if (uiInstance.fishFossilToggle != null)
        {
            uiInstance.fishFossilToggle.onValueChanged.AddListener((selected) =>
            {
                if (selected)
                {
                    fish.SwitchModel(false);
                }
            });
        }
      
        if (uiInstance.liveFishToggle != null) uiInstance.liveFishToggle.isOn = true;

        if (uiInstance.chiselToggle != null)
        {
            uiInstance.chiselToggle.onValueChanged.AddListener((selected) =>
            {
                if (selected)
                {
                    currentTool = Tool.Chisel;
                }
            });
        }

        if (uiInstance.brushToggle != null)
        {
            uiInstance.brushToggle.onValueChanged.AddListener((selected) =>
            {
                if (selected)
                {
                    currentTool = Tool.Brush;
                }
            });

            uiInstance.brushToggle.isOn = true;
        }

        currentTool = Tool.Brush;

        if (uiInstance.fishUI == null || uiInstance.fishUI.hotSpotToggles == null) return;

        testDist = new Vector3[uiInstance.fishUI.hotSpotToggles.Length];

        if (uiInstance.fishUI != null && uiInstance.fishUI.hotSpotToggles != null)
        {
            for (int i = 0; i < uiInstance.fishUI.hotSpotToggles.Length; i++)
            {
                //testDist[i] = fish.hotspotPoints[i].transform.InverseTransformDirection(uiInstance.fishUI.hotSpotToggles[i].transform.position) - fish.hotspotPoints[i].transform.position;
                testDist[i] = fish.hotspotPoints[i].transform.InverseTransformPoint(uiInstance.fishUI.hotSpotToggles[i].transform.position);
                testDist[i] = new Vector3(testDist[i].x, testDist[i].y, 0.2f);            
            }

            //TestDir();
        }

        if (fish != null && uiInstance.fishUI != null && uiInstance.fishUI.hotSpotToggles != null)
        {
            fish.OnFishStateChanged += (fish) =>
            {
                for (int i = 0; i < uiInstance.fishUI.hotSpotToggles.Length; i++)
                {
                    uiInstance.fishUI.hotSpotToggles[i].gameObject.SetActive(fish);
                    //CAN DO ANIMATIONS
                }
            };

            uiInstance.fishUI.OnShow += (show) =>
            {
                fish.gameObject.SetActive(show);
            };
        }

        if (uiInstance.artifactsLayout == null) return;

        if (fossil != null)
        {
            Image c = uiInstance.congratulationsUI;

            if (c != null) c.DOFade(0f, 0f);

            if (uiInstance.toolTipUI != null)
            {
                uiInstance.toolTipUI.SetDescription(Tool.Brush, "Use the brush to discover fossils and artifacts");

                fossil.artifactDigMaskCamera.OnObjectDiscovered += () =>
                {
                    uiInstance.toolTipUI.SetDescription(Tool.Brush, "You've discovered a fossil! Continue to brush to uncover it");
                };

                fossil.artifactDigMaskCamera.OnObjectUnmasked += () =>
                {
                    uiInstance.toolTipUI.SetDescription(Tool.Chisel, "Well done! Now USE THE CHISEL to remove the fossil from the rock");
                };

                fossil.OnChiselComplete += () =>
                {
                    uiInstance.toolTipUI.SetDescription(Tool.Brush, "Well done! Now USE THE BRUSH to remove the sand from the fossil");
                };
            }

            fossil.secondLayerBrushing.OnObjectUnmasked += () =>
            {
                if (c != null)
                {
                    uiInstance.toolTipUI.Hide(2f);

                    c.DOFade(1f, 2f).SetEase(Ease.Flash).onComplete += () =>
                    {
                        fossil.secondLayerBrushing.Dust.GetComponent<SpriteRenderer>().DOFade(0f, 2f);
                        uiInstance.fishUI.Show(true);
                    };

                }
            };
        }

        uiInstance.fishUI.Show(false);

    }

    // Update is called once per frame
    public void Update()
    {
        if (uiInstance.fishModelPos != null)
        {
            Vector3 pos = uiInstance.fishModelPos.transform.position;
            fish.transform.position = new Vector3(pos.x, pos.y, -3);

            Vector2 localPos = Vector2.zero;

            //RectTransformUtility.ScreenPointToLocalPointInRectangle(uiManager.screenRect, Camera.main.WorldToScreenPoint(fish.transform.position), Camera.main, out localPos);

            //Debug.Log(localPos);
            //fish.transform.rotation.eulerAngles.z;

            //Vector3 angle = uiInstance.fossilUI.transform.rotation.eulerAngles;
            //fish.transform.rotation = Quaternion.Euler(angle);
                    
        }

        if (uiInstance.fishUI != null && uiInstance.fishUI.hotSpotToggles != null)
        {
            for (int i = 0; i < uiInstance.fishUI.hotSpotToggles.Length; i++)
            {
                Vector3 pos = fish.hotspotPoints[i].transform.TransformDirection(testDist[i]);

                Vector3 finalPos = pos + fish.hotspotPoints[i].transform.position;

                finalPos = fish.hotspotPoints[i].transform.TransformPoint(testDist[i]);

                uiInstance.fishUI.hotSpotToggles[i].transform.position = pos + fish.hotspotPoints[i].transform.position;
                uiInstance.fishUI.hotSpotToggles[i].transform.position = new Vector3(finalPos.x, finalPos.y, -5);

                //Raycast
                RaycastHit hit;
                if (Physics.Raycast(uiInstance.fishUI.hotSpotToggles[i].transform.position, Vector3.forward, out hit, Mathf.Infinity, 1 << 6))
                {
                    Debug.Log("colliding with model");
                }
            }


            for (int j = 0; j < uiInstance.fishUI.arrows.Length; j++)
            {
                Vector3 rot = fish.rotateParent.transform.rotation.eulerAngles;

                Transform arrow = uiInstance.fishUI.arrows[j];

                arrow.transform.rotation = fish.rotateParent.transform.rotation;
            }
        }


       
    }

    public void StartGame()
    {
        GameManager.Instance.screenSaver.SetActive(false);
    }

    public void EndGame()
    {

    }

    void ReloadGame()
    {

    }
 
}
