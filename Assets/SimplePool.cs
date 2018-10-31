using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePool : MonoBehaviour {
    
    private static LinkedList<GameObject> Pool;

    private void Awake()
    {
        Pool = new LinkedList<GameObject>();
    }

    public static GameObject GetGO()
    {
        if (Pool == null)
            Pool = new LinkedList<GameObject>();

        GameObject GO;

        if (Pool.Count > 0)
        {
            GO = Pool.First.Value;
            Pool.RemoveFirst();
            GO.GetComponent<MeshRenderer>().enabled = true;
            return GO;
        }

        GO = new GameObject();
        GO.name = "Trail";
        GO.AddComponent<MeshFilter>();
        GO.AddComponent<MeshRenderer>().sharedMaterial = ProjectileTrailRenderer.TrailMaterialStatic;

        return GO;
    }

    public static void PutGO(GameObject _GO)
    {
        if (Pool == null)
            Pool = new LinkedList<GameObject>(); 

        Pool.AddLast(_GO);
        _GO.GetComponent<MeshRenderer>().enabled = false;
        _GO.GetComponent<MeshFilter>().mesh.Clear();
    }
}
