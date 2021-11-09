using gui = UnityEngine.GUILayout;
using System;
using System.Diagnostics;
#if UNITY_EDITOR
using UnityEditor;
#endif
using ObscuredFloat = System.Single;using ObscuredBool = System.Boolean;using ObscuredInt = System.Int32;using ObscuredVector3 =UnityEngine.Vector3;using ObscuredQuaternion =UnityEngine.Quaternion;using ObscuredString  = System.String;
using UnityEngine;
using Debug = UnityEngine.Debug;

//[RequireComponent(typeof(Trigger))]
public class WeaponPickable : Pickable,ISkinBase, IOnPlayerStay, IBotPickable, IDrawInventory,IPointable
{
     
    [FieldAtr]
    public Rarity probability= Rarity.Common;
    public int arrayId = -1; //{ get { return arrayId; } }
    
#if game
public override void Reset()
    {
        base.Reset();
        isKinematic = true;
    }    
    public float Probability { get { return (probability == Rarity.Common ? 1 : probability == Rarity.Normal ? .6f : .3f)*probabilityFactor; } }
    public override bool CanTake(Player pl)
    {
        return gun is C4 == false;
        // return pl.gunsDict.TryGet(arrayId).CanTake(1, true);
    }
    public override void Awake()
    {
        timedropped = Time.time;
        base.Awake();
    }
    internal float timedropped;
    public override void AutoTake(Player pl)
    {
        if(Time.time - timedropped < 2)
            return;
        timedropped = Time.time;
        base.AutoTake(pl);
    }

    public GunBase gun { get { return gunsDict.TryGet(arrayId); }  }
    public bool isKinematic
    {
        get { return rigidbody.isKinematic; }
        set { rigidbody.isKinematic = value; }
    }
    public override string name
    {
        get { return gun ? gun.gunName : base.name; }
        set { base.name = value; }
    }


    public new GameObject gameObject
    {
        get { return base.gameObject; }
    }

    

    public new bool enabled
    {
        get { return transformCache.active; }
        set { transformCache.active = value; }
    }

    public override void OnLoadAsset()
    {
        base.OnLoadAsset();
        print(LogTypes.other, "Loading ", gameObject);
//        colliders = GetComponentsInChildren<Collider>();
        
        
        
        Check();
        _Game.StartCoroutine(AddMethod(delegate
        {
            if (gun == null) Debug.LogError(name + " unasingned", gameObject);
        }));
    }
    [Conditional("UNITY_EDITOR")]
    private void Check()
    {
        if(collider==null)
            Debug.LogError("Collider not found for pickable ");
        else if (collider?.gameObject?.layer != Layer.pickable)
            Debug.LogError(gameObject.name + " should be layer pickable", gameObject);
    }

    public override void Start()
    {
        if (gun.special)
            print("Special create");
        initPos = new PosRot(transform, true); //do may not executed if OnplayerConnected disabled it, move to sync
        DestroyAfter();
        base.Start();
    }

    protected override void OnCreate(bool b)
    {
        RegisterMaxLimit(this, b, 2);
        Register(this, b);
        base.OnCreate(b);
    }
    public void OnPlayerStay(Player pl, Trigger other)
    {
        if (!pl.IsMine || !enabled) return;
        GunBase wep = pl.gunsDict[arrayId];
        if (pl.observing && isDebug)
            _Hud.CenterTextUpd(t + "Item: " + name);

        var c4 = arrayId == pl.c4.id;
        if (!c4 || !Classic || pl.team == TeamEnum.Terrorists && !pl.bot)
        {
            
            if (_Loader.loaderPrefs.autoTake )
                AutoTake(pl);
            if (wep.canTake ? pl.Input2.GetKeyDown(KeyCode.F, t+(Android ?"Take ": "Press F to Take ") + wep.gunName, "Take") : pl.SameGroupGun(wep) && pl.Input2.GetKeyDown(KeyCode.F, pl.SameGroupGun(wep).CompareTextSimple(wep), "Take"))
                RPCBuyOrTake(pl,gun);
        }
    }
    
    
    public GunInfo gunInfo { get { return weapons[arrayId]; } set { weapons[arrayId] = value; } }
    

    
    public override void OnPlConnected(PhotonPlayer photonPlayer)
    {
        base.OnPlConnected(photonPlayer);
        if (dontSendDataOnPlConnected || customServer) return;
        CallRPC(InitDrop, enabled, gunInfo, isKinematic);
    }


    
    [PunRPCBuffered]
    public void InitDrop2(bool Enabled, GunInfo info, bool IsKinematic, Vector3 pos, Vector3 rot)
    {
        print("InitDrop");
        gunInfo = info;
        infinite = false;
        enabled = Enabled; //spectator will not see those because of trigger based cull optimization 
        isKinematic = IsKinematic;
        rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        this.UpdateTextures();
    }
    
    [PunRPCBuffered]
    public void InitDrop(bool Enabled, GunInfo info, bool IsKinematic)
    {
        print("InitDrop");
        gunInfo = info;
        infinite = false;
        enabled = Enabled; //spectator will not see those because of trigger based cull optimization 
        isKinematic = IsKinematic;
        rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        this.UpdateTextures();
    }

    
    
    public void Draw()
    {
        using (BeginHorizontal())
        {
            if(gui.Button("Take"))
                RPCBuyOrTake(_Player,gun);
            gui.Label(name);
        }
    }
    public PosRot initPos;

    [PunRPC]
    public override void OnReset()
    {
        if (gun.special)
            print("Special Destroy " + pernamentOrNested);
        base.OnReset();
        enabled = true;

        
//        if (pernamentOrNested && initPos != null)
//        {
//            pos = initPos.pos;
//            rot = initPos.rot;
//        }


    }
    [PunRPC,ViaServer]
    public override void Buy(int plid, int gunId,int take)
    {
        base.Buy(plid, gunId,take);
        enabled = false;
        InitRespawn();
    }

    
    public override void OnLevelEditorGUI()
    {
        base.OnLevelEditorGUI();
        if (GUILayout.Button("gun Settings"))
            _LevelEditor.SelectTool(gun);
    }
    public void OnPoint(Player pl)
    {
        if (pl.IsMainPlayer && gun is C4)
            PlayTutorial(Tutorial.take);
    }
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (transformCache && enabled)
            Handles.Label(transform.position, gunInfo.count + "");
    }
    #endif
    public Player pl { get { return gunBase?.pl; } }
    
    public Bundle skinBundle { get; set; }
      public GunBase gunBase { get; set; }
    // public bool loading { get; set; }
    public Action resetTextures { get; set; }
#endif

  
}