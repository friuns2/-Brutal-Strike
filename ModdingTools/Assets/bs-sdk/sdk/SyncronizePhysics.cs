public class SyncronizePhysics:PhotonView
{
    #if game
    public void Start()
    {
        synchronization = ViewSynchronization.ReliableDeltaCompressed;
        onSerializeRigidBodyOption = OnSerializeRigidBody.All;
        onSerializeTransformOption = OnSerializeTransform.PositionAndRotation;
    }
    #endif
}