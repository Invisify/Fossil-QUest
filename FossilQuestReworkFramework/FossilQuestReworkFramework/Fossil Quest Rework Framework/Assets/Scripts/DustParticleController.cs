using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public class DustParticleController : MonoBehaviour
{
    // Start is called before the first frame update
    public ParticleSystem particles;

    void Start()
    {
        TouchManager.Instance.OnFingerDown += (f) => 
        {
            //particles.Stop();
            //particles.Clear();
        };
        TouchManager.Instance.OnFingerMove += (f) => 
        {
            GameInstance g = GameManager.Instance.GetGameInstance(f.screenPosition);

            if (g?.dustParticles != this || g?.CurrentTool == Tool.Chisel)
            {
                particles.Stop();
                return;
            }

            if (particles.isStopped && g.CurrentTool == Tool.Brush)
                particles.Play();

            ChangeDirection(f); 
        };

        TouchManager.Instance.OnFingerUp += (f) =>
        {
            if (GameManager.Instance.GetGameInstance(f.screenPosition)?.dustParticles != this) return;

            particles.Stop();
        };
    }

    // Update is called once per frame
    void Update()
    {

    }

    void ChangeDirection(Finger f)
    {
        transform.position = Camera.main.ScreenToWorldPoint(f.screenPosition);
        transform.position = new Vector3(transform.position.x, transform.position.y, -5f);

        //particles.s.rotation = Quaternion.FromToRotation(Vector3.right, f.currentTouch.delta);
        //transform.rotation = Quaternion.FromToRotation(Vector3.right, f.currentTouch.delta);
    }
}
