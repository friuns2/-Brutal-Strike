using UnityEngine;
// ReSharper disable ArrangeTypeMemberModifiers
#pragma warning disable 108,114

class GameMod : Game{
    float timeStart;
    LightSun ls;
    void StartGame(){
        base.StartGame();
        timeStart=Time.time;
            
    }
    void Init()
    {
        ls = GameObject.FindObjectOfType<LightSun>();
        RenderSettings.fog=true;
        RenderSettings.fogEndDistance = 5;
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogColor = Color.black;
        bs.roomSettings.playerSpeedFactor = 1;
        bs.roomSettings.enableRun = false;
        
    }
    void Update()
    {
        var i = (timeStart - Time.time+5)/5;
        RenderSettings.ambientIntensity = Mathf.Max(.3f,i);
        RenderSettings.reflectionIntensity = Mathf.Max(.3f,i);
        RenderSettings.fogEndDistance = Mathf.Max(15f,i*1000f);
        ls.intensivity =  Mathf.Max(.3f,i);
        base.Update();
    }
}