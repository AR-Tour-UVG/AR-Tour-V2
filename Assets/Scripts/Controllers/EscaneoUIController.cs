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
    private EventCallback<ClickEvent> onHambClickHandler;

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

        // ===== MÉTODO UXML (comentado temporalmente) =====
        /*
        // Instanciar el árbol del menú DIRECTAMENTE en el root
        menuHamburguesaVisual = menuHamburguesaUXML.Instantiate();
        if (menuHamburguesaVisual == null)
        {
            Debug.LogError("Fallo al instanciar el UXML del menú (resultado null). Revisa el asset.");
            return;
        }
        
        // Agregar DIRECTAMENTE al root para máxima prioridad visual
        // Al agregarlo al final, debería tener prioridad visual
        root.Add(menuHamburguesaVisual);
        */

        // ===== MÉTODO PROGRAMÁTICO (más confiable) =====
        // Crear overlay programáticamente
        var menuOverlayProgrammatico = new VisualElement { name = "menu_overlay_programmatico" };
        menuOverlayProgrammatico.style.position = Position.Absolute;
        menuOverlayProgrammatico.style.top = 0;
        menuOverlayProgrammatico.style.left = 0;
        menuOverlayProgrammatico.style.right = 0;
        menuOverlayProgrammatico.style.bottom = 0;
        menuOverlayProgrammatico.style.width = Length.Percent(100);
        menuOverlayProgrammatico.style.height = Length.Percent(100);
        menuOverlayProgrammatico.style.backgroundColor = new Color(0, 0, 0, 0.4f);
        menuOverlayProgrammatico.style.display = DisplayStyle.None;
        menuOverlayProgrammatico.pickingMode = PickingMode.Position;

        // Crear el menú programáticamente
        var menuProgrammatico = new VisualElement { name = "menu_hamburguesa_programmatico" };
        menuProgrammatico.style.position = Position.Absolute;
        menuProgrammatico.style.top = 0;
        menuProgrammatico.style.right = 0;
        menuProgrammatico.style.width = Length.Percent(60);
        menuProgrammatico.style.height = Length.Percent(100);
        menuProgrammatico.style.backgroundColor = new Color(4f / 255f, 8f / 255f, 19f / 255f, 0.95f);
        menuProgrammatico.style.flexDirection = FlexDirection.Column;
        menuProgrammatico.pickingMode = PickingMode.Position;

        // Crear header del menú
        var headerProgrammatico = new VisualElement { name = "menu_header_programmatico" };
        headerProgrammatico.style.flexDirection = FlexDirection.Row;
        headerProgrammatico.style.justifyContent = Justify.SpaceBetween;
        headerProgrammatico.style.alignItems = Align.Center;
        headerProgrammatico.style.paddingTop = 20;
        headerProgrammatico.style.paddingBottom = 15;
        headerProgrammatico.style.paddingLeft = 20;
        headerProgrammatico.style.paddingRight = 20;
        headerProgrammatico.style.flexShrink = 0;

        // Título del menú
        var tituloProgrammatico = new Label("Menú") { name = "titulo_programmatico" };
        tituloProgrammatico.style.color = Color.white;
        tituloProgrammatico.style.fontSize = 25;
        tituloProgrammatico.style.unityTextAlign = TextAnchor.MiddleLeft;

        // Botón cerrar
        var btnCerrarProgrammatico = new Button(() => {
            Debug.Log("¡¡¡Botón cerrar programático clickeado!!!");
            menuOverlayProgrammatico.style.display = DisplayStyle.None;
        })
        { name = "boton_cerrar_programmatico", text = "?" };

        btnCerrarProgrammatico.style.width = 35;
        btnCerrarProgrammatico.style.height = 35;
        btnCerrarProgrammatico.style.backgroundColor = new Color(1f, 1f, 1f, 0.1f);
        btnCerrarProgrammatico.style.borderTopWidth = 1;
        btnCerrarProgrammatico.style.borderRightWidth = 1;
        btnCerrarProgrammatico.style.borderBottomWidth = 1;
        btnCerrarProgrammatico.style.borderLeftWidth = 1;
        btnCerrarProgrammatico.style.borderTopColor = new Color(1f, 1f, 1f, 0.3f);
        btnCerrarProgrammatico.style.borderRightColor = new Color(1f, 1f, 1f, 0.3f);
        btnCerrarProgrammatico.style.borderBottomColor = new Color(1f, 1f, 1f, 0.3f);
        btnCerrarProgrammatico.style.borderLeftColor = new Color(1f, 1f, 1f, 0.3f);
        btnCerrarProgrammatico.style.borderTopLeftRadius = 5;
        btnCerrarProgrammatico.style.borderTopRightRadius = 5;
        btnCerrarProgrammatico.style.borderBottomLeftRadius = 5;
        btnCerrarProgrammatico.style.borderBottomRightRadius = 5;
        btnCerrarProgrammatico.style.color = Color.white;
        btnCerrarProgrammatico.style.fontSize = 18;
        btnCerrarProgrammatico.style.unityTextAlign = TextAnchor.MiddleCenter;

        headerProgrammatico.Add(tituloProgrammatico);
        headerProgrammatico.Add(btnCerrarProgrammatico);

        // Contenedor de botones
        var contenedorBotonesProgrammatico = new VisualElement { name = "contenedor_botones_programmatico" };
        contenedorBotonesProgrammatico.style.flexGrow = 1;
        contenedorBotonesProgrammatico.style.width = Length.Percent(100);
        contenedorBotonesProgrammatico.style.paddingLeft = 20;
        contenedorBotonesProgrammatico.style.paddingRight = 20;
        contenedorBotonesProgrammatico.style.paddingTop = 10;

        // Crear botones del menú
        string[] opcionesMenu = { "Reconectar Sensores", "Reiniciar Tour", "Diagnóstico de Conexión", "Reportar un Problema", "Salir a Inicio" };

        foreach (string opcion in opcionesMenu)
        {
            var btnOpcion = new Button(() => {
                Debug.Log($"Opción clickeada: {opcion}");
                menuOverlayProgrammatico.style.display = DisplayStyle.None;
            })
            { text = opcion };

            btnOpcion.style.color = Color.white;
            btnOpcion.style.backgroundColor = new Color(0, 0, 0, 0);
            btnOpcion.style.borderBottomWidth = 1;
            btnOpcion.style.borderBottomColor = new Color(1f, 1f, 1f, 0.3f);
            btnOpcion.style.fontSize = 16;
            btnOpcion.style.paddingTop = 15;
            btnOpcion.style.paddingBottom = 15;
            btnOpcion.style.paddingLeft = 15;
            btnOpcion.style.paddingRight = 15;
            btnOpcion.style.marginBottom = 5;
            btnOpcion.style.unityTextAlign = TextAnchor.MiddleLeft;

            contenedorBotonesProgrammatico.Add(btnOpcion);
        }

        menuProgrammatico.Add(headerProgrammatico);
        menuProgrammatico.Add(contenedorBotonesProgrammatico);
        menuOverlayProgrammatico.Add(menuProgrammatico);

        // Agregar al root real
        root.Add(menuOverlayProgrammatico);

        // Configurar el click del overlay para cerrar
        menuOverlayProgrammatico.RegisterCallback<ClickEvent>((evt) => {
            if (evt.target == menuOverlayProgrammatico)
            {
                Debug.Log("Click fuera del menú - cerrando");
                menuOverlayProgrammatico.style.display = DisplayStyle.None;
            }
        });

        // Evitar que clics en el menú cierren el overlay
        menuProgrammatico.RegisterCallback<ClickEvent>(evt => evt.StopPropagation());

        Debug.Log("Menú creado programáticamente");
        Debug.Log($"Menú programático agregado al root. Índice: {root.IndexOf(menuOverlayProgrammatico)}");
        Debug.Log($"Total hijos en root: {root.childCount}");

        // Asignar las referencias para que el controlador original funcione
        menuHamburguesaVisual = menuOverlayProgrammatico;

        Debug.Log("Menú creado programáticamente como respaldo");

        // Vincula el cambiador por si lo necesitas en el menú
        menuHamburguesaController.cambiador = cambiador;

        // Como ahora usamos el menú programático, inicializar de forma simple
        menuInicializado = true; // El menú programático ya está listo

        // Crear una función simple para mostrar el menú programático
        System.Action mostrarMenuProgrammatico = () => {
            Debug.Log("Mostrando menú programático...");
            var menuOverlay = root.Q<VisualElement>("menu_overlay_programmatico");
            if (menuOverlay != null)
            {
                menuOverlay.style.display = DisplayStyle.Flex;
                menuOverlay.BringToFront();
                Debug.Log($"Menú programático mostrado. Índice: {root.IndexOf(menuOverlay)}");
            }
            else
            {
                Debug.LogError("No se encontró el menu_overlay_programmatico");
            }
        };

        // Botón hamburguesa - ahora usa el menú programático
        botonMenuHamburguesa = root.Q<VisualElement>("icono_menu_hamburguesa");
        if (botonMenuHamburguesa != null)
        {
            onHambClickHandler = (ClickEvent evt) => {
                Debug.Log("EscaneoUIController: ¡Click en icono de menú hamburguesa detectado!");
                mostrarMenuProgrammatico();
            };
            botonMenuHamburguesa.RegisterCallback(onHambClickHandler);
        }
        else
        {
            Debug.LogError("No se encontró el VisualElement 'icono_menu_hamburguesa' en el UXML.");
        }

        ConfigurarBotonSimularConexion();
    }

    private void OnDisable()
    {
        if (botonMenuHamburguesa != null && onHambClickHandler != null)
        {
            botonMenuHamburguesa.UnregisterCallback(onHambClickHandler);
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