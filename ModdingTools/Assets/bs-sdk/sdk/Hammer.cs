using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Hammer : GunBase
{
    public FloorCell wall;
    public FloorCell floor;
    public FloorCell ramp;
    public FloorCell cur;
    public const int _4 = 4;
    private int poseID;
    private Vector4?[] validPoses;
    private List<Vector4> poses;
    public bool singlePose;
    public AudioClip2 attackSound;
    #if game
    public void FixedUpdate2()
    {
        var position = _ObsCamera.pos + _ObsCamera.forward * _4;
        if (Physics.Raycast(_ObsCamera.pos, _ObsCamera.forward, out RaycastHit h, _4, Layer.levelMask))
            position = h.point;
        
            
        poses = GetPoses(position);
        if (Input2.GetKeyDown(KeyCode.H))
            singlePose = !singlePose;

        if (singlePose)
        {
            poseID = 0;
            poses.RemoveRange(1, poses.Count - 1);
        }

        if(validPoses==null || validPoses.Length !=poses.Count)
            validPoses= new Vector4?[poses.Count];
        // UpdateDelete(h);
        
        
        SetPlace(poses[poseID], poses[poseID].w); //restore pose for validate
        validPoses[poseID] = cur.Validate() ? poses[poseID] :(Vector4?) null;
        poseID = (poseID + 1) % poses.Count;
        SetPlace(poses[poseID], poses[poseID].w); //set pose for triggers

    }
    public override void Update2()
    {
        base.Update2();
        if (!pl.IsMainPlayer || pl.deadOrKnocked)
            return;
        FixedUpdate2();
        UpdateInput();
        if (validPoses == null) return;
        Vector4? pose = validPoses.FirstOrDefault(a => a != null);
        bool valid = pose != null;
        if (!valid) pose = poses[0];
        LogScreen(pose);
        SetPlace((Vector4)pose, pose.Value.w); //set pose for render
        gameRes.flickerMaterial.SetColor("_EmissionColor", valid ? Color.cyan : Color.red);
        if (IsMine && pl.Input2.GetMouseButtonDown(0) && valid && info.secondaryCount>0)
        {
            info.secondaryCount-=1;
            PlayAnimation(Anims.shoot, .1f, true, 1);
            pl.weaponAnimationEventsAudio.PlayOneShot(attackSound);
            cur.Create();
        }

    }
    private List<Vector4> GetPoses(Vector3 position)
    {
        List<Vector4> poses = TempList<Vector4>.GetTempList();
        SetPlaceVarRot(poses, position, _ObsCamera.eulerAngles.y);
        SetPlaceVarRot(poses, _ObsCamera.pos + _ObsCamera.forward * _4, _ObsCamera.eulerAngles.y);
        return poses;
    }
    private void UpdateDelete(RaycastHit h)
    {
        var target = h.transform?.GetComponentInParent<FloorCell>();
        if (pl.Input2.GetMouseButtonDown(1) && target != null)
        {
            PlayAnimation(Anims.shoot2, 0, true, 1);
            pl.weaponAnimationEventsAudio.PlayOneShot(attackSound);
            Destroy(target.gameObject);
        }
    }
    public Vector4 ToVector4(Vector4 v, float w)
    {
        v.w = w;
        return v;
    }
    private void SetPlaceVarRot(List<Vector4> poses, Vector3 position, float eulerAnglesY)
    {
        poses.Add(ToVector4(position, eulerAnglesY));
        poses.Add(ToVector4(position, eulerAnglesY - 30));
        poses.Add(ToVector4(position, eulerAnglesY + 30));
    }

    private void SetPlace(Vector3 position, float eulerAnglesY)
    {
        var posi = Vector3Int.RoundToInt(position / _4);
        position = posi * _4;
        cur.pos = position;
        var r = Vector3Int.RoundToInt(new Vector3(0, eulerAnglesY / 90));
        cur.rot = Quaternion.Euler(r * 90);
    }
    private void UpdateInput()
    {
        var old = cur;
        if (Input2.GetKeyDown(KeyCode.Mouse1))
        {
            RPCPlayAnimation(Anims.shoot2);
            cur = cur == wall ? ramp : cur == floor ? wall : floor;
            _Hud.CenterText(t + "Selected " + cur.name, 1);
        }

        // if (Input2.GetKeyDownP(KeyCode.C,false,5)) cur = ramp;
        // if (Input2.GetKeyDownP(KeyCode.X,false,5)) cur = floor;
        // // if (Input2.GetKeyDownP(KeyCode.Z,false,5)) cur = wall;
        
        if (old != cur)
            old.selected = false;
        cur.selected = true;
    }

    public override void OnSelectGun(bool selected)
    {
        cur.selected = selected && pl.IsMainPlayer;
        base.OnSelectGun(selected);
    }

    public override void ResetBullets()
    {
        base.ResetBullets();
        if (have)
            info.secondaryCount = secondaryCountDef;

    }
#endif
}