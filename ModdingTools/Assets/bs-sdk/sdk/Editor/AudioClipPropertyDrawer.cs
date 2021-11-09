using System;
using Unity.Collections;
using UnityEditor;
using UnityEngine;


[CustomPropertyDrawer(typeof(AudioClip2))]
public partial class AudioClipPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, prop);


        var arraySize = prop.FindPropertyRelative(nameof(ac.audioClips)).arraySize;
        if (arraySize == 0)
        {
            Debug.LogError("."+prop.name +" Sounds 0 (happens when AudioCLip2 used in array)");
            prop.FindPropertyRelative(nameof(ac.audioClips)).InsertArrayElementAtIndex(0);
            prop.FindPropertyRelative(nameof(ac.volume)).floatValue=1;
            return;
        }

        SerializedProperty audioClip = prop.FindPropertyRelative(nameof(ac.audioClips)+".Array.data[0]"); //AudioClip2 should never be null or exception will be thrown
        
        EditorGUI.PropertyField(position, audioClip, new GUIContent( prop.name+(arraySize > 1 ? " ("+ arraySize+ ")" : "")));

        if (GUI.changed)
        {
            prop.FindPropertyRelative(nameof(ac.name)).stringValue = prop.serializedObject.targetObject.GetType() + "." + prop.name;
            CalculateLoundness(prop, audioClip);
        }

        EditorGUI.EndProperty();
    }
    private const AudioClip2 ac=null;
    private static void CalculateLoundness(SerializedProperty prop, SerializedProperty audioClip)
    {
        var clip = audioClip.objectReferenceValue as AudioClip;
        if (clip)
        {
            
            var loudnless = prop.FindPropertyRelative(nameof(ac.audioClipLoudness));
            if (loudnless.floatValue == 0)
            {
                float[] samples = new float[clip.samples * clip.channels];
                clip.GetData(samples, 0);
                float max = 0;
                foreach (var t in samples)
                    max = Mathf.Max(max, t);
                loudnless.floatValue = max;
            }
        }
    }
}
