using System.Collections;
using ObscuredFloat = System.Single;using ObscuredBool = System.Boolean;using ObscuredInt = System.Int32;using ObscuredVector3 =UnityEngine.Vector3;using ObscuredQuaternion =UnityEngine.Quaternion;using ObscuredString  = System.String;
using UnityEngine;


public enum MedkitType
{
    life,
    armor,
    speed,
    accuracy
}

public class MedKit : GunBase
{
    [FieldAtrStart]
    [Tooltip("animation time")]
    public ObscuredFloat healTime = 3;
    public ObscuredFloat maxHeal = .7f;
    [Tooltip("seconds effect lasts, adds secondaryCount health")]
    public ObscuredFloat restoreLifeOverTime = 1f;
    public ObscuredBool dontMoveWhileShooting = false;
    // public float procentage = 1f;
    public MedkitType medkitType;
    [FieldAtrEnd]
    
    
    

    protected int tmp;
#if game

    public ObscuredInt heal { get { return (int)info.secondaryCount; } }
    public void OnEnable()
    {
        // StartCoroutine2(RegisterUpdate(UpdateCor()),stopOnDisable:true);
    }

    
    [PunRPC]
    public void Heal()
    {
        pl.castsStack.Add(new Cast { medkit = this, time = restoreLifeOverTime });
    }

    public virtual void UpdateEffect()
    {
        float units = heal / restoreLifeOverTime;


        if (medkitType == MedkitType.life)
        {
            var add = Mathf.Clamp(pl.lifeDef - pl.life, 0, (pl.life > pl.lifeDef * maxHeal ? .3f : 1) * units * TimeCached.deltaTime);
            pl.life += add;
        }
        else if (medkitType == MedkitType.armor)
        {
            foreach(var a in pl.guns)
            {
                if (a.have && a is Armor armor)
                {
                    var add = Mathf.Clamp(armor.defLife - armor.life, 0, (armor.life > armor.defLife * maxHeal ? .3f : 1) *units * TimeCached.deltaTime);
                    armor.life += add;
                }
            }
        }
        
    }

    void Update()
    {
        if (IsMine)
            RPCSetMouseButton(MouseButton && have /*&& (pl.lifeDef * maxHeal > pl.life || restoreLifeOverTime > 3)*/);
        if (remoteMouseButtonDown)
        {
            // if (dontMoveWhileShooting)
                // pl.plStateToSet = PlayerStateEnum.Healing;
                
//                    pl.RPCSetPlayerState(PlayerStateEnum.Executing);
            var tm = (Time.time - time) / healTime;
            if (tm < 1)
            {
//                    if (pl.deadOrKnocked) break;
                if (pl.observing)
                    _Hud.SetProgress(tm);
            }
            else
            {
                if (IsMine)
                {
                    CallRPC(Heal);
                    Count--;
                }
                remoteMouseButtonDown = false;
                // pl.unpressKey.Add(KeyCode.Mouse0);
            }
            
        }
        
    }
    [PunRPC]
    public override void SetMouseButton(bool value, Vector3 hpos, int ip, Vector3 dc)
    {
        base.SetMouseButton(value, hpos, ip, dc);
        if (value)
        {
            isShooting = true;
            PlayAnimation(Anims.startShoot, 0);
            time = Time.time;
        }
        else
        {
            isShooting = false;
            pl.weaponAnimationEventsAudio.Stop();
            if(!pl.deadOrKnocked)
                PlayAnimation(Anims.draw, 0); //startShoot is endless
            StopAllCoroutines();
        }
    }
    private float time;
    
#endif


}