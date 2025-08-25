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

        // Cargar fonts con la ruta correcta
        var outfitSemiBold = Resources.Load<Font>("Assets/UI Toolkit/Fonts/Outfit-SemiBold");
        var outfitRegular = Resources.Load<Font>("Assets/UI Toolkit/Fonts/Outfit-Regular");

        // Si no funcionan con esa ruta, intenta sin "Assets/"
        if (outfitSemiBold == null)
        {
            outfitSemiBold = Resources.Load<Font>("UI Toolkit/Fonts/Outfit-SemiBold");
            Debug.Log("Intentando cargar font con ruta alternativa");
        }

        if (outfitRegular == null)
        {
            outfitRegular = Resources.Load<Font>("UI Toolkit/Fonts/Outfit-Regular");
            Debug.Log("Intentando cargar font con ruta alternativa");
        }

        if (outfitSemiBold == null)
            Debug.LogWarning("No se pudo cargar la font Outfit-SemiBold. Verifica que esté en Resources/UI Toolkit/Fonts/");
        if (outfitRegular == null)
            Debug.LogWarning("No se pudo cargar la font Outfit-Regular. Verifica que esté en Resources/UI Toolkit/Fonts/");

        // Crear menú programáticamente mejorado
        CrearMenuProgramatico(outfitSemiBold, outfitRegular);

        // Configurar botón hamburguesa
        ConfigurarBotonHamburguesa();
        ConfigurarBotonSimularConexion();
    }

    private void CrearMenuProgramatico(Font outfitSemiBold, Font outfitRegular)
    {
        // Crear overlay que NO cubre toda la pantalla, solo la parte del menú
        var menuOverlayProgrammatico = new VisualElement { name = "menu_overlay_programmatico" };
        menuOverlayProgrammatico.style.position = Position.Absolute;
        menuOverlayProgrammatico.style.top = 0;
        menuOverlayProgrammatico.style.right = 0; // Solo del lado derecho
        menuOverlayProgrammatico.style.width = Length.Percent(60); // Solo 60% del ancho
        menuOverlayProgrammatico.style.height = Length.Percent(100);

        // Fondo con color específico #112d2f
        menuOverlayProgrammatico.style.backgroundColor = new Color(0f, 0f, 4f / 255f, 0.95f); // rgba(0,0,4) con 95% opacidad
        menuOverlayProgrammatico.style.display = DisplayStyle.None;
        menuOverlayProgrammatico.pickingMode = PickingMode.Position;

        // Crear el menú principal
        var menuProgrammatico = new VisualElement { name = "menu_hamburguesa_programmatico" };
        menuProgrammatico.style.width = Length.Percent(100);
        menuProgrammatico.style.height = Length.Percent(100);
        menuProgrammatico.style.flexDirection = FlexDirection.Column;
        menuProgrammatico.pickingMode = PickingMode.Position;

        // Header del menú
        var headerProgrammatico = new VisualElement { name = "menu_header_programmatico" };
        headerProgrammatico.style.flexDirection = FlexDirection.Row;
        headerProgrammatico.style.justifyContent = Justify.SpaceBetween;
        headerProgrammatico.style.alignItems = Align.Center;
        headerProgrammatico.style.paddingTop = 20;
        headerProgrammatico.style.paddingBottom = 15;
        headerProgrammatico.style.paddingLeft = 20;
        headerProgrammatico.style.paddingRight = 20;
        headerProgrammatico.style.flexShrink = 0;

        // Título con tamaño de fuente ajustado
        var tituloProgrammatico = new Label("Menú") { name = "titulo_programmatico" };
        tituloProgrammatico.style.color = Color.white;
        tituloProgrammatico.style.fontSize = 22; // no aumentar!
        tituloProgrammatico.style.unityTextAlign = TextAnchor.MiddleLeft;

        if (outfitSemiBold != null)
        {
            tituloProgrammatico.style.unityFont = outfitSemiBold;
            Debug.Log("Font Outfit-SemiBold aplicada al título");
        }

        // Botón cerrar sin fondo ni bordes
        var btnCerrarProgrammatico = new Button(() => {
            Debug.Log("Cerrando menú programático");
            menuOverlayProgrammatico.style.display = DisplayStyle.None;
        })
        { name = "boton_cerrar_programmatico", text = "×" };

        btnCerrarProgrammatico.style.width = 35;
        btnCerrarProgrammatico.style.height = 35;
        btnCerrarProgrammatico.style.backgroundColor = new Color(0, 0, 0, 0); // Completamente transparente
        btnCerrarProgrammatico.style.borderTopWidth = 0;
        btnCerrarProgrammatico.style.borderRightWidth = 0;
        btnCerrarProgrammatico.style.borderBottomWidth = 0;
        btnCerrarProgrammatico.style.borderLeftWidth = 0;
        btnCerrarProgrammatico.style.color = Color.white;
        btnCerrarProgrammatico.style.fontSize = 24; // Un poco más grande para compensar la falta de fondo
        btnCerrarProgrammatico.style.unityTextAlign = TextAnchor.MiddleCenter;

        // Efecto hover más sutil para el botón cerrar (solo cambio de opacidad del texto)
        btnCerrarProgrammatico.RegisterCallback<MouseEnterEvent>(_ => {
            btnCerrarProgrammatico.style.color = new Color(1f, 1f, 1f, 0.7f); // Texto más tenue
        });
        btnCerrarProgrammatico.RegisterCallback<MouseLeaveEvent>(_ => {
            btnCerrarProgrammatico.style.color = Color.white; // Texto normal
        });

        headerProgrammatico.Add(tituloProgrammatico);
        headerProgrammatico.Add(btnCerrarProgrammatico);

        // Contenedor de botones
        var contenedorBotonesProgrammatico = new VisualElement { name = "contenedor_botones_programmatico" };
        contenedorBotonesProgrammatico.style.flexGrow = 1;
        contenedorBotonesProgrammatico.style.width = Length.Percent(100);
        contenedorBotonesProgrammatico.style.paddingLeft = 20;
        contenedorBotonesProgrammatico.style.paddingRight = 20;
        contenedorBotonesProgrammatico.style.paddingTop = 10;

        // Opciones del menú con texto más pequeño
        string[] opcionesMenu = {
            "Reconectar Sensores",
            "Reiniciar Tour",
            "Diagnóstico de Conexión",
            "Reportar un Problema",
            "Salir a Inicio"
        };

        foreach (string opcion in opcionesMenu)
        {
            var btnOpcion = new Button(() => {
                Debug.Log($"Opción clickeada: {opcion}");

                // Cerrar menú primero
                menuOverlayProgrammatico.style.display = DisplayStyle.None;

                // Manejar acción específica
                switch (opcion)
                {
                    case "Salir a Inicio":
                        if (cambiador != null)
                            cambiador.MostrarInicio();
                        break;
                    case "Reconectar Sensores":
                        Debug.Log("Reconectando sensores...");
                        break;
                    case "Reiniciar Tour":
                        Debug.Log("Reiniciando tour...");
                        break;
                    case "Diagnóstico de Conexión":
                        Debug.Log("Mostrando diagnóstico...");
                        break;
                    case "Reportar un Problema":
                        Debug.Log("Abriendo reporte...");
                        break;
                }
            })
            { text = opcion };

            // Estilo mejorado para los botones
            btnOpcion.style.color = Color.white;
            btnOpcion.style.backgroundColor = new Color(0, 0, 0, 0);
            btnOpcion.style.borderTopWidth = 0;
            btnOpcion.style.borderRightWidth = 0;
            btnOpcion.style.borderBottomWidth = 1;
            btnOpcion.style.borderLeftWidth = 0;
            btnOpcion.style.borderBottomColor = new Color(1f, 1f, 1f, 0.3f);
            btnOpcion.style.fontSize = 14; // Reducido de 18 a 14 para que quepa el texto
            btnOpcion.style.paddingTop = 15;
            btnOpcion.style.paddingBottom = 15;
            btnOpcion.style.paddingLeft = 0;
            btnOpcion.style.paddingRight = 5; // Un poco de padding derecho
            btnOpcion.style.marginBottom = 5;
            btnOpcion.style.unityTextAlign = TextAnchor.MiddleLeft;

            // Permitir que el texto se ajuste (wrap)
            btnOpcion.style.whiteSpace = WhiteSpace.Normal;
            btnOpcion.style.flexWrap = Wrap.Wrap;

            if (outfitRegular != null)
            {
                btnOpcion.style.unityFont = outfitRegular;
                Debug.Log($"Font Outfit-Regular aplicada a botón: {opcion}");
            }

            // Efectos hover
            btnOpcion.RegisterCallback<MouseEnterEvent>(_ => {
                btnOpcion.style.backgroundColor = new Color(1f, 1f, 1f, 0.1f);
            });
            btnOpcion.RegisterCallback<MouseLeaveEvent>(_ => {
                btnOpcion.style.backgroundColor = new Color(0, 0, 0, 0);
            });

            contenedorBotonesProgrammatico.Add(btnOpcion);
        }

        // Ensamblar el menú
        menuProgrammatico.Add(headerProgrammatico);
        menuProgrammatico.Add(contenedorBotonesProgrammatico);
        menuOverlayProgrammatico.Add(menuProgrammatico);

        // Agregar al root
        root.Add(menuOverlayProgrammatico);

        // Evitar que clics en el menú cierren el overlay
        menuProgrammatico.RegisterCallback<ClickEvent>(evt => evt.StopPropagation());

        // Asignar referencia
        menuHamburguesaVisual = menuOverlayProgrammatico;
        menuInicializado = true;

        Debug.Log("Menú programático creado exitosamente");
        Debug.Log($"Menú agregado al root. Índice: {root.IndexOf(menuOverlayProgrammatico)}");
    }

    private void ConfigurarBotonHamburguesa()
    {
        botonMenuHamburguesa = root.Q<VisualElement>("icono_menu_hamburguesa");
        if (botonMenuHamburguesa != null)
        {
            onHambClickHandler = (ClickEvent evt) => {
                Debug.Log("Click en menú hamburguesa detectado");
                MostrarMenuProgramatico();
            };
            botonMenuHamburguesa.RegisterCallback(onHambClickHandler);
            Debug.Log("Botón hamburguesa configurado correctamente");
        }
        else
        {
            Debug.LogError("No se encontró el VisualElement 'icono_menu_hamburguesa' en el UXML.");
        }
    }

    private void MostrarMenuProgramatico()
    {
        if (!menuInicializado)
        {
            Debug.LogError("El menú no está inicializado");
            return;
        }

        var menuOverlay = root.Q<VisualElement>("menu_overlay_programmatico");
        if (menuOverlay != null)
        {
            menuOverlay.style.display = DisplayStyle.Flex;
            menuOverlay.BringToFront();
            Debug.Log("Menú programático mostrado y traído al frente");
        }
        else
        {
            Debug.LogError("No se encontró el menu_overlay_programmatico");
        }
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
            Debug.Log("Botón simular configurado correctamente");
        }
        else
        {
            Debug.LogError("No se encontró el Button 'BotonSimularSensores' en el UXML.");
        }
    }
}