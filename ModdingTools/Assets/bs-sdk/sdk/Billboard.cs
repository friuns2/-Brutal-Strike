using UnityEngine;

public class Billboard:bs, IOnStartGame
{
    #if game
    private Vector3 localPos;
    public void Start()
    {
        r = GetComponent<Renderer>();
        Register<IOnStartGame>(this, true);    
    }
    private Renderer r;
    public void Update()
    {
        // if (!r.isVisible) return;
        var mg = (CameraMainTransform.position - transform.position).magnitude/4;
        // r.enabled = mg > 1;
        r.material.SetFloat("_InvFade", Mathf.Min(mg, 3));
        tr.forward= -CameraMainTransform.forward;
        
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        Register<IOnStartGame>(this, false);
    }
    
    public void OnStartGame()
    {
        var transformCache = r.GetComponent<TransformCache>();
        using(transformCache.DisableCheck())
            transformCache.active = false;
    }
#endif
}