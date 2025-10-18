using UnityEngine;

// Disallow multiple AreaInstance components on a single GameObject
[DisallowMultipleComponent]
// Require a BoxCollider component for area detection and pathfinding
[RequireComponent(typeof(BoxCollider))]
/// <summary>
/// Class <c>AreaInstance</c> represents a runtime instance of an <see cref="AreaDefinition"/> in the scene.
/// </summary>
/// <remarks>
/// - Each instance uses a BoxCollider as its trigger area.
/// - Events are raised when something enters or exits the area.
/// </remarks>
public class AreaInstance : MonoBehaviour
{
    /// <summary>The area definition that this instance represents.</summary>
    [SerializeField]
    private AreaDefinition definition;

    /// <summary>The BoxCollider component used for area detection.</summary>
    private BoxCollider _box;

    /// <summary>Gets the area definition associated with this instance.</summary>
    public AreaDefinition Definition => definition;

    /// <summary>Gets the BoxCollider used for navigation targeting.</summary>
    public BoxCollider NavTarget => _box;

    /// <summary>Event triggered when something enters the area.</summary>
    public event System.Action<AreaInstance> Entered;

    /// <summary>Event triggered when something exits the area.</summary>
    public event System.Action<AreaInstance> Exited;

    /// <summary>
    /// Unity lifecycle method called when the script instance is being loaded.
    /// </summary>
    /// <remarks>
    /// Initializes the attached BoxCollider and sets it as a trigger.
    /// </remarks>
    private void Awake()
    {
        _box = GetComponent<BoxCollider>(); // Get the BoxCollider component
        _box.isTrigger = true; // Set area as trigger
    }

    // Methods to raise the Entered and Exited events
    /// <summary>
    /// Raises the Entered event to notify subscribers that something has entered the area.
    /// </summary>
    internal void RaiseEntered() => Entered?.Invoke(this);

    /// <summary>
    /// Raises the Exited event to notify subscribers that something has exited the area.
    /// </summary>
    internal void RaiseExited() => Exited?.Invoke(this);
}
