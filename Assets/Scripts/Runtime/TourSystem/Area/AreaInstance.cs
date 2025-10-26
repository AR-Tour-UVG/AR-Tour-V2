using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider))]
public class AreaInstance : MonoBehaviour
{
    [SerializeField]
    private AreaDefinition definition;

    private BoxCollider _box;

    public AreaDefinition Definition => definition;

    public BoxCollider NavTarget => _box;

    public event System.Action<AreaInstance> Entered;

    public event System.Action<AreaInstance> Exited;

    private void Awake()
    {
        _box = GetComponent<BoxCollider>();
        _box.isTrigger = true;
    }

    internal void RaiseEntered() => Entered?.Invoke(this);

    internal void RaiseExited() => Exited?.Invoke(this);
}
