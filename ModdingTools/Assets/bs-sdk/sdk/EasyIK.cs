using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
public class EasyIK : MonoBehaviour
{
    public new Transform transform;
    public int numberOfJoints => jtrs.Length;
    public Transform ikTarget;
    public int iterations = 100;
    public float tolerance = 0.0001f;
    public Transform[] jtrs = new Transform[3];
    private Vector3 startPosition;
    private Vector3[] jpos;
    private float[] boneLength;
    private float jointChainLength;
    private float distanceToTarget;
    private Quaternion[] startRotation;
    private Vector3[] jointStartDirection;
    private Quaternion ikTargetStartRot;
    private Quaternion lastJointStartRot;
    private Vector3 originalPos;
    public Transform poleTarget;
    void Start()
    {
        jointChainLength = 0;
        // jointTransforms = new Transform[numberOfJoints];
        jpos = new Vector3[numberOfJoints];
        boneLength = new float[numberOfJoints];
        jointStartDirection = new Vector3[numberOfJoints];
        startRotation = new Quaternion[numberOfJoints];
        ikTargetStartRot = ikTarget.rotation;
        jtrs[0] = transform.parent.parent;
        jtrs[1] = transform.parent;
        jtrs[2] = transform;
        originalPos = jtrs[0].localPosition;
        for (var i = 0; i < jtrs.Length; i += 1)
        {
            var current = jtrs[i];
            // var next = current.GetChild(0);
            
            jtrs[i] = current;
            if (i == jtrs.Length - 1)
                lastJointStartRot = current.rotation;
            else
            {
                var next = jtrs[i + 1];
                boneLength[i] = Vector3.Distance(current.position, next.position);
                jointChainLength += boneLength[i];
                jointStartDirection[i] = next.position - current.position;
                startRotation[i] = current.rotation;
            }
            // current = current.GetChild(0);
        }
    }

    void PoleConstraint()
    {
        if (poleTarget != null && numberOfJoints < 4)
        {
            var limbAxis = (jpos[2] - jpos[0]).normalized;
            Vector3 poleDirection = (poleTarget.position - jpos[0]).normalized;
            Vector3 boneDirection = (jpos[1] - jpos[0]).normalized;
            Vector3.OrthoNormalize(ref limbAxis, ref poleDirection);
            Vector3.OrthoNormalize(ref limbAxis, ref boneDirection);
            Quaternion angle = Quaternion.FromToRotation(boneDirection, poleDirection);
            jpos[1] = angle * (jpos[1] - jpos[0]) + jpos[0];
        }
    }
    void Backward()
    {
        for (int i = jpos.Length - 1; i >= 0; i -= 1)
        {
            if (i == jpos.Length - 1)
                jpos[i] = ikTarget.transform.position;
            else
                jpos[i] = jpos[i + 1] + (jpos[i] - jpos[i + 1]).normalized * boneLength[i];
        }
    }
    void Forward()
    {
        for (int i = 0; i < jpos.Length; i += 1)
        {
            if (i == 0)
                jpos[i] = startPosition;
            else
                jpos[i] = jpos[i - 1] + (jpos[i] - jpos[i - 1]).normalized * boneLength[i - 1];
        }
    }
    private void LateUpdate()
    {
        
        for (int i = 0; i < jtrs.Length; i += 1)
            jpos[i] = jtrs[i].position;

        // var over = .03f;
        // var overShoot = (jpos[0]-jpos.Last()).magnitude-jointChainLength+over;
        // if (overShoot > 0)
        //     jtrs[0].position = jpos[0] = Vector3.MoveTowards(jpos[0], jpos.Last(), overShoot);
        // else
        //     jtrs[0].position = jpos[0] = Vector3.MoveTowards(jpos[0], originalPos, -overShoot );

        distanceToTarget = Vector3.Distance(jpos[0], ikTarget.position);
        if (distanceToTarget > jointChainLength)
        {
            var direction = ikTarget.position - jpos[0];
            for (int i = 1; i < jpos.Length; i += 1)
                jpos[i] = jpos[i - 1] + direction.normalized * boneLength[i - 1];
        }
        else
        {
            float distToTarget = Vector3.Distance(jpos[jpos.Length - 1], ikTarget.position);
            float counter = 0;
            while (distToTarget > tolerance)
            {
                startPosition = jpos[0];
                Backward();
                Forward();
                counter += 1;
                if (counter > iterations)
                    break;
            }
        }
        PoleConstraint();
        for (int i = 0; i < jpos.Length - 1; i += 1)
        {
            jtrs[i].position = jpos[i];
            var targetRotation = Quaternion.FromToRotation(jointStartDirection[i], jpos[i + 1] - jpos[i]);
            jtrs[i].rotation = targetRotation * startRotation[i];
        }
        Quaternion offset = lastJointStartRot * Quaternion.Inverse(ikTargetStartRot);
        jtrs.Last().rotation = ikTarget.rotation * offset;
    }
   
}