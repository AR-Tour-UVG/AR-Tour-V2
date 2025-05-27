using UnityEngine;
using UnityEngine.UIElements;

public class ARTourUIController : MonoBehaviour
{
    public CambiadorDePantallas cambiador;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        var botonSalir = root.Q<Button>("boton_salir");

        if (botonSalir != null)
        {
            botonSalir.clicked += () =>
            {
                Debug.Log("Clic en cerrar - Regresando a inicio");
                cambiador.MostrarInicio();
            };
        }
    }
}
