using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using static UnityEditor.PlayerSettings;
using DG.Tweening.Core.Easing;

public class Node
{
    public int x;
    public int y;
    public Vector3 worldPos;
    public bool covered;

    public Node(Vector3 _worldPos, bool _covered)
    {
        worldPos = _worldPos;
        covered = _covered;
    }

}

public class MaskCamera : MonoBehaviour
{
    public GameObject Dust;
    public GameObject ObjectToUnmask;
    public GameObject FossilChiselObject;

    public Collider2D Collider;

    Rect ScreenRect;
    public RenderTexture rt;
    //public SpriteMask mask;

    public Material EraserMaterial;
    private bool firstFrame;
    private Vector2? newHolePosition;

    public float sizeMultiplier;

    public bool _requestReadPixel = false;
    public float lastPercent = 0;
    private Action<float> percentCallback;

    //test
    private Node[,] grid;
    public Vector2 gridWorldSize;
    public float nodeSize;

    [Range(0, 1)]
    public float completionRatio;

    int gridSizeX;
    int gridSizeY;

    int gridUncoveredCount = 0;

    private List<Node> nodes;

    Vector3 touchPos;

    private bool discovered = false;

    public bool debugMode = false;

    public event Action OnObjectDiscovered;
    public event Action OnObjectUnmasked;

    public int TotalNodes { get { return gridSizeX * gridSizeY; } }

    //public float GetPercentageValue { get { return gridUncoveredCount / (gridSizeX * gridSizeY); } }
    public float GetPercentageValue { get { return (float)(TotalNodes - nodes.Count) / (float)TotalNodes; } }

    /// <summary>
    /// </summary>
    /// <param name="_callback"></param>
    public void GetPercent(Action<float> _callback)
    {
        percentCallback = _callback;
        _requestReadPixel = true;
    }
    /// <summary>
    /// </summary>
    public void EnableGetPercent()
    {
        _requestReadPixel = true;
    }

    /// <summary>
    /// </summary>
    /// <param name="imageSize"></param>
    /// <param name="imageLocalPosition"></param>
    private void CutHole(Vector2 imageSize, Vector2 imageLocalPosition)
    {
        Rect textureRect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
        Rect positionRect = new Rect(
            (imageLocalPosition.x - 0.5f * EraserMaterial.mainTexture.width * sizeMultiplier) / imageSize.x,
            (imageLocalPosition.y - 0.5f * EraserMaterial.mainTexture.height * sizeMultiplier) / imageSize.y,
            EraserMaterial.mainTexture.width * sizeMultiplier / imageSize.x,
            EraserMaterial.mainTexture.height * sizeMultiplier / imageSize.y);
        GL.PushMatrix();
        GL.LoadOrtho();

        for (int i = 0; i < EraserMaterial.passCount; i++)
        {
            EraserMaterial.SetPass(i);
            GL.Begin(GL.QUADS);
            GL.Color(Color.white);
            GL.TexCoord2(textureRect.xMin, textureRect.yMax);
            GL.Vertex3(positionRect.xMin, positionRect.yMax, 0.0f);
            GL.TexCoord2(textureRect.xMax, textureRect.yMax);
            GL.Vertex3(positionRect.xMax, positionRect.yMax, 0.0f);
            GL.TexCoord2(textureRect.xMax, textureRect.yMin);
            GL.Vertex3(positionRect.xMax, positionRect.yMin, 0.0f);
            GL.TexCoord2(textureRect.xMin, textureRect.yMin);
            GL.Vertex3(positionRect.xMin, positionRect.yMin, 0.0f);
            GL.End();
        }
        GL.PopMatrix();
        ////////////////////////
        ///
        //Debug.Log("imageLocalPos: " + imageLocalPosition);
        /*for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector2 worldPos = grid[x, y].worldPos;

                //Debug.Log("gridscreenPos: " + worldPos);

                float rad = EraserMaterial.mainTexture.width * sizeMultiplier / imageSize.x;

                //Debug.Log("inputscreenPos: " + rad);

                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                float dist = (mousePos - worldPos).sqrMagnitude;
                //Debug.Log("distance: " + dist);

                if (dist < rad * rad)
                {
                    if (!grid[x, y].covered)
                    {
                        grid[x, y].covered = true;
                        gridUncoveredCount += 1;

                        if (GetPercentageValue == 1)
                        {
                            //ObjectToUnmask.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.None;
                            ObjectToUnmask.SetActive(false);
                            FossilChiselObject.SetActive(true);
                        }

                        Debug.Log("gridUncoveredCount: " + gridUncoveredCount  + " / " + (gridSizeX * gridSizeY));
                    }                 
                }

            }
        }*/

        //Debug.Log(GetPercentageValue);

        for (int i = 0; i < nodes.Count; i++)
        {
            Vector2 worldPos = nodes[i].worldPos;

            float rad = EraserMaterial.mainTexture.width * sizeMultiplier / imageSize.x;

            Vector2 mousePos = Camera.main.ScreenToWorldPoint(touchPos);

            float dist = (mousePos - worldPos).sqrMagnitude;

            if (dist < rad * rad)
            {
                nodes.Remove(nodes[i]);

                if (GetPercentageValue >= completionRatio)
                {
                    /*if (FossilChiselObject != null)
                    {
                        ObjectToUnmask.SetActive(false);
                        FossilChiselObject.SetActive(true);
                    }*/

                    //ObjectToUnmask.SetActive(false);
                    nodes.Clear();

                    if (OnObjectUnmasked != null) OnObjectUnmasked();
                    enabled = false;
                    break;
                }
            }
        }
    }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    int _left;
    int _top;
    public IEnumerator Start()
    {
        //transform.position = Vector3.zero;

        firstFrame = true;
        _requestReadPixel = false;

        float x = Dust.GetComponent<Renderer>().bounds.min.x;
        float y = Dust.GetComponent<Renderer>().bounds.min.y;
        float width = Dust.GetComponent<Renderer>().bounds.size.x;
        float height = Dust.GetComponent<Renderer>().bounds.size.y;

        //Get Erase effect boundary area
        ScreenRect.x = x;
        ScreenRect.y = y;

        ScreenRect.width = width;
        ScreenRect.height = height;

        ScreenRect = Rect.MinMaxRect(Dust.GetComponent<Renderer>().bounds.min.x, Dust.GetComponent<Renderer>().bounds.min.y, Dust.GetComponent<Renderer>().bounds.max.x, Dust.GetComponent<Renderer>().bounds.max.y);

        //Create new render texture for camera target texture
        rt = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.Default);

        yield return rt.Create();
        _left = rt.width / 5;
        _top = rt.height / 4;
        tex = new Texture2D(rt.width - 2 * _left, rt.height - 2 * _top, TextureFormat.RGB24, false);

        GetComponent<Camera>().targetTexture = rt;

        //Set Mask Texture to dust material to Generate Dust erase effect
        Dust.GetComponent<Renderer>().material.SetTexture("_MaskTex", rt);

        CreateGrid();

        if (debugMode) yield return null;

        TouchManager.Instance.OnFingerMove += (f) =>
        {
            //if (EventSystem.current.IsPointerOverGameObject(f.currentTouch.touchId)) return;
            if (GameManager.Instance.GetGameInstance(f.screenPosition)?.CurrentTool != Tool.Brush) return;

            //GameManager.Instance.GetGameInstance(f.screenPosition)?.uiInstance.touchFeedback.UpdatePosition(f.screenPosition);

            UpdateMask(f.screenPosition);

        };

        TouchManager.Instance.OnFingerDown += (f) =>
        {
            //if (EventSystem.current.IsPointerOverGameObject(f.currentTouch.touchId)) return;
            if (GameManager.Instance.GetGameInstance(f.screenPosition)?.CurrentTool != Tool.Brush)
            {
                //GameManager.Instance.GetGameInstance(f.screenPosition)?.uiInstance.touchFeedback.SetSprite(f.screenPosition, Tool.Chisel, false);
                return;
            }

         
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(f.screenPosition), Vector2.zero);

            if (hit.collider != null)
            {
                //Debug.Log(hit.collider.gameObject.name);

                if (hit.collider.gameObject != ObjectToUnmask) return;
            }

            GameManager.Instance.GetGameInstance(f.screenPosition)?.uiInstance.touchFeedback.SetSprite(f.screenPosition, Tool.Brush, true);
        };

        TouchManager.Instance.OnFingerUp += (f) =>
        {
            if (GameManager.Instance.GetGameInstance(f.screenPosition)?.CurrentTool != Tool.Brush) return;

            GameManager.Instance.GetGameInstance(f.screenPosition).uiInstance.touchFeedback.IsFingerUp = true;
        };

    }

    void CreateGrid()
    {
        if (gridWorldSize.x == 0 && gridWorldSize.y == 0) return;

        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeSize);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeSize);

        nodes = new List<Node>();

        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = Dust.transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.down * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeSize + nodeSize / 2) + Vector3.down * (y * nodeSize + nodeSize / 2);

                //calculate circle
                Vector3 dist = worldPoint - Dust.transform.position;

                bool covered = false;

                //if (dist.sqrMagnitude > gridWorldSize.x/2 * gridWorldSize.x / 2)
                //covered = true;

                if (!Collider.OverlapPoint(worldPoint))
                {
                    covered = true;
                    gridUncoveredCount += 1;

                }

                Node newNode = new Node(worldPoint, covered);
                grid[x, y] = newNode;

                if (!covered)
                    nodes.Add(newNode);
            }
        }

        Debug.Log("gridUncoveredCount: " + gridUncoveredCount);
    }

    public void UpdateMask(Vector3 pos)
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(pos), Vector2.zero);

        if (hit.collider != null)
        {
            //Debug.Log(hit.collider.gameObject.name);

            if (hit.collider.gameObject == ObjectToUnmask)
            {
                newHolePosition = null;

                Debug.Log("TESTTTTTTTTTTTTTTTTTTTTTTTTTTT");

                Vector2 v = GetComponent<Camera>().ScreenToWorldPoint(pos);
                if (ScreenRect.Contains(v))
                {
                    if (!discovered)
                    {
                        if (OnObjectDiscovered != null) OnObjectDiscovered();
                        discovered = true;
                    }

                    newHolePosition = new Vector2(1600 * (v.x - ScreenRect.xMin) / ScreenRect.width, 1200 * (v.y - ScreenRect.yMin) / ScreenRect.height);

                    touchPos = pos;
                    //_requestReadPixel = true;
                }

            }
        }


    }

    /// <summary>
    /// </summary>
    public void FixedUpdate()
    {
        /*newHolePosition = null;
        if (Input.GetMouseButton(0))
        {
            Vector2 v = GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
            if (ScreenRect.Contains(v))
            {
                //newHolePosition = new Vector2(1600 * (v.x - ScreenRect.xMin) / ScreenRect.width, 1200 * (v.y - ScreenRect.yMin) / ScreenRect.height);

                //_requestReadPixel = true;

            }
        }*/
    }

    /// <summary>
    /// </summary>
    public void OnPostRender()
    {
        if (firstFrame)
        {
            firstFrame = false;
            GL.Clear(false, true, new Color(0.0f, 0.0f, 0.0f, 0.0f));
        }
        if (newHolePosition != null)
        {
            CutHole(new Vector2(1600.0f, 1200.0f), newHolePosition.Value);
        }

        if (_requestReadPixel)
        {
            tex.ReadPixels(new Rect(_left, _top, rt.width - 2 * _left, rt.height - 2 * _top), 0, 0);
            _requestReadPixel = false;
            lastPercent = caculatorPrercent(tex);
            Debug.Log("Percent:" + lastPercent);
            if (percentCallback != null)
            {
                percentCallback(lastPercent);
            }
        }
    }

    /// <summary>
    /// </summary>
    Texture2D tex;
    public static float caculatorPrercent(Texture2D tex)
    {
        int count = 0;
        for (int x = 0; x < tex.width; x++)
        {
            for (int y = 0; y < tex.height; y++)
            {
                if (tex.GetPixel(x, y).r == 1)
                    count++;
            }
        }
        float _percent = (count * 100 / ((tex.width) * (tex.height)));
        return _percent;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if (newHolePosition != null)
            Gizmos.DrawWireSphere(newHolePosition.Value, 1f);

        if (tex != null)
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(tex.width, tex.height));
        }

        if (Dust != null)
            Gizmos.DrawWireCube(Dust.transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y));

        /*if (grid != null)
        {
            foreach (Node n in grid)
            {
                Gizmos.color = (!n.covered) ? Color.white : Color.red;
                //Gizmos.DrawWireCube(n.worldPos, Vector3.one * (nodeSize - .005f));
                Gizmos.DrawWireCube(n.worldPos, Vector3.one * (nodeSize - nodeSize / 50f));
            }
        }*/

        if (nodes != null)
        {
            foreach (Node n in nodes)
            {
                Gizmos.color = (!n.covered) ? Color.white : Color.red;
                //Gizmos.DrawWireCube(n.worldPos, Vector3.one * (nodeSize - .005f));
                Gizmos.DrawWireCube(n.worldPos, Vector3.one * (nodeSize - nodeSize / 50f));
            }
        }
    }
}