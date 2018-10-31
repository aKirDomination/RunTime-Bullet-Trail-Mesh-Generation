using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProjectileTrailRenderer : MonoBehaviour
{
    private Gun _gun;
    private GameObject[] TrailList;
    private List<Vector3> vertList = new List<Vector3>();
    private List<int> triangleList = new List<int>();

    private float TimeToDestroyTrail = 0f;
    private float DistanceForUpdateTrailMesh = 1f;

    public Material TrailMaterial;
    public static Material TrailMaterialStatic;

    private void Start()
    {
        _gun = GetComponent<Gun>();

        if (_gun != null)
        {
            _gun.onProjectileCreated += OnProjectileCreated;
            _gun.onProjectileRemoved += OnProjectileRemoved;
            _gun.onProjectileMoved += OnProjectileMoved;
        }
        TrailMaterialStatic = TrailMaterial;
    }
    
    void OnProjectileCreated(int index, ref Gun.Projectile projectile)
    {
        AddTrailRendererObject(index);
        projectile.deltaPosition = projectile.position;
        CreateTrailMesh(index, ref projectile);
    }

    void OnProjectileRemoved(int index, ref Gun.Projectile projectile)
    {
        StartCoroutine(RemoveTrail(TrailList[index]));
    }

    void OnProjectileMoved(int index, ref Gun.Projectile projectile)
    {
        UpdateTrailMesh(index, ref projectile);
    }

    private void AddTrailRendererObject(int index)
    {
        if (TrailList == null)
        {
            TrailList = new GameObject[_gun.maxProjectileCount];
        }

        TrailList[index] = SimplePool.GetGO();
    }

    IEnumerator RemoveTrail(GameObject obj)
    {
        yield return new WaitForSeconds(TimeToDestroyTrail);
        SimplePool.PutGO(obj);
    }

    void CreateTrailMesh(int index, ref Gun.Projectile projectile)
    {
        Mesh mesh = TrailList[index].GetComponent<MeshFilter>().mesh;

        Vector3[] vertices = new Vector3[2];
        vertices[0] = new Vector3(projectile.position.x - _gun.particleSize, projectile.position.y - _gun.particleSize, projectile.position.z);
        vertices[1] = new Vector3(projectile.position.x + _gun.particleSize, projectile.position.y - _gun.particleSize, projectile.position.z);
        mesh.vertices = vertices;
    }

    void UpdateTrailMesh(int index, ref Gun.Projectile projectile)
    {
        if (Vector3.Distance(projectile.deltaPosition, projectile.position) < DistanceForUpdateTrailMesh)
            return;

        Mesh mesh = TrailList[index].GetComponent<MeshFilter>().mesh;

        mesh.GetVertices(vertList);
        vertList.Add(new Vector3(projectile.position.x - _gun.particleSize, projectile.position.y - _gun.particleSize, projectile.position.z));
        vertList.Add(new Vector3(projectile.position.x + _gun.particleSize, projectile.position.y - _gun.particleSize, projectile.position.z));
        mesh.SetVertices(vertList);

        mesh.GetTriangles(triangleList, 0);
        triangleList.Add(vertList.Count - 1);
        triangleList.Add(vertList.Count - 3);
        triangleList.Add(vertList.Count - 2);

        triangleList.Add(vertList.Count - 3);
        triangleList.Add(vertList.Count - 4);
        triangleList.Add(vertList.Count - 2);
        mesh.SetTriangles(triangleList, 0);

        mesh.RecalculateBounds();

        projectile.deltaPosition = projectile.position;
    }
}