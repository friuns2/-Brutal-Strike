using UnityEngine;

public class ParticleOffsetter : MonoBehaviour
{
    public float offset = 3;
#if game
    public void Start()
    {
        tr = transform;
    }
    public Transform tr;
    public void LateUpdate()
    {
        var handsCamera = bs._ObsCamera.handsCamera;
        Vector3 scr = handsCamera.WorldToScreenPoint(tr.position);
        scr.z += offset;
        var wp = handsCamera.ScreenToWorldPoint(scr);
        for (int i = tr.childCount - 1; i >= 0; i--)
            tr.GetChild(i).position = wp;
    }
#endif
}