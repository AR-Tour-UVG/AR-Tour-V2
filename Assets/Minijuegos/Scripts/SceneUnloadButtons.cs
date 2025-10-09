using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Minijuegos.Scripts
{
    public class SceneUnloadButtons : MonoBehaviour
    {
        [SerializeField]
        private UIDocument uxml;
        [SerializeField]
        private Scene scene;
        [SerializeField]
        private string menuName;

        [SerializeField]
        private string[] sceneNames;
        [SerializeField]
        private string[] buttonNames;

        //void Start()
        //{
        //    VisualElement mainMenu = uxml.rootVisualElement.Q<VisualElement>(menuName);
        //    scene = Scene.Instance;

        //    for (int i = 0; i < sceneNames.Length; i++)
        //    {
        //        Button button = mainMenu.Q<Button>(buttonNames[i]);
        //        int index = i;
        //        button.clicked += () => LoadScene(sceneNames[index]);
        //    }
        //}

        public void LoadScene(string sceneName)
        {
            Scene.Instance.UnloadScene(sceneName);
        }

        void OnEnable()
        {
            print("enabling buttons");
            VisualElement mainMenu = uxml.rootVisualElement.Q<VisualElement>(menuName);

            for (int i = 0; i < sceneNames.Length; i++)
            {
                Button button = mainMenu.Q<Button>(buttonNames[i]);
                int index = i;

                // Asegúrate de limpiar eventos anteriores para evitar múltiples asignaciones
                button.clicked -= null;
                button.clicked += () => LoadScene(sceneNames[index]);
            }
        }
    }
}