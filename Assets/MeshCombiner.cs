using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCombiner : MonoBehaviour
{
    /* [SerializeField] private List<MeshFilter> _sourceMeshFilters;*/
    //[SerializeField] private MeshFilter _targetMeshFilter;
    /*[SerializeField] Material FenceMaterial;*/
    /*private GameObject _mazeWithCombinedMeshes;*/


    /* public void CombineMeshes()
     {
         if(_mazeWithCombinedMeshes != null)
         {
             Destroy(_mazeWithCombinedMeshes);
         }

             _mazeWithCombinedMeshes = new GameObject
         {
             name = "MazeWithCombinedMeshes"          
         };
         var mazeMesh = _mazeWithCombinedMeshes.AddComponent<MeshFilter>();
         _mazeWithCombinedMeshes.AddComponent<MeshRenderer>();

         if(_sourceMeshFilters.Count != 0)
         {
             _sourceMeshFilters.Clear();
         }

         var maze = GameObject.FindGameObjectWithTag("Walls");

         foreach(Transform wall in maze.transform)
         {
             var meshFilter = wall.transform.GetChild(0).GetComponent<MeshFilter>();

             if(meshFilter != null)
             {
                 _sourceMeshFilters.Add(meshFilter);
             }
         }

         CombineInstance[] combine = new CombineInstance[_sourceMeshFilters.Count];

         for (var i = 0; i < _sourceMeshFilters.Count; i++)
         {
             combine[i].mesh = _sourceMeshFilters[i].mesh;
             combine[i].transform = _sourceMeshFilters[i].transform.localToWorldMatrix;
         }

         var mesh = new Mesh();
         mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
         mesh.CombineMeshes(combine, true, true);
         Destroy(maze);
         //_targetMeshFilter.mesh = mesh;
         mazeMesh.mesh = mesh;
         _mazeWithCombinedMeshes.GetComponent<Renderer>().material = FenceMaterial;
     }*/

    public void CombineMeshes(GameObject ParentOfTheGameObjectsToBeCombined, string NewNameForTheGameObjectWithTheCombinedMeshes, string Tag, Material MaterialToBeUsed)
    {
        var CombinedMesh = new GameObject
        {
            name = NewNameForTheGameObjectWithTheCombinedMeshes.ToString() ,
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
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.CombineMeshes(combine, true, true);
        Destroy(ParentOfTheGameObjectsToBeCombined);//Destroys the object after being combined 
       /*foreach(Transform trans in ParentOfTheGameObjectsToBeCombined.transform) //can make this virtural I think, or abstract, make it so that you can add a line after the base function has run 
        {
            if (trans.GetComponentInChildren<MeshFilter>())
            {
                Destroy(trans.GetComponentInChildren<MeshFilter>());
            }

            if (trans.GetComponentInChildren<MeshRenderer>())
            {
                Destroy(trans.GetComponentInChildren<MeshFilter>());
            }
        }*/      
        targetMeshFilter.mesh = mesh;
        CombinedMesh.GetComponent<Renderer>().material = MaterialToBeUsed;
        CombinedMesh.AddComponent<MeshCollider>();
    }

    public IEnumerator CombineMeshesDelay(GameObject ParentOfTheGameObjectsToBeCombined, string NewNameForTheGameObjectWithTheCombinedMeshes, string Tag, Material MaterialToBeUsed, float time)
    {
        yield return new WaitForSeconds(time);
        CombineMeshes(ParentOfTheGameObjectsToBeCombined, NewNameForTheGameObjectWithTheCombinedMeshes, Tag, MaterialToBeUsed);        
    }
}
