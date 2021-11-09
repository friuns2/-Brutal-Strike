//public enum AmmoType { m556, m762,ACP45 }

public class Ammo : AttachmentBase, IPassive
{
    public override void Awake()
    {
        base.Awake();
    }
    #if game
    public override void OnPreLoadAsset()
    {
        if (bs.roomSettings.mpVersion < 17) //exclude shotgun
            canAttachToWeapon = new SubsetListWeaponType(new[] {WeaponType.Pistol, WeaponType.Rifle, WeaponType.Smg, WeaponType.Sniper});
        base.OnPreLoadAsset();
    }
#endif
}