using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(BoxCollider))]
public class AreaGizmo : MonoBehaviour
{
    [Header("Gizmo Settings")]
    [Tooltip("Show gizmo in play mode")]
    public bool showInPlayMode = true;

    [Tooltip("Draw solid cube")]
    public bool drawSolid = true;

    [Tooltip("Draw wireframe cube")]
    public bool drawWire = true;

    [Header("Area Color Settings")]
    [Tooltip("Base color for gizmo (alpha ignored)")]
    public Color baseColor = new(0f, 1f, 0f, 1f);

    [Range(0f, 1f)]
    [Tooltip("Alpha value for solid cube")]
    public float solidAlpha = 0.5f;

    [Range(0f, 1f)]
    [Tooltip("Alpha value for wireframe cube")]
    public float wireAlpha = 1.0f;

    void OnDrawGizmos()
    {
        if (Application.isPlaying && !showInPlayMode)
            return;

        var bc = GetComponent<BoxCollider>();

        if (!bc)
            return;

        var m = Matrix4x4.TRS(
            bc.transform.TransformPoint(bc.center),
            bc.transform.rotation,
            Vector3.Scale(bc.transform.lossyScale, bc.size)
        );

        var prevM = Gizmos.matrix;
        var prevC = Gizmos.color;
        Gizmos.matrix = m;

        if (drawSolid)
        {
            var c = baseColor;
            c.a = solidAlpha;
            Gizmos.color = c;
            Gizmos.DrawCube(Vector3.zero, Vector3.one);
        }

        if (drawWire)
        {
            var c = baseColor;
            c.a = wireAlpha;
            Gizmos.color = c;
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        }

        Gizmos.color = prevC;
        Gizmos.matrix = prevM;
    }
}
