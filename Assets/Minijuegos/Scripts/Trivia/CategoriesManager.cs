using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Minijuegos.Scripts.Trivia
{
    public class CategoriesManager : MonoBehaviour
    {

        [SerializeField]
        private VisualTreeAsset categoryTemplate;

        [SerializeField]
        private GameManager gameManager;

        [SerializeField]
        private UIDocument uxml;
        private VisualElement root;

        private List<Categoria> categorias = new();
        private bool isRouletteActive = false;

        private List<Button> categoryButtons = new();
        private Dictionary<Button, StyleColor> originalStyles = new();

        public Categoria currentCategory;

        private void Start()
        {
            root = uxml.rootVisualElement;
        }

        public void GenerateButtons(List<Categoria> categories)
        {
            var container = root.Q<VisualElement>("categories-container");

            foreach (var category in categories)
            {
                var categoryButton = categoryTemplate.Instantiate().Q<Button>();

                categoryButton.text = category.nombre.ToUpper();
                ColorUtility.TryParseHtmlString("#ECEAF5", out Color color);
                container.hierarchy.Add(categoryButton);

                category.nombre = category.nombre.ToLower();
                categorias.Add(category);

                categoryButtons.Add(categoryButton);
                originalStyles[categoryButton] = color;
            }
        }

        public void StartRouletteEffect()
        {
            Time.timeScale = 1;
            if (isRouletteActive)
            {
                return;
            }

            StartCoroutine(RouletteEffect());
        }

        public void ResetCategoryButtons()
        {
            foreach (var button in categoryButtons)
            {
                button.style.backgroundColor = originalStyles[button];
            }
        }

        private IEnumerator RouletteEffect()
        {
            int index = 0;
            float delay = 0.2f;
            float acceleration = 0.95f;
            float deceleration = 1.05f;
            float minDelay = 0.05f;
            int rounds = 3;
            int steps = rounds * categoryButtons.Count;
            isRouletteActive = true;

            for (int i = 0; i < steps; i++)
            {
                HighlightButton(index);

                yield return new WaitForSeconds(delay);

                UnhighlightButton(index);

                if (delay > minDelay)
                {
                    delay *= acceleration;
                }

                index = (index + 1) % categoryButtons.Count;
            }

            while (delay < 0.5f)
            {
                HighlightButton(index);

                yield return new WaitForSeconds(delay);

                UnhighlightButton(index);

                delay *= deceleration;
                index = (index + 1) % categoryButtons.Count;
            }

            index = Random.Range(0, categoryButtons.Count);
            HighlightButton(index);

            currentCategory = categorias.Find(c => c.nombre == categoryButtons[index].text.ToLower());

            gameManager.StartCategory(currentCategory);
            
            isRouletteActive = false;
        }

        private void HighlightButton(int index)
        {
            categoryButtons[index].style.backgroundColor = new Color(0.82f, 0.98f, 0.65f);
        }

        private void UnhighlightButton(int index)
        {
            var button = categoryButtons[index];
            if (originalStyles.TryGetValue(button, out var styles))
            {
                button.style.backgroundColor = styles.value;
            }
        }

    }
}