using UnityEngine;

public class LocalPhotonView:MonoBehaviour
{
    #if game
    void Awake()
    {
        GetComponent<PhotonView>().isLocal = true;
    }
    #endif
}