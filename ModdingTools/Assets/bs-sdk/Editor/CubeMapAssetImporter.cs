using UnityEditor;

public class CubeMapAssetImporter : AssetPostprocessor
{
    void OnPreprocessTexture()
    {

        if (assetPath.Contains("cubemap"))
        {
            TextureImporter myTextureImporter = (TextureImporter) assetImporter;
            myTextureImporter.textureShape = TextureImporterShape.TextureCube;

        }
    }
}