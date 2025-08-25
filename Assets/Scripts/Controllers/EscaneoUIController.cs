using UnityEngine;
using UnityEngine.UIElements;

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
}
