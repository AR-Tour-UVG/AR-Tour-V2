using UnityEngine;
using UnityEngine.UIElements;

public class InicioUIController : MonoBehaviour
{
    public CambiadorDePantallas cambiador;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        var botonExpress = root.Q<Button>("boton_ruta_express");
        var botonCompleta = root.Q<Button>("boton_ruta_completa");

        if (botonExpress != null)
        {
            botonExpress.clicked += () =>
            {
                EstadoRuta.TipoRuta = "Ruta Express";
                cambiador.MostrarEscaneo();
            };
        }

        if (botonCompleta != null)
        {
            botonCompleta.clicked += () =>
            {
                EstadoRuta.TipoRuta = "Ruta Completa";
                cambiador.MostrarEscaneo();
            };
        }
    }
}