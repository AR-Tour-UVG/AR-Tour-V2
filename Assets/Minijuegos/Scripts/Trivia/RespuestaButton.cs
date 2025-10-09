using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Minijuegos.Scripts.Trivia
{
    public class RespuestaButton
    {
        [SerializeField]
        private GameManager gameManager;

        private int index;
        public Button button;

        public void Initialize(VisualElement root, int index)
        {
            this.index = index + 1;
            string nombre = "respuesta-" + this.index;
            button = root.Q<Button>(nombre);

            if (button != null)
            {
                button.clicked += Revisar;
            }

            gameManager = Object.FindFirstObjectByType<GameManager>();
        }

        public void Revisar()
        {
            gameManager.Respuesta(index, button);
        }

        public void SetText(string text)
        {
            if (button != null)
            {
                button.text = text;
            }
        }
    }
}