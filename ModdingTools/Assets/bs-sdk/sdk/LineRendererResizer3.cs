using UnityEngine;
using System.Collections;

public class LineRendererResizer3 : bs, IOnPoolDestroy
{
    public float size = .1f;
#if game
    private Vector3 oldScale;
    public override void Awake()
    {
//        camTransform = mainCameraTransform;
        base.Awake();
        oldScale = tr.localScale;
        mainCameraFieldOfView = mainCamera.fieldOfView;
        // Update();
    }

    private float mainCameraFieldOfView;
//    private Transform camTransform;
    public void Update()
    {
        Plane plane = new Plane(mainCameraForward, mainCameraPos);
        float dist = Mathf.Pow(plane.GetDistanceToPoint(tr.position), .7f);
        tr.localScale = oldScale * Mathf.Max(dist * size * (mainCameraFieldOfView / 70f), 1);
    }
    public void OnPoolSpawn(bool b)
    {
        // if (b && isActiveAndEnabled)
            // Update();
    }
#endif
}
