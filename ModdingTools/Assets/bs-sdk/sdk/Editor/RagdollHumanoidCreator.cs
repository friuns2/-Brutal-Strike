using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;


public class RagdollHumanoidCreator : RagdollBuilder
{
    [MenuItem("Tools/Ragdoll...", false, 2000)]
    private static void CreateWizard()
    {
        DisplayWizard<RagdollHumanoidCreator>("Create Ragdoll");
    }
    public Animator animator;

    public Transform head = null;
    public Transform leftArm = null;
    public Transform leftElbow = null;
    public Transform leftFoot = null;
    public Transform leftHips = null;
    public Transform leftKnee = null;
    public Transform middleSpine = null;
    public Transform pelvis;
    public Transform rightArm = null;
    public Transform rightElbow = null;
    public Transform rightFoot = null;
    public Transform rightHips = null;
    public Transform rightKnee = null;
    private void OnDrawGizmos()
    {
        if (!(bool)pelvis)
            return;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(pelvis.position, pelvis.TransformDirection(right));
        Gizmos.color = Color.green;
        Gizmos.DrawRay(pelvis.position, pelvis.TransformDirection(up));
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(pelvis.position, pelvis.TransformDirection(forward));
    }

    public override void CalculateAxes()
    {
        if (head != null && pelvis != null)
            up = CalculateDirectionAxis(pelvis.InverseTransformPoint(head.position));
        if (rightElbow != null && pelvis != null)
        {
            Vector3 normalCompo;
            Vector3 tangentCompo;
            DecomposeVector(out normalCompo, out tangentCompo, pelvis.InverseTransformPoint(rightElbow.position), up);
            right = CalculateDirectionAxis(tangentCompo);
        }
        forward = Vector3.Cross(right, up);
        if (flipForward)
            forward = -forward;
    }

    public override void PrepareBones()
    {
        if ((bool)pelvis)
        {
            worldRight = pelvis.TransformDirection(right);
            worldUp = pelvis.TransformDirection(up);
            worldForward = pelvis.TransformDirection(forward);
        }
        bones = new ArrayList();
        boneInfo = new BoneInfo();
        boneInfo.name = "Pelvis";
        boneInfo.anchor = pelvis;
        boneInfo.parent = null;
        boneInfo.density = 2.5f;
        bones.Add(boneInfo);
        AddMirroredJoint("Hips", leftHips, rightHips, "Pelvis", worldRight, worldForward, -20f, 70f, 30f, typeof(CapsuleCollider), 0.3f, 1.5f);
        AddMirroredJoint("Knee", leftKnee, rightKnee, "Hips", worldRight, worldForward, -80f, 0.0f, 0.0f, typeof(CapsuleCollider), 0.25f, 1.5f);
        AddJoint("Middle Spine", middleSpine, "Pelvis", worldRight, worldForward, -20f, 20f, 10f, null, 1f, 2.5f);
        AddMirroredJoint("Arm", leftArm, rightArm, "Middle Spine", worldUp, worldForward, -70f, 10f, 50f, typeof(CapsuleCollider), 0.25f, 1f);
        AddMirroredJoint("Elbow", leftElbow, rightElbow, "Arm", worldForward, worldUp, -90f, 0.0f, 0.0f, typeof(CapsuleCollider), 0.2f, 1f);
        AddJoint("Head", head, "Middle Spine", worldRight, worldForward, -40f, 25f, 25f, null, 1f, 1f);
    }
    private Bounds GetBreastBounds(Transform relativeTo)
    {
        Bounds bounds = new Bounds();
        bounds.Encapsulate(relativeTo.InverseTransformPoint(leftHips.position));
        bounds.Encapsulate(relativeTo.InverseTransformPoint(rightHips.position));
        bounds.Encapsulate(relativeTo.InverseTransformPoint(leftArm.position));
        bounds.Encapsulate(relativeTo.InverseTransformPoint(rightArm.position));
        Vector3 size = bounds.size;
        size[SmallestComponent(bounds.size)] = size[LargestComponent(bounds.size)] / 2f;
        bounds.size = size;
        return bounds;
    }

    public override void AddBreastColliders()
    {
        if (middleSpine != null && pelvis != null)
        {
            Bounds bounds = Clip(GetBreastBounds(pelvis), pelvis, middleSpine, false);
            BoxCollider boxCollider1 = pelvis.gameObject.AddComponent<BoxCollider>();
            boxCollider1.center = bounds.center;
            boxCollider1.size = bounds.size;
            bounds = Clip(GetBreastBounds(middleSpine), middleSpine, middleSpine, true);
            BoxCollider boxCollider2 = middleSpine.gameObject.AddComponent<BoxCollider>();
            boxCollider2.center = bounds.center;
            boxCollider2.size = bounds.size;
        }
        else
        {
            Bounds bounds = new Bounds();
            bounds.Encapsulate(pelvis.InverseTransformPoint(leftHips.position));
            bounds.Encapsulate(pelvis.InverseTransformPoint(rightHips.position));
            bounds.Encapsulate(pelvis.InverseTransformPoint(leftArm.position));
            bounds.Encapsulate(pelvis.InverseTransformPoint(rightArm.position));
            Vector3 size = bounds.size;
            size[SmallestComponent(bounds.size)] = size[LargestComponent(bounds.size)] / 2f;
            BoxCollider boxCollider = pelvis.gameObject.AddComponent<BoxCollider>();
            boxCollider.center = bounds.center;
            boxCollider.size = size;
        }
    }

    public override void AddHeadCollider()
    {
        if ((bool)head.GetComponent<Collider>())
            Destroy(head.GetComponent<Collider>());
        float num = Vector3.Distance(leftArm.transform.position, rightArm.transform.position) / 4f;
        SphereCollider sphereCollider = head.gameObject.AddComponent<SphereCollider>();
        sphereCollider.radius = num;
        Vector3 zero = Vector3.zero;
        int direction;
        float distance;
        CalculateDirection(head.InverseTransformPoint(pelvis.position), out direction, out distance);
        zero[direction] = (double)distance <= 0.0 ? num : -num;
        sphereCollider.center = zero;
    }

    void OnEnable()
    {
        var a = animator = Selection.activeGameObject.GetComponentInChildren<Animator>();
        if (a.isHuman)
        {
            leftHips = a.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
            rightHips = a.GetBoneTransform(HumanBodyBones.RightUpperLeg);
            leftKnee = a.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
            rightKnee = a.GetBoneTransform(HumanBodyBones.RightLowerLeg);
            rightFoot = a.GetBoneTransform(HumanBodyBones.LeftFoot);
            leftFoot = a.GetBoneTransform(HumanBodyBones.LeftFoot);
            head = a.GetBoneTransform(HumanBodyBones.Head);
            leftArm = a.GetBoneTransform(HumanBodyBones.LeftUpperArm);
            rightArm = a.GetBoneTransform(HumanBodyBones.RightUpperArm);
            leftElbow = a.GetBoneTransform(HumanBodyBones.LeftLowerArm);
            rightElbow = a.GetBoneTransform(HumanBodyBones.RightLowerArm);
            middleSpine = a.GetBoneTransform(HumanBodyBones.Spine);
            pelvis = a.GetBoneTransform(HumanBodyBones.Hips);

        }

    }
    
        
    private static List<PosRot> sp = new List<PosRot>();
    [MenuItem("Tools/Pose/Save Pose")]
    public static void SavePose()
    {
        sp = new List<PosRot>();
        foreach (var t in Selection.activeGameObject.GetComponentsInChildren<Transform>())
            sp.Add(new PosRot() { pos = t.position, rot = t.rotation, scale = t.localScale, tr = t });

    }
    [MenuItem("Tools/Pose/Load Pose")]
    public static void LoadPose()
    {
        foreach (PosRot t in sp)
        {
            t.tr.position = t.pos;
            t.tr.rotation = t.rot;
            t.tr.localScale = t.scale;
        }
    }

    public class PosRot
    {
        public Vector3 pos;
        public Quaternion rot;
        public Vector3 scale;
        public Transform tr;
    }

    
    public override void OnWizardCreate()
    {
        Debug.Log("OnWizardCreate");
        Selection.activeGameObject = animator.gameObject;
        foreach (var a in animator.gameObject.GetComponentsInChildren<Collider>())
            DestroyImmediate(a);
        
        foreach (var a in animator.gameObject.GetComponentsInChildren<Joint>())
            DestroyImmediate(a);

        foreach (var a in animator.gameObject.GetComponentsInChildren<Rigidbody>())
            DestroyImmediate(a);
        
        SavePose();
#pragma warning disable 618
        Undo.CreateSnapshot();
#pragma warning restore 618
        animator.GetType().GetMethod("WriteDefaultPose", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(animator, null);
        base.OnWizardCreate();
        LoadPose();

        foreach (var a in animator.GetBoneTransform(HumanBodyBones.Hips).GetComponentsInChildren<CharacterJoint>())
        {
            a.axis = Quaternion.Euler(rotateAxis) * a.axis;
            a.swingAxis = Quaternion.Euler(rotateAxis) * a.swingAxis;
            a.autoConfigureConnectedAnchor = false;
            a.enableProjection = true;
        }
        foreach (var a in animator.GetBoneTransform(HumanBodyBones.Hips).GetComponentsInChildren<Transform>())
            a.gameObject.layer = LayerMask.NameToLayer("Ragdoll");
        
    }
}