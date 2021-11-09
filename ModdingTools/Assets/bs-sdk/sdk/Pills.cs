





using System.Linq;
using UnityEngine;

public class Pills:MedKit
{
    #if game
    public override void UpdateEffect()
    {
        if (pl.castsStack.Any(a => a.medkit is Vodka))
        {
            //pl.life -= heal / restoreLifeOverTime/2 * Time.deltaTime;
        }
        // else
            base.UpdateEffect();
    }
#endif
}