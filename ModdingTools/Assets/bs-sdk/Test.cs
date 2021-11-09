using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Test :MonoBehaviour
{
    public string path;
    void Start()
    {
        var name= AssetBundle.LoadFromFile(path).GetAllScenePaths().First() ;
        SceneManager.LoadScene(name, LoadSceneMode.Additive);

        //File.ReadAllBytes(path);
    }
}