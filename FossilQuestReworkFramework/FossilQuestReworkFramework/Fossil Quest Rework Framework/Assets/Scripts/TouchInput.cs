using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITouchable
{
    void OnMouseDown();
    void OnMouse();

    void OnMouseUp();
}

public class TouchInput : MonoBehaviour
{
    // Start is called before the first frame update
    MaskCamera maskCam;
    void Start()
    {
        maskCam = GetComponent<MaskCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            maskCam.UpdateMask(Input.mousePosition);
        }
    }
}
