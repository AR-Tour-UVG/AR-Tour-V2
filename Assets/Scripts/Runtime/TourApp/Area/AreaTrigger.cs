using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(AreaInstance))]
public class AreaTrigger : MonoBehaviour
{
    private AreaInstance _area;

    private void Awake()
    {
        _area = GetComponent<AreaInstance>();
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        Debug.Log("[AreaTrigger] Player entered area: " + _area.Definition.name);
        _area.RaiseEntered();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        Debug.Log("[AreaTrigger] Player exited area: " + _area.Definition.name);
        _area.RaiseExited();
    }
}
