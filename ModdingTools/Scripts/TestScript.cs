using UnityEngine;

class TestScript
{
    void Init()
    {
        RenderSettings.fog = true;
        RenderSettings.fogEndDistance = 12;
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogColor = Color.black;
        Debug.Log("FinnisH");
    }
    void Update()
    {
    }
    void OnGUI()
    {
    }
}