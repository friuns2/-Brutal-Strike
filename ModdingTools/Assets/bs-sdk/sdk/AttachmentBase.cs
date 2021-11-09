using System.Collections.Generic;
using System.Linq;


public class AttachmentBase : GunBase, IPassive,IOnLoadAsset
{
    public SubsetListWeaponType canAttachToWeapon = new SubsetListWeaponType(new[] {WeaponType.Rifle, WeaponType.Smg});
    public bool generateForEachWeaponType;
#if game
    public bool CanAttachToWeapon(GunBase wep)
    {
        return canAttachToWeapon.Contains(wep.weaponType);
    }
  
    public void GenerateAssets()
    {
        if (generateForEachWeaponType )
        {
            int i = 0;
            foreach (var weaponType in canAttachToWeapon)
            {
                AttachmentBase g = Instantiate(this, transform.parent);
                g.generateForEachWeaponType = false;
                g.canAttachToWeapon = new SubsetListWeaponType(new[]{ weaponType });
                
                g.id += i + 1;
                g.gunName = gunName + " " + weaponType;
                g.name = name + " " + weaponType;
                g.weaponPickable = Instantiate(g.weaponPickable, g.weaponPickable.parent);
                g.weaponPickable.gameObject.SetActive(true);
                g.weaponPickable .probabilityFactor = 1f/g.canAttachToWeapon.Count();
                // g.weaponPickable.OnLoadAsset();
                g.weaponPickable.name = g.weaponPickable.path = weaponPickable.name + " " + weaponType;
                // g.OnLoadAsset();    
                i++;
            }
            
            disabled = true;
            //DestroyImmediate(gameObject);
        }
    }
    public override void OnPreLoadAsset()
    {
        base.OnPreLoadAsset();
        GenerateAssets();
    }
    public override void OnLoadAsset()
    {
        
        //else
        base.OnLoadAsset();
    }

    public override bool useless { get { return  !pl.guns.Any((Weapon a) => a.have && a.AttachmentAvailable(this)); } }
#else
    public void OnGenerateAsset()
    {
    }
#endif
}