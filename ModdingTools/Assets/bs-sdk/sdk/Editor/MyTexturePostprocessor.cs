using System.IO;
using UnityEditor;

class MyTexturePostprocessor : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        var assetPath = Path.GetFileNameWithoutExtension(this.assetPath);
        if (assetPath.StartsWith("Lightmap") && assetPath.EndsWith("light"))
        {
            TextureImporter textureImporter  = (TextureImporter)assetImporter;
            textureImporter.isReadable = true;
        }
        
    }
}