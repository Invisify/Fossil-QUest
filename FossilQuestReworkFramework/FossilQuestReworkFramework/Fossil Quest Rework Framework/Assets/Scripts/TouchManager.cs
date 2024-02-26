using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.InputSystem.EnhancedTouch;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.EventSystems;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using Action = System.Action;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
public struct DoubleTouch
{
    public Finger finger1;
    public Finger finger2;

    public Vector3 f1StartPos;
    public Vector3 f2StartPos;

    public DoubleTouch (Finger f1, Finger f2)
    {
        finger1 = f1;
        finger2 = f2;

        f1StartPos = finger1.screenPosition;
        f2StartPos = finger2.screenPosition;
    }
}

public class TouchManager : Singleton<TouchManager>
{
    List<Finger>[] touchMap;
    Dictionary<Touch, bool> touchUIMap;

    bool isOnUI;

    public event System.Action<Finger> OnFingerUp;
    public event System.Action<Finger> OnFingerMove;
    public event System.Action<Finger> OnFingerDown;
    public event System.Action<Finger> OnFingerStationary;

    public event System.Action<Finger, Finger> OnTwoFingerDown;
    public event System.Action<Finger, Finger> OnTwoFingerMove;
    public event System.Action<Finger, Finger> OnTwoFingerUp;

    private List<DoubleTouch> doubleInput;

    //TEST VARIABLES
    Quaternion originalRotation;
    float startAngle;

    Vector2 prevPos;
    Vector2 newHitPos;
    RectTransform fishUI;

    Vector3 dir;

    void Start()
    {
        touchMap = new List<Finger>[GameManager.Instance.gameInstances.Length];

        for (int i = 0; i < touchMap.Length; i++)
        {
            touchMap[i] = new List<Finger>();
        }

        doubleInput = new List<DoubleTouch>();

        //fishUI = GameObject.Find("FishUI").GetComponent<RectTransform>();

    }

    private void OnEnable()
    {
        TouchSimulation.Enable();
        EnhancedTouchSupport.Enable();

        EnhancedTouch.Touch.onFingerDown += FingerDown;
        EnhancedTouch.Touch.onFingerUp += FingerUp;
        EnhancedTouch.Touch.onFingerMove += FingerMove;
    }

    private void OnDisable()
    {
        EnhancedTouch.Touch.onFingerDown -= FingerDown;
        EnhancedTouch.Touch.onFingerUp -= FingerUp;
        EnhancedTouch.Touch.onFingerMove -= FingerMove;

        TouchSimulation.Disable();
        EnhancedTouchSupport.Disable();
    }

    private void FingerDown(Finger f)
    {
        //Debug.Log(f.currentTouch);

        //Debug.Log("Active Touches: " + EnhancedTouch.Touch.activeTouches.Count);

        GameInstance[] gameInstances = GameManager.Instance.gameInstances;

        for (int i = 0; i < gameInstances.Length; i++)
        {
            if (gameInstances[i] == null) continue;
            if (RectTransformUtility.RectangleContainsScreenPoint(gameInstances[i].uiInstance.screenRect, f.screenPosition, Camera.main))
            {
                for (int j = 0; j < touchMap[i].Count; j++)
                {
                    //IF FINGERS TOUCH AT ALMOST SAME TIME
                    if (f.currentTouch.startTime - touchMap[i][j].currentTouch.startTime < 0.1f)
                    {
                        doubleInput.Add(new DoubleTouch(f, touchMap[i][j]));

                        //if (OnTwoFingerDown != null) OnTwoFingerDown(f, touchMap[i][j]);

                        break;
                    }
                }

                //allows only 2 touches per screen
                //if (touchMap[i].Count > 2) break;

                touchMap[i].Add(f);

                //Debug.Log("gameInstance: " + i);

                //gameInstances[i].

                /*PointerEventData pointer = new PointerEventData(EventSystem.current);
                pointer.position = Input.mousePosition;

                List<RaycastResult> raycastResults = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointer, raycastResults);

                if (raycastResults.Count > 0)
                {
                    foreach (var go in raycastResults)
                    {
                        Debug.Log(go.gameObject.name, go.gameObject);
                    }
                }*/

                if (GameManager.Instance.GetGameInstance(f.screenPosition)?.CurrentTool == Tool.Chisel)
                {
                    RaycastHit2D hit2 = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(f.screenPosition), Vector2.zero, Mathf.Infinity, (1 << 8));

                    if (hit2)
                    {
                        Debug.Log(hit2.collider.name);

                        GameManager.Instance.GetGameInstance(f.screenPosition)?.uiInstance.touchFeedback.SetSprite(f.screenPosition, Tool.Chisel, true);

                        Explodable explodable = hit2.collider.GetComponent<Explodable>();

                        Vector3 point = hit2.transform.position;

                        Rigidbody2D[] rigidbodies = hit2.transform.GetComponentsInChildren<Rigidbody2D>(true);

                        Debug.Log(rigidbodies.Length);

                        if (explodable)
                            explodable.explode();

                        //1 because 0 is parent object
                        for (int j = 1; j < rigidbodies.Length; j++)
                        {
                            Rigidbody2D rb = rigidbodies[j];

                            Debug.Log(rb.name);

                            if (rb != null)
                            {
                                rb.transform.position = new Vector3(rb.transform.position.x, rb.transform.position.y, rb.transform.position.z - 0.01f);
                                rb.GetComponent<Collider2D>().isTrigger = false;

                                AddExplosionForce(rb, 30f, point, 1f);
                            }

                        }
                    }
                    else
                    {
                        GameManager.Instance.GetGameInstance(f.screenPosition)?.uiInstance.touchFeedback.SetSprite(f.screenPosition, Tool.Chisel, false);
                    }
                }
                else
                {
                    GameManager.Instance.GetGameInstance(f.screenPosition)?.uiInstance.touchFeedback.SetSprite(f.screenPosition, Tool.Brush, false);
                }
               
            }     
        }
      
        if (OnFingerDown != null) OnFingerDown(f);

        //TEST ROTATION!!

    }

    private void FingerUp(Finger f)
    {
        GameInstance[] gameInstances = GameManager.Instance.gameInstances;

        if (Mathf.Abs(f.currentTouch.startScreenPosition.x - f.currentTouch.screenPosition.x) > 100 && f.currentTouch.time - f.currentTouch.startTime < 1)
        {
            Debug.Log("SWIPED?");
            for (int i = 0; i < gameInstances.Length; i++)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(gameInstances[i].uiInstance.screenRect, f.currentTouch.startScreenPosition, Camera.main))
                {
                    gameInstances[i].StartGame();
                }
            }
        }

        for (int i = 0; i < touchMap.Length; i++)
        {
            if (touchMap[i].Contains(f))
            {
                touchMap[i].Remove(f);
                break;
            }
        }

        for (int k = 0; k < doubleInput.Count; k++)
        {
            if (doubleInput[k].finger1 == f || doubleInput[k].finger2 == f)
            {
                //if (OnTwoFingerUp != null) OnTwoFingerUp(doubleInput[k].finger1, doubleInput[k].finger2);

                doubleInput.RemoveAt(k);
                break;
            }
        }

        if (OnFingerUp != null) OnFingerUp(f);
    }

    private void FingerMove(Finger f)
    {
        //find array
        /*if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(f.currentTouch.touchId))
        {
            Debug.Log("ONUI");
            return;
        }*/


        //TEMPORARY TESTING
        /*for (int k = 0; k < doubleInput.Count; k++)
        {
            if (doubleInput[k].finger1 == f || doubleInput[k].finger2 == f)
            {
                if (OnTwoFingerMove != null) OnTwoFingerMove(doubleInput[k].finger1, doubleInput[k].finger2);
            }
        }*/

        //Debug.Log((f.currentTouch.delta - f.currentTouch.history[f.currentTouch.history.Count - 1].delta).magnitude);

        //NEED A METHOD FOR CHECKING STATIONARY TOUCH

        if (OnFingerMove != null) OnFingerMove(f);

    }

    private void FingerStationary(Finger f)
    {
        if (OnFingerStationary != null) OnFingerStationary(f);
    }

    private void LateUpdate()
    {
       
    }

    void CalculateAngle(Touch t)
    {   
        dir = (t.screenPosition - prevPos);
        dir = (Vector3)t.screenPosition - Camera.main.WorldToScreenPoint(fishUI.transform.position);

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        Quaternion rotation = Quaternion.AngleAxis(angle - startAngle, Vector3.forward);

        rotation = originalRotation * rotation;

        Vector3 rot = fishUI.transform.localEulerAngles;
        rot.z = rotation.eulerAngles.z;
        fishUI.transform.rotation = Quaternion.Euler(rot);

        //fishUI.transform.Rotate(0, 0, angle);

        prevPos = t.screenPosition;
    }

    //WORKS FOR NOW
    void CalculateAngle2(Touch t)
    {
        prevPos = newHitPos;
        //newHitPos = GetPositionFromHit(inputPos);

        newHitPos =  (Vector3)t.screenPosition - Camera.main.WorldToScreenPoint(fishUI.transform.position);

        float diffAngle = Vector3.SignedAngle(newHitPos, prevPos, transform.forward);

        if (!RectTransformUtility.RectangleContainsScreenPoint(fishUI, t.screenPosition, Camera.main)) return;

        fishUI.transform.Rotate(0, 0, -diffAngle );

    }

    void Update()
    {
        foreach (EnhancedTouch.Touch t in EnhancedTouch.Touch.activeTouches)
        {
            if (t.began)
                isOnUI = !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(t.touchId);

            if (t.phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                //Debug.Log("Touch " + t.finger.index + " Began");
                newHitPos = Vector3.zero;
                dir = Vector3.zero;

                //originalRotation = fishUI.transform.rotation;
                //startAngle = fishUI.transform.rotation.eulerAngles.z;
            }
            else if (t.phase == UnityEngine.InputSystem.TouchPhase.Moved)
            {
                //Debug.Log("Touch " + t.finger.index + " Moved");
                //CalculateAngle(t);
                //CalculateAngle2(t);
            }
            else if (t.phase == TouchPhase.Stationary)
            {
                //Debug.Log("Touch " + t.finger.index + " Stationary");

                //FingerStationary(t.finger);
            }
            else if (t.phase == UnityEngine.InputSystem.TouchPhase.Ended)
            {

            }
        }

        for (int i = 0; i < touchMap.Length; i++)
        {
            //check
            if (touchMap[i].Count == 1)
            {
                //Debug.Log("1 On Finger Move");


            }
            else if (touchMap[i].Count >= 2)
            {
                //Debug.Log("2 On Finger Move");
                //CalculateZoomLogicHere

                for (int j = 0; j < touchMap[i].Count; j++)
                {
                    double startTime = touchMap[i][j].currentTouch.startTime;

                    
                }
            }
        }

        //Debug.Log("doubleInputCount: " + doubleInput.Count);
     
        /*for (int k = 0; k < doubleInput.Count; k++)
        {
            Touch firstTouch = doubleInput[k].finger1.currentTouch;
            Touch secondTouch = doubleInput[k].finger2.currentTouch;

            Debug.Log(firstTouch.phase + " || " + secondTouch.phase);

            if (firstTouch.phase == TouchPhase.Moved || secondTouch.phase == TouchPhase.Moved)
            {
                CalculateAngle2(secondTouch);
                
                if (firstTouch.phase == TouchPhase.Moved && secondTouch.phase == TouchPhase.Moved)
                {

                    float newMultiTouchDistance = Vector2.Distance(firstTouch.screenPosition, secondTouch.screenPosition);
                    float lastMultiTouchDistance = Vector2.Distance(doubleInput[k].f1StartPos, doubleInput[k].f2StartPos);

                    if (newMultiTouchDistance < lastMultiTouchDistance)
                    {
                        //Camera.main.orthographicSize += Time.deltaTime;

                        if (fishUI.transform.localScale.x > 0.5f)
                        {
                            fishUI.transform.localScale -= Vector3.one * Time.deltaTime;

                        }
                    }
                    else if (newMultiTouchDistance > lastMultiTouchDistance)
                    {
                        //Camera.main.orthographicSize -= Time.deltaTime;
                        if (fishUI.transform.localScale.x < 1.2f)
                        {
                            fishUI.transform.localScale += Vector3.one * Time.deltaTime;
                        }
                    }
                }

            }

            DoubleTouch t = doubleInput[k];
            t.f1StartPos = doubleInput[k].finger1.screenPosition;
            t.f2StartPos = doubleInput[k].finger2.screenPosition;

            doubleInput[k] = t;
        }*/
    }

    void OldRotate1()
    {
        // ... or check the delta angle between them ... 
        //turnAngle = Angle(firstTouch.screenPosition, secondTouch.screenPosition);
        //float prevTurn = Angle(firstTouch.screenPosition - firstTouch.delta, secondTouch.screenPosition - secondTouch.delta );

        /*turnAngle = Angle(firstTouch.screenPosition, secondTouch.screenPosition);
        float prevTurn = Angle(firstTouch.screenPosition - (Vector2)doubleInput[k].f1StartPos, secondTouch.screenPosition - (Vector2)doubleInput[k].f2StartPos);

        //turnAngle = Angle(firstTouch.screenPosition, firstTouch.screenPosition);
        //float prevTurn = Angle(firstTouch.screenPosition - (Vector2)doubleInput[k].f1StartPos, firstTouch.screenPosition - (Vector2)doubleInput[k].f1StartPos);

        Debug.Log(turnAngle);

        turnAngleDelta = Mathf.DeltaAngle(prevTurn, turnAngle);

        turnAngleDelta = Angle(firstTouch.delta, firstTouch.screenPosition);

        // ... if it's greater than a minimum threshold, it's a turn! 
        if (Mathf.Abs(turnAngleDelta) > 0)
        {
            turnAngleDelta *= Mathf.PI / 2; 
        }
        else
        {
            turnAngle = turnAngleDelta = 0;
        }

        float pinchAmount = 0;
        Quaternion desiredRotation = transform.rotation;

        if (Mathf.Abs(turnAngleDelta) > 0)
        {
            Vector3 rotationDeg = Vector3.zero;
            rotationDeg.z = turnAngleDelta * 1f;
            desiredRotation *= Quaternion.Euler(rotationDeg);
        }

        fishUI.transform.rotation = desiredRotation;*/


    }
    float GetAngle()
    {
        float angle = 0f;

        Vector3 dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        return angle;
    }

    private float Angle(Vector2 pos1, Vector2 pos2)
    {
        Vector2 from = pos2 - pos1;
        Vector2 to = new Vector2(1, 0);
        float result = Vector2.Angle(from, to);
        Vector3 cross = Vector3.Cross(from, to);

        if (cross.z > 0)
        {
            result = 360f - result;
        }
        return result;
    }

    void AddExplosionForce(Rigidbody2D body, float explosionForce, Vector3 explosionPosition, float explosionRadius, float upliftModifier = 0)
    {
        var dir = (body.transform.position - explosionPosition);
        float wearoff = 1 - (dir.magnitude / explosionRadius);
        Vector3 baseForce = dir.normalized * explosionForce * wearoff;
        baseForce.z = 0;
        body.AddForce(baseForce);

        if (upliftModifier != 0)
        {
            float upliftWearoff = 1 - upliftModifier / explosionRadius;
            Vector3 upliftForce = Vector2.up * explosionForce * upliftWearoff;
            upliftForce.z = 0;
            body.AddForce(upliftForce);
        }

        Destroy(body.gameObject, 0.2f);

    }
}
