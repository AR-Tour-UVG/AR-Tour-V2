using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(AreaInstance))]
/// <summary>
/// Class <c>AreaTrigger</c> detects player entry and exit events for an <c>AreaInstance</c>.
/// </summary>
/// <remarks>
/// - Requires both a BoxCollider and an AreaInstance on the same GameObject.
/// - Raises events when the player enters or exits the collider trigger.
/// </remarks>
public class AreaTrigger : MonoBehaviour
{
    private AreaInstance _area;

    /// <summary>
    /// Unity lifecycle method called when the script instance is being loaded.
    /// </summary>
    /// <remarks>
    /// Initializes the reference to the <c>AreaInstance</c> and ensures the BoxCollider is set as a trigger.
    /// </remarks>
    private void Awake()
    {
        // Get the AreaInstance component
        _area = GetComponent<AreaInstance>();
        // Ensure the BoxCollider is set as a trigger
        GetComponent<BoxCollider>().isTrigger = true;
    }

    /// <summary>
    /// Unity callback invoked when another collider enters this trigger.
    /// </summary>
    /// <param name="other">The collider that entered the trigger.</param>
    /// <remarks>
    /// Checks for the "Player" tag. Logs entry and raises the <c>Entered</c> event.
    /// </remarks>
    private void OnTriggerEnter(Collider other)
    {
        // Only respond to objects tagged as "Player"
        if (!other.CompareTag("Player"))
            return;
        // Log entry and raise the Entered event
        Debug.Log("[AreaTrigger] Player entered area: " + _area.Definition.name);
        // Raise the Entered event
        _area.RaiseEntered();
    }

    /// <summary>
    /// Unity callback invoked when another collider exits this trigger.
    /// </summary>
    /// <param name="other">The collider that exited the trigger.</param>
    /// <remarks>
    /// Checks for the "Player" tag. Logs exit and raises the <c>Exited</c> event.
    /// </remarks>
    private void OnTriggerExit(Collider other)
    {
        // Only respond to objects tagged as "Player"
        if (!other.CompareTag("Player"))
            return;
        // Log exit and raise the Exited event
        Debug.Log("[AreaTrigger] Player exited area: " + _area.Definition.name);
        // Raise the Exited event
        _area.RaiseExited();
    }
}
