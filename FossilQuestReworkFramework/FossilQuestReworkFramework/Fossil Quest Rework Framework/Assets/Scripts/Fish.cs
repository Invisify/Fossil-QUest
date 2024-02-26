using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using Finger = UnityEngine.InputSystem.EnhancedTouch.Finger;

public class Fish : MonoBehaviour
{
    public GameObject fossilModel;
    public GameObject fishModel;
    public GameObject rotateParent;

    public GameObject[] hotspotPoints;

    public Vector2 modelRotateSpeed = Vector2.one;

    Vector3 prevPos;
    Vector3 pos;

    Vector3 originalPos;

    private float startAngle;
    public Quaternion originalRotation;

    private Vector3 originalFossilPos;

    bool isDragging;

    public GameObject hitBox;
    EventTrigger eventTrigger;

    public event System.Action<bool> OnFishStateChanged;

    private void Awake()
    {
        originalPos = transform.position;

        originalPos = fossilModel.transform.InverseTransformPoint(rotateParent.transform.position); 

        originalFossilPos = rotateParent.transform.InverseTransformPoint(fishModel.transform.position);

        Debug.Log(originalFossilPos);
    }

    void Start()
    {
        TouchManager.Instance.OnFingerDown += (f) => 
        {
            pos = f.screenPosition;
            prevPos = f.screenPosition;
        };

        TouchManager.Instance.OnFingerMove += (f) => 
        {
            RotateFish(f);
            //RotateFish2(f);
            //Debug.Log(f.currentTouch.delta.normalized);
        };

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            //Switch();
        }

        /*if (Input.GetMouseButtonDown(0))
        {
            pos = Input.mousePosition - prevPos;
        }

        if (Input.GetMouseButton(0))
        {          
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 6))
            {
                if (hit.collider.gameObject != this.gameObject) return;
                
                pos = Input.mousePosition - prevPos;

                Debug.Log(pos);

                float xMove = 0f;
                float yMove = 0f;

                //PROBABLY NEED CHANGE CODE ACCORDING TO DIRECTION
                if (Vector3.Dot(rotateParent.transform.up, Vector3.up) >= 0)
                {
                    yMove = -Vector3.Dot(pos, gameObject.transform.right);
                }
                else
                {
                    yMove = Vector3.Dot(pos, gameObject.transform.right);
                }

                if (Vector3.Dot(rotateParent.transform.forward, Vector3.forward) > 0)
                {
                    xMove = Vector3.Dot(pos, gameObject.transform.up);
                }
                else
                {
                    xMove = -Vector3.Dot(pos, gameObject.transform.up);
                }

                //CORRECT FOR X AND Y AXIS WITHOUT Z
                Vector3 rot = rotateParent.gameObject.transform.localEulerAngles;
                rot.x += xMove * modelRotateSpeed.y;
                rot.y += yMove * modelRotateSpeed.x;

                rotateParent.gameObject.transform.localRotation = Quaternion.Euler(rot);
            }
        }

        prevPos = Input.mousePosition;*/

    }

    void RotateFish(Finger f)
    {
        Ray ray = Camera.main.ScreenPointToRay(f.screenPosition);
        RaycastHit hit;

        //pos = f.currentTouch.delta.normalized;
        pos = ((Vector3)f.screenPosition - prevPos);
        Vector3 dir = pos.normalized;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 6))
        {
            if (hit.collider.gameObject != this.gameObject) return;


            //pos = (Vector3)f.screenPosition - prevPos;

            //Debug.Log(pos);

            float xMove = 0f;
            float yMove = 0f;

            //PROBABLY NEED CHANGE CODE ACCORDING TO DIRECTION
            if (Vector3.Dot(transform.up, Vector3.up) >= 0)
            {
                yMove = -Vector3.Dot(pos, gameObject.transform.right);
                //yMove = -Vector3.Dot(pos, Vector3.right);
            }
            else
            {
                yMove = Vector3.Dot(pos, gameObject.transform.right);
                //yMove = Vector3.Dot(pos, Vector3.right);
            }

            //IT WORKS FOR NOW!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            yMove = -Vector3.Dot(pos, gameObject.transform.right);

            if (Vector3.Dot(rotateParent.transform.forward, Vector3.forward) > 0)
            {
                xMove = Vector3.Dot(pos, gameObject.transform.up);
                //xMove = Vector3.Dot(pos, Vector3.up);
            }
            else
            {
                xMove = -Vector3.Dot(pos, gameObject.transform.up);
                //xMove = -Vector3.Dot(pos, Vector3.up);
            }

            //CORRECT FOR X AND Y AXIS WITHOUT Z
            Vector3 rot = rotateParent.gameObject.transform.localEulerAngles;
            rot.x += xMove * modelRotateSpeed.y;
            rot.y += yMove * modelRotateSpeed.x;

            rotateParent.gameObject.transform.localRotation = Quaternion.Euler(rot);
        }

        prevPos = f.screenPosition;
    }

    void RotateFish2(Finger f)
    {
        Ray ray = Camera.main.ScreenPointToRay(f.screenPosition);
        RaycastHit hit;

        //pos = f.currentTouch.delta.normalized;
        pos = ((Vector3)f.screenPosition - prevPos);
        Vector3 dir = pos.normalized;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 6))
        {
            if (hit.collider.gameObject != this.gameObject) return;

            Debug.Log(pos);

            float xMove = 0f;
            float yMove = 0f;

            //rotateParent.transform.Rotate(Vector3.up, dir.x * modelRotateSpeed.x, Space.World);
            //rotateParent.transform.Rotate(Vector3.right, dir.y * modelRotateSpeed.y, Space.World);
            Vector3 right = Vector3.Cross(Camera.main.transform.position, rotateParent.transform.position - Camera.main.transform.position);
            Vector3 up = Vector3.Cross(Camera.main.transform.position - rotateParent.transform.position, right);

            //transform.rotation = Quaternion.AngleAxis(f.currentTouch.delta.x, up) * transform.rotation;
            //transform.rotation = Quaternion.AngleAxis(f.currentTouch.delta.y, right) * transform.rotation;

            transform.Rotate(f.currentTouch.delta.x * modelRotateSpeed.x, f.currentTouch.delta.y, modelRotateSpeed.y);
        }

        prevPos = f.screenPosition;
    }

    public void Switch()
    {
        Sequence seq = DOTween.Sequence();

        fishModel.SetActive(!fishModel.activeSelf);
        fossilModel.SetActive(!fossilModel.activeSelf);

        if (fossilModel.activeSelf)
            fossilModel.transform.DOMove(transform.position, 1f);


        /*fishModel.GetComponent<Renderer>().material.DOFade(System.Convert.ToInt32(fishModel.activeSelf), System.Convert.ToInt32(!fishModel.activeSelf), 1f).OnComplete(() =>
        {

        });*/
    }

    public void SwitchModel(bool fish)
    {     
        Debug.Log("fish: " + fish);

        //impt rmb to put transform!
        DOTween.Kill(fossilModel.transform);

        if (!fish)
        {
            fishModel.SetActive(fish);
            fossilModel.SetActive(!fish);

            //fossilModel.transform.position = rotateParent.transform.TransformPoint(originalFossilPos);

            fossilModel.transform.DOLocalMove(Vector3.zero, 1f);

            //fossilModel.transform.DOMove(fishModel.transform.TransformPoint(originalPos), 1f);

            if (OnFishStateChanged != null) OnFishStateChanged(fish);
        }           
        else
        {
            /*fossilModel.transform.DOLocalMove(originalFossilPos, 1f).OnComplete(() => 
            {
                fossilModel.SetActive(!fish);
                fishModel.SetActive(fish);

                if (OnFishStateChanged != null) OnFishStateChanged(fish);
            });*/

            fossilModel.transform.DOMove(rotateParent.transform.TransformPoint(originalFossilPos), 1f).OnComplete(() =>
            {
                fossilModel.SetActive(!fish);
                fishModel.SetActive(fish);

                if (OnFishStateChanged != null) OnFishStateChanged(fish);
            });
        }
      
    }

}
