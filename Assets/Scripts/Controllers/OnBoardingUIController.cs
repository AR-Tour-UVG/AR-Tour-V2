using UnityEngine;
using UnityEngine.UIElements;

public class OnBoardingUIController : MonoBehaviour
{
    public CambiadorDePantallas cambiador;

    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Botón siguiente
        Button botonSiguiente = root.Q<Button>("BotonSiguiente");
        if (botonSiguiente != null)
        {
            botonSiguiente.clicked += () =>
            {
                cambiador.SiguienteOnboarding();
            };
        }

        // Botón iniciar (solo en último onboarding)
        Button botonIniciar = root.Q<Button>("BotonIniciar");
        if (botonIniciar != null)
        {
            botonIniciar.clicked += () =>
            {
                PlayerPrefs.SetInt("OnboardingCompletado", 1);
                cambiador.MostrarInicio();
            };
        }
    }
}
