using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;
public class C4 : GunBase
{
    public static C4 prototype;
    public Bomb bomb;

#if game
    public override void OnLoadAsset()
    {
        prototype = this;
        base.OnLoadAsset();
    }
    public override void Awake()
    {
        base.Awake();
        pl.c4 = this;
    }
   
    
    public override void Update2()
    {
        base.Update2();
        
        if(IsMine)
            RPCSetMouseButton(have && (!menuLoaded||pl.bombIsNear ||!Classic) && MouseButton && pl.move == Vector3.zero);


        var planting = remoteMouseButtonDown && (Mission || pl.team == TeamEnum.Terrorists);
        //SetBool(AnimParams.planting, planting);
        
        if (planting)
        {
            pl.plStateToSet = PlayerStateEnum.Executing;
            
            pl.StopMove();
//            PlayAnimation(Anims.pressButton, 0);
            
            float tm = (Time.time - lastMouseButtonDown) / reloadTime;
            if (IsMine)
                if (tm < 1)
                    _Hud.SetProgress(tm);
                else if (pl.ControllerIsGrounded)
                {
                    _Player.IncreaseScore(_Player.gameScore.bombPlanted);
                    InstantiateSceneObject(bomb, pl.pos, false);
                    //PhotonNetwork.InstantiateSceneObject(bulletPrefab.name, pl.pos, bulletPrefab.transform.localRotation, 0, null);
                    Count--;
                    pl.LastGun();
                }
        }
//        else
//            SetTrigger(AnimParams.Interrupt);
    }
    public override void OnTake(GunInfo info)
    {
        base.OnTake(info);
        if (IsMine && canSelect)
            pl.RPCSelectGun(id);
    }
    public override void OnSelectGun(bool selected)
    {
        base.OnSelectGun(selected);
        if (pl.IsMainPlayer)
        {
            if (selected)
                PlayTutorial(Tutorial.plant);

            var bombPlaces = GetInstances<BombPlace>();
            for (var i = 0; i < bombPlaces.Count; i++)
            {
                BombPlace a = bombPlaces[i];
                if (a.txt)
                {
                    a.txt.gameObject.SetActive(selected);
                    a.txt.text = i == 0 ? "A" : i == 1 ? "B" : "C";
                }
            }
        }
    }
#if UNITY_EDITOR
    public override void ParseAnimEditor(Anims anim, AnimationClip clip)
    {
        if (anim == Anims.pressButton)
            reloadTime = clip.length; ;
        base.ParseAnimEditor(anim, clip);
    }
#endif

#endif
}