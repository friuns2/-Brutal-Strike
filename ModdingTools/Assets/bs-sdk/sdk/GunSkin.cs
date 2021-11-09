using System;
using System.Linq;
#if game
using TriLib;
#endif
using UnityEngine;
using UnityEngine.Serialization;

// [RequireComponent(typeof(TransformCache))]
public class GunSkin : bs,ISkinBase, IPosRot,IOnLoadAsset
{
    public HumanBodyBones attachTo2 = HumanBodyBones.RightHand;
    public Transform muzzleFlash;
    public TransformCache muzzleFlashTC;
    public TransformCache transformCache;
    public BoxCollider bx;
    public new GameObject gameObject { get { return base.gameObject; } }
    public Transform attachTo;
    #if game
    public override void OnDestroy()
    {
        base.OnDestroy();
    }
    public Transform sword;
    public override void Awake()
    {
        base.Awake();
        //if (muzzleFlash)
        //{
        //    var o = new GameObject();
        //    o.name = "MuzzleFlash";
        //    o.transform.SetParent(muzzleFlash.parent);
        //    o.transform.SetPositionAndRotation(muzzleFlash.position, muzzleFlash.rotation);
        //    muzzleFlash.SetParent(_Game.resourceBundle.transform, false);
        //    muzzleFlash = o.transform;
        //}
        //muzzleCache = muzzleFlash.gameObject.AddComponent<TransformCache>();
        //muzzleCache.Init();
        //muzzleCache.Hide(true);
        
//        if (muzzleFlashTC)
//            muzzleFlashTC = muzzleFlash.gameObject.SetActive2(false);
        transformCache = transform.Component<TransformCache>();
        if (muzzleFlashTC)
        {
            transformCache.Add(muzzleFlashTC);
//            muzzleFlashTC.SetActive2(false);
        }
        
        
        enabled = false;
       
        
    }
    private void Start()
    {
        foreach (var a in GetComponentsInChildren<Collider>())
            Physics.IgnoreCollision(a, pl.controller.controller);
    }

    private Renderer[] m_renderers2=null;
    public override Renderer[] renderers { get { return m_renderers2 ?? (m_renderers2 = GetComponentsInChildren<MeshRenderer>().Where(a => muzzleFlashTC == null || a.transform != muzzleFlashTC.transform).ToArray()); } set { } }
    public new bool enabled { get { return base.enabled; } set { base.enabled = value; } }
//    public void OnValidate()
//    {
//        if (string.IsNullOrEmpty(gameObject.scene.name)) return;
//        transformCache= transform.Component<TransformCache>();
//        if (muzzleFlash)
//        {
//            muzzleFlashTC = muzzleFlash.gameObject.Component<TransformCache>();
//            muzzleFlashTC.SetDirty();
//        }
//        SetDirty();
//        
//    }
    public void LateUpdate()
    {
        transformCache.visible = gunBase.pl.visible;
        if (gunBase.pl.visible) 
//        if (renderers[0].isVisible)
            using(Profile("upd visible"))
            SetPositionAndRotation(attachTo); //do should not be null 
    }
    public PosRot posRot { get; set; }
    public Player pl { get { return gunBase.pl; } }
    // public bool loading { get; set; }
    public Bundle skinBundle { get; set; }
        public GunBase gunBase { get; set; }
    public Action resetTextures { get; set; }
#endif

    public void OnLoadAsset()
    {
        bx = GetComponentInChildren<BoxCollider>(true);
    }
}
