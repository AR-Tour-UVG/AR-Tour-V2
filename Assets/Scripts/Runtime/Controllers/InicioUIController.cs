using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Linq;
using UnityEngine.Animations;
using Unity.AI.Navigation;

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
            // Deshabilitar temporalmente
            botonCompleta.SetEnabled(false);

            botonCompleta.clicked += () =>
            {
                StartCoroutine(AnimacionSalidaYCambio("Ruta Completa"));
            };
        }

        if (botonMinijuegos != null)
        {
            // Deshabilitar temporalmente
            botonCompleta.SetEnabled(false);

            botonMinijuegos.clicked += () =>
            {
                // Aquí puedes agregar navegación a minijuegos
                Debug.Log("Navegando a minijuegos...");
            };
        }
    }

    IEnumerator SecuenciaAnimacionEntrada()
    {
        // Preparar elementos para animación (estados iniciales)
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

        // 3. Activar flotación del logo
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
        // Animación de salida rápida
        if (contenedorPrincipal != null)
        {
            contenedorPrincipal.RemoveFromClassList("fade-in-active");
            contenedorPrincipal.AddToClassList("fade-in");
        }

        // Esperar un poquito para que se vea la transición
        yield return new WaitForSeconds(0.3f);

        // Cambiar de pantalla
        EstadoRuta.TipoRuta = tipoRuta;
        cambiador.MostrarEscaneo();
    }

    private static bool s_LogicLoadedOrLoading = false;

    private IEnumerator LoadLogicThenShowEscaneo(string tipoRuta)
    {
        // quick fade-out like before
        if (contenedorPrincipal != null)
        {
            contenedorPrincipal.RemoveFromClassList("fade-in-active");
            contenedorPrincipal.AddToClassList("fade-in");
        }

        yield return new WaitForSeconds(0.2f);

        // Load the logic scene if not already loaded
        if (!s_LogicLoadedOrLoading)
        {
            s_LogicLoadedOrLoading = true; // Set loading flag

            // Load the scene asynchronously and additively (aka "multi-scene")
            var op = SceneManager.LoadSceneAsync("TestRoom", LoadSceneMode.Additive);

            // Check if the operation is valid
            if (op == null)
            {
                Debug.LogError("[InicioUIController] Failed to start loading scene 'TestRoom'. Check the scene name and Build Settings.");
                s_LogicLoadedOrLoading = false; // Reset loading flag
                yield break;
            }

            yield return op; // Wait for the operation to complete

            // Make the scene active
            var scene = SceneManager.GetSceneByName("TestRoom");
            if (!scene.IsValid() || !scene.isLoaded)
            {
                Debug.LogError("[InicioUIController] 'TestRoom' is not valid or not loaded after asynchronous operation. Check the scene asset.");
                s_LogicLoadedOrLoading = false; // Reset loading flag
                yield break;
            }

            SceneManager.SetActiveScene(scene); // Activate the loaded scene
            Debug.Log($"[InicioUIController] Successfully loaded and activated scene '{scene.name}'.");

            // List roots to check for expected objects
            var roots = scene.GetRootGameObjects();
            Debug.Log($"[InicioUIController] 'TestRoom' has {roots.Length} root GameObjects:");
            Debug.Log("\t" + string.Join(", ", roots.Select(r => r.name)));

            // Ensure NavMesh is available with a baked NavMesh or need a runtime build
            var surface = Object.FindAnyObjectByType<NavMeshSurface>();
            if (surface != null)
            {
                Debug.Log($"[InicioUIController] Found NavMeshSurface in {scene.name}. Checking for NavMesh data...");

                // Let the component register its baked NavMeshData on this frame
                yield return null;

                // Calculate triangulation to ensure data is available
                var triangulation = NavMesh.CalculateTriangulation();
                if (triangulation.vertices.Length == 0 || triangulation.indices.Length == 0)
                {
                    Debug.LogWarning("[InicioUIController] No baked NavMesh detected. Building at runtime as a fallback...");
                    var sw = System.Diagnostics.Stopwatch.StartNew(); // Start timing the build process
                    surface.BuildNavMesh(); // Build the NavMesh
                    sw.Stop(); // Stop timing
                    Debug.Log($"[InicioUIController] NavMesh built in {sw.ElapsedMilliseconds}ms.");
                }
                else
                {
                    Debug.Log("[InicioUIController] NavMeshSurface has valid baked data.");
                    yield break; // NavMesh is ready, exit coroutine
                }

                surface.BuildNavMesh(); // Build the NavMesh
                Debug.Log("[InicioUIController] NavMesh built successfully.");
            }
            else
            {
                Debug.LogError("[InicioUIController] No NavMeshSurface found. Ensure a NavMesh is baked or add a surface.");
            }
        }
        // Switch the overlay to Escaneo
        EstadoRuta.TipoRuta = tipoRuta;
        Debug.Log($"[InicioUIController] The current route type is: {EstadoRuta.TipoRuta}");
        cambiador.MostrarEscaneo();
        Debug.Log($"[InicioUIController] Switching overlay to Escaneo.");
    }
}