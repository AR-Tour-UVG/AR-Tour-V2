using System.IO;
using UnityEngine;

public class JsonLoader : MonoBehaviour
{
    [Header("Anchor Map JSON")]
    [Tooltip("Filename of the JSON file containing the anchor map.")]
    public string jsonFile = "testRoom";

    void Awake()
    {
        var fileNoExt = Path.GetFileNameWithoutExtension(jsonFile);
        var resourcePath = $"AnchorMaps/{fileNoExt}";

        // Load the JSON file from Resources/AnchorMaps
        TextAsset jsonAsset = Resources.Load<TextAsset>(resourcePath);
        if (jsonAsset == null)
        {
            Debug.LogError($"JsonLoader: Failed to load JSON file: {jsonFile}");
            return;
        }

        // Initialize the plugin with the JSON content
        Debug.Log($"JsonLoader: Successfully Loaded anchor map '{jsonFile}'. Initializing UWBplugin...");
        UWBLocator.InitializeAnchorMap(jsonAsset.text);

    }
}