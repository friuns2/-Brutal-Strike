
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
// ReSharper disable PossibleMultipleEnumeration

public class IkHands:MonoBehaviour
{
    public class Tree
    {
        public List<Tree> childs = new List<Tree>();
        public float dist;
        // public int Dist => Mathf.RoundToInt(childs.Max(b => b.dist) * 100);
        public Transform tr;
        public Tree parent;
        public int level;
        public override string ToString()
        {
            return tr.ToString();
        }
    }
    public SkinnedMeshRenderer weaponsk;
    public Transform[] hands;
    public void Awake()
    {
        // Destroy(a);
    }
    public void Start()
    {
        
        lhand = GameObject.Find("LeftHandAnchor").transform;
        rhand =GameObject.Find("RightHandAnchor").transform;
        center = GameObject.Find("CenterEyeAnchor").transform;
        if (hands.Length == 0)
            FindHands();
        
        for (var i = 0; i < hands.Length; i++)
        {
            var hand = hands[i];
            var ikTarget = new GameObject("IkTarget").transform;
            var easyIK = ikTarget.gameObject.AddComponent<IKCCD>();
            // IkHelper he = ikTarget.gameObject.AddComponent<IkHelper>();
            // he.transform = hand;
            easyIK.transform = hand;
            easyIK.Target = ikTarget;
            if (i == 1)
            {
                // weapon = new GameObject("Weapon").transform;
                // weapon.SetPositionAndRotation(a.position, rhand.rotation);
                // transform.parent = weapon;
                foreach (var bone in weaponsk.bones)
                    bone.parent = hand;
            }
            ikTarget.transform.SetParent( i == 0 ? lhand : rhand,false);
        }
        GetComponentInChildren<Animator>().enabled = false;
    }
    

    // private Transform weapon;
    public void LateUpdate()
    {
        
        // weapon.position = rhand.position;
        // weapon.rotation = Quaternion.Lerp(weapon.rotation, rhand.rotation, Time.deltaTime * 30);
        
        transform.position = center.position + Vector3.down * .3f;
        transform.rotation = center.rotation;

    }

    public Transform center;
    public Transform lhand;
    public Transform rhand;
    
    private List<Tree> allBranches = new List<Tree>();
    [ContextMenu("Find Hands")]
    private void FindHands()
    {
        // foreach (var b in groups)
        // {
        //     Debug.Log("group "+b.Key.Item2);
        //     foreach (var a in b)
        //         Debug.Log(a.tr,a.tr);
        // }
        allBranches.Clear();
        Tree tree = new Tree() { tr = transform };
        FillTree(tree);
        var groups = allBranches.GroupBy(a => (a.level, (int)(a.dist * 1000)));
        groups = groups.Where(a => a.Count() == 2).OrderByDescending(a => a.Key.Item2).Take(2);
        hands = groups.SelectMax(a => a.First().level).Select(a => a.tr).ToArray();
    }
    private void FillTree(Tree root)
    {
        for (int i = 0; i < root.tr.childCount; i++)
        {
            var tr = root.tr.transform.GetChild(i);
            
            Tree child = new Tree() { tr = tr };
            child.parent = root;
            child.dist =  (root.tr.position - tr.position).magnitude;
            child.level = root.level + 1;
            root.childs.Add(child);
            // if (sm.bones.Contains(tr))
                allBranches.Add(child);
            FillTree(child);
        }
    }
}