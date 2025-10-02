using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(BoxCollider))]
///<summary>
/// Class <c>AreaGizmo</c> draws a gizmo in the editor to visualize the area defined by a BoxCollider.
/// </summary>
/// <remarks>
/// Attach this script to a GameObject with a BoxCollider to see the gizmo in the editor.
/// Configure the appearance using the exposed fields in the Inspector.
/// </remarks>
public class AreaGizmo : MonoBehaviour
{
    [Header("Gizmo Settings")]
    [Tooltip("Show gizmo in play mode")]
    public bool showInPlayMode = true; // whether to show gizmo during play mode
    [Tooltip("Draw solid cube")]
    public bool drawSolid = true; // whether to draw solid cube
    [Tooltip("Draw wireframe cube")]
    public bool drawWire = true; // whether to draw wireframe cube

    [Header("Area Color Settings")]
    [Tooltip("Base color for gizmo (alpha ignored)")]
    public Color baseColor = new(0f, 1f, 0f, 1f); // base color (alpha ignored)

    [Range(0f, 1f)]
    [Tooltip("Alpha value for solid cube")]
    public float solidAlpha = 0.5f; // alpha for solid cube
    [Range(0f, 1f)]
    [Tooltip("Alpha value for wireframe cube")]
    public float wireAlpha = 1.0f; // alpha for wireframe cube

    /// <summary>
    /// Unity callback that draws gizmos in the Scene view.
    /// </summary>
    /// <remarks>
    /// Uses the attached <see cref="BoxCollider"/> to determine the gizmo's size,
    /// position, and orientation. Restores previous Gizmos state after drawing.
    /// </remarks>
    void OnDrawGizmos()
    {
        // Only draw if allowed in current mode
        if (Application.isPlaying && !showInPlayMode) return;

        var bc = GetComponent<BoxCollider>(); // get the BoxCollider

        // If no BoxCollider, nothing to draw
        if (!bc) return;

        // Compute the transformation matrix for the BoxCollider
        var m = Matrix4x4.TRS(
            bc.transform.TransformPoint(bc.center), // position
            bc.transform.rotation, // rotation
            Vector3.Scale(bc.transform.lossyScale, bc.size) // scale
        );

        // Save previous Gizmos state and set new matrix/color
        var prevM = Gizmos.matrix; var prevC = Gizmos.color;
        // Apply the BoxCollider's transform matrix
        Gizmos.matrix = m;

        // Draw solid and/or wireframe cube as configured
        if (drawSolid)
        {
            var c = baseColor; c.a = solidAlpha; // set alpha
            Gizmos.color = c; // set color
            Gizmos.DrawCube(Vector3.zero, Vector3.one); // draw unit
        }

        // Draw 
        if (drawWire)
        {
            var c = baseColor; c.a = wireAlpha;
            Gizmos.color = c;
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        }

        Gizmos.color = prevC; Gizmos.matrix = prevM;
    }
}
