using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Minijuegos.Scripts.FlappyBird
{
    /// <summary>
    /// Ver referencia de tutorial https://github.com/zigurous/unity-flappy-bird-tutorial?tab=readme-ov-file con el código original y los assets.
    /// Obtenido de https://www.youtube.com/watch?v=ihvBiJ1oC9U&list=WL
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        private int score;
        public UIDocument uxml;
        private VisualElement mainUI;
        private VisualElement pauseMenu;
        private Label scoreText;
        private Button playButton;
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
                
                if (playButton != null)
                {
                    playButton.clicked += Play;
                }
                
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
                if (scoreText != null)
                {
                    scoreText.text = score.ToString();
                }
                
                if (pauseMenu != null)
                {
                    pauseMenu.style.display = DisplayStyle.None;
                }
                
                Time.timeScale = 1;
                player.enabled = true;
                
                Pipes[] pipes = Object.FindObjectsByType<Pipes>(FindObjectsSortMode.None);
                foreach (Pipes pipe in pipes)
                {
                    Destroy(pipe.gameObject);
                }
                
                died = false;
            }
            else
            {
                if (pauseMenu != null)
                {
                    pauseMenu.style.display = DisplayStyle.None;
                }
                Time.timeScale = 1;
                player.enabled = true;
            }
        }
        
        public void StartMenu()
        {
            if (!uiInitialized) return;
            
            Time.timeScale = 0;
            
            if (pauseMenu != null)
            {
                pauseMenu.style.display = DisplayStyle.Flex;
                Label pauseLabel = pauseMenu.Q<Label>("pause-label");
                if (pauseLabel != null)
                {
                    pauseLabel.text = "";
                }
            }
            
            if (playButton != null)
            {
                playButton.text = "Jugar";
            }
            
            if (player != null)
            {
                player.enabled = false;
            }
        }
        
        public void Pause()
        {
            if (!uiInitialized) return;
            
            Time.timeScale = 0;
            
            if (pauseMenu != null)
            {
                pauseMenu.style.display = DisplayStyle.Flex;
                Label pauseLabel = pauseMenu.Q<Label>("pause-label");
                if (pauseLabel != null)
                {
                    pauseLabel.text = "";
                }
            }
            
            if (playButton != null)
            {
                playButton.text = "Volver a intentar";
            }
            
            if (player != null)
            {
                player.enabled = false;
            }
        }
        
        public void IncreaseScore()
        {
            score++;
            if (scoreText != null)
            {
                scoreText.text = score.ToString();
            }
        }
        
        public void GameOver()
        {
            if (!uiInitialized) return;
            
            died = true;
            
            if (pauseMenu != null)
            {
                pauseMenu.style.display = DisplayStyle.Flex;
                var pauseLabel = pauseMenu.Q<Label>("pause-label");
                if (pauseLabel != null)
                {
                    pauseLabel.text = "Game Over";
                }
            }
            
            Time.timeScale = 0;
            
            if (player != null)
            {
                player.enabled = false;
            }
        }
        
        private void OnDestroy()
        {
            if (playButton != null)
            {
                playButton.clicked -= Play;
            }
        }
    }
}