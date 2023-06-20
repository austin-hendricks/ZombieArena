using System.Collections.Generic;
using UnityEngine;

namespace HendricksAustin.Lab6
{
    public class MeshCombiner : MonoBehaviour
    {
        [SerializeField] private List<MeshFilter> sourceMeshFilters;
        [SerializeField] private MeshFilter targetMeshFilter;

        [ContextMenu("Combine Meshes")]
        private void CombineMeshes()
        {
            var combine = new CombineInstance[sourceMeshFilters.Count];
            
            for (int i = 0; i < sourceMeshFilters.Count; i++)
            {
                combine[i].mesh = sourceMeshFilters[i].sharedMesh;
                combine[i].transform = sourceMeshFilters[i].transform.localToWorldMatrix;
            }

            Mesh mesh = new();
            mesh.CombineMeshes(combine);
            targetMeshFilter.mesh = mesh;
        }
    }
}


