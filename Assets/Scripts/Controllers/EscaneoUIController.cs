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
            Debug.LogError("MenuHamburguesaUXML no está asignado en el inspector.");
            return;
        }
        if (menuHamburguesaController == null)
        {
            Debug.LogError("MenuHamburguesaUIController no está asignado en el inspector.");
            return;
        }
        if (cambiador == null)
        {
            Debug.LogWarning("CambiadorDePantallas no está asignado en el inspector. El botón de simular conexión no funcionará.");
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

        // Instanciar el árbol del menú
        // (En versiones recientes Instantiate() es preferible; CloneTree() también sirve.)
        menuHamburguesaVisual = menuHamburguesaUXML.Instantiate();
        if (menuHamburguesaVisual == null)
        {
            Debug.LogError("Fallo al instanciar el UXML del menú (resultado null). Revisa el asset.");
            return;
        }

        overlay.Add(menuHamburguesaVisual);

        // Vincula el cambiador por si lo necesitas en el menú
        menuHamburguesaController.cambiador = cambiador;

        // Inicializa y marca estado SOLO si fue exitoso
        menuInicializado = menuHamburguesaController.Inicializar(menuHamburguesaVisual);

        // Botón hamburguesa
        botonMenuHamburguesa = root.Q<VisualElement>("icono_menu_hamburguesa");
        if (botonMenuHamburguesa != null)
        {
            _onHambClickHandler = (ClickEvent evt) => OnBotonMenuHamburguesaClicked();
            botonMenuHamburguesa.RegisterCallback(_onHambClickHandler);
        }
        else
        {
            Debug.LogError("No se encontró el VisualElement 'icono_menu_hamburguesa' en el UXML.");
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
                    Debug.Log("[BYPASS] Simulando conexión de 3 sensores...");
                    cambiador.MostrarTour();
                }
                else
                {
                    Debug.LogError("CambiadorDePantallas no está asignado en el inspector.");
                }
            };
        }
        else
        {
            Debug.LogError("No se encontró el Button 'BotonSimularSensores' en el UXML.");
        }
    }

    private void OnBotonMenuHamburguesaClicked()
    {
        Debug.Log("EscaneoUIController: ¡Click en icono de menú hamburguesa detectado!");

        if (menuInicializado && menuHamburguesaController != null && menuHamburguesaController.EstaInicializado)
        {
            Debug.Log("EscaneoUIController: Menú inicializado, llamando a MostrarMenu...");
            menuHamburguesaController.MostrarMenu();
        }
        else
        {
            Debug.LogError("El menú hamburguesa no está correctamente inicializado. Verifica el UXML y la inicialización.");
        }
    }
}
