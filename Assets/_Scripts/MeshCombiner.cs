using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MeshCombiner : MonoBehaviour
{
    //Function used for combining the meshes of the environmental objects 
    public void CombineMeshes(GameObject ParentOfTheGameObjectsToBeCombined, string NewNameForTheGameObjectWithTheCombinedMeshes, string Tag, Material MaterialToBeUsed)
    {
        //Sets the name and the tag of the object, the tag is later used to delete objects when generating another maze
        var CombinedMesh = new GameObject
        {
            name = NewNameForTheGameObjectWithTheCombinedMeshes.ToString(),
            tag = Tag
        };

        List<MeshFilter> sourceMeshFilters = new List<MeshFilter>();

        if (sourceMeshFilters.Count != 0)
        {
            sourceMeshFilters.Clear();
        }

        //Gets every mesh filter of the children of the gameObject and puts it in a list
        foreach (Transform mesh in ParentOfTheGameObjectsToBeCombined.transform)
        {
            var meshFilter = mesh.transform.GetComponent<MeshFilter>();

            if (meshFilter != null)
            {
                sourceMeshFilters.Add(meshFilter);
            }
        }

        CombineInstance[] mainCombine = new CombineInstance[sourceMeshFilters.Count];

        //Takes all of the meshes from the list and puts it in the combine instance
        for (var i = 0; i < sourceMeshFilters.Count; i++)
        {
            mainCombine[i].mesh = sourceMeshFilters[i].mesh;
            mainCombine[i].transform = sourceMeshFilters[i].transform.localToWorldMatrix;
        }
       
            //Creates a new mesh that contains all of the meshes combined
            Mesh combinedMesh = new Mesh();
            combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; //Change made so the mesh can have a number of vertices big enough to hold larger mazes 
            combinedMesh.CombineMeshes(mainCombine);

            var combineMeshFilter = CombinedMesh.AddComponent<MeshFilter>();

            
            combineMeshFilter.sharedMesh = combinedMesh; //Assign the combined mesh to the MeshFilter
            CombinedMesh.AddComponent<MeshRenderer>();
            CombinedMesh.GetComponent<Renderer>().material = MaterialToBeUsed;
            /*CombinedMesh.AddComponent<MeshCollider>(); //Adds a collider so the player can detect the walls */
        
            Destroy(ParentOfTheGameObjectsToBeCombined); //Destroys the object after being combined  
    }

    //Function used to combine the meshes of the mazes in 4 different meshes 
    public void CombineMeshesIn4(GameObject ParentOfTheGameObjectsToBeCombined, string NewNameForTheGameObjectWithTheCombinedMeshes, string Tag, Material MaterialToBeUsed)
    {
        var CombinedMesh = new GameObject
        {
            name = NewNameForTheGameObjectWithTheCombinedMeshes.ToString(),
            tag = Tag
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
            int meshesPerObject = totalMeshes / 4; //Number of meshes per resulting object
            int remainingMeshes = totalMeshes % 4; //Number of remaining meshes

            int currentIndex = 0; //Track the current index in the combine instance array

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
                splitObject.tag = Tag;//Sets the tag so the player can detect collision with it

                splitObject.transform.SetParent(CombinedMesh.transform);

                //Create a MeshFilter component for the resulting object
                MeshFilter meshFilter = splitObject.AddComponent<MeshFilter>();

                //Combine the meshes into a single mesh for the resulting object
                Mesh combinedMesh = new Mesh();
                combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                combinedMesh.CombineMeshes(splitCombineInstances);

                //Assign the combined mesh to the MeshFilter
                meshFilter.sharedMesh = combinedMesh;

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
