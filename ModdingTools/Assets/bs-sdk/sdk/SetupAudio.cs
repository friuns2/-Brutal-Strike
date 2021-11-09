

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
[DisallowMultipleComponent]
// [RequireComponent(typeof(AudioSource))]
public class SetupAudio:bs
{
    #if game
    public override void Awake()
    {
        base.Awake();
        if(!audio.outputAudioMixerGroup)
            audio.outputAudioMixerGroup = bs._Loader.masterAudioMixer.outputAudioMixerGroup;
    }
    public void Reset()
    {
        var audio = GetComponent<AudioSource>();
        if(audio.spatialBlend!=0)
            audio.spatialize = true;
        audio.volume *= volumeFactor;
        audio.dopplerLevel = 0;
        audio.outputAudioMixerGroup = FindObjectOfType<Loader>().masterAudioMixer.outputAudioMixerGroup;
    }

    
#if UNITY_EDITOR
    public override void OnInspectorGUI()
    {
        var audio = GetComponent<AudioSource>();
        audio.spatialize = EditorGUILayout.Toggle("spatialize",audio.spatialize);
        if (GUILayout.Button("Setup Curves"))
        {
            var gameRes = FindObjectOfType<GameRes>();
            audio.SetCustomCurve(AudioSourceCurveType.SpatialBlend,gameRes.spatialBlend);
            audio.SetCustomCurve(AudioSourceCurveType.ReverbZoneMix,gameRes.reverbZoneBlend);
        }
        base.OnInspectorGUI();
    }
#endif
#endif
}