using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class SkinRandomizer2 : MonoBehaviour,IOnStartGame,IOnPreMatch
{
    public Transform[] t;
    #if game
    private void Awake()
    {
        Randomize();
//        bs.Register(new OnPreMatchToAction() {a = OnStartGame}, true);
        bs.Register<IOnPreMatch>(this,true);
        bs.Register<IOnStartGame>(this,true);
    }
    private void OnDestroy()
    {
        bs.Register<IOnPreMatch>(this,false);
        bs.Register<IOnStartGame>(this,false);
    }
    private void Randomize()
    {
        var i = Random.Range(0, t.Length);
        for (int j = 0; j < t.Length - 1; j++)
            t[j].gameObject.SetActive(i == j);
    }

    public void OnStartGame()
    {
        Randomize();
    }
    public void OnPreMatch()
    {
        Randomize();
    }
    #endif
}
//public class OnPreMatchToAction:IOnPreMatch
//{
//    public Action a;
//    public void OnPreMatch()
//    {
//        a?.Invoke();
//    }
//}