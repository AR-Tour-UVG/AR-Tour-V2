using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Collections;

public class InicioUIController : MonoBehaviour
{
    public CambiadorDePantallas cambiador;

    private VisualElement contenedorPrincipal;
    private VisualElement contenedorBienvenida;
    private VisualElement logoARTour;
    private Label textoBienvenida;
    private Label textoARTour;
    private VisualElement contenedorRutaExpress;
    private VisualElement contenedorRutaCompleta;
    private VisualElement contenedorMinijuegos;
    private VisualElement logoUvg;
    private VisualElement backgroundCIT;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Obtener referencias a elementos
        ObtenerReferencias(root);

        // Configurar botones
        ConfigurarBotones(root);

        // Iniciar secuencia de animaciones
        StartCoroutine(SecuenciaAnimacionEntrada());
    }

    void ObtenerReferencias(VisualElement root)
    {
        contenedorPrincipal = root.Q<VisualElement>("ContenedorPincipal");
        contenedorBienvenida = root.Q<VisualElement>("ContenedorBienvenida");
        logoARTour = root.Q<VisualElement>("LogoARTour");
        textoBienvenida = root.Q<Label>("TextoBienvenida");
        textoARTour = root.Q<Label>("TextoARTour");
        contenedorRutaExpress = root.Q<VisualElement>("ContenedorRutaExpress");
        contenedorRutaCompleta = root.Q<VisualElement>("ContenedorRutaCompleta");
        contenedorMinijuegos = root.Q<VisualElement>("ContenedorMinijuegos");
        logoUvg = root.Q<VisualElement>("LogoUvg");
        backgroundCIT = root.Q<VisualElement>("BackgroundCIT");
    }

    void ConfigurarBotones(VisualElement root)
    {
        var botonExpress = root.Q<Button>("boton_ruta_express");
        var botonCompleta = root.Q<Button>("boton_ruta_completa");
        var botonMinijuegos = root.Q<Button>("boton_minijuegos");

        if (botonExpress != null)
        {
            botonExpress.clicked += () =>
            {
                //StartCoroutine(AnimacionSalidaYCambio("Ruta Express"));
                StartCoroutine(LoadLogicThenShowEscaneo("Ruta Express"));
            };
        }

        if (botonCompleta != null)
        {
            botonCompleta.clicked += () =>
            {
                StartCoroutine(AnimacionSalidaYCambio("Ruta Completa"));
            };
        }

        if (botonMinijuegos != null)
        {
            botonMinijuegos.clicked += () =>
            {
                // Aqu� puedes agregar navegaci�n a minijuegos
                Debug.Log("Navegando a minijuegos...");
            };
        }
    }

    IEnumerator SecuenciaAnimacionEntrada()
    {
        // Preparar elementos para animaci�n (estados iniciales)
        PrepararElementosParaAnimacion();

        yield return new WaitForSeconds(0.1f);

        // 1. Fade in del contenedor principal
        if (contenedorPrincipal != null)
        {
            contenedorPrincipal.RemoveFromClassList("fade-in");
            contenedorPrincipal.AddToClassList("fade-in-active");
        }

        yield return new WaitForSeconds(0.2f);

        // 2. Slide del contenedor bienvenida desde arriba
        if (contenedorBienvenida != null)
        {
            contenedorBienvenida.RemoveFromClassList("slide-from-top");
            contenedorBienvenida.AddToClassList("slide-from-top-active");
        }

        yield return new WaitForSeconds(0.1f);

        // 3. Activar flotaci�n del logo
        if (logoARTour != null)
        {
            logoARTour.AddToClassList("floating-logo");
        }

        yield return new WaitForSeconds(0.2f);

        // 4. Textos de bienvenida
        if (textoBienvenida != null)
        {
            textoBienvenida.RemoveFromClassList("text-slide-up");
            textoBienvenida.AddToClassList("text-slide-up-active");
        }

        yield return new WaitForSeconds(0.1f);

        if (textoARTour != null)
        {
            textoARTour.RemoveFromClassList("text-slide-up");
            textoARTour.AddToClassList("text-slide-up-active");
        }

        yield return new WaitForSeconds(0.1f);

        // 5. Botones de ruta (escalonados)
        if (contenedorRutaExpress != null)
        {
            contenedorRutaExpress.RemoveFromClassList("button-slide-in");
            contenedorRutaExpress.AddToClassList("button-slide-in-active");
        }

        yield return new WaitForSeconds(0.2f);

        if (contenedorRutaCompleta != null)
        {
            contenedorRutaCompleta.RemoveFromClassList("button-slide-in");
            contenedorRutaCompleta.AddToClassList("button-slide-in-active");
        }

        yield return new WaitForSeconds(0.2f);

        if (contenedorMinijuegos != null)
        {
            contenedorMinijuegos.RemoveFromClassList("button-slide-in");
            contenedorMinijuegos.AddToClassList("button-slide-in-active");
        }

        yield return new WaitForSeconds(0.2f);

        // 6. Logo UVG al final
        if (logoUvg != null)
        {
            logoUvg.RemoveFromClassList("logo-fade-in");
            logoUvg.AddToClassList("logo-fade-in-active");
        }

        // 7. Activar efectos de fondo
        if (backgroundCIT != null)
        {
            backgroundCIT.AddToClassList("breathing-bg");
        }
    }

    void PrepararElementosParaAnimacion()
    {
        // Establecer estados iniciales
        contenedorPrincipal?.AddToClassList("fade-in");
        contenedorBienvenida?.AddToClassList("slide-from-top");
        textoBienvenida?.AddToClassList("text-slide-up");
        textoARTour?.AddToClassList("text-slide-up");
        contenedorRutaExpress?.AddToClassList("button-slide-in");
        contenedorRutaCompleta?.AddToClassList("button-slide-in");
        contenedorMinijuegos?.AddToClassList("button-slide-in");
        logoUvg?.AddToClassList("logo-fade-in");
    }

    IEnumerator AnimacionSalidaYCambio(string tipoRuta)
    {
        // Animaci�n de salida r�pida
        if (contenedorPrincipal != null)
        {
            contenedorPrincipal.RemoveFromClassList("fade-in-active");
            contenedorPrincipal.AddToClassList("fade-in");
        }

        // Esperar un poquito para que se vea la transici�n
        yield return new WaitForSeconds(0.3f);

        // Cambiar de pantalla
        EstadoRuta.TipoRuta = tipoRuta;
        cambiador.MostrarEscaneo();
    }

    private static bool s_LogicLoadedOrLoading = false;

    private IEnumerator LoadLogicThenShowEscaneo(string tipoRuta)
    {
        // quick fade-out like before (optional)
        if (contenedorPrincipal != null)
        {
            contenedorPrincipal.RemoveFromClassList("fade-in-active");
            contenedorPrincipal.AddToClassList("fade-in");
        }

        yield return new WaitForSeconds(0.2f);

        if (!s_LogicLoadedOrLoading)
        {
            s_LogicLoadedOrLoading = true;

            // IMPORTANT: "TestRoom" must be added in File → Build Settings → Scenes In Build
            var op = SceneManager.LoadSceneAsync("TestRoom", LoadSceneMode.Additive);
            if (op == null)
            {
                Debug.LogError("Failed to start loading scene 'TestRoom'. Check the scene name and Build Settings.");
                s_LogicLoadedOrLoading = false;
                yield break;
            }
            while (!op.isDone) yield return null;

            // Make TestRoom the active scene so lighting/NavMesh work as expected
            var logic = SceneManager.GetSceneByName("TestRoom");
            if (logic.IsValid())
                SceneManager.SetActiveScene(logic);
            else
                Debug.LogWarning("Loaded 'TestRoom' but Scene is not valid? Check the scene asset.");
        }

        // Switch the overlay to Escaneo
        EstadoRuta.TipoRuta = tipoRuta;
        cambiador.MostrarEscaneo();
    }
}