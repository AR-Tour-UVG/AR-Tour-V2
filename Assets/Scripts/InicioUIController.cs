using UnityEngine;
using UnityEngine.UIElements;

public class InicioUIController : MonoBehaviour
{
    public CambiadorDePantallas cambiador;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        var botonExpress = root.Q<Button>("boton_ruta_express");

        if (botonExpress != null)
        {
            botonExpress.clicked += () =>
            {
                Debug.Log("Botón Ruta Express presionado");
                cambiador.MostrarEscaneo();
            };
        }
    }
}
