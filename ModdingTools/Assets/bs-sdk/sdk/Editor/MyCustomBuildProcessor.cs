// using System.Collections.Generic;
// using UnityEditor.Build;
// using UnityEditor.Rendering;
// using UnityEngine;
// using UnityEngine.Rendering;
//
// class MyCustomBuildProcessor : IPreprocessShaders
// {
//     ShaderKeyword m_Blue;
//
//     public MyCustomBuildProcessor()
//     {
//         m_Blue = new ShaderKeyword("_BLUE");
//     }
//
//     public int callbackOrder { get { return 0; } }
//
//     public void OnProcessShader(Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> data)
//     {
//         data.Clear();
//         // for (int i = data.Count - 1; i >= 0; --i)
//         // {
//         //     if (!data[i].shaderKeywordSet.IsEnabled(m_Blue))
//         //         continue;
//         //
//         //     data.RemoveAt(i);
//         // }
//     }
// }