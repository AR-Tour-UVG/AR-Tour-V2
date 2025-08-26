using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
public class EscaneoUIController : MonoBehaviour
{
    private VisualElement root;
    private VisualElement botonMenuHamburguesa;
    private VisualElement menuHamburguesaVisual;
    private Button botonSimular;

    [SerializeField] private MenuHamburguesaUIController menuHamburguesaController;
    public VisualTreeAsset menuHamburguesaUXML;
    public CambiadorDePantallas cambiador;

    private bool menuInicializado = false;

#if UNITY_IOS && !UNITY_EDITOR
    private bool _connected = false;
#endif

    // Guarda el handler para desuscribir bien
    private EventCallback<ClickEvent> _onHambClickHandler;

    private void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        if (menuHamburguesaUXML == null)
        {
            Debug.LogError("MenuHamburguesaUXML no est� asignado en el inspector.");
            return;
        }
        if (menuHamburguesaController == null)
        {
            Debug.LogError("MenuHamburguesaUIController no est� asignado en el inspector.");
            return;
        }
        if (cambiador == null)
        {
            Debug.LogWarning("CambiadorDePantallas no est� asignado en el inspector. El bot�n de simular conexi�n no funcionar�.");
        }

        // Asegura un contenedor overlay
        var overlay = root.Q<VisualElement>("OverlayContainer");
        if (overlay == null)
        {
            overlay = new VisualElement { name = "OverlayContainer" };
            overlay.style.position = Position.Absolute;
            overlay.style.top = 0; overlay.style.left = 0; overlay.style.right = 0; overlay.style.bottom = 0;
            overlay.style.width = Length.Percent(100);
            overlay.style.height = Length.Percent(100);
            root.Add(overlay);
        }

        // Instanciar el �rbol del men�
        // (En versiones recientes Instantiate() es preferible; CloneTree() tambi�n sirve.)
        menuHamburguesaVisual = menuHamburguesaUXML.Instantiate();
        if (menuHamburguesaVisual == null)
        {
            Debug.LogError("Fallo al instanciar el UXML del men� (resultado null). Revisa el asset.");
            return;
        }

        overlay.Add(menuHamburguesaVisual);

        // Vincula el cambiador por si lo necesitas en el men�
        menuHamburguesaController.cambiador = cambiador;

        // Inicializa y marca estado SOLO si fue exitoso
        menuInicializado = menuHamburguesaController.Inicializar(menuHamburguesaVisual);

        // Bot�n hamburguesa
        botonMenuHamburguesa = root.Q<VisualElement>("icono_menu_hamburguesa");
        if (botonMenuHamburguesa != null)
        {
            _onHambClickHandler = (ClickEvent evt) => OnBotonMenuHamburguesaClicked();
            botonMenuHamburguesa.RegisterCallback(_onHambClickHandler);
        }
        else
        {
            Debug.LogError("No se encontr� el VisualElement 'icono_menu_hamburguesa' en el UXML.");
        }

        ConfigurarBotonSimularConexion();
#if UNITY_IOS && !UNITY_EDITOR
        // iOS: hide simulate button (not needed)
        botonSimular = root.Q<Button>("BotonSimularSensores");
        if (botonSimular != null)
        {
            botonSimular.style.display = DisplayStyle.None;
        }
        _connected = false;
        // Start waiting for the first valid UWB coordinate
        if (!_connected)
        {
            _connected = true;
            StartCoroutine(WaitForFirstValidCoordinate());
        }
#endif
    }

    private void OnDisable()
    {
        if (botonMenuHamburguesa != null && _onHambClickHandler != null)
        {
            botonMenuHamburguesa.UnregisterCallback(_onHambClickHandler);
        }
    }

    private void ConfigurarBotonSimularConexion()
    {
        botonSimular = root.Q<Button>("BotonSimularSensores");
        if (botonSimular != null)
        {
#if UNITY_IOS && !UNITY_EDITOR
            // No-op on iOS (we hide/disable it above)
            botonSimular.clicked += () => {};
#else
            botonSimular.clicked += () =>
            {
                if (cambiador != null)
                {
                    Debug.Log("[BYPASS] Simulando conexi�n de 3 sensores...");
                    cambiador.MostrarTour();
                }
                else
                {
                    Debug.LogError("CambiadorDePantallas no est� asignado en el inspector.");
                }
            };
#endif
        }
        else
        {
            Debug.LogError("No se encontr� el Button 'BotonSimularSensores' en el UXML.");
        }
    }

    private void OnBotonMenuHamburguesaClicked()
    {
        Debug.Log("EscaneoUIController: �Click en icono de men� hamburguesa detectado!");

        if (menuInicializado && menuHamburguesaController != null && menuHamburguesaController.EstaInicializado)
        {
            Debug.Log("EscaneoUIController: Men� inicializado, llamando a MostrarMenu...");
            menuHamburguesaController.MostrarMenu();
        }
        else
        {
            Debug.LogError("El men� hamburguesa no est� correctamente inicializado. Verifica el UXML y la inicializaci�n.");
        }
    }

#if UNITY_IOS && !UNITY_EDITOR
    private IEnumerator WaitForFirstUwbFix()
    {
        // Minimal: first true = connected.
        while (true)
        {
            if (UWBLocator.TryGetPosition(out var pos))
            {
                Debug.Log($"UWB first fix received: {pos}");
                // Move to Tour overlay
                if (cambiador != false) cambiador.MostrarTour();
                yield break;
            }
            yield return null; // check every frame
        }
    }
#endif
}
