using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.Networking;
using UnityEngine.Video;
using System.IO;

public class GameManager : Singleton<GameManager>
{
    public int gameType;

    public GameInstance[] gameInstances;

    private float timer;

    public float resetTimer;

    public GameObject screenSaver;

    public Canvas mainCanvas;

    private GameData[] gameData;
    private GameData currData;

    public UIInstance uiInstancePrefab;
    public GameObject artifactUIPrefab;
    public DustParticleController dustParticlePrefab;

    public GameObject backGround;

    public bool debug = true;
    public bool spawnArtifacts = true;
    public bool spawnFossil = true;

    public bool hideArtifacts = false;
    public bool hideFossil = false;

    private void Awake()
    {
        LoadData();
        CreateGameInstances();

    }

    // Start is called before the first frame update
    void Start()
    {
        if (screenSaver)
        {
            screenSaver.SetActive(true);
            screenSaver.transform.SetAsLastSibling();
        }
           
        TouchManager.Instance.OnFingerDown += (f) => { timer = 0f; };

        Debug.Log("H: " + Screen.height + " W: " + Screen.width);
    }

    // Update is called once per frame
    void Update()
    {
        if (screenSaver && !screenSaver.activeSelf)
            timer += Time.deltaTime;

        if (timer > resetTimer)
        {
            //ScreenSaver;
            SceneManager.LoadScene("SampleScene");
            timer = 0f;
        }

        for (int i = 0; i < gameInstances.Length; i++)
        {
            if (gameInstances[i] != null)
                gameInstances[i].Update();
        }
    }

    void LoadData()
    {
        LoadStreamingAsset();

        gameData = Resources.LoadAll<GameData>("Game");

        Debug.Log("GameType: " + gameType);
        Debug.Log(gameData.Length);

        for (int i = 0; i < gameData.Length; i++)
        {
            if (gameType == gameData[i].GameType)
            {
                currData = gameData[i];

                Debug.Log("Game Type Successfully Found");
                return;
            }
        }

        //if did not find any game data based on game type, set to default
        currData = gameData.Where(x => x.GameType == 0).First();

        Debug.LogWarning("Game Type Not Found, Setting Game Type to Default");
    }

    void CreateGameInstances()
    {
        gameInstances = new GameInstance[currData.NoOfScreens];

        for (int i = 0; i < currData.NoOfScreens; i++)
        {
            if (uiInstancePrefab == null) break;
            if (debug) break;

            ScreenSize s = ScreenData.screenSizes[currData.NoOfScreens - 1];

            UIInstance newUIInstance = Instantiate(uiInstancePrefab, mainCanvas.transform);
            newUIInstance.name = "UI Instance " + i;
            RectTransform r = newUIInstance.screenRect;

            r.sizeDelta = s.sizes[i];
            r.anchoredPosition = s.anchoredPos[i];
            r.transform.rotation = Quaternion.Euler(s.rotations[i]);

            r.anchorMin = s.anchors[i];
            r.anchorMax = s.anchors[i];
            r.pivot = s.pivots[i];

            //try gameobject stuff

            GameObject newGameParent = new GameObject();
            newGameParent.transform.position = Vector3.zero;
            newGameParent.name = "Game Instance " + i;

            Fossil newFossil = Instantiate(currData.FishPositionData.ArtifactData.ArtifactGameObject, newGameParent.transform).GetComponent<Fossil>();

            Vector3 newPos = r.transform.position;
            newPos = Camera.main.WorldToScreenPoint(newPos);

            Vector3 offset = currData.FishPositionData.PositionInRect;
            newPos += (r.transform.right * offset.x) + (r.transform.up * offset.y);
            newPos = Camera.main.ScreenToWorldPoint(newPos);
            newPos.z = 3f;
            newFossil.artifactParent.transform.position = newPos;
            newFossil.artifactParent.transform.rotation = Quaternion.Euler(s.rotations[i]);

            //newFossil.firstLayerBrushing.transform.rotation = Quaternion.Euler(s.rotations[i]);
            //newFossil.firstLayerBrushing.GetComponent<Camera>().rect = r.rect;

            newFossil.secondLayerBrushing.OnObjectUnmasked += () =>
            {
                if (newUIInstance.congratulationsUI != null)
                {
                    newUIInstance.congratulationsUI.DOFade(1f, 2f).SetEase(Ease.Flash).onComplete += () =>
                    {
                        newFossil.FinalObject.DOFade(0f, 2f);
                        newUIInstance.congratulationsUI.DOFade(0f, 2f).SetDelay(2f);
                    };
                }
            };

            //CREATE FISH DISCOVER UI
            GameObject fishUI = Instantiate(artifactUIPrefab, newUIInstance.artifactsLayout.transform);
            fishUI.GetComponent<Image>().sprite = currData.FishData.ArtifactHiddenUI;
            fishUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = currData.FishData.name;

            newFossil.secondLayerBrushing.OnObjectUnmasked += () =>
            {

                Image fishImage = fishUI.GetComponent<Image>();
                Debug.Log(fishUI.name);

                fishImage.sprite = currData.FishData.ArtifactDiscoveredUI;
            };


            Artifact[] artifacts = null;

            if (spawnArtifacts)
            {

                artifacts = new Artifact[currData.ArtifactPositionData.Length];
                for (int k = 0; k < currData.ArtifactPositionData.Length; k++)
                {
                    int index = k;

                    //CREATE ARTIFACT DISCOVER UI
                    GameObject newUI = Instantiate(artifactUIPrefab, newUIInstance.artifactsLayout.transform);
                    newUI.GetComponent<Image>().sprite = currData.ArtifactPositionData[k].ArtifactData.ArtifactHiddenUI;
                    newUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = currData.ArtifactPositionData[k].ArtifactData.name;

                    Artifact newArtifact = Instantiate(currData.ArtifactPositionData[k].ArtifactData.ArtifactGameObject, newGameParent.transform);
                    newArtifact.artifactDigMaskCamera.OnObjectUnmasked += () =>
                    {
                        GameObject artifactUI = newUI;
                        artifactUI.GetComponent<Image>().sprite = currData.ArtifactPositionData[index].ArtifactData.ArtifactDiscoveredUI;
                    };

                    newPos = r.transform.position;
                    newPos = Camera.main.WorldToScreenPoint(newPos);

                    offset = currData.ArtifactPositionData[k].PositionInRect;
                    newPos += (r.transform.right * offset.x) + (r.transform.up * offset.y);
                    newPos = Camera.main.ScreenToWorldPoint(newPos);
                    newPos.z = 3f;
                    newArtifact.artifactParent.transform.position = newPos;
                    newArtifact.artifactParent.transform.rotation = Quaternion.Euler(s.rotations[i]);

                    artifacts[k] = newArtifact;
                }
            }

            if (hideArtifacts) foreach (Artifact a in artifacts) a.gameObject.SetActive(false);
            if (hideFossil) newFossil.gameObject.SetActive(false);

            Vector3 pos = newUIInstance.fishModelPos.transform.position;
            pos.z = -3;

            Fish newFish = Instantiate(currData.FishData.Fish3DModel, pos, Quaternion.identity, newGameParent.transform);

            DustParticleController newDustParticle = Instantiate(dustParticlePrefab, newGameParent.transform);
            newDustParticle.particles.Stop();

            GameInstance newGameInstance = new GameInstance(newUIInstance, newDustParticle, newFossil, artifacts, newFish);
            gameInstances[i] = newGameInstance;
        }
    }

    void CreateUIInstance()
    {

    }

    public GameInstance GetGameInstance (Vector2 screenPos)
    {
        GameInstance game = null;
        
        for (int i = 0; i < gameInstances.Length; i++)
        {
            if (gameInstances[i] == null) continue;
            if (RectTransformUtility.RectangleContainsScreenPoint(gameInstances[i].uiInstance.screenRect, screenPos, Camera.main))
            {
                game = gameInstances[i];
                break;
            }
        }

        return game;
    }

    public bool IsInGame(Vector2 screenPos)
    {
        bool isinGame = false;

        for (int i = 0; i < gameInstances.Length; i++)
        {
            if (gameInstances[i] == null) continue;
            if (RectTransformUtility.RectangleContainsScreenPoint(gameInstances[i].uiInstance.screenRect, screenPos, Camera.main))
            {
                isinGame = true;
                break;
            }
        }
        return isinGame;
    }
    private void OnDrawGizmos()
    {
        if (gameInstances != null)
        {
            for (int i = 0; i < gameInstances.Length; i++)
            {
                if (gameInstances[i] == null) continue;
                if (gameInstances[i].uiInstance == null) continue;

                RectTransform r = gameInstances[i].uiInstance.GetComponent<RectTransform>();

                Vector3[] worldCorners = new Vector3[4];

                r.GetWorldCorners(worldCorners);

                Vector3 pos = r.TransformPoint(r.rect.center);

                pos.z = (backGround != null) ? backGround.transform.position.z : 0f;

                Vector2 size = new Vector2(
                    r.lossyScale.x * r.rect.size.x,
                    r.lossyScale.y * r.rect.size.y);

                //CHEAT METHOD
                if (r.rotation.eulerAngles.z == 90 || r.rotation.eulerAngles.z == 270)
                    size = new Vector2(
                    r.lossyScale.y * r.rect.size.y,
                    r.lossyScale.x * r.rect.size.x
                    );

                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(pos, size);
            }
        }
    }

    void LoadStreamingAsset()
    {
        string filePath = Application.streamingAssetsPath + "/" + "GameType.txt";

        if (File.Exists(filePath))
        {
            string result = File.ReadAllText(filePath);

            Debug.Log("Loaded file: " + result);

            if (!int.TryParse(result, out gameType))
            {
                gameType = 0;

                Debug.LogWarning("string is not a number, setting to default 0");
            }
        }
        else
        {
            Debug.LogWarning("File does not exist. Please create a text file and input a number");
            gameType = 0;
        }   

       
    }
}
