using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class DisableOnAwake : MonoBehaviour
{
    
    public void Awake()
    {
        if (!enabled) return;
        gameObject.SetActive(false);
        var tr = transform;
        for (int i = transform.childCount - 1; i >= 0; i--)
            tr.GetChild(i).gameObject.SetActive(true);
        enabled = false;
    }
}