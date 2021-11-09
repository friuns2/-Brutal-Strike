//
//using System;
//using System.IO;
//using System.Linq;
//using UnityEditor;
//using UnityEngine;
//
//
//    [InitializeOnLoad]
//    public static class SceneHelper
//    {
//        static SceneHelper()
//        {
//            SceneView.onSceneGUIDelegate += OnSceneGuiDelegate;
//        }
//        private static void OnSceneGuiDelegate(SceneView Sceneview)
//        {
//            Handles.BeginGUI();
//            if (GUI.Button(new Rect(10, 10, 50, 30), "Cam"))
//                Sceneview.AlignViewToObject(Camera.main.transform);
//
//            Handles.EndGUI();
//
//            Handles.DrawLine(Vector3.zero, Vector3.up);
//        }
//    }
//

