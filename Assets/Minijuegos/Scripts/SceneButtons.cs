using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Assets.Minijuegos.Scripts
{
    public class SceneButtons : MonoBehaviour
    {
        [SerializeField]
        private UIDocument uxml;
        [SerializeField]
        private Scene scene;
        [SerializeField]
        private string menuName;

        public string mainMenuName;
        public string gameMenuName;

        [SerializeField]
        private string[] sceneNames;
        [SerializeField]
        private string[] buttonNames;

        private Dictionary<Button, Action> buttonActions = new Dictionary<Button, Action>();


        //void Start()
        //{
        //    scene = Scene.Instance;
        //    VisualElement mainMenu = uxml.rootVisualElement.Q<VisualElement>(menuName);

        //    for (int i = 0; i < sceneNames.Length; i++)
        //    {
        //        Button button = mainMenu.Q<Button>(buttonNames[i]);
        //        int index = i;
        //        button.clicked += () => LoadScene(sceneNames[index]);
        //    }
        //}

        public void LoadScene(string sceneName)
        {
            if (SceneManager.GetSceneByName(sceneName).isLoaded)
            {
                return;
            }

            if (sceneName == mainMenuName && SceneManager.GetActiveScene().name == gameMenuName)
            {
                scene.LoadMenu();
                SceneManager.LoadScene(sceneName);
                return;
            }

            Scene.Instance.LoadScene(sceneName);
        }

        void OnEnable()
        {
            VisualElement mainMenu = uxml.rootVisualElement.Q<VisualElement>(menuName);

            for (int i = 0; i < sceneNames.Length; i++)
            {
                Button button = mainMenu.Q<Button>(buttonNames[i]);
                int index = i;

                if (buttonActions.ContainsKey(button))
                {
                    button.clicked -= buttonActions[button];
                }

                Action action = () => LoadScene(sceneNames[index]);
                buttonActions[button] = action;

                // Suscribir el evento
                button.clicked += action;
            }
        }
    }
}