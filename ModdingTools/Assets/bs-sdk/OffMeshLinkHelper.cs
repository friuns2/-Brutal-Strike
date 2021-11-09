using UnityEngine;
using UnityEngine.AI;
[ExecuteInEditMode]
public class OffMeshLinkHelper : bs
{
    public bool valid;

    public override void Awake()
    {
        base.Awake();
//        NavMesh.onPreUpdate+=OnPreUpdate;
    }
    [ContextMenu("validate")]
    public void Validate()
    {
        if (!gameObject.activeInHierarchy) return;
        var mf = GetComponent<OffMeshLink>();
        if (NavMesh.SamplePosition(mf.startTransform.position, out NavMeshHit h, 2, -1) &&
            NavMesh.SamplePosition(mf.endTransform.position, out NavMeshHit h2, 2, -1))
        {
            mf.startTransform.position = h.position;
            mf.endTransform.position = h2.position;
            mf.endTransform.hasChanged = mf.startTransform.hasChanged = false;
            valid = true;
        }
        else
            Debug.Log("OffMeshLink failed validate " + gameObject.name, gameObject);
        
    }

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