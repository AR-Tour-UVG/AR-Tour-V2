using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class OnBoardingUIController : MonoBehaviour
{
    public CambiadorDePantallas cambiador;
    private VisualElement contenedorPantalla;

    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        contenedorPantalla = root.Q<VisualElement>("ContenedorPantalla");

        // Aplicar animación de entrada
        IniciarAnimacionEntrada();

        // Botón siguiente
        Button botonSiguiente = root.Q<Button>("BotonSiguiente");
        if (botonSiguiente != null)
        {
            botonSiguiente.clicked += () =>
            {
                StartCoroutine(AnimacionSiguiente());
            };
        }

        // Botón iniciar (solo en último onboarding)
        Button botonIniciar = root.Q<Button>("BotonIniciar");
        if (botonIniciar != null)
        {
            botonIniciar.clicked += () =>
            {
                StartCoroutine(AnimacionIniciar());
            };
        }
    }

    void IniciarAnimacionEntrada()
    {
        if (contenedorPantalla != null)
        {
            // Empezar fuera de pantalla
            contenedorPantalla.AddToClassList("slide-enter");

            // Después de un frame, animar hacia la posición normal
            StartCoroutine(AnimarEntrada());
        }
    }

    IEnumerator AnimarEntrada()
    {
        yield return null; // Esperar un frame

        contenedorPantalla.RemoveFromClassList("slide-enter");
        contenedorPantalla.AddToClassList("slide-enter-active");
    }

    IEnumerator AnimacionSiguiente()
    {
        // Inmediatamente cambiar a la siguiente pantalla ANTES de animar
        cambiador.SiguienteOnboarding();

        // La animación de salida ya no es necesaria porque la nueva pantalla
        // se encarga de su propia animación de entrada
        yield return null;
    }

    IEnumerator AnimacionIniciar()
    {
        Button botonIniciar = GetComponent<UIDocument>().rootVisualElement.Q<Button>("BotonIniciar");

        if (botonIniciar != null)
        {
            // Aplicar fadeout al botón
            botonIniciar.AddToClassList("fadeout");

            // Esperar que termine la animación
            yield return new WaitForSeconds(0.8f);
        }

        // Guardar progreso y cambiar pantalla
        PlayerPrefs.SetInt("OnboardingCompletado", 1);
        cambiador.MostrarInicio();
    }
}