using UnityEngine;
using UnityEngine.UIElements;
using Assets.Minijuegos.Scripts;

public class MinijuegosUIController : MonoBehaviour
{
    public CambiadorDePantallas cambiador;
    public Scene sceneManager; // Referencia a Scene.cs que controla carga de minijuegos

    private VisualElement root;
    private Button botonBreakout;
    private Button botonFlappy;
    private Button botonTrivia;
    private Button botonVolver;

    private void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        botonBreakout = root.Q<Button>("boton_breakout");
        botonFlappy = root.Q<Button>("boton_flappy");
        botonTrivia = root.Q<Button>("boton_trivia");
        botonVolver = root.Q<Button>("boton_volver");

        // Botones de minijuegos
        if (botonBreakout != null)
        botonBreakout.clicked += () => {
            Debug.Log("🎮 Cargando Breakout...");
            sceneManager.LoadScene("Breakout");
        };


        if (botonFlappy != null)
            botonFlappy.clicked += () => {
                Debug.Log("🎮 Cargando FlappyBird...");
                sceneManager.LoadScene("FlappyBird");
            };

        if (botonTrivia != null)
            botonTrivia.clicked += () => {
                Debug.Log("🎮 Cargando Trivia...");
                sceneManager.LoadScene("Trivia");
            };

        if (botonVolver != null)
            botonVolver.clicked += () => cambiador.MostrarInicio();
    }
}
