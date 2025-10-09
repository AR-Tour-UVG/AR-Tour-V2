using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Minijuegos.Scripts.Trivia
{
    public class SortButton : MonoBehaviour
    {
        [SerializeField]
        private UIDocument uxml;
        [SerializeField]
        private CategoriesManager categoriesManager;

        private void Awake()
        {
            VisualElement root = uxml.rootVisualElement;
            Button sortButton = root.Q<Button>("SortButton");

            if(sortButton != null)
            {
                sortButton.clicked += Sort;
            }
        }

        private void Sort()
        {
            categoriesManager.StartRouletteEffect();
        }
    }
}