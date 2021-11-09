using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.U2D;

public class GameRes : bs, IOnLoadAsset
{
    public PlayerClass defaultPlayerCLass;
    // [ReorderableList(enumType = typeof(Tutorial))]
    public AudioClip[] tutorial;
    // public Vector3 bulletSmokeVelocity;
    public Sprite skullIcon;
    public Sprite headshotIcon;
    public Sprite armorBreak;
    public Sprite knifeIcon;
    public Sprite penetrationIcon;
    [FormerlySerializedAs("victoryRedTeam")] public AudioClip2 victoryTer;
    [FormerlySerializedAs("victoryBlueTeam")] public AudioClip2 victoryCT;
    public AudioClip2 winGame;
    public AudioClip2 drawTeam;
    public AudioClip2 go;
    public AudioClip2 headShotAnouncher;
    public AudioClip2 revenge;
    public AudioClip2 multiKillSounds;
    [FormerlySerializedAs("buySound")]
    public AudioClip2 takeSound;
    public RuntimeAnimatorController playerAnimController;
    public AnimationDict defAnimations;
    public AudioClip2 openDoorSound;
    public GameObject holeDecal;
    public GameObject glassHoleDecal;
    public GameObject BloodDecal;
    public Transform concerneParticle;
    public Transform metalParticle;
    public Transform spark;
    public Transform dust;
    public Transform[] bloodParticleBig;
    public Transform[] bloodParticle;
    public Transform[] bloodParticle2;
    public Transform bloodDrops;
    public Transform bloodParticleSimple;
    public Transform[] bloodSquirts;
    public AudioClip2 spraySound;
    public AudioClip2 flesh;
    public GameObject SprayPrefab;
    public GameObject ExpDecal;
    public AnimationClip[] camDamage;
    public AnimationClip camExplosion;
    public HandsSkin handsPrefab;
    public AudioSource playClipAtPoint;
    public AnimationCurve spatialBlend;
    public AnimationCurve reverbZoneBlend;
    public PlayerCrate deadPlayerCrate;
    // public ShootRange shootRange;
    public AudioSource bulletWhistle;
    public Material flickerMaterial;
    public ParticleSystem footStepDecal;
    public Texture2D[] bloodLevels = new Texture2D[2];
    public Transform bloodHit;
    public Transform bloodHit2;
    public ParticleSystem[] bulletTrails;
    public NavMeshSurface navMeshPrefab;
  
    // public Material[] handsCustomTexture;
    public AudioClip2 checkPoint;
#if game
  public DamageTest damageText;
  // [ContextMenu("CopyAnimations")]
  //   public void CopyAnimations()
  //   {
  //       // defAnimations = FindObjectOfType<PlayerSkin>().animations;
  //       SetDirty();
  //   }
    public void OnLoadAsset()
    {
        _Game.res = this;
        foreach(Transform a in transform)
            a.gameObject.SetActive(true);
        SetupAudioCurve(playClipAtPoint);
    }
    public static void SetupAudioCurve(AudioSource source)
    {
        source.spatialize=true;        
        source.SetCustomCurve(AudioSourceCurveType.SpatialBlend,gameRes.spatialBlend);
        source.SetCustomCurve(AudioSourceCurveType.ReverbZoneMix,gameRes.reverbZoneBlend);
    }
    #else
    public void OnLoadAsset()
    {
    }
    
#endif
    
}