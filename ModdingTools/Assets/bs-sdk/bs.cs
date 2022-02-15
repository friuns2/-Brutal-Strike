using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[SelectionBase]
public class bs : Base,IOnInspectorGUI
{
    
    protected ObsCamera _ObsCamera; 
    protected Animator m_Animator;
    protected  Animator animator { get { return m_Animator ?? (m_Animator = GetComponentInChildren<Animator>()); } set { m_Animator = value; } }
    public static Vector3 ZeroY(Vector3 v, float a = 0)
    {
        v.y *= a;
        return v;
    }
    public void SetDirty(MonoBehaviour g = null)
    {
#if (UNITY_EDITOR)
        UnityEditor.EditorUtility.SetDirty(g ? g : this);
#endif
    }
    public virtual void OnInspectorGUI()
    {
    }
    public static RoomSettings roomSettings=new RoomSettings();
    protected static Player _Game;
    internal List<Collider> levelColliders;
    
    public virtual void OnEditorGUI()
    {

    }
}
// public class RoomSettings
// {
//     public bool enableBotSupport=true;
//     public bool enableKnocking;
// }
public class PosRot
{
    public List<PosRot> child = new List<PosRot>();
    public string name;

    public Vector3 pos;
    public Quaternion rot;
    public Vector3 scale = Vector3.one;
    public PosRot(Transform self)
    {
     
        //t = self;
        //parent = self.parent;
        pos = self.localPosition;
        rot = self.localRotation;
        scale = self.localScale;
        name = self.name;
        foreach (Transform t in self)
        {
            child.Add(new PosRot(t));
        }
    }
    public void Restore(Transform t)
    {
       
        if (t == null) return;
        if (t.localPosition != pos || t.localRotation!= rot || t.localScale != t.localScale)
        {
            t.localScale = scale;
            t.localPosition = pos;
            t.localRotation = rot;
            Debug.Log(name + " changed", t);
        }
        List<Transform> used = new List<Transform>();
        foreach(var a in child)
        {
            var nw = t.Cast<Transform>().FirstOrDefault(b => b.name == a.name && !used.Contains(b));
            if (nw)
            {
                used.Add(nw);
                a.Restore(nw);
            }
            else
            {
                Debug.Log("not found " + a.name);
            }
        }
        
    }
}


public class ObjectBase : bs
{
    
}
public class bsNetwork : bs
{
    
}
public class AssetBase : bs
{
    
}
