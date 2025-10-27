using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class PathHolder : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private bool visualizePath = true;
    [SerializeField] private float pathWidth = 0.3f;
    [SerializeField] private int segmentsPerCurve = 10;
    [SerializeField] private Material pathMaterial;

    public Vector3[] Waypoints => waypoints?.Select(w => w.position).ToArray() ?? new Vector3[0];

    private GameObject pathMeshObject;

    private void Start()
    {
        if (visualizePath)
        {
            UpdatePathVisualization();
        }
    }

    private void UpdatePathVisualization()
    {
        ClearPathMesh();

        Vector3[] controlPoints = Waypoints;
        if (controlPoints.Length < 2) return;
        
        List<Vector3> interpolatedPositions = new List<Vector3>();
        List<Vector3> interpolatedTangents = new List<Vector3>();

        for (int i = 0; i < controlPoints.Length - 1; i++)
        {
            Vector3 p0 = (i == 0) ? controlPoints[0] : controlPoints[i - 1];
            Vector3 p1 = controlPoints[i];
            Vector3 p2 = controlPoints[i + 1];
            Vector3 p3 = (i + 2 < controlPoints.Length) ? controlPoints[i + 2] : p2;
            
            if (i == 0)
            {
                float t = 0f;
                Vector3 pos = GetCatmullRomPosition(t, p0, p1, p2, p3);
                Vector3 tan = GetCatmullRomTangent(t, p0, p1, p2, p3);
                interpolatedPositions.Add(pos);
                interpolatedTangents.Add(tan);
            }
            
            for (int j = 1; j <= segmentsPerCurve; j++)
            {
                float t = j / (float)segmentsPerCurve;
                Vector3 pos = GetCatmullRomPosition(t, p0, p1, p2, p3);
                Vector3 tan = GetCatmullRomTangent(t, p0, p1, p2, p3);
                interpolatedPositions.Add(pos);
                interpolatedTangents.Add(tan);
            }
        }
        
        pathMeshObject = new GameObject("PathMesh");
        pathMeshObject.transform.parent = transform;

        MeshFilter meshFilter = pathMeshObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = pathMeshObject.AddComponent<MeshRenderer>();
        meshRenderer.shadowCastingMode = ShadowCastingMode.Off;

        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;
        
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        float halfWidth = pathWidth / 2f;
        float accumulatedLength = 0f;
        List<float> lengthsAtPoints = new List<float> { 0f };
        
        for (int i = 1; i < interpolatedPositions.Count; i++)
        {
            accumulatedLength += Vector3.Distance(interpolatedPositions[i - 1], interpolatedPositions[i]);
            lengthsAtPoints.Add(accumulatedLength);
        }
        
        for (int i = 0; i < interpolatedPositions.Count; i++)
        {
            Vector3 position = interpolatedPositions[i];
            Vector3 tangent = interpolatedTangents[i].normalized;
            Vector3 right = Vector3.Cross(tangent, Vector3.up).normalized;

            vertices.Add(position - right * halfWidth);
            vertices.Add(position + right * halfWidth);
            
            float v = lengthsAtPoints[i];
            uvs.Add(new Vector2(0f, v));
            uvs.Add(new Vector2(1f, v));
        }
        
        for (int i = 0; i < interpolatedPositions.Count - 1; i++)
        {
            int baseIndex = i * 2;
            triangles.Add(baseIndex);
            triangles.Add(baseIndex + 1);
            triangles.Add(baseIndex + 2);
            
            triangles.Add(baseIndex + 1);
            triangles.Add(baseIndex + 3);
            triangles.Add(baseIndex + 2);
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        
        if (pathMaterial != null)
        {
            meshRenderer.material = pathMaterial;
        }
    }

    private Vector3 GetCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        Vector3 a = 2f * p1;
        Vector3 b = p2 - p0;
        Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
        Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;

        return 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));
    }

    private Vector3 GetCatmullRomTangent(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        Vector3 a = p2 - p0;
        Vector3 b = 2f * (2f * p0 - 5f * p1 + 4f * p2 - p3);
        Vector3 c = 3f * (-p0 + 3f * p1 - 3f * p2 + p3);

        return 0.5f * (a + (b * t) + (c * t * t));
    }

    private void ClearPathMesh()
    {
        if (pathMeshObject != null)
        {
            Destroy(pathMeshObject);
            pathMeshObject = null;
        }
    }

    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length < 2) return;
        Gizmos.color = Color.blue;
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
        }
    }
}