using UnityEngine;
// [ExecuteInEditMode]
public class Gib : BloodDecalDrop,IOnCollisionEnter,IOnPlayerEnter,IOnStartGame
{
    public Transform elbow;
    public Rigidbody[] rigids;
    public float maxVel = 1;
    public AudioClip[] clips;
    #if game
    public void OnCollisionEnter(Collision other)
    {
        // Debug.Log(other.impulse.magnitude);
        if (other.impulse.magnitude > maxVel)
            PlayOneShot(clips.Random());
        else
            part.Stop();
    }
    
    
    
    [ContextMenu("da")]
    void OnValidate2()
    {
        if (rigids.Length == 0 && !string.IsNullOrEmpty(gameObject.scene.name))
        {
            rigids = GetComponentInParent<Rigidbody>().GetComponentsInChildren<Rigidbody>();
          
            gameObject.SetDirty();
        }
        // foreach (var a in rigids)
        {
            var tr = GetComponentInParent<Trigger>();
            if (tr)
            {
                tr.handler = this;
                tr.gameObject.SetDirty();
            }
        }
    }
    public void OnPlayerEnter(Player pl, Trigger other, bool b)
    {
        if (!b) return;
        pl.bloodStepStart = TimeCached.time;
        // var mainModule = part.main;
        // mainModule.duration = .1f;
        PlayOneShot(clips.Random());
        part.Play();
        var rg = rigids.Random();
        rg.velocity = pl.controller.velocity.magnitude * (ZeroY(pos - pl.pos).normalized + Vector3.up * Mathf.Pow(Random.value, 2)) * 1 * rigids.Length;
        rg.AddTorque(Random.insideUnitSphere * 10);
    }
    public override void Start()
    {
        base.Start();
        Register<IOnStartGame>(this, true);
        decal = part.subEmitters.GetSubEmitterSystem(0);
        part.Play();
    }
    public override void OnDestroy()
    {
        Register<IOnStartGame>(this, false);
        base.OnDestroy();
    }

    public void OnStartGame()
    {
        _Pool.Save(transform.root);
    }
    #else
    public void OnCollisionEnter(Collision other)
    {
    }
#endif
    
}