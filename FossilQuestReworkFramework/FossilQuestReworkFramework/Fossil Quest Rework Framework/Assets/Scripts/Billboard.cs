using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    // Start is called before the first frame update
    Quaternion originalRot;
    Transform camTransform;

    void Start()
    {
        originalRot = transform.rotation;
        camTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
       
        transform.rotation = originalRot * camTransform.transform.rotation;
    }
}
