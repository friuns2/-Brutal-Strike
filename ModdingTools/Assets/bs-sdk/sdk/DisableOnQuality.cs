using UnityEngine;

public class DisableOnQuality : QualityControllerBase
{
#if game
    public QualityLevel minQuality = QualityLevel.High;
    public QualityLevel minQualityAndroid = QualityLevel.High;
    public MonoBehaviour[] disable = new MonoBehaviour[0];
    public override void OnQualityChanged()
    {
        bool enabled = bs.qualityLevel >= (Android ? minQualityAndroid : minQuality);
        if (disable.Length > 0)
            disable.ForEach(a => a.enabled = enabled);
        else
        {
            gameObject.SetActive(enabled);
        }
    }
#endif
}