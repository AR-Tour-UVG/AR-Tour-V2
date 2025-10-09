using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Assets.Minijuegos.Scripts.Breakout
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private GameObject levelCanvas;
        [SerializeField] private GameObject gameAssets;
        [SerializeField] private GameObject[] liveIcons;
        [SerializeField] private LevelManager levelManager;
        [SerializeField] private BallController ballController;
        public UIDocument uxml;
        
        private VisualElement menuContainer;
        private VisualElement menuBackground;
        private VisualElement mainUI;
        private Button restartButton;
        private Button exitButton;
        private bool lost;
        private int tiles;
        private int lives;
        private bool uiInitialized = false;

        private void Awake()
        {
            // CRÍTICO: Ocultar el menú INMEDIATAMENTE antes de cualquier otra cosa
            if (uxml != null && uxml.rootVisualElement != null)
            {
                var menu = uxml.rootVisualElement.Q<VisualElement>("MenuContainer");
                var bg = uxml.rootVisualElement.Q<VisualElement>("menu-background");
                
                if (menu != null)
                {
                    menu.style.display = DisplayStyle.None;
                    Debug.Log("✅ Menú ocultado en Awake");
                }
                
                if (bg != null)
                {
                    bg.style.display = DisplayStyle.None;
                    Debug.Log("✅ Background ocultado en Awake");
                }
            }
        }

        private void Start()
        {
            lives = liveIcons.Length;
            lost = false;
            Time.timeScale = 1;
            
            // Si uxml no está asignado, intentar encontrarlo en la escena
            if (uxml == null)
            {
                Debug.LogWarning("⚠️ UIDocument no asignado, buscando en la escena...");
                uxml = FindFirstObjectByType<UIDocument>();
                
                if (uxml == null)
                {
                    Debug.LogError("❌ No se encontró ningún UIDocument en la escena!");
                }
                else
                {
                    Debug.Log($"✅ UIDocument encontrado: {uxml.gameObject.name}");
                }
            }
            
            // Ocultar assets del juego hasta que todo esté listo
            gameAssets.SetActive(false);
            levelCanvas.SetActive(true);
            
            Debug.Log("Generating level 1");
            
            // Inicializar UI con corrutina para esperar a que UIDocument esté listo
            StartCoroutine(WaitAndInitializeUI());
            
            // Arrancar juego después de un pequeño delay
            Invoke(nameof(StartGameAssets), 1f);
        }

        private System.Collections.IEnumerator WaitAndInitializeUI()
        {
            Debug.Log("🔄 Esperando a que UIDocument esté listo...");
            
            // Esperar hasta que UIDocument esté completamente inicializado
            int attempts = 0;
            while (uxml == null || uxml.rootVisualElement == null)
            {
                attempts++;
                if (attempts > 100) // Timeout después de ~3 segundos
                {
                    Debug.LogError("❌ UIDocument no se inicializó después de 100 frames");
                    yield break;
                }
                yield return null;
            }
            
            Debug.Log($"✅ UIDocument listo después de {attempts} frames");
            
            // Ahora sí inicializar
            InitializeUI();
        }

        private void InitializeUI()
        {
            Debug.Log("🎮 Iniciando InitializeUI...");
            
            if (uxml == null)
            {
                Debug.LogError("❌ uxml es null");
                return;
            }
            
            if (uxml.rootVisualElement == null)
            {
                Debug.LogError("❌ rootVisualElement es null");
                return;
            }

            Debug.Log("✅ UIDocument y rootVisualElement están OK");

            // Obtener contenedor completo del menú
            menuContainer = uxml.rootVisualElement.Q<VisualElement>("MenuContainer");
            menuBackground = uxml.rootVisualElement.Q<VisualElement>("menu-background");
            
            if (menuContainer == null)
            {
                Debug.LogError("❌ No se encontró MenuContainer");
                return;
            }

            Debug.Log("✅ MenuContainer encontrado");

            mainUI = menuContainer.Q<VisualElement>("ContainerPauseMenu");
            
            if (mainUI == null)
            {
                Debug.LogError("❌ No se encontró ContainerPauseMenu");
                return;
            }
            
            Debug.Log("✅ ContainerPauseMenu encontrado");
            
            restartButton = mainUI.Q<Button>("restart-button");
            exitButton = mainUI.Q<Button>("exit-button");

            // Asegurar que el menú y background están ocultos
            menuContainer.style.display = DisplayStyle.None;
            if (menuBackground != null)
                menuBackground.style.display = DisplayStyle.None;

            // Registrar eventos de botones
            if (restartButton != null)
            {
                restartButton.clicked += ResetGame;
                Debug.Log("✅ Botón Restart registrado");
            }
            else
            {
                Debug.LogError("❌ No se encontró restart-button");
            }

            if (exitButton != null)
            {
                exitButton.clicked += ExitGame;
                Debug.Log("✅ Botón Exit registrado");
            }
            else
            {
                Debug.LogError("❌ No se encontró exit-button");
            }

            uiInitialized = true;
            Debug.Log("✅✅✅ UI de Breakout inicializada correctamente - uiInitialized = TRUE");
        }

        private void ExitGame()
        {
            Debug.Log("🚪 Saliendo del minijuego");
            Time.timeScale = 1; // Restaurar tiempo antes de salir
            SceneManager.LoadScene("PantallaPrincipal");
        }

        private void Update()
        {
            tiles = GameObject.FindGameObjectsWithTag("Tile").Length;
        }

        private void StartGameAssets()
        {
            Debug.Log("Starting game assets");
            levelCanvas.SetActive(false);
            gameAssets.SetActive(true);
        }

        public void LoseLife()
        {
            lives--;
            liveIcons[lives].SetActive(false);
            
            if (lives <= 0)
            {
                lost = true;
                Pause();
            }
        }

        public void LevelUp()
        {
            levelManager.NextLevel();
            levelText.text = levelManager.GetCurrentLevel().ToString();
            gameAssets.SetActive(false);
            levelCanvas.SetActive(true);
            ballController.ResetBall();
            Invoke(nameof(StartGameAssets), 1f);
        }

        public bool UpdateTiles()
        {
            tiles -= 1;
            if (tiles <= 0)
            {
                LevelUp();
                return true;
            }
            return false;
        }

        public void ResetGame()
        {
            if (!uiInitialized)
            {
                Debug.LogWarning("⚠️ UI no inicializada");
                return;
            }

            Debug.Log("🔄 ResetGame llamado - Lost: " + lost);
            
            if (lost)
            {
                // Si perdió, reiniciar escena completamente
                Time.timeScale = 1;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            else
            {
                // Si solo está pausado, continuar
                Time.timeScale = 1;
                if (menuContainer != null)
                    menuContainer.style.display = DisplayStyle.None;
                if (menuBackground != null)
                    menuBackground.style.display = DisplayStyle.None;
                    
                Debug.Log("✅ Menú ocultado - Juego continuado");
            }
        }

        public void Pause()
        {
            Debug.Log($"🔍 Pause llamado - uiInitialized: {uiInitialized}");
            
            if (!uiInitialized)
            {
                Debug.LogWarning("⚠️ UI no inicializada, no se puede pausar");
                Debug.LogWarning($"   uxml: {(uxml == null ? "NULL" : "OK")}");
                Debug.LogWarning($"   rootVisualElement: {(uxml?.rootVisualElement == null ? "NULL" : "OK")}");
                Debug.LogWarning($"   menuContainer: {(menuContainer == null ? "NULL" : "OK")}");
                return;
            }

            Debug.Log("⏸️ Pause ejecutándose - Lost: " + lost);
            
            Time.timeScale = 0;
            
            if (menuContainer != null)
                menuContainer.style.display = DisplayStyle.Flex;
            if (menuBackground != null)
                menuBackground.style.display = DisplayStyle.Flex;

            Label pauseLabel = mainUI?.Q<Label>("pause-label");
            
            if (lost)
            {
                if (pauseLabel != null)
                    pauseLabel.text = "Game Over";
                if (restartButton != null)
                    restartButton.text = "Volver a intentar";
            }
            else
            {
                if (pauseLabel != null)
                    pauseLabel.text = "Pausa";
                if (restartButton != null)
                    restartButton.text = "Continuar";
            }
            
            Debug.Log("✅ Menú de pausa mostrado correctamente");
        }

        private void OnDestroy()
        {
            // Limpiar eventos
            if (restartButton != null)
                restartButton.clicked -= ResetGame;
            if (exitButton != null)
                exitButton.clicked -= ExitGame;
        }
    }
}