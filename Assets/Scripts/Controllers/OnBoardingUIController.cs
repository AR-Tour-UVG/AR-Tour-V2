using UnityEngine;
using UnityEngine.UIElements;

public class OnboardingController : MonoBehaviour
{
    public UIDocument uiDocument;
    public CambiadorDePantallas cambiador;
    private ScrollView scrollView;
    private Button btnNext;
    private Button btnPrev;
    private int currentSlide = 0;
    private int totalSlides = 4;

    private void OnEnable()
    {
        var root = uiDocument.rootVisualElement;

        scrollView = root.Q<ScrollView>("ScrollOnboarding");

        btnNext = new Button() { text = "Siguiente" };
        btnPrev = new Button() { text = "Anterior" };

        btnPrev.SetEnabled(false);

        var buttonContainer = new VisualElement();
        buttonContainer.style.flexDirection = FlexDirection.Row;
        buttonContainer.style.justifyContent = Justify.SpaceBetween;
        buttonContainer.Add(btnPrev);
        buttonContainer.Add(btnNext);

        root.Add(buttonContainer);

        btnNext.clicked += NextSlide;
        btnPrev.clicked += PrevSlide;
    }

    private void NextSlide()
    {
        if (currentSlide < totalSlides - 1)
        {
            currentSlide++;
            SnapToSlide();
        }
        else
        {
            Debug.Log("Onboarding finalizado");
            cambiador.MostrarInicio();
        }
    }

    private void PrevSlide()
    {
        if (currentSlide > 0)
        {
            currentSlide--;
            SnapToSlide();
        }
    }

    private void SnapToSlide()
    {
        float target = currentSlide * scrollView.contentContainer.layout.width;
        scrollView.scrollOffset = new Vector2(target, 0);

        btnPrev.SetEnabled(currentSlide > 0);
        btnNext.text = (currentSlide == totalSlides - 1) ? "Continuar" : "Siguiente";
    }
}
