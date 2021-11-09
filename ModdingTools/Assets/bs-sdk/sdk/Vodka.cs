using System.Linq;
using UnityEngine;

public class Vodka : MedKit
{
    #if game
    public override void UpdateEffect()
    {
        
        // if (pl.castsStack.Any(a => a.medkit is Pills))
        // {
        //     pl.life -= heal / restoreLifeOverTime/2 * TimeCached.deltaTime;
        // }
        // else
            base.UpdateEffect();
    }
#endif
}