using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using System.Linq;
using System;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using System.Text.RegularExpressions;
using UnityEditor;


public class BloodDecalDrop:MonoBehaviour{}
public class AssetBase : bs
{
    
}
public interface IPosRot
{
    
}


public class FieldAtr:Attribute
{
    public bool inherit;
    public bool dontDraw;
}
// [Serializable]
// public class AnimationDict : SerializableDictionary<Anims, AnimationClip>
// {
//
// }
public enum Anims
{
    shoot, shoot2, reload, endReload, draw, startShoot, endThrow, pressButton, fly,parachute,Executing,idle,Die
}
public interface IOnTriggerInit{}
public interface IOnPlayerNear
{
    
}
public interface ISkipIdent
{
    
    
}public interface IVarParseSkipIdent { }
public interface IVarParseDraw
{
    
}
[Flags]
public enum WeaponSetId:int { a1 = 1, a2 = 2, a3 = 4, b1 = 8, b2 = 16, b3 = 32, b4 = 64, b5 = 128 }
public class Input2
{
    
}
namespace EnumsNET
{

}
public class ObjectBase : bs
{
    
}
public interface ISkinBase
{
    
}
public class bsNetwork : bs
{
    
}

public class FieldAtrEnd : Attribute
{
    
}
public class PunRPC : Attribute
{
    
}
public class FieldAtrStart:Attribute
{
    public bool inherit { get { return false; } set { } }
}
public interface IOnInspectorGUI
{
    void OnInspectorGUI();
}

public class Tutorial
{
    
}
public interface ISetLife
{
    
}
public interface IPointable
{
    
}
namespace UnityExtensions
{
public class ReorderableList:Attribute
{
    public Type enumType;
}

}

[SelectionBase]
public class bs : Base,IOnInspectorGUI
{
    public Animator m_Animator;
    public Animator animator { get { return m_Animator ?? (m_Animator = GetComponentInChildren<Animator>()); } set { m_Animator = value; } }
    
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
    protected static bs _Game;
    internal List<Collider> levelColliders;
    public virtual void Awake()
    {
        
    }
    public virtual void OnEditorGUI()
    {

    }
}
public class RoomSettings
{
    public bool enableBotSupport=true;
    public bool enableKnocking;
}
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
public interface IOnLoadAsset{}

public interface  IOnLevelEditorGUI{}
public interface IDontDisable{}

namespace ExitGames.Client.Photon
{

}
namespace shop{}

public interface IOnPlayerStay:ITriggerEvent
{
    
}
public interface IBotPickable
{}
public interface IDrawInventory
{
    
}
public interface IOnPoolDestroy
{
    
}
public interface ITriggerEvent
{
    
}
public interface IOnPlayerEnter:ITriggerEvent
{
    
}
public interface IOnPlayerExit:ITriggerEvent
{
    
}
public class QualityControllerBase:bs{}
public class TriggerBase : bs
{
    
}
public interface IOnStartGame{}
public interface  IOnPreMatch{}
public interface ItemBaseVarParseEnable{}
public interface IPreLoadAsset{}
public interface IVarParseValueChanged{}
public static class PhotonNetwork
{
    public static string[] playerList = new string[0];

}
public static class Debug2
{
    public static void Log(string applyingSkinFrom)
    {
        Debug.Log(applyingSkinFrom);
    }
}