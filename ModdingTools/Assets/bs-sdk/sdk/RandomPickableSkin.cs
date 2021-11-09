using UnityEngine;

public class RandomPickableSkin : bs, IOnLoadAsset
{
    public static RandomPickableSkin prefab;
    public Transform place;
    public TransformCache trc;
    public void OnLoadAsset()
    {
        prefab = this;
    }
    // public void Start()
    // {
    //     trc.visible = false;
    // }
    // public void OnPlayerNearEnter(Player pl, bool enter)
    // {
    //     if (pl.IsMainPlayer)
    //         trc.visible = enter;
    // }
}