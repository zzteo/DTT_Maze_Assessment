using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MeshCombiner : MonoBehaviour
{
    public void CombineMeshesIn4(GameObject ParentOfTheGameObjectsToBeCombined, string NewNameForTheGameObjectWithTheCombinedMeshes, string Tag, Material MaterialToBeUsed)
    {
        var CombinedMesh = new GameObject
        {
            name = NewNameForTheGameObjectWithTheCombinedMeshes.ToString(),
        };
          
        List<MeshFilter> sourceMeshFilters = new List<MeshFilter>();

        if (sourceMeshFilters.Count != 0)
        {
            sourceMeshFilters.Clear();
        }

        foreach (Transform mesh in ParentOfTheGameObjectsToBeCombined.transform)
        {
            var meshFilter = mesh.transform.GetChild(0).GetComponent<MeshFilter>();

            if (meshFilter != null)
            {
                sourceMeshFilters.Add(meshFilter);
            }
        }

        CombineInstance[] mainCombine = new CombineInstance[sourceMeshFilters.Count];

        for (var i = 0; i < sourceMeshFilters.Count; i++)
        {
            mainCombine[i].mesh = sourceMeshFilters[i].mesh;
            mainCombine[i].transform = sourceMeshFilters[i].transform.localToWorldMatrix;
        }
        
            int totalMeshes = mainCombine.Length;
            int meshesPerObject = totalMeshes / 4; // Number of meshes per resulting object
            int remainingMeshes = totalMeshes % 4; // Number of remaining meshes

            int currentIndex = 0; // Track the current index in the originalCombineInstances array

            for (int i = 0; i < 4; i++)
            {
                int meshesToAssign = meshesPerObject;
                if (i < remainingMeshes)
                {
                    meshesToAssign++;
                }

                CombineInstance[] splitCombineInstances = new CombineInstance[meshesToAssign];

                for (int j = 0; j < meshesToAssign; j++)
                {
                    splitCombineInstances[j] = mainCombine[currentIndex];
                    currentIndex++;
                }

                GameObject splitObject = new GameObject("SplitObject_" + i);
                splitObject.tag = Tag;

                splitObject.transform.SetParent(CombinedMesh.transform);

                // Create a MeshFilter component for the resulting object
                MeshFilter meshFilter = splitObject.AddComponent<MeshFilter>();

                // Combine the meshes into a single mesh for the resulting object
                Mesh combinedMesh = new Mesh();
                combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                combinedMesh.CombineMeshes(splitCombineInstances);

                // Assign the combined mesh to the MeshFilter
                meshFilter.sharedMesh = combinedMesh;

                // Optionally, create a MeshRenderer component for the resulting object
                splitObject.AddComponent<MeshRenderer>();
                splitObject.GetComponent<Renderer>().material = MaterialToBeUsed;
                splitObject.AddComponent<MeshCollider>();
            }
        Destroy(ParentOfTheGameObjectsToBeCombined);//Destroys the object after being combined  
        }

    public IEnumerator CombineMeshesIn4Delay(GameObject ParentOfTheGameObjectsToBeCombined, string NewNameForTheGameObjectWithTheCombinedMeshes, string Tag, Material MaterialToBeUsed, float time)
    {
        yield return new WaitForSeconds(time);
        CombineMeshesIn4(ParentOfTheGameObjectsToBeCombined, NewNameForTheGameObjectWithTheCombinedMeshes, Tag, MaterialToBeUsed);
    }


    ////////////////////////////////////////Old Combine Code//////////////////////////////////////////////////

    /*public void CombineMeshes(GameObject ParentOfTheGameObjectsToBeCombined, string NewNameForTheGameObjectWithTheCombinedMeshes, string Tag, Material MaterialToBeUsed)
    {
        var CombinedMesh = new GameObject
        {
            name = NewNameForTheGameObjectWithTheCombinedMeshes.ToString(),
            tag = Tag
        };
        var targetMeshFilter = CombinedMesh.AddComponent<MeshFilter>();
        CombinedMesh.AddComponent<MeshRenderer>();

        List<MeshFilter> sourceMeshFilters = new List<MeshFilter>();

        if (sourceMeshFilters.Count != 0)
        {
            sourceMeshFilters.Clear();
        }

        foreach (Transform mesha in ParentOfTheGameObjectsToBeCombined.transform)
        {
            var meshFilter = mesha.transform.GetChild(0).GetComponent<MeshFilter>();

            if (meshFilter != null)
            {
                sourceMeshFilters.Add(meshFilter);
            }
        }

        CombineInstance[] combine = new CombineInstance[sourceMeshFilters.Count];

        for (var i = 0; i < sourceMeshFilters.Count; i++)
        {
            combine[i].mesh = sourceMeshFilters[i].mesh;
            combine[i].transform = sourceMeshFilters[i].transform.localToWorldMatrix;
        }

        var mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;//Change made so the mesh can have a number of vertices big enough to hold a maze 250x250 
        mesh.CombineMeshes(combine, true, true);
        Destroy(ParentOfTheGameObjectsToBeCombined);//Destroys the object after being combined     
        targetMeshFilter.mesh = mesh;
        CombinedMesh.GetComponent<Renderer>().material = MaterialToBeUsed;
        CombinedMesh.AddComponent<MeshCollider>();
    }
    public IEnumerator CombineMeshesDelay(GameObject ParentOfTheGameObjectsToBeCombined, string NewNameForTheGameObjectWithTheCombinedMeshes, string Tag, Material MaterialToBeUsed, float time)
    {
        yield return new WaitForSeconds(time);
        CombineMeshes(ParentOfTheGameObjectsToBeCombined, NewNameForTheGameObjectWithTheCombinedMeshes, Tag, MaterialToBeUsed);
    }*/
} 
