using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(PathProvider))]
public class PathRenderer : MonoBehaviour
{
    [SerializeField, Range(0f, 0.05f)]
    private float yOffset = 0.015f;

    [SerializeField, Range(0.01f, 1f)]
    private float width = 0.05f;
    private PathProvider provider;
    private LineRenderer line;

    private void Reset()
    {
        provider = GetComponent<PathProvider>();
    }

    private void Awake()
    {
        provider = GetComponent<PathProvider>();
        line = GetComponent<LineRenderer>();
        line.useWorldSpace = true;
        line.alignment = LineAlignment.View;
        line.widthMultiplier = width;
        line.positionCount = 0;
    }

    private void LateUpdate()
    {
        if (!provider || provider.Paused)
        {
            Clear();
            return;
        }

        var path = provider.CurrentPath;
        var corners = path?.corners;
        if (corners == null || corners.Length < 2)
        {
            Clear();
            return;
        }

        float baseY = provider.transform.position.y + yOffset;
        var pts = new Vector3[corners.Length];
        for (int i = 0; i < corners.Length; i++)
        {
            pts[i] = new Vector3(corners[i].x, baseY, corners[i].z);
        }

        line.widthMultiplier = width;
        line.positionCount = pts.Length;
        line.SetPositions(pts);
    }

    private void Clear()
    {
        if (line.positionCount != 0)
            line.positionCount = 0;
    }
}
