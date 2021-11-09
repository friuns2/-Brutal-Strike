#region

using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

#endregion

namespace UnityEditor
{
    public class RagdollBuilder : ScriptableWizard
    {
        internal ArrayList bones;
        public bool flipForward = false;
        internal Vector3 forward = Vector3.forward;

        public float limitMultiplier = 1;

        internal Vector3 right = Vector3.right;

        internal BoneInfo boneInfo;
        public Vector3 rotateAxis;
        public float strength = 0.0f;
        public float totalMass = 20f;
        internal Vector3 up = Vector3.up;
        internal Vector3 worldForward = Vector3.forward;
        internal Vector3 worldRight = Vector3.right;
        internal Vector3 worldUp = Vector3.up;
        public virtual void PrepareBones() { }
        private string CheckConsistency()
        {
            PrepareBones();
            Hashtable hashtable = new Hashtable();
            IEnumerator enumerator1 = bones.GetEnumerator();
            try
            {
                while (enumerator1.MoveNext())
                {
                    BoneInfo current = (BoneInfo)enumerator1.Current;
                    if ((bool)current.anchor)
                    {
                        if (hashtable[current.anchor] != null)
                        {
                            BoneInfo boneInfo = (BoneInfo)hashtable[current.anchor];
                            return string.Format("{0} and {1} may not be assigned to the same bone.", current.name, boneInfo.name);
                        }
                        hashtable[current.anchor] = current;
                    }
                }
            }
            finally
            {
                IDisposable disposable;
                if ((disposable = enumerator1 as IDisposable) != null)
                    disposable.Dispose();
            }
            IEnumerator enumerator2 = bones.GetEnumerator();
            try
            {
                while (enumerator2.MoveNext())
                {
                    BoneInfo current = (BoneInfo)enumerator2.Current;
                    if (current.anchor == null)
                        return string.Format("{0} has not been assigned yet.\n", current.name);
                }
            }
            finally
            {
                IDisposable disposable;
                if ((disposable = enumerator2 as IDisposable) != null)
                    disposable.Dispose();
            }
            return "";
        }


        public void DecomposeVector(out Vector3 normalCompo, out Vector3 tangentCompo, Vector3 outwardDir, Vector3 outwardNormal)
        {
            outwardNormal = outwardNormal.normalized;
            normalCompo = outwardNormal * Vector3.Dot(outwardDir, outwardNormal);
            tangentCompo = outwardDir - normalCompo;
        }

        public virtual void CalculateAxes() { }
        private void OnWizardUpdate()
        {
            errorString = CheckConsistency();
            CalculateAxes();
            if (errorString.Length != 0)
                helpString = "Drag all bones from the hierarchy into their slots.\nMake sure your character is in T-Stand.\n";
            else
                helpString = "Make sure your character is in T-Stand.\nMake sure the blue axis faces in the same direction the chracter is looking.\nUse flipForward to flip the direction";
            isValid = errorString.Length == 0;
        }

        public virtual void AddBreastColliders() { }
        public virtual void AddHeadCollider() { }
        public virtual void OnWizardCreate()
        {
            OnWizardUpdate();
            Cleanup();
            BuildCapsules();
            AddBreastColliders();
            AddHeadCollider();
            BuildBodies();
            BuildJoints();
            CalculateMass();
        }

        private BoneInfo FindBone(string name)
        {
            IEnumerator enumerator = bones.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    BoneInfo current = (BoneInfo)enumerator.Current;
                    if (current.name == name)
                        return current;
                }
            }
            finally
            {
                IDisposable disposable;
                if ((disposable = enumerator as IDisposable) != null)
                    disposable.Dispose();
            }
            return null;
        }

        internal void AddMirroredJoint(string name, Transform leftAnchor, Transform rightAnchor, string parent, Vector3 worldTwistAxis, Vector3 worldSwingAxis, float minLimit, float maxLimit, float swingLimit, Type colliderType, float radiusScale, float density)
        {
            AddJoint("Left " + name, leftAnchor, parent, worldTwistAxis, worldSwingAxis, minLimit, maxLimit, swingLimit, colliderType, radiusScale, density);
            AddJoint("Right " + name, rightAnchor, parent, worldTwistAxis, worldSwingAxis, minLimit, maxLimit, swingLimit, colliderType, radiusScale, density);
        }
        internal BoneInfo AddJoint(string name, Transform anchor, string parent, Vector3 worldTwistAxis, Vector3 worldSwingAxis, float minLimit, float maxLimit, float swingLimit, Type colliderType, float radiusScale, float density)
        {
            BoneInfo p = null;
            if (FindBone(parent) != null)
                p = FindBone(parent);
            else if (name.StartsWith("Left"))
                p = FindBone("Left " + parent);
            else if (name.StartsWith("Right"))
                p = FindBone("Right " + parent);
            return AddJoint(name, anchor, p, worldTwistAxis, worldSwingAxis, minLimit, maxLimit, swingLimit, colliderType, radiusScale, density);
        }
        internal BoneInfo AddJoint(string name, Transform anchor, BoneInfo parent, Vector3 worldTwistAxis, Vector3 worldSwingAxis, float minLimit, float maxLimit, float swingLimit, Type colliderType, float radiusScale, float density)
        {
            BoneInfo boneInfo = new BoneInfo();
            boneInfo.name = name;
            boneInfo.anchor = anchor;
            boneInfo.axis = worldTwistAxis;
            boneInfo.normalAxis = worldSwingAxis;
            boneInfo.minLimit = minLimit;
            boneInfo.maxLimit = maxLimit;
            boneInfo.swingLimit = swingLimit * limitMultiplier;
            boneInfo.density = density;
            boneInfo.colliderType = colliderType;
            boneInfo.radiusScale = radiusScale;
            boneInfo.parent = parent;
            boneInfo.parent.children.Add(boneInfo);
            bones.Add(boneInfo);
            return boneInfo;
        }

        private void BuildCapsules()
        {
            IEnumerator enumerator = bones.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    BoneInfo current = (BoneInfo)enumerator.Current;
                    if (current.colliderType == typeof(CapsuleCollider))
                    {
                        int direction;
                        float distance;
                        if (current.children.Count == 1)
                        {
                            Vector3 position = ((BoneInfo)current.children[0]).anchor.position;
                            CalculateDirection(current.anchor.InverseTransformPoint(position), out direction, out distance);
                        }
                        else
                        {
                            Vector3 position = current.anchor.position - current.parent.anchor.position + current.anchor.position;
                            CalculateDirection(current.anchor.InverseTransformPoint(position), out direction, out distance);
                            if (current.anchor.GetComponentsInChildren(typeof(Transform)).Length > 1)
                            {
                                Bounds bounds = new Bounds();
                                foreach (Transform componentsInChild in current.anchor.GetComponentsInChildren(typeof(Transform)))
                                    bounds.Encapsulate(current.anchor.InverseTransformPoint(componentsInChild.position));
                                distance = (double)distance <= 0.0 ? bounds.min[direction] : bounds.max[direction];
                            }
                            else if (this is RagdollGenericCreator)
                                continue;
                        }
                        CapsuleCollider capsuleCollider = current.anchor.gameObject.AddComponent<CapsuleCollider>();
                        capsuleCollider.direction = direction;
                        Vector3 zero = Vector3.zero;
                        zero[direction] = distance * 0.5f;
                        capsuleCollider.center = zero;
                        capsuleCollider.height = Mathf.Abs(distance);
                        capsuleCollider.radius = Mathf.Abs(distance * current.radiusScale);
                    }
                }
            }
            finally
            {
                IDisposable disposable;
                if ((disposable = enumerator as IDisposable) != null)
                    disposable.Dispose();
            }
        }

        private void Cleanup()
        {
            IEnumerator enumerator = bones.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    BoneInfo current = (BoneInfo)enumerator.Current;
                    if ((bool)current.anchor)
                    {
                        foreach (Object componentsInChild in current.anchor.GetComponentsInChildren(typeof(Joint)))
                            DestroyImmediate(componentsInChild);
                        foreach (Object componentsInChild in current.anchor.GetComponentsInChildren(typeof(Rigidbody)))
                            DestroyImmediate(componentsInChild);
                        foreach (Object componentsInChild in current.anchor.GetComponentsInChildren(typeof(Collider)))
                            DestroyImmediate(componentsInChild);
                    }
                }
            }
            finally
            {
                IDisposable disposable;
                if ((disposable = enumerator as IDisposable) != null)
                    disposable.Dispose();
            }
        }

        private void BuildBodies()
        {
            IEnumerator enumerator = bones.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    BoneInfo current = (BoneInfo)enumerator.Current;
                    current.anchor.gameObject.AddComponent<Rigidbody>();
                    current.anchor.GetComponent<Rigidbody>().mass = current.density;
                }
            }
            finally
            {
                IDisposable disposable;
                if ((disposable = enumerator as IDisposable) != null)
                    disposable.Dispose();
            }
        }

        private void BuildJoints()
        {
            IEnumerator enumerator = bones.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    BoneInfo current = (BoneInfo)enumerator.Current;
                    if (current.parent != null)
                    {
                        CharacterJoint characterJoint = current.anchor.gameObject.AddComponent<CharacterJoint>();
                        current.joint = characterJoint;
                        characterJoint.axis = CalculateDirectionAxis(current.anchor.InverseTransformDirection(current.axis));
                        characterJoint.swingAxis = CalculateDirectionAxis(current.anchor.InverseTransformDirection(current.normalAxis));
                        characterJoint.anchor = Vector3.zero;
                        characterJoint.connectedBody = current.parent.anchor.GetComponent<Rigidbody>();
                        characterJoint.enablePreprocessing = false;
                        SoftJointLimit softJointLimit = new SoftJointLimit();
                        softJointLimit.contactDistance = 0.0f;
                        softJointLimit.limit = current.minLimit;
                        characterJoint.lowTwistLimit = softJointLimit;
                        softJointLimit.limit = current.maxLimit;
                        characterJoint.highTwistLimit = softJointLimit;
                        softJointLimit.limit = current.swingLimit;
                        characterJoint.swing1Limit = softJointLimit;
                        softJointLimit.limit = 0.0f;
                        characterJoint.swing2Limit = softJointLimit;
                    }
                }
            }
            finally
            {
                IDisposable disposable;
                if ((disposable = enumerator as IDisposable) != null)
                    disposable.Dispose();
            }
        }

        private void CalculateMassRecurse(BoneInfo bone)
        {
            float mass = bone.anchor.GetComponent<Rigidbody>().mass;
            IEnumerator enumerator = bone.children.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    BoneInfo current = (BoneInfo)enumerator.Current;
                    CalculateMassRecurse(current);
                    mass += current.summedMass;
                }
            }
            finally
            {
                IDisposable disposable;
                if ((disposable = enumerator as IDisposable) != null)
                    disposable.Dispose();
            }
            bone.summedMass = mass;
        }

        private void CalculateMass()
        {
            CalculateMassRecurse(boneInfo);
            float num = totalMass / boneInfo.summedMass;
            IEnumerator enumerator = bones.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                    ((BoneInfo)enumerator.Current).anchor.GetComponent<Rigidbody>().mass *= num;
            }
            finally
            {
                IDisposable disposable;
                if ((disposable = enumerator as IDisposable) != null)
                    disposable.Dispose();
            }
            CalculateMassRecurse(boneInfo);
        }

        public static void CalculateDirection(Vector3 point, out int direction, out float distance)
        {
            direction = 0;
            if (Mathf.Abs(point[1]) > (double)Mathf.Abs(point[0]))
                direction = 1;
            if (Mathf.Abs(point[2]) > (double)Mathf.Abs(point[direction]))
                direction = 2;
            distance = point[direction];
        }

        public static Vector3 CalculateDirectionAxis(Vector3 point)
        {
            int direction = 0;
            float distance;
            CalculateDirection(point, out direction, out distance);
            Vector3 zero = Vector3.zero;
            zero[direction] = (double)distance <= 0.0 ? -1f : 1f;
            return zero;
        }

        internal static int SmallestComponent(Vector3 point)
        {
            int index = 0;
            if (Mathf.Abs(point[1]) < (double)Mathf.Abs(point[0]))
                index = 1;
            if (Mathf.Abs(point[2]) < (double)Mathf.Abs(point[index]))
                index = 2;
            return index;
        }

        internal static int LargestComponent(Vector3 point)
        {
            int index = 0;
            if (Mathf.Abs(point[1]) > (double)Mathf.Abs(point[0]))
                index = 1;
            if (Mathf.Abs(point[2]) > (double)Mathf.Abs(point[index]))
                index = 2;
            return index;
        }

        //private static int SecondLargestComponent(Vector3 point)
        //{
        //    int num1 = SmallestComponent(point);
        //    int num2 = LargestComponent(point);
        //    if (num1 < num2)
        //    {
        //        int num3 = num2;
        //        num2 = num1;
        //        num1 = num3;
        //    }
        //    if (num1 == 0 && num2 == 1)
        //        return 2;
        //    return num1 == 0 && num2 == 2 ? 1 : 0;
        //}

        public Bounds Clip(Bounds bounds, Transform relativeTo, Transform clipTransform, bool below)
        {
            int index = LargestComponent(bounds.size);
            if (Vector3.Dot(worldUp, relativeTo.TransformPoint(bounds.max)) > (double)Vector3.Dot(worldUp, relativeTo.TransformPoint(bounds.min)) == below)
            {
                Vector3 min = bounds.min;
                min[index] = relativeTo.InverseTransformPoint(clipTransform.position)[index];
                bounds.min = min;
            }
            else
            {
                Vector3 max = bounds.max;
                max[index] = relativeTo.InverseTransformPoint(clipTransform.position)[index];
                bounds.max = max;
            }
            return bounds;
        }


        public class BoneInfo
        {
            public Transform anchor;
            public Vector3 axis;
            public readonly ArrayList children = new ArrayList();
            public Type colliderType;
            public float density;
            public CharacterJoint joint;
            public float maxLimit;
            public float minLimit;
            public string name;
            public Vector3 normalAxis;
            public BoneInfo parent;
            public float radiusScale;
            public float summedMass;
            public float swingLimit;
        }
    }
}