using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TestHotSpots : MonoBehaviour
{
    // Start is called before the first frame update
    public RectTransform[] hotSpots;

    public Vector3[] originalPos;

    public Collider fishCollider;

    void Start()
    {
        hotSpots = new RectTransform[transform.childCount];
        originalPos = new Vector3[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            hotSpots[i] = transform.GetChild(i).GetComponent<RectTransform>();

            originalPos[i] = hotSpots[i].transform.localPosition;
        }



    }

    // Update is called once per frame
    void Update()
    {


        for (int i = 0; i < hotSpots.Length; i++)
        {
            hotSpots[i].gameObject.SetActive(true);

            Vector3[] corners = new Vector3[4];

            hotSpots[i].transform.GetChild(0).GetComponent<RectTransform>().GetWorldCorners(corners);

            for (int j = 0; j < corners.Length; j++)
            {
                Debug.DrawRay(corners[j], Vector3.forward);

                if (Input.GetMouseButtonUp(0))
                {

                    RaycastHit hit;
                    if (Physics.Raycast(corners[j], Vector3.forward, out hit, Mathf.Infinity, 1 << 10))
                    {
                        Debug.Log("colliding with model");


                        Vector3 pos = hotSpots[i].transform.position;

                        Vector3 dist = (pos - hit.point);
                        Vector3 dir = dist.normalized;

                        dist.z = 0;
                        dir.z = 0;

                        hotSpots[i].transform.position += (dir * 0.1f);

                    }
                    else
                    {
                        hotSpots[i].transform.localPosition = originalPos[i];
                        break;
                    }

                    /*Vector3 pos = hotSpots[i].transform.position;

                    Vector3 dist = (pos - hit.point);
                    Vector3 dir = dist.normalized;

                    dist.z = 0;
                    dir.z = 0;

                    hotSpots[i].transform.position += (dir * 0.1f);*/


                }


            }

            if (Input.GetMouseButton(0))
            {

            }

        }
    }

    void Raycast()
    {
        for (int i = 0; i < hotSpots.Length; i++)
        {
            for (int k = 0; k < 5; k++)
            {
                Vector3[] corners = new Vector3[4];

                hotSpots[i].transform.GetChild(0).GetComponent<RectTransform>().GetWorldCorners(corners);

                for (int j = 0; j < corners.Length; j++)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(corners[j], Vector3.forward, out hit, Mathf.Infinity, 1 << 10))
                    {
                        Debug.Log("colliding with model");


                        Vector3 pos = hotSpots[i].transform.position;

                        Vector3 dist = (pos - hit.point);
                        Vector3 dir = dist.normalized;

                        dist.z = 0;
                        dir.z = 0;

                        hotSpots[i].transform.position += (dir * 0.1f);

                    }
                    else
                    {
                        break;
                    }
                }
            }
                    
        }
    }
}
