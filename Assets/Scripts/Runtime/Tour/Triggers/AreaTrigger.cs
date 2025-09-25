using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class AreaTrigger : MonoBehaviour
{
    [SerializeField] private AreaDefinition areaDef;

    private void Reset()
    {
        // Auto-configure collider as trigger
        var col = GetComponent<BoxCollider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Optional: only react to player (tagged "Player")
        if (!other.CompareTag("Player"))
            return;

        Debug.Log($"Entered {areaDef.AreaName}");

        if (areaDef.AreaText != null)
            Debug.Log($"Text: {areaDef.AreaText.text}");

        for (int i = 0; i < areaDef.AudioClips.Count; i++)
            Debug.Log($"Audio {i}: {areaDef.AudioClips[i].name}");
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        Debug.Log($"Exited {areaDef.AreaName}");
    }
}
