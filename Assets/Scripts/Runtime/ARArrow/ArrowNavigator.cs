using UnityEngine;
using UnityEngine.AI;

public class ArrowNavigator : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private PathProvider pathProvider; // Assign in Inspector

    [SerializeField]
    private Transform arrowModel; // Assign your arrow mesh

    private NavMeshPath currentPath;

    private void OnEnable()
    {
        if (pathProvider != null)
            pathProvider.OnPathUpdated += HandlePathUpdated;
    }

    private void OnDisable()
    {
        if (pathProvider != null)
            pathProvider.OnPathUpdated -= HandlePathUpdated;
    }

    private void HandlePathUpdated(NavMeshPath path)
    {
        currentPath = path;
    }

    private void Update()
    {
        if (currentPath == null || currentPath.corners.Length < 2)
            return;

        Vector3 playerPos = currentPath.corners[0]; // "this" should be the player/phone
        Vector3 nextCorner = currentPath.corners[1]; // corner[0] is current pos

        Vector3 dir = nextCorner - playerPos;
        dir.y = 0; // keep arrow flat on the ground plane

        if (dir.sqrMagnitude > 0.01f)
        {
            // Calculate angle in degrees
            float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg; // note: X,Z plane

            // Apply rotation around Z axis
            arrowModel.localRotation = Quaternion.Euler(-90f, 0f, angle + 90f); // negative if needed to match your arrow model
        }
    }
}
