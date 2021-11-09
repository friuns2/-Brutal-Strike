using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

//[RequireComponent(typeof(DestroyOnGameStart))]
public class Bomb : AssetBase
{
    public AudioClip2 pip;
    public AudioClip2 explode;
    public AudioClip2 difuse;
    public AudioClip2 difuseSay2;
    // public AudioClip2 bombpl2;
    public AudioClip2 c4armed;
    public AudioClip2 c4_plant;
    public float bombDeffuseTime = 10f;
    internal float bombTime = 45f;

    //public GameObject detonator;
    public float DifusseStart;
#if game
    public override void Awake()
    {
        
        base.Awake();
    }
    public override void Start()
    {
        base.Start();
        PlayTutorial(Tutorial.planted);
        
        _Game.PlayOneShot(c4_plant);
        PlayOneShot(c4armed);
        // PlayOneShot(c4_plant);
        _Hud.CenterText("Bomb has been planted", 1);
        bombTime = roomSettings.bombTimeSec;
        
        if (showTutorial)
            DelayCall(bombTime - gameRes.tutorial[(int) Tutorial.count].length, delegate { PlayTutorial(Tutorial.count); });

        //_Game.bombs.Add(this);
    }
    protected override void OnCreate(bool b)
    {
        base.OnCreate(b);
        Register(this, b);
    }

    //public override void OnDestroy()
    //{
    //    base.OnDestroy();
    //    if (exiting) return;
    //    _Game.bombs.Remove(this);
    //}
    public bool exploded;
    public bool defused;
    public void Update()
    {
        if (exploded || defused) return;

        UpdateBomb(_Player);
        bombTime -= Time.deltaTime;
        if (TimeElapsed(bombTime > 20 ? 1 : bombTime > 9 ? .5f : .2f))
            PlayOneShot(pip);
        if (IsMine)
        {
            if (bombTime < 0)
                RPCExplode();
        }
        if (Physics.Raycast(pos + Vector3.up, Vector3.down, out RaycastHit h, 10, Layer.levelMask))
            pos = Vector3.MoveTowards(pos, h.point, Time.deltaTime * 3);
    }
    public void UpdateBomb(Player player)
    {
        if (defused || exploded||player.deadOrZKn) return;
        if ((player.team == (Mission?TeamEnum.Terrorists:TeamEnum.CounterTerrorists) || _Game.waitForPls) && (player.pos - pos).magnitude < 1 && (player.bot ||player.Input2.GetKey(KeyCode.F, hint: "Defuse") ) && !player.hit)
        {
            // if (DifusseStart == 0)
            //     for (int i = 0; i < 3; i++)
            //         MyTimer.DelayCall(this, _ => { _.PlayOneShot(_.pip); }, Random.value);
            DifusseStart += Time.deltaTime / bombDeffuseTime;
            player.plStateToSet = PlayerStateEnum.Executing;
            if (player.observing) _Hud.SetProgress(DifusseStart);
            if (DifusseStart > 1)
                RPCDifuse();
        }
        else
            DifusseStart = 0;
    }

    public void RPCExplode()
    {
        CallRPC(Explode);
        //NetworkDestroy();
    }
    public Transform explosion;
    [ContextMenu("Explode")]
    [PunRPC]
    private void Explode()
    {
        PlayTutorial(Tutorial.explode);
        audio.spread = .5f;
        PlayOneShot(explode,20);
        exploded = true;
        foreach (var a in _Game.playersAll)
            if (a.alive && a.IsMine && (a.pos - pos).magnitude < 10)
                a.RPCDie();

        if (explosion)
            exp = InstantiateAddRot(explosion, pos, Quaternion.identity);
        _Game.ExplodePhysics(pos, 2000, radius: 10);
        _ObsCamera.PlayExplodeAnim();
    }
    Transform exp;
    public override void OnReset()
    {
        Destroy(exp);
        DifusseStart = 0;
        audio.spread = 0;
        base.OnReset();
    }
    public void RPCDifuse()
    {
        _Player.IncreaseScore(_Player.gameScore.bombDeffused);
        CallRPC(Difuse);
        //NetworkDestroy();
    }
    [ContextMenu("Diffuse")]
    [PunRPC]
    public void Difuse()
    {
        _Game.PlayOneShot(difuseSay2,1);
        PlayOneShot(difuse);
        _Hud.CenterText("Bomb has been defused", 1);
        defused = true;
    }
    private BombTarget m_bombTarget;
    public BombTarget bombTarget
    {
        get
        {
            return m_bombTarget ?? (m_bombTarget = new BombTarget() {bomb = this});
        }
    }

#endif
}