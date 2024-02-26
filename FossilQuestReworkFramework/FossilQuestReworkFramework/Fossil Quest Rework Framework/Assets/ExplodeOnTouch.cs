using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeOnTouch : MonoBehaviour
{
    public float force;
    public float radius;

    public float upliftModifier;

    public float disappearTime;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit2 = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1 << 8);

            if (hit2)
            {
                //if (gameInstances[i].CurrentTool == Tool.Chisel)
                //{
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

                        AddExplosionForce(rb, force, point, radius, upliftModifier);
                    }
                        
                }
         
            }
        }
       
    }

    void AddExplosionForce(Rigidbody2D body, float explosionForce, Vector3 explosionPosition, float explosionRadius, float upliftModifier = 0)
    {
        var dir = (body.transform.position - explosionPosition);
        float wearoff = 1 - (dir.magnitude / explosionRadius);
        Vector3 baseForce = dir.normalized * explosionForce * wearoff;

        baseForce.z = body.transform.position.z + 0.1f;
        body.AddForce(baseForce);

        if (upliftModifier != 0)
        {
            float upliftWearoff = 1 - upliftModifier / explosionRadius;
            Vector3 upliftForce = Vector2.up * explosionForce * upliftWearoff;
            upliftForce.z = 0;
            body.AddForce(upliftForce);
        }

        Destroy(body.gameObject, disappearTime);

    }
}
    

