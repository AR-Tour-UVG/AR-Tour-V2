using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(900)]
public class TourUIBinder : MonoBehaviour
{
    private FloorManager fm;
    private ARTourUIController ui;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Unhook();
    }

    private void OnSceneLoaded(Scene _, LoadSceneMode __)
    {
        Rehook();
    }

    private void Rehook()
    {
        Unhook();

        // Find current floor + UI in the active scene(s)
        fm = FindFirstObjectByType<FloorManager>(FindObjectsInactive.Include);
        ui = FindFirstObjectByType<ARTourUIController>(FindObjectsInactive.Include);

        if (!fm || !ui) return;

        fm.AreaConfirmed += OnAreaConfirmed;
        fm.GuidingToNext += OnGuidingToNext;
    }

    private void Unhook()
    {
        if (fm != null)
        {
            fm.AreaConfirmed -= OnAreaConfirmed;
            fm.GuidingToNext -= OnGuidingToNext;
            fm = null;
        }
    }

    private void OnAreaConfirmed(AreaDefinition area)
    {
        // Header shows where we are. Footer shows POI content.
        ui.ActualizarUbicacion(area?.AreaName ?? "");
        ui.ShowAreaPOI(area);
    }

    private void OnGuidingToNext(AreaDefinition next)
    {
        if (next == null) return;
        // Header shows the direction to next POI.
        ui.ActualizarInstruccion("Siguiente", next.AreaName);
    }
}
