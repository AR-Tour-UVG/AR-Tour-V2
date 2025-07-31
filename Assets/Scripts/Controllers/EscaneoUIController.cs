using UnityEngine;
using UnityEngine.UIElements;

public class PantallaEscaneo : MonoBehaviour
{
    public CambiadorDePantallas cambiador;

    private Button botonSimular;

    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        botonSimular = root.Q<Button>("BotonSimularSensores");

        if (botonSimular != null)
        {
            // BYPASS TEMPORAL PARA SIMULAR CONEXIÓN DE 3 SENSORES
            // TODO: Reemplazar este bypass por la lógica real de conexión a sensores.
            // Cuando se detecten al menos 3 sensores, llamar a MostrarTour();
            // Ejemplo:
            // cambiador.MostrarTour():
            botonSimular.clicked += () =>
            {
                Debug.Log("[BYPASS] Simulando conexión de 3 sensores...");
                cambiador.MostrarTour();
            };
        }
    }
}
