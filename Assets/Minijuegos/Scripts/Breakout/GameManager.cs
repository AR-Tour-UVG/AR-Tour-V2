using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Assets.Minijuegos.Scripts
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI levelText;
        [SerializeField]
        private GameObject levelCanvas;
        [SerializeField]
        private GameObject gameAssets;
        [SerializeField]
        private GameObject[] liveIcons;
        [SerializeField]
        private LevelManager levelManager;
        [SerializeField]
        private BallController ballController;

        public UIDocument uxml;
        private VisualElement mainUI;
        private VisualElement pauseBackground;
        private Button restartButton;
        private bool lost;

        private int tiles;
        private GameObject tilePrefab;
        private int lives;

        private void Start()
        {
            lives = liveIcons.Length;
            lost = false;

            Time.timeScale = 1;
            gameAssets.SetActive(false);
            levelCanvas.SetActive(true);

            print("Generating level 1");
            Invoke("StartGameAssets", 1f);

            mainUI = uxml.rootVisualElement.Q<VisualElement>("pause-menu");
            pauseBackground = uxml.rootVisualElement.Q<VisualElement>("menu-background");
            restartButton = mainUI.Q<Button>("restart-button");

            mainUI.style.display = DisplayStyle.None;
            pauseBackground.style.display = DisplayStyle.None;
            restartButton.clicked += ResetGame;
        }

        private void Update()
        {
            tiles = GameObject.FindGameObjectsWithTag("Tile").Length;
        }

        private void StartGameAssets()
        {
            print("Starting game assets");
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

            Invoke("StartGameAssets", 1f);
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
            Time.timeScale = 1;

            if (lost)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            else
            {
                mainUI.style.display = DisplayStyle.None;
                pauseBackground.style.display = DisplayStyle.None;
            }
        }

        public void Pause()
        {
            Time.timeScale = 0;
            mainUI.style.display = DisplayStyle.Flex;
            pauseBackground.style.display = DisplayStyle.Flex;

            if (lost)
            {
                mainUI.Q<Label>("pause-label").text = "Game Over";
                restartButton.text = "Volver a intentar";
            }
            else
            {
                mainUI.Q<Label>("pause-label").text = "Pausa";
                restartButton.text = "Continuar";
            }
        }
    }
}