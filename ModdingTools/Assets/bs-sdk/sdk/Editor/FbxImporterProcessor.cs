// using UnityEditor;
//
// public class FbxImporterProcessor : AssetPostprocessor
// {
//     
//     void OnPreprocessModel()
//     {
//         // var assetPath = Path.GetFileNameWithoutExtension(this.assetPath);
//         // if (assetPath.EndsWith("light"))
//         {
//             var imp = (ModelImporter) assetImporter;
//             if(imp.animationType != ModelImporterAnimationType.None && imp.animationType != ModelImporterAnimationType.Legacy)
//                 imp.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
//         }
//     }
// }