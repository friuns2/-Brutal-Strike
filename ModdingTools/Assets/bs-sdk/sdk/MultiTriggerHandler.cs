using System.Linq;
using UnityEngine;

public class MultiTriggerHandler:bs,IOnPlayerEnter,IOnPlayerStay
{
    public MonoBehaviour[] handlers;
    private void Reset()
    {
        handlers = GetComponents<ITriggerEvent>().Where(a => a != this).Cast<MonoBehaviour>().ToArray();
        GetComponentInParent<Trigger>().handler = this;
    }
    #if game
    public void OnPlayerEnter(Player pl, Trigger other, bool b)
    {
        foreach (var a in handlers)
            if (a is IOnPlayerEnter e)
                e.OnPlayerEnter(pl, other, b);
    }
    public void OnPlayerStay(Player pl, Trigger other)
    {
        foreach (var a in handlers)
            if (a is IOnPlayerStay e)
                e.OnPlayerStay(pl, other);
    }
#endif
}