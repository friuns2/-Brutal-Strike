using System;
using System.Collections.Generic;
using System.Linq;
// using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class RandomPlacer:bs
{
    public GameObject prefab;
    public bool underRoof = true;
    public bool navMeshValidate = true;
    public int count = 10;
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position, transform.lossyScale);
         
    }
    
    [ContextMenu("Generate")]
    public void GenerateRandomPos()
    {
        if (!FindObjectOfType<NavMeshSurface>()) navMeshValidate = false; 
        for (int i = 0; i < count; i++)
        {
            Instantiate(prefab, GetSpawnPoint(), Quaternion.LookRotation(lastHit.normal));
        }
    }
    private RaycastHit lastHit;
    private Vector3 GetSpawnPoint()
    {
        Vector3? lastPos = null;
        for (int i = 0; i < 100; i++)
        {
            var sc = transform.lossyScale;
            var v2 = new Vector3(Random.Range(-sc.x, sc.x), 9999f/2, Random.Range(-sc.z, sc.z)) / 2;
            var sp = transform.position + v2;
            
            List<RaycastHit> hits = RaycastAll(new Ray(sp, Vector3.down), 9999, Layer.levelMask | 1 << Layer.water, QueryTriggerInteraction.Collide).TakeWhile(a => a.transform.gameObject.layer != Layer.water).ToList();
            if (underRoof)
                hits = hits.Skip(1).ToList();
            
            hits.Shuffle();
            foreach (var h in hits)
                if (!h.collider.isTrigger && Vector3.Dot(h.normal, Vector3.up) > .5f)
                {
                    lastHit = h;
                    Vector3 hPoint = h.point;
                    if (navMeshValidate)
                    {
                        if (!NavMesh.SamplePosition(h.point, out NavMeshHit nh, 1f, 1))
                            continue;
                        if (lastPos == null)
                        {
                            lastPos = nh.position;
                            continue;
                        }
                        var navMeshPath = new NavMeshPath();
                        for (int j = 0; j < 10; j++)
                        {
                            NavMesh.CalculatePath(lastPos.Value, nh.position, -1, navMeshPath);
                            if (navMeshPath.status == NavMeshPathStatus.PathComplete)
                                break;
                        }
                        if (navMeshPath.status == NavMeshPathStatus.PathComplete)
                            return nh.position;

                        lastPos = nh.position;
                    }
                    else
                        return h.point;

                    Debug.DrawRay(hPoint, Vector3.up, Color.black, 10);
                }
        }
        throw new Exception("failed to find pos at ");
    }
    public static List<RaycastHit> RaycastAll(Ray ray, float maxDist = 99999, int layerMask = ~0, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
    {
        List<RaycastHit> list = new List<RaycastHit>();
        var totalDist = 0f;
        while (totalDist <maxDist  && Physics.Raycast(ray, out var h, maxDist, layerMask, queryTriggerInteraction))
        {
            // var d = (h.point - ray.origin).magnitude;
            totalDist += h.distance;
            h.distance = totalDist;
            ray.origin = h.point + ray.direction * .1f;
            list.Add(h);
        }
        return list;
    }
    
    
}