using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "NewArrowSceneDefinition", menuName = "AR Tour/Arrow Scene")]
public sealed class ArrowSceneDefinition : ScriptableObject
{
    [SerializeField, Tooltip("Path to the scene asset in the build (read-only).")]
    private string scenePath;

#if UNITY_EDITOR
    [SerializeField, Tooltip("Assign the scene asset; its path is stored into 'scenePath'.")]
    private SceneAsset sceneAsset;

    private void OnValidate()
    {
        if (sceneAsset == null)
            return;
        var path = AssetDatabase.GetAssetPath(sceneAsset);
        if (scenePath != path)
        {
            scenePath = path;
            EditorUtility.SetDirty(this);
        }
    }
#endif

    public string ScenePath => scenePath;
}
