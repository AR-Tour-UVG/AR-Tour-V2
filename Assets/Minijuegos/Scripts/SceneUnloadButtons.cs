using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

namespace Assets.Minijuegos.Scripts
{
    public class SceneUnloadButtons : MonoBehaviour
    {
        [SerializeField]
        private UIDocument uiDocument;
        
        private Button exitButton;
        private Scene sceneManager;
        
    private void OnEnable()
    {
        StartCoroutine(WaitForUIDocument());
    }

    private System.Collections.IEnumerator WaitForUIDocument()
    {
        // Esperar a que el UIDocument exista en el GameObject
        int tries = 0;
        while (uiDocument == null && tries < 10)
        {
            uiDocument = GetComponent<UIDocument>();
            tries++;
            yield return new WaitForSeconds(0.1f);
        }

        if (uiDocument == null)
        {
            Debug.LogError("⚠️ UIDocument sigue sin encontrarse tras varios intentos.");
            yield break;
        }

        // Esperar a que el rootVisualElement esté disponible
        while (uiDocument.rootVisualElement == null)
            yield return null;

        InitializeUI();
    }

        private void InitializeUI()
        {
            if (uiDocument == null || uiDocument.rootVisualElement == null)
            {
                Debug.LogError("⚠️ No se pudo inicializar UI en SceneUnloadButtons");
                return;
            }
            
            var root = uiDocument.rootVisualElement;
            exitButton = root.Q<Button>("exit-button");
            
            if (exitButton != null)
            {
                exitButton.clicked += OnExitButtonClicked;
                Debug.Log("✅ Botón de salida inicializado correctamente");
            }
            else
            {
                Debug.LogWarning("⚠️ No se encontró el botón 'exit-button'");
            }
            
            sceneManager = Scene.Instance;
            if (sceneManager == null)
            {
                Debug.LogWarning("⚠️ Scene Manager no encontrado");
            }
        }
        
        private void OnExitButtonClicked()
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            Debug.Log($"🚪 Saliendo del minijuego: {currentSceneName}");
            
            if (sceneManager != null)
            {
                sceneManager.UnloadScene(currentSceneName);
            }
            else
            {
                Debug.LogError("⚠️ Scene Manager no disponible para descargar escena");
            }
        }
        
        private void OnDisable()
        {
            if (exitButton != null)
            {
                exitButton.clicked -= OnExitButtonClicked;
            }
        }
    }
}