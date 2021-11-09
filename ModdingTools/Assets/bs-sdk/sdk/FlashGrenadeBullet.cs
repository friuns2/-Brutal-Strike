






using UnityEngine;

public class FlashGrenadeBullet : GrenadeBullet
{
    public Animation flash;
    public float damageBehind=.3f;
    public bool vibrate=true; 
    #if game
    public override void SetDamage(Player pl, float damage)
    {
        if (!pl.IsMainPlayer) return;

        damage *= Mathf.Max(damageBehind, Vector3.Dot(_ObsCamera.forward, (transform.position - _ObsCamera.pos).normalized));
        
        flash.SetParent(_ObsCamera.handsCamera.transform,false);
        flash.gameObject.SetActive(true);
        flash.Play();

        var tx = (Texture2D)gun.gunSkin.renderer.sharedMaterial.GetTexture("_MetallicGlossMap");
        if (tx)
            flash.GetComponent<SpriteRenderer>().sprite = Sprite.Create(tx, new Rect(0, 0, tx.width, tx.height), new Vector2(.5f, .5f));
        flash[flash.clip.name].speed = 1f / damage;
        _Loader.stunnedEffect = 0;
        flash.GetComponent<AudioSource>()?.Play();
        #if UNITY_ANDROID
        if (vibrate)
            Handheld.Vibrate();
        #endif
        // base.SetDamage(pl, damage);
    }
    #endif
}