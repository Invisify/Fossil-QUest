using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public enum TouchAnimation
{
    Chisel,
    Brush,
    ChiselError,
    BrushError
}

public class TouchFeedback : MonoBehaviour
{
    public Image chiselSprite;
    public Image brushSprite;

    public Image chiselWrongSprite;
    public Image brushWrongSprite;

    public Image imageUI;
    private RectTransform imageRect;

    private bool isPlaying = false;
    private bool isFingerUp = false;

    private Sequence seq;
    private Tween tween;

    public bool IsFingerUp { get => isFingerUp; set => isFingerUp = value; }

    private void Awake()
    {
       imageRect = imageUI.GetComponent<RectTransform>();
    }
    void Start()
    {
        Hide();

        chiselSprite.gameObject.SetActive(false);
        brushSprite.gameObject.SetActive(false);
        chiselWrongSprite.gameObject.SetActive(false);
        brushWrongSprite.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetSprite(Vector2 screenPos, Tool tool, bool isCorrect)
    {
        //TEMP METHOD
        PointerEventData pointer = new PointerEventData(EventSystem.current);
        pointer.position = screenPos;

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, raycastResults);

        Debug.Log("raycastResults Count:" + raycastResults.Count);

        if (raycastResults.Count > 0)
        {
            return;
        }

        gameObject.SetActive(true);

        IsFingerUp = false;

        seq.Kill();

        if (imageUI != null) imageUI.gameObject.SetActive(false);

        if (tool == Tool.Brush)
        {          
            if (isCorrect)
            {
                imageUI = brushSprite;

                float t = 0.1f;

                seq = DOTween.Sequence();

                Vector3 targetRot = new Vector3(0, 0, -30);
                

                //seq.PrependCallback(() => { isPlaying = true; });
                seq.Append(imageUI.transform.DOLocalRotate(targetRot, t));
                seq.Append(imageUI.transform.DOLocalRotate(Vector3.zero, t));
                seq.AppendCallback(() => 
                {
                    isPlaying = false;

                    Hide();
                    //if (IsFingerUp) Hide();
                    //else seq.Restart(); 
                });

            }
            else
            {
                imageUI = brushWrongSprite;

                //Vector3 localPos = imageRect.localPosition;
                //imageRect.pivot = Vector2.one * 0.5f;
                //imageRect.localPosition = localPos;

                //SetPivot(imageRect, Vector2.one * 0.5f);

                seq = DOTween.Sequence();

                float t = 0.1f;
                
                //Dont use SetLoop
                seq.Append(imageUI.transform.DOLocalRotate(new Vector3(0, 0, 10f), t));
                seq.Append(imageUI.transform.DOLocalRotate(Vector3.zero, t));
                seq.Append(imageUI.transform.DOLocalRotate(new Vector3(0, 0, 10f), t));
                seq.Append(imageUI.transform.DOLocalRotate(Vector3.zero, t));
                seq.Append(imageUI.transform.DOLocalRotate(new Vector3(0, 0, 10f), t));
                seq.Append(imageUI.transform.DOLocalRotate(Vector3.zero, t));
                seq.AppendCallback(() => { Hide(); });
               
            }
        }
        else if (tool == Tool.Chisel)
        {
            if (isCorrect)
            {            
                imageUI = chiselSprite;

                Vector3 endRot = new Vector3(0, 0, -30f);

                imageUI.transform.rotation = Quaternion.Euler(endRot);

                imageUI.transform.DOLocalRotate(Vector3.zero, .3f).SetEase(Ease.OutCubic).onComplete += () =>
                {
                    imageUI.transform.rotation = Quaternion.Euler(endRot);
                    Hide();
                };
            }
            else 
            {
                imageUI = chiselWrongSprite;

                //SetPivot(imageRect, Vector2.one * 0.5f);

                seq = DOTween.Sequence();

                float t = 0.1f;

                seq.Append(imageUI.transform.DOLocalRotate(new Vector3(0, 0, 10f), t));
                seq.Append(imageUI.transform.DOLocalRotate(Vector3.zero, t));
                seq.Append(imageUI.transform.DOLocalRotate(new Vector3(0, 0, 10f), t));
                seq.Append(imageUI.transform.DOLocalRotate(Vector3.zero, t));
                seq.Append(imageUI.transform.DOLocalRotate(new Vector3(0, 0, 10f), t));
                seq.Append(imageUI.transform.DOLocalRotate(Vector3.zero, t));
                seq.AppendCallback(() => { Hide(); });

            }       
        }

        imageUI.gameObject.SetActive(true);
        UpdatePosition(screenPos);
    }

    public void Hide()
    {             
        seq.Kill();

        //gameObject.SetActive(false);

        if (imageUI)
            imageUI.gameObject.SetActive(false);
    }

    public void UpdatePosition(Vector2 screenPos)
    {
        if (isPlaying) return;
        
        RectTransform r = GameManager.Instance.GetGameInstance(screenPos)?.uiInstance.screenRect;

        Vector2 localPoint = screenPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(r, screenPos, Camera.main, out localPoint);

        //localPoint.y += r.sizeDelta.y / 2;

        Vector2 p = Vector2.one * 0.5f;
        p = r.pivot - p;
        localPoint += p * r.sizeDelta;

        Vector3 pos = localPoint;

        pos.z = -400f;

        GetComponent<RectTransform>().anchoredPosition3D = pos;

        isPlaying = false;

    }

    public void SetPivot( RectTransform rectTransform, Vector2 pivot)
    {
        Vector3 deltaPosition = rectTransform.pivot - pivot;    // get change in pivot
        deltaPosition.Scale(rectTransform.rect.size);           // apply sizing
        deltaPosition.Scale(rectTransform.localScale);          // apply scaling
        deltaPosition = rectTransform.localRotation * deltaPosition; // apply rotation

        rectTransform.pivot = pivot;                            // change the pivot
        rectTransform.localPosition -= deltaPosition;           // reverse the position change
    }
}
