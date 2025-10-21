using System.Linq;
using UnityEngine;

public class PathHolder : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;
    public Vector3[] Waypoints => waypoints.Select(w => w.position).ToArray();

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