using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace doru.MeshTest
{
public class MeshTest : MonoBehaviour
{
    public MeshFilter mf;
    private new class Tr
    {
        public int[] ind;
        public int group;
        public bool went;
    }
    Dictionary<Vector3, List<Tr>> lab;
    internal Vector3[] vertices;
    internal Vector3[] normals;
    internal Vector2[] uvs;
    internal Vector4[] tangents;
    List<Tr> flat = new List<Tr>();
    internal List<MeshElement> elements = new List<MeshElement>();
    public void Awake2()
    {
        if (mf == null)
            mf = GetComponent<MeshFilter>();

        elements.Clear();
        lab = new Dictionary<Vector3, List<Tr>>();
        var mesh = mf.mesh;
        vertices = mesh.vertices;
        normals = mesh.normals;
        tangents = mesh.tangents;
        uvs = mesh.uv;
        for (int x = 0; x < mesh.subMeshCount; x++)
        {
            var trs = mesh.GetIndices(x);
            for (int i = 0; i < trs.Length; i += 3)
            {
                var tr = new Tr() {ind = new[] {trs[i], trs[i + 1], trs[i + 2]}, group = x};
                flat.Add(tr);
                for (int j = 0; j < 3; j++)
                {
                    List<Tr> lt;

                    var vector3 = vertices[trs[i + j]];
                    if (!lab.TryGetValue(vector3, out lt)) //position to indexes dictionary
                        lab[vector3] = lt = new List<Tr>();
                    lt.Add(tr);
                }
            }
        }
        foreach (Tr triangle in flat)
        {
            if (!triangle.went) //searching not scanned positions, creating element and start scanning
            {
                MeshElement element = new MeshElement();
                element.MeshTest = this;
                elements.Add(element);
                Go(triangle, element);
            }
        }
        var colors = new Color32[mesh.vertexCount];
        var max = elements.Max(a => vertices[a.indexes[0]].y);
        foreach (MeshElement a in elements)
        {
            var alpha =  1f- vertices[a.indexes[0]].y / max;
            Color32 color = Random.ColorHSV(0, 1, 0, 1, 0, 1, alpha,alpha);
            foreach (int d in a.indexes)
                colors[d] = color;
        }
        mesh.colors32 = colors;
    }
    private void Go(Tr tr, MeshElement element)
    {
        if (tr.went) return;
        tr.went = true;
        element.materialGroup = tr.group;
        element.indexes.AddRange(tr.ind);
        foreach (int i in tr.ind)
        {
            if (lab.TryGetValue(vertices[i], out List<Tr> lt)) // query vertices at same position and recurse 
                foreach (Tr t in lt)
                    Go(t, element);
        }
    }
    [ContextMenu("break")]
    public void Break()
    {
        foreach (var element in elements)
        {
            element.GenerateVertex();
            Mesh m = new Mesh();
            m.vertices = element.vertex.ToArray();
            m.triangles = element.nwlist.ToArray();
            m.uv = element.uvs.ToArray();
            m.tangents = element.tangents.ToArray();
            m.normals = element.normals.ToArray();
            m.RecalculateBounds();
            var g = new GameObject();
            g.transform.SetPositionAndRotation(transform.position,transform.rotation);
            g.transform.localScale = transform.lossyScale;
            var r = g.AddComponent<MeshRenderer>();
            var sharedMaterials = GetComponent<Renderer>().sharedMaterials;
            r.sharedMaterial = sharedMaterials[element.materialGroup % sharedMaterials.Length];
            g.AddComponent<MeshFilter>().mesh = m;
//            var c = g.AddComponent<BoxCollider>();
            
            var g2 = new GameObject("oskolok");
            Destroy(g2, 5);
            var g2transform = g2.transform;
            g2transform.position = r.bounds.center;
            g.transform.parent = g2transform;
            var rg = g2.AddComponent<Rigidbody>();
            rg.interpolation = RigidbodyInterpolation.Extrapolate;
            rg.angularVelocity = Random.insideUnitSphere * 3;
        }
        Destroy(gameObject);
    }
    

}

}
