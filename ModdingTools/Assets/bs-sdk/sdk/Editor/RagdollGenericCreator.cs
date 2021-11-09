using System;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class RagdollGenericCreator : RagdollBuilder
{

    [MenuItem("Tools/Generic Ragdoll...", false, 2000)]
    private static void CreateWizard()
    {
        DisplayWizard<RagdollGenericCreator>("Create Ragdoll");
    }
    public Transform pelvis;
    public Transform head;
    public Transform rightElbow;
    public override void CalculateAxes()
    {
        if (pelvis)
            up = CalculateDirectionAxis(pelvis.transform.InverseTransformPoint(head.position));

        if (rightElbow && pelvis)
        {
            Vector3 normalCompo;
            Vector3 tangentCompo;
            DecomposeVector(out normalCompo, out tangentCompo, pelvis.transform.InverseTransformPoint(rightElbow.position), up);
            right = CalculateDirectionAxis(tangentCompo);
        }
        forward = Vector3.Cross(right, up);
        if (flipForward)
            forward = -forward;
    }
    public RagdollHelper[] ragdollHelpers;
    void OnEnable()
    {
        pelvis = Selection.activeTransform;
        ragdollHelpers = pelvis.GetComponentsInChildren<RagdollHelper>();
        head = ragdollHelpers.FirstOrDefault(a => a.head);
        rightElbow = ragdollHelpers.FirstOrDefault(a => a.rightElbow);
    }
    public override void PrepareBones()
    {
        base.PrepareBones();
        if (pelvis.transform)
        {
            worldRight = pelvis.transform.TransformDirection(right);
            worldUp = pelvis.transform.TransformDirection(up);
            worldForward = pelvis.transform.TransformDirection(forward);
        }

        bones = new ArrayList();
        boneInfo = new BoneInfo();
        boneInfo.name = pelvis.name;
        boneInfo.anchor = pelvis.transform;
        boneInfo.parent = null;
        boneInfo.density = 2.5f;
        bones.Add(boneInfo);

        foreach (Transform a in pelvis)
            AddBone(a, boneInfo);


        //AddMirroredJoint("Knee", leftKnee, rightKnee, "Hips", worldRight, worldForward, -80f, 0.0f, 0.0f, typeof(CapsuleCollider), 0.25f, 1.5f);
        //AddJoint("Middle Spine", middleSpine, "Pelvis", worldRight, worldForward, -20f, 20f, 10f, null, 1f, 2.5f);
        //AddMirroredJoint("Arm", leftArm, rightArm, "Middle Spine", worldUp, worldForward, -70f, 10f, 50f, typeof(CapsuleCollider), 0.25f, 1f);
        //AddMirroredJoint("Elbow", leftElbow, rightElbow, "Arm", worldForward, worldUp, -90f, 0.0f, 0.0f, typeof(CapsuleCollider), 0.2f, 1f);
        //AddJoint("Head", head, "Middle Spine", worldRight, worldForward, -40f, 25f, 25f, null, 1f, 1f);
    }
    public override void OnWizardCreate()
    {
        base.OnWizardCreate();
        foreach (var a in pelvis.GetComponentsInChildren<RagdollHelper>())
            a.Execute();
    }
    public float colliderScale=.2f;
    private void AddBone(Transform bone, BoneInfo parent)
    {
        var info = AddJoint(bone.name, bone.transform, parent, worldRight, worldForward, -20f, 20f, 10f, typeof(CapsuleCollider), colliderScale, 2.5f);

        foreach (Transform a in bone)
            AddBone(a, info);
        //if (bone.haveLeftBone)
        //{
        //    AddMirroredJoint(bone.name + "Limb", bone.left.transform, bone.right.transform, bone.name, worldUp, worldForward, -70f, 10f, 50f, typeof(CapsuleCollider), 0.25f, 1f);
        //}
    }
}