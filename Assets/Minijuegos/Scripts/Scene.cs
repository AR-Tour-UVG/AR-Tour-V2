using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Assets.Minijuegos.Scripts
{
    public class Scene : MonoBehaviour
    {
        public static Scene Instance { get; private set; }
        public string gameMenuName;
        public string menuName;
        
        [SerializeField]
        public GameObject[] disableObjects;
        
        private CambiadorDePantallas cambiador;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            
            // Buscar referencia al gestor de pantallas actual
            cambiador = FindFirstObjectByType<CambiadorDePantallas>();
        }
        
        /// <summary>
        /// Carga una escena de minijuego de forma aditiva,
        /// ocultando temporalmente la UI principal.
        /// </summary>
        public void LoadScene(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("⚠️ Nombre de escena no válido o vacío en LoadScene().");
                return;
            }
            
            // Validar que el array no sea null
            if (disableObjects == null || disableObjects.Length == 0)
            {
                Debug.LogWarning("⚠️ disableObjects está vacío o no configurado.");
            }
            else
            {
                foreach (GameObject obj in disableObjects)
                {
                    // Verificar que el objeto no sea null
                    if (obj == null)
                    {
                        Debug.LogWarning("⚠️ Se encontró un objeto null en disableObjects, saltando...");
                        continue;
                    }
                    
                    Debug.Log($"🔹 Revisando objeto: {obj.name}");
                    
                    UIDocument uiDocument = obj.GetComponent<UIDocument>();
                    if (uiDocument != null)
                    {
                        // Verificar que rootVisualElement esté inicializado
                        if (uiDocument.rootVisualElement != null)
                        {
                            uiDocument.rootVisualElement.style.display = DisplayStyle.None;
                            uiDocument.rootVisualElement.pickingMode = PickingMode.Ignore;
                            Debug.Log($"✅ UIDocument de {obj.name} ocultado correctamente");
                        }
                        else
                        {
                            Debug.LogWarning($"⚠️ rootVisualElement de {obj.name} es null, desactivando GameObject");
                            obj.SetActive(false);
                        }
                    }
                    else
                    {
                        obj.SetActive(false);
                        Debug.Log($"✅ GameObject {obj.name} desactivado");
                    }
                }
            }
            
            Debug.Log($"🎮 Cargando escena: {sceneName}");
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }
        
        /// <summary>
        /// Descarga la escena del minijuego y restaura la UI principal.
        /// </summary>
        public void UnloadScene(string sceneName)
        {
            Debug.Log($"🔄 Descargando escena: {sceneName}");
            
            // Restaurar la UI principal
            if (disableObjects != null && disableObjects.Length > 0)
            {
                foreach (GameObject obj in disableObjects)
                {
                    // Verificar que el objeto no sea null
                    if (obj == null)
                    {
                        continue;
                    }
                    
                    UIDocument uiDocument = obj.GetComponent<UIDocument>();
                    if (uiDocument != null)
                    {
                        // Verificar que rootVisualElement esté inicializado
                        if (uiDocument.rootVisualElement != null)
                        {
                            uiDocument.rootVisualElement.style.display = DisplayStyle.Flex;
                            uiDocument.rootVisualElement.pickingMode = PickingMode.Position;
                            Debug.Log($"✅ UIDocument de {obj.name} restaurado correctamente");
                        }
                        else
                        {
                            Debug.LogWarning($"⚠️ rootVisualElement de {obj.name} es null, activando GameObject");
                            obj.SetActive(true);
                        }
                    }
                    else
                    {
                        obj.SetActive(true);
                        Debug.Log($"✅ GameObject {obj.name} activado");
                    }
                }
            }
            
            // Descargar el minijuego
            SceneManager.UnloadSceneAsync(sceneName);
            
            // Mostrar pantalla de inicio si se cierra el juego
            if (cambiador != null)
            {
                cambiador.MostrarMinijuegos();
            }
        }
        
        /// <summary>
        /// Llamado cuando se regresa al menú principal.
        /// </summary>
        public void LoadMenu()
        {
            if (cambiador != null)
            {
                cambiador.MostrarInicio();
            }
            
            Destroy(gameObject);
        }
    }
}