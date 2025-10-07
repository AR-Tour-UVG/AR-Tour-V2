using UnityEngine;

// Execute in edit mode
[ExecuteAlways]
// Require a BoxCollider component
[RequireComponent(typeof(BoxCollider))]
///<summary>
/// Class <c>AreaGizmo</c> draws a gizmo in the editor to visualize the area defined by a BoxCollider.
/// </summary>
/// <remarks>
/// - Attach this script to a GameObject with a BoxCollider to see the gizmo in the editor.
/// - Configure the appearance using the exposed fields in the Inspector.
/// </remarks>
public class AreaGizmo : MonoBehaviour
{
    [Header("Gizmo Settings")]
    [Tooltip("Show gizmo in play mode")]
    /// <summary>Whether to show the gizmo during play mode.</summary>
    public bool showInPlayMode = true;

    [Tooltip("Draw solid cube")]
    /// <summary>Whether to draw a solid cube gizmo.</summary>
    public bool drawSolid = true;
    
    [Tooltip("Draw wireframe cube")]
    /// <summary>Whether to draw a wireframe cube gizmo.</summary>
    public bool drawWire = true;

    [Header("Area Color Settings")]
    [Tooltip("Base color for gizmo (alpha ignored)")]
    /// <summary>Base color for the gizmo (alpha ignored).</summary>
    public Color baseColor = new(0f, 1f, 0f, 1f);

    [Range(0f, 1f)]
    [Tooltip("Alpha value for solid cube")]
    /// <summary>Alpha value for the solid cube.</summary>
    public float solidAlpha = 0.5f;
    
    [Range(0f, 1f)]
    [Tooltip("Alpha value for wireframe cube")]
    /// <summary>Alpha value for the wireframe cube.</summary>
    public float wireAlpha = 1.0f;

    /// <summary>
    /// Unity callback that draws gizmos in the Scene view.
    /// </summary>
    /// <remarks>
    /// - Uses the attached BoxCollider to determine the gizmo's size,
    /// position, and orientation. 
    /// - Restores previous Gizmos state after drawing.  
    /// For Unity OnDrawGizmos reference, see <see href="https://docs.unity3d.com/6000.0/Documentation/ScriptReference/MonoBehaviour.OnDrawGizmos.html">OnDrawGizmos</see>.
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

        // Draw wireframe cube over solid if both enabled
        if (drawWire)
        {
            var c = baseColor; c.a = wireAlpha; // set alpha
            Gizmos.color = c; // set color
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one); // draw unit
        }

        Gizmos.color = prevC; Gizmos.matrix = prevM; // restore previous state
    }
}
