



using System.Collections.Generic;
using UnityEngine;

public class RagdollHelper : Base
{
    public List<Transform> targets = new List<Transform>();
    public Transform root;
    [ContextMenu("Fill Targets")]
    void Fill()
    {
        foreach (var a in GetComponentsInParent<Transform>())
        {
            if (a != root)
                targets.Add(a);
            else
                break;
        }
    }
    public bool rightElbow;
    public bool head;
    
    public float scaleFactor = 1;
    public void Execute()
    {
        foreach (var a in targets)
        {
            var c = a.GetComponent<CapsuleCollider>();
            if (c)
                c.radius *= scaleFactor;
        }
    }
    public static implicit operator Transform(RagdollHelper value)
    {
        if(value)
            return value.transform;
        return null;
    }
}