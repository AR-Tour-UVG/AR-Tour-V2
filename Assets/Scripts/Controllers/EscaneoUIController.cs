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

        // ===== M�TODO UXML (comentado temporalmente) =====
        /*
        // Instanciar el �rbol del men� DIRECTAMENTE en el root
        menuHamburguesaVisual = menuHamburguesaUXML.Instantiate();
        if (menuHamburguesaVisual == null)
        {
            Debug.LogError("Fallo al instanciar el UXML del men� (resultado null). Revisa el asset.");
            return;
        }
        
        // Agregar DIRECTAMENTE al root para m�xima prioridad visual
        // Al agregarlo al final, deber�a tener prioridad visual
        root.Add(menuHamburguesaVisual);
        */

        // ===== M�TODO PROGRAM�TICO (m�s confiable) =====
        // Crear overlay program�ticamente
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

        // Crear el men� program�ticamente
        var menuProgrammatico = new VisualElement { name = "menu_hamburguesa_programmatico" };
        menuProgrammatico.style.position = Position.Absolute;
        menuProgrammatico.style.top = 0;
        menuProgrammatico.style.right = 0;
        menuProgrammatico.style.width = Length.Percent(60);
        menuProgrammatico.style.height = Length.Percent(100);
        menuProgrammatico.style.backgroundColor = new Color(4f / 255f, 8f / 255f, 19f / 255f, 0.95f);
        menuProgrammatico.style.flexDirection = FlexDirection.Column;
        menuProgrammatico.pickingMode = PickingMode.Position;

        // Crear header del men�
        var headerProgrammatico = new VisualElement { name = "menu_header_programmatico" };
        headerProgrammatico.style.flexDirection = FlexDirection.Row;
        headerProgrammatico.style.justifyContent = Justify.SpaceBetween;
        headerProgrammatico.style.alignItems = Align.Center;
        headerProgrammatico.style.paddingTop = 20;
        headerProgrammatico.style.paddingBottom = 15;
        headerProgrammatico.style.paddingLeft = 20;
        headerProgrammatico.style.paddingRight = 20;
        headerProgrammatico.style.flexShrink = 0;

        // T�tulo del men�
        var tituloProgrammatico = new Label("Men�") { name = "titulo_programmatico" };
        tituloProgrammatico.style.color = Color.white;
        tituloProgrammatico.style.fontSize = 25;
        tituloProgrammatico.style.unityTextAlign = TextAnchor.MiddleLeft;

        // Bot�n cerrar
        var btnCerrarProgrammatico = new Button(() => {
            Debug.Log("���Bot�n cerrar program�tico clickeado!!!");
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

        // Crear botones del men�
        string[] opcionesMenu = { "Reconectar Sensores", "Reiniciar Tour", "Diagn�stico de Conexi�n", "Reportar un Problema", "Salir a Inicio" };

        foreach (string opcion in opcionesMenu)
        {
            var btnOpcion = new Button(() => {
                Debug.Log($"Opci�n clickeada: {opcion}");
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
                Debug.Log("Click fuera del men� - cerrando");
                menuOverlayProgrammatico.style.display = DisplayStyle.None;
            }
        });

        // Evitar que clics en el men� cierren el overlay
        menuProgrammatico.RegisterCallback<ClickEvent>(evt => evt.StopPropagation());

        Debug.Log("Men� creado program�ticamente");
        Debug.Log($"Men� program�tico agregado al root. �ndice: {root.IndexOf(menuOverlayProgrammatico)}");
        Debug.Log($"Total hijos en root: {root.childCount}");

        // Asignar las referencias para que el controlador original funcione
        menuHamburguesaVisual = menuOverlayProgrammatico;

        Debug.Log("Men� creado program�ticamente como respaldo");

        // Vincula el cambiador por si lo necesitas en el men�
        menuHamburguesaController.cambiador = cambiador;

        // Como ahora usamos el men� program�tico, inicializar de forma simple
        menuInicializado = true; // El men� program�tico ya est� listo

        // Crear una funci�n simple para mostrar el men� program�tico
        System.Action mostrarMenuProgrammatico = () => {
            Debug.Log("Mostrando men� program�tico...");
            var menuOverlay = root.Q<VisualElement>("menu_overlay_programmatico");
            if (menuOverlay != null)
            {
                menuOverlay.style.display = DisplayStyle.Flex;
                menuOverlay.BringToFront();
                Debug.Log($"Men� program�tico mostrado. �ndice: {root.IndexOf(menuOverlay)}");
            }
            else
            {
                Debug.LogError("No se encontr� el menu_overlay_programmatico");
            }
        };

        // Bot�n hamburguesa - ahora usa el men� program�tico
        botonMenuHamburguesa = root.Q<VisualElement>("icono_menu_hamburguesa");
        if (botonMenuHamburguesa != null)
        {
            onHambClickHandler = (ClickEvent evt) => {
                Debug.Log("EscaneoUIController: �Click en icono de men� hamburguesa detectado!");
                mostrarMenuProgrammatico();
            };
            botonMenuHamburguesa.RegisterCallback(onHambClickHandler);
        }
        else
        {
            Debug.LogError("No se encontr� el VisualElement 'icono_menu_hamburguesa' en el UXML.");
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