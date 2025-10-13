using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Assets.Minijuegos.Scripts.FlappyBird
{
    public class GameManager : MonoBehaviour
    {
        private int score;
        public UIDocument uxml;
        private VisualElement mainUI;
        private VisualElement pauseMenu;
        private Label scoreText;
        private Button playButton;
        private Button backButton; // 🔹 Nuevo
        public PlayerController player;
        private bool died;
        private bool uiInitialized = false;

        private void Start()
        {
            Application.targetFrameRate = 60;
            InitializeUI();
        }

        private void InitializeUI()
        {
            if (uxml == null)
            {
                Debug.LogError("⚠️ UIDocument no asignado en FlappyBird GameManager");
                return;
            }

            if (uxml.rootVisualElement == null)
            {
                Debug.LogWarning("⚠️ rootVisualElement no está listo en FlappyBird, reintentando...");
                Invoke(nameof(InitializeUI), 0.1f);
                return;
            }

            try
            {
                mainUI = uxml.rootVisualElement.Q<VisualElement>("MainUI");
                pauseMenu = uxml.rootVisualElement.Q<VisualElement>("pause-menu");
                scoreText = mainUI?.Q<Label>("Score");
                playButton = pauseMenu?.Q<Button>("restart-button");
                backButton = pauseMenu?.Q<Button>("back-button"); // 🔹 Buscar botón regresar

                if (playButton != null)
                    playButton.clicked += Play;

                if (backButton != null)
                    backButton.clicked += ReturnToMainMenu; // 🔹 Asignar evento al botón

                uiInitialized = true;
                Debug.Log("✅ UI de FlappyBird inicializada correctamente");

                StartMenu();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"⚠️ Error al inicializar UI de FlappyBird: {e.Message}");
            }
        }

        public void Play()
        {
            if (!uiInitialized) return;

            if (died)
            {
                score = 0;
                scoreText.text = score.ToString();
                pauseMenu.style.display = DisplayStyle.None;

                Time.timeScale = 1;
                player.enabled = true;

                Pipes[] pipes = Object.FindObjectsByType<Pipes>(FindObjectsSortMode.None);
                foreach (Pipes pipe in pipes)
                    Destroy(pipe.gameObject);

                died = false;
            }
            else
            {
                pauseMenu.style.display = DisplayStyle.None;
                Time.timeScale = 1;
                player.enabled = true;
            }
        }

        public void StartMenu()
        {
            if (!uiInitialized) return;

            Time.timeScale = 0;
            pauseMenu.style.display = DisplayStyle.Flex;

            Label pauseLabel = pauseMenu.Q<Label>("pause-label");
            if (pauseLabel != null) pauseLabel.text = "";

            playButton.text = "Jugar";
            player.enabled = false;
        }

        public void Pause()
        {
            if (!uiInitialized) return;

            Time.timeScale = 0;
            pauseMenu.style.display = DisplayStyle.Flex;

            Label pauseLabel = pauseMenu.Q<Label>("pause-label");
            if (pauseLabel != null) pauseLabel.text = "";

            playButton.text = "Volver a intentar";
            player.enabled = false;
        }

        public void IncreaseScore()
        {
            score++;
            scoreText.text = score.ToString();
        }

        public void GameOver()
        {
            if (!uiInitialized) return;

            died = true;
            pauseMenu.style.display = DisplayStyle.Flex;

            var pauseLabel = pauseMenu.Q<Label>("pause-label");
            if (pauseLabel != null) pauseLabel.text = "Game Over";

            Time.timeScale = 0;
            player.enabled = false;
        }

        // 🔹 NUEVA FUNCIÓN: Regresar al menú de minijuegos
        private void ReturnToMainMenu()
        {
            Debug.Log("🚪 Regresando al menú de minijuegos...");

            // Descargar escena del minijuego
            SceneManager.LoadScene("PantallaPrincipal");

            // Reanudar el tiempo del juego principal
            Time.timeScale = 1;
        }

        private void OnDestroy()
        {
            if (playButton != null)
                playButton.clicked -= Play;
            if (backButton != null)
                backButton.clicked -= ReturnToMainMenu;
        }
    }
}
