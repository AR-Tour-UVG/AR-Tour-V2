using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Minijuegos.Scripts.FlappyBird
{
    /// <summary>
    /// Ver referencia de tutorial https://github.com/zigurous/unity-flappy-bird-tutorial?tab=readme-ov-file con el c�digo original y los assets.
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


        private void Start()
        {
            Application.targetFrameRate = 60;

            mainUI = uxml.rootVisualElement.Q<VisualElement>("MainUI");
            pauseMenu = uxml.rootVisualElement.Q<VisualElement>("pause-menu");

            scoreText = mainUI.Q<Label>("Score");
            playButton = pauseMenu.Q<Button>("restart-button");

            StartMenu();

            playButton.clicked += Play;
        }

        public void Play()
        {
            if(died)
            {
                score = 0;
                scoreText.text = score.ToString();

                pauseMenu.style.display = DisplayStyle.None;

                Time.timeScale = 1;
                player.enabled = true;

                Pipes[] pipes = Object.FindFirstObjectByType<Pipes>().GetComponentsInChildren<Pipes>();
                foreach (Pipes pipe in pipes)
                {
                    Destroy(pipe.gameObject);
                }
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
            Time.timeScale = 0;

            pauseMenu.style.display = DisplayStyle.Flex;
            pauseMenu.Q<Label>("pause-label").text = "";
            playButton.text = "Jugar";

            player.enabled = false;
        }

        public void Pause() 
        { 
            Time.timeScale = 0;

            pauseMenu.style.display = DisplayStyle.Flex;
            pauseMenu.Q<Label>("pause-label").text = "";
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
            died = true;
            pauseMenu.style.display = DisplayStyle.Flex;
            pauseMenu.Q<Label>("pause-label").text = "Game Over";
            Time.timeScale = 0;
            player.enabled = false;
        }
    }
}