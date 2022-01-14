using UnityEngine;

public class IkHelper : MonoBehaviour
{
    public new Transform transform;

    public Transform[] Bones= new Transform[3];
    public Quaternion[] rotations = new Quaternion[3];
    public virtual void Start()
    {
        Bones[0] = transform.parent.parent;
        Bones[1] = transform.parent;
        Bones[2] = transform;
        CompleteLength = 0;
        for (int i = 1; i < Bones.Length; i++)
            CompleteLength += (Bones[i].position - Bones[i - 1].position).magnitude;
        rotations.Fill(i => Bones[i].localRotation);
        originalPos = Bones[0].localPosition;
    }
    internal Vector3 originalPos;
    protected float CompleteLength;
    public bool enableCorrection=true;
    public bool enableCorrectionRotation=true;
    public void Update()
    {
        if (enableCorrection)
        {

            var over = .03f;
            var overShoot = (Bones[0].position - Bones.Last().position).magnitude - CompleteLength + over;

            //
            if (overShoot > 0)
                Bones[0].position = Vector3.MoveTowards(Bones[0].position, Bones.Last().position, overShoot);
            else
                Bones[0].localPosition = Vector3.MoveTowards(Bones[0].localPosition, originalPos, -overShoot);
        }
        
        if (enableCorrectionRotation)
            for (int i = 0; i < rotations.Length; i++)
                Bones[i].localRotation = rotations[i];
    }
}