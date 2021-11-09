#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
using UnityEngine;

[ExecuteInEditMode]
public class DisableIfSceneLoaded : MonoBehaviour
{
#if game
    
    void Start()
    {
#if UNITY_EDITOR
        foreach (Transform a in transform)
            a.gameObject.SetActive(EditorSceneManager.loadedSceneCount == 1);
#endif
        if (Application.isPlaying && bs._Game)
            gameObject.SetActive(false);
    }

#endif
}