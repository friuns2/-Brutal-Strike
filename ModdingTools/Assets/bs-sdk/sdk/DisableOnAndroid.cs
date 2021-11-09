public class DisableOnAndroid : QualityControllerBase
{
    public bool hqOnly; 
    #if game
    public override void OnQualityChanged()
    {
        gameObject.SetActive(_Loader.loaderPrefs.extraParticles && (!hqOnly || bs.qualityLevel > QualityLevel.High));
    }
#endif
}