// Scripts/Runtime/Tour/Triggers/AreaTriggerGizmo.cs
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(BoxCollider))]
public class AreaTriggerGizmo : MonoBehaviour
{
    public bool showInPlayMode = true;
    public bool drawSolid = true;
    public bool drawWire = true;

    [Header("Single color input")]
    public Color baseColor = new Color(0f, 1f, 0f, 1f); // alpha ignored

    [Range(0f, 1f)]
    public float solidAlpha = 0.5f;   // 50% translucent for fill
    [Range(0f, 1f)]
    public float wireAlpha  = 1.0f;   // 100% opaque for wire

    void OnDrawGizmos()
    {
        if (Application.isPlaying && !showInPlayMode) return;

        var bc = GetComponent<BoxCollider>();
        if (!bc) return;

        // Match collider
        var m = Matrix4x4.TRS(
            bc.transform.TransformPoint(bc.center),
            bc.transform.rotation,
            Vector3.Scale(bc.transform.lossyScale, bc.size)
        );

        var prevM = Gizmos.matrix; var prevC = Gizmos.color;
        Gizmos.matrix = m;

        if (drawSolid)
        {
            var c = baseColor; c.a = solidAlpha;
            Gizmos.color = c;
            Gizmos.DrawCube(Vector3.zero, Vector3.one);
        }

        if (drawWire)
        {
            var c = baseColor; c.a = wireAlpha;
            Gizmos.color = c;
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        }

        Gizmos.color = prevC; Gizmos.matrix = prevM;
    }
}
