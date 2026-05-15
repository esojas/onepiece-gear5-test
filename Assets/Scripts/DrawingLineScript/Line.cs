using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private GameObject colliderHolder;

    List<Vector3> points;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void UpdateLine(Vector3 mousePos)
    {
        if (points == null)
        {
            points = new List<Vector3>();
            colliderHolder = new GameObject("Colliders");
            colliderHolder.transform.SetParent(transform);
            SetPoint(mousePos);
        }
        if (points.Count == 0 || Vector3.Distance(points[^1], mousePos) > 0.1f)
            SetPoint(mousePos);
    }

    void SetPoint(Vector3 point)
    {
        points.Add(point);

        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPosition(points.Count - 1, point);

        SphereCollider sphere = colliderHolder.AddComponent<SphereCollider>();
        sphere.center = colliderHolder.transform.InverseTransformPoint(point);
        sphere.radius = lineRenderer.startWidth / 2f;
        sphere.isTrigger = true;
    }
}
