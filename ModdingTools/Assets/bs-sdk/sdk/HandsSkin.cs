#pragma warning disable 618
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor.Animations;
using UnityEditor;

#endif

public class HandsSkin : bs, ISkinBase, IPosRot, IOnLoadAsset
{
    [SerializeField] public TransformCache tc;
    public new GameObject gameObject { get { return base.gameObject; } }
    public ParticleSystem capsules;
    public ParticleSystem smoke;
    public ParticleSystem muzzleFlash;
    public Light MuzzleFlashLight;
    public Light flashLight;
    public Transform MuzzleFlash; //old
    public Transform[] MuzzleFlash2 = new Transform[0];
    public Transform[] vrHandsTr = new Transform[0];
    public Vector3[] vrHands = new Vector3[2];
    public Transform vrForward;
    // public bool leftController;
    public Transform crosshair;
    // public Renderer handsRenderer;
    // public Transform crosshair2;
    public Transform[] attachments = new Transform[0];

    [NonSerialized] private Renderer[] m_renderers;
    public float animationSpeed = 1;

#if UNITY_EDITOR
    [ContextMenu("TPose")]
    public void TPose()
    {
        var animator = GetComponentInChildren<Animator>();
        
    }
    [ContextMenu("AutoFillAnimations")]
    public void AutoFillAnimations()
    {
        var animator = GetComponentInChildren<Animator>();
        var assetPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(animator.avatar));
        foreach (var path in Directory.GetFiles(assetPath, "*.fbx", SearchOption.AllDirectories))
        {
            AnimationClip anim = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            if (anim)
            {
                Debug.Log(anim.name);
                var orig = Resources.FindObjectsOfTypeAll<AnimatorController>().First(a => a.name == "WeaponHands");
                var over = animator.runtimeAnimatorController as AnimatorOverrideController;
                if (!over)
                    animator.runtimeAnimatorController = over = new AnimatorOverrideController(orig);
                over[anim.name] = anim;
            }
        }
    }
#endif

#if game
    public Transform muzzleFlashPos => MuzzleFlash2[(gunBase as Weapon)?.shootIndex ?? 0];


    public Player pl { get { return _ObsPlayer; } }
    // public bool loading { get; set; }
    public override Renderer[] renderers { get { return m_renderers ?? (m_renderers = GetComponentsInChildren<Renderer>(true).Where(a => a is MeshRenderer || a is SkinnedMeshRenderer).ToArray()); } }

    public new Animator animator { get { return base.animator; } }
    public override void Awake()
    {
        base.Awake();
        if (!crosshair)
            crosshair = tr;

        
        // if (handsRenderer == null)
        // handsRenderer = renderers.FirstOrDefault(a => a.sharedMaterial?.name == "v_hands");
        foreach (var a in GetComponentsInChildren<Transform>())
            a.gameObject.layer = Layer.hands;
        tc = gameObject.Component<TransformCache>();
        if (flashLight)
            _Game.hasLight = true;
        if (animator)
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;

        foreach (SkinnedMeshRenderer a in GetComponentsInChildren<SkinnedMeshRenderer>(true))
        {
            a.updateWhenOffscreen = true;
//            a.lightProbeUsage = LightProbeUsage.UseProxyVolume;
//            a.lightProbeProxyVolumeOverride = _ObsCamera.gameObject;
            a.receiveShadows = false;
            a.shadowCastingMode = ShadowCastingMode.Off;
            a.probeAnchor = _ObsCamera.transform;
        }
        // if(muzzleFlash)
        foreach (var a in GetComponentsInChildren<ParticleSystem>(true))
            a.playOnAwake = false;

        if (MuzzleFlash2 == null || MuzzleFlash2.Length == 0)
            MuzzleFlash2 = new Transform[] {MuzzleFlash};
        
        if (oculus)
        {
            if (!vrForward)
                vrForward = transform;
            // transform.localScale *= 1.5f;
            foreach (var a in GetComponentsInChildren<ParticleOffsetter>())
                a.enabled = false;
            if (vrHands[0]==Vector3.zero)
            {
                InitVrHands();
            }
        }
        
    }
     
    [ContextMenu("InitVrHands")]
    private void InitVrHands()
    {
        if(vrHandsTr.Length>0)
            for (int i = 0; i < 2; i++)
                vrHands[i] = vrHandsTr[i].position - tr.position;
        else
        {
            try
            {
                (animator.runtimeAnimatorController as AnimatorOverrideController)["idle"].SampleAnimation(animator.gameObject, 0);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            try
            {
                vrHands[1] = GetComponentsInChildren<Transform>().FirstOrDefault(a => Regex.Match(a.GetName(),"(left|l).hand",RegexOptions.IgnoreCase).Success).position - tr.position;
                vrHands[0] = GetComponentsInChildren<Transform>().FirstOrDefault(a => Regex.Match(a.GetName(),"(right|r).hand",RegexOptions.IgnoreCase).Success).position - tr.position;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            // vrHands[0] = GetComponentsInChildren<Transform>().FirstOrDefault(a => a.GetName().ContainsFastIc("left") && a.GetName().ContainsFastIc("hand")).position - tr.position;
            // vrHands[1] = GetComponentsInChildren<Transform>().FirstOrDefault(a => a.GetName().ContainsFastIc("right") && a.GetName().ContainsFastIc("hand")).position - tr.position;
        }
    }

    public void Fade(string stateName, float normalizedTransitionDuration, bool playFromBegining = false, int layer = -1)
    {
        if (animator.gameObject.activeInHierarchy)
        {
            // if (playFromBegining || !animator.GetNextAnimatorStateInfo(1).IsName(stateName))
                animator.CrossFade(stateName, normalizedTransitionDuration, layer, playFromBegining ? 0 : float.NegativeInfinity);
        }
    }
    public void OnEnable()
    {
        if (animator)
            animator.speed = animationSpeed;
    }
    private void Reset()
    {
        foreach (Transform c in GetComponentsInChildren<Transform>(true))
            c.gameObject.layer = LayerMask.NameToLayer("Hands");
    }

    public PosRot posRot { get; set; }

    public void ShotAnim()
    {
        capsules.Play(true);
        if (smoke)
        {
            smoke.Clear();
            smoke.Play();
        }
        if (muzzleFlash)
        {
            muzzleFlash.transform.position = muzzleFlashPos.position;
            if (pl.weapon.flashHider)
                muzzleFlash.Emit(2);
            else
                muzzleFlash.Play(true);

            muzzleFlash.transform.localEulerAngles += new Vector3(0, 0, random.RangeMinusOne(0, 360));
        }
    }
    public Bundle skinBundle { get; set; }
    public GunBase gunBase { get; set; }
    public Action resetTextures { get; set; }
#endif
#if UNITY_EDITOR
    public override void OnInspectorGUI()
    {
        GUILayout.Label("Attachments");
        var attachmentTypes = Enum<AttachmentSlots>.values;
        if (attachments.Length != attachmentTypes.Length)
            Array.Resize(ref attachments, attachmentTypes.Length);

        foreach (AttachmentSlots a in attachmentTypes)
            // if (!gun || gun.attachmentAvailable.Contains(a))
        {
            var objectField = (Transform) EditorGUILayout.ObjectField(a.ToString(), attachments[(int) a], typeof(Transform), true);
            if (attachments[(int) a] != objectField)
            {
                attachments[(int) a] = objectField;
                EditorUtility.SetDirty(this);
            }
        }
        base.OnInspectorGUI();
    }

#endif


    public void OnLoadAsset()
    {
        foreach (var a in GetComponentsInChildren<Collider>(true))
            a.enabled = false;
    }
}