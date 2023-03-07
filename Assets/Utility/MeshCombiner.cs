using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Object;

namespace Utility
{
    public static class MeshCombiner
    {
        public static void MeshCombine(this GameObject gameObject, bool destroyObjects = false, params GameObject[] ignore)
        {
            // Set values to zero, wonky things happen to mesh when not centered about the origin
            var originalPosition = gameObject.transform.position;
            var originalRotation = gameObject.transform.rotation;
            var originalScale = gameObject.transform.localScale;
            gameObject.transform.position = Vector3.zero;
            gameObject.transform.rotation = Quaternion.identity;
            gameObject.transform.localScale = Vector3.one;

            // Getting every component in children may begin to get costly with giant maps,
            // shouldn't be an issue in this project
            var materials = new List<Material>();
            var combineInstanceLists = new List<List<CombineInstance>>();
            var meshFilters = gameObject.GetComponentsInChildren<MeshFilter>().Where(m =>
                !ignore.Contains(m.gameObject) && !ignore.Any(i => m.transform.IsChildOf(i.transform))).ToArray();

            foreach (var meshFilter in meshFilters)
            {
                var meshRenderer = meshFilter.GetComponent<MeshRenderer>();

                if (!meshRenderer ||
                    !meshFilter.sharedMesh ||
                    meshRenderer.sharedMaterials.Length != meshFilter.sharedMesh.subMeshCount)
                {
                    continue;
                }

                for (var s = 0; s < meshFilter.sharedMesh.subMeshCount; s++)
                {
                    var materialArrayIndex = materials.FindIndex(m => m.name == meshRenderer.sharedMaterials[s].name);
                    if (materialArrayIndex == -1)
                    {
                        materials.Add(meshRenderer.sharedMaterials[s]);
                        materialArrayIndex = materials.Count - 1;
                    }

                    combineInstanceLists.Add(new List<CombineInstance>());

                    var combineInstance = new CombineInstance
                    {
                        transform = meshRenderer.transform.localToWorldMatrix,
                        subMeshIndex = s,
                        mesh = meshFilter.sharedMesh
                    };
                    combineInstanceLists[materialArrayIndex].Add(combineInstance);
                }
            }

            // Get/Create mesh filter & renderer
            var meshFilterCombine = gameObject.GetComponent<MeshFilter>();
            if (meshFilterCombine == null)
            {
                meshFilterCombine = gameObject.AddComponent<MeshFilter>();
            }

            var meshRendererCombine = gameObject.GetComponent<MeshRenderer>();
            if (meshRendererCombine == null)
            {
                meshRendererCombine = gameObject.AddComponent<MeshRenderer>();
            }

            // Combine by material index into per-material meshes
            // as well as creating CombineInstance array for next step
            var meshes = new Mesh[materials.Count];
            var combineInstances = new CombineInstance[materials.Count];

            for (var m = 0; m < materials.Count; m++)
            {
                var combineInstanceArray = combineInstanceLists[m].ToArray();
                meshes[m] = new Mesh();
                meshes[m].CombineMeshes(combineInstanceArray, true, true);

                combineInstances[m] = new CombineInstance
                {
                    mesh = meshes[m],
                    subMeshIndex = 0
                };
            }

            // Combine into one
            meshFilterCombine.sharedMesh = new Mesh();
            meshFilterCombine.sharedMesh.CombineMeshes(combineInstances, false, false);

            // Destroy other meshes
            foreach (var oldMesh in meshes)
            {
                oldMesh.Clear();
                DestroyImmediate(oldMesh);
            }

            // Assign materials
            var materialsArray = materials.ToArray();
            meshRendererCombine.materials = materialsArray;

            if (destroyObjects)
            {
                var toDestroy = meshFilters.Select(m => m.transform);
                var toSave = new List<Transform>(8);
                for (var i = meshFilters.Length - 1; i >= 0; i--)
                {
                    if (meshFilters[i].gameObject == gameObject)
                    {
                        continue;
                    }

                    //Check if any children should be saved
                    for (var c = 0; c < meshFilters[i].transform.childCount; c++)
                    {
                        var child = meshFilters[i].transform.GetChild(c);
                        if (!toDestroy.Contains(child))
                        {
                            toSave.Add(child);
                        }
                    }

                    //Move toSave children to root object
                    for (var s = toSave.Count - 1; s >= 0; s--)
                    {
                        toSave[s].parent = gameObject.transform;
                    }

                    toSave.Clear();

                    Destroy(meshFilters[i].gameObject);
                }
            }
            else
            {
                for (var i = meshFilters.Length - 1; i >= 0; i--)
                {
                    if (meshFilters[i].gameObject == gameObject)
                    {
                        continue;
                    }

                    Destroy(meshFilters[i].GetComponent<MeshRenderer>());
                    Destroy(meshFilters[i]);
                }
            }

            // Reapply original positions once conversion is complete
            gameObject.transform.position = originalPosition;
            gameObject.transform.rotation = originalRotation;
            gameObject.transform.localScale = originalScale;
        }
    }
}

