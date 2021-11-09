#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.AI;
[ExecuteInEditMode]
public class OffMeshLinkValidator : bs
{
    public bool valid;

    public bool jump;
#if game
    public void Start()
    {
        var o = GetComponent<OffMeshLink>();
        if (jump)
            NavMeshSurface.instance.jumps.Add(new NavMeshSurface.Link() {start = o.startTransform.position, end = o.endTransform.position, biDirectional = o.biDirectional, jumpStart = true, jumpEnd = true});
    }
    [ContextMenu("validate")]
    public void Validate2()
    {
        #if UNITY_EDITOR
        if(!Application.isPlaying)
            Undo.RegisterCompleteObjectUndo(transform,"offMeshLinkValidate");
        #endif
        if (!gameObject.activeInHierarchy) return;
        var mf = GetComponent<OffMeshLink>();
        
        if (SamplePosition(mf.startTransform.position, out NavMeshHit h, 2, -1) &&
            SamplePosition(mf.endTransform.position, out NavMeshHit h2, 2, -1))
        {
            mf.startTransform.position = h.position;
            mf.endTransform.position = h2.position;
            mf.endTransform.hasChanged = mf.startTransform.hasChanged = false;
            valid = true;
        }
        else
            Debug.Log("OffMeshLink failed validate " + gameObject.name, gameObject);
        
    }
    public static bool SamplePosition(Vector3 pos, out NavMeshHit navMeshHit,float max,int layer=-1,int steps=10)
    {
        if (DownCast(pos, out RaycastHit h, 2))
            pos = h.point;
        for (float i = 1; i <= steps; i++)
            if (NavMesh.SamplePosition(pos, out navMeshHit, i/steps*max, layer))
                return true;

        navMeshHit = default(NavMeshHit);
        return false;
    }
#endif
//    [ContextMenu("validate")]
//    void OnValidate2()
//    {
//        if (valid || !gameObject.activeInHierarchy) return;
//        var mf = GetComponent<OffMeshLink>();
//
//        if (NavMesh.SamplePosition(mf.startTransform.position, out NavMeshHit h, 1, -1) &&
//            NavMesh.SamplePosition(mf.endTransform.position, out NavMeshHit h2, 1, -1))
//        {
//            mf.startTransform.position = h.position;
//            mf.endTransform.position = h2.position;
//            mf.endTransform.hasChanged = mf.startTransform.hasChanged = false;
//            valid = true;
//        }
//        else
//            Debug.Log("failed validate " + gameObject.name, gameObject);
//    }
//
//    void Update()
//    {
//        foreach (Transform a in transform)
//            if (a.hasChanged)
//                valid = false;
//    }



}