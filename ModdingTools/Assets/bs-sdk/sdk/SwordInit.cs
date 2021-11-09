

using System;
using System.Collections.Generic;
using UnityEngine;


public partial class Sword
{
    #if game
    private List<GameObject> m_effects = new List<GameObject>();
    public List<GameObject> Effects
    {
        get
        {
            if (m_effects.Count == 0)
                foreach (var a in animator.runtimeAnimatorController.animationClips)
                foreach (var b in a.events)
                {
                    if (b.objectReferenceParameter)
                        m_effects.Add((GameObject)b.objectReferenceParameter);
                }
            return m_effects;
        }
    }

    [Obsolete] public new Transform transform { get { return base.transform; } }
    [Obsolete] public new GameObject gameObject { get { return base.gameObject; } }
    // public float sampleInterval = .07f;
    public new Animator animator;
    #endif
     
}