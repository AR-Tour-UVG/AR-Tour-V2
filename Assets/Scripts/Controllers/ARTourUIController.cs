using UnityEngine;
using UnityEngine.UIElements;

public class ARTourUIController : MonoBehaviour
{
    public CambiadorDePantallas cambiador;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Botón salir
        var botonSalir = root.Q<Button>("boton_salir");
        if (botonSalir != null)
        {
            botonSalir.clicked += () =>
            {
                Debug.Log("Clic en cerrar - Regresando a inicio");
                cambiador.MostrarInicio();
            };
        }

        // Texto dinámico de tipo de ruta (Ruta Express o Ruta Completa)
        var textoRuta = root.Q<Label>("TextoRuta");
        if (textoRuta != null)
        {
            textoRuta.text = EstadoRuta.TipoRuta;
        }
    }
}
