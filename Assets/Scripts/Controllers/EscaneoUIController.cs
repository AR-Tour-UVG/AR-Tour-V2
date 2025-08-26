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
            Debug.LogWarning("No se pudo cargar la font Outfit-SemiBold. Verifica que est� en Resources/UI Toolkit/Fonts/");
        if (outfitRegular == null)
            Debug.LogWarning("No se pudo cargar la font Outfit-Regular. Verifica que est� en Resources/UI Toolkit/Fonts/");

        // Crear men� program�ticamente mejorado
        CrearMenuProgramatico(outfitSemiBold, outfitRegular);

        // Configurar bot�n hamburguesa
        ConfigurarBotonHamburguesa();
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

    private void CrearMenuProgramatico(Font outfitSemiBold, Font outfitRegular)
    {
        // Crear overlay que NO cubre toda la pantalla, solo la parte del men�
        var menuOverlayProgrammatico = new VisualElement { name = "menu_overlay_programmatico" };
        menuOverlayProgrammatico.style.position = Position.Absolute;
        menuOverlayProgrammatico.style.top = 0;
        menuOverlayProgrammatico.style.right = 0; // Solo del lado derecho
        menuOverlayProgrammatico.style.width = Length.Percent(60); // Solo 60% del ancho
        menuOverlayProgrammatico.style.height = Length.Percent(100);

        // Fondo con color espec�fico #112d2f
        menuOverlayProgrammatico.style.backgroundColor = new Color(0f, 0f, 4f / 255f, 0.95f); // rgba(0,0,4) con 95% opacidad
        menuOverlayProgrammatico.style.display = DisplayStyle.None;
        menuOverlayProgrammatico.pickingMode = PickingMode.Position;

        // Crear el men� principal
        var menuProgrammatico = new VisualElement { name = "menu_hamburguesa_programmatico" };
        menuProgrammatico.style.width = Length.Percent(100);
        menuProgrammatico.style.height = Length.Percent(100);
        menuProgrammatico.style.flexDirection = FlexDirection.Column;
        menuProgrammatico.pickingMode = PickingMode.Position;

        // Header del men�
        var headerProgrammatico = new VisualElement { name = "menu_header_programmatico" };
        headerProgrammatico.style.flexDirection = FlexDirection.Row;
        headerProgrammatico.style.justifyContent = Justify.SpaceBetween;
        headerProgrammatico.style.alignItems = Align.Center;
        headerProgrammatico.style.paddingTop = 20;
        headerProgrammatico.style.paddingBottom = 15;
        headerProgrammatico.style.paddingLeft = 20;
        headerProgrammatico.style.paddingRight = 20;
        headerProgrammatico.style.flexShrink = 0;

        // T�tulo con tama�o de fuente ajustado
        var tituloProgrammatico = new Label("Men�") { name = "titulo_programmatico" };
        tituloProgrammatico.style.color = Color.white;
        tituloProgrammatico.style.fontSize = 22; // no aumentar!
        tituloProgrammatico.style.unityTextAlign = TextAnchor.MiddleLeft;

        if (outfitSemiBold != null)
        {
            tituloProgrammatico.style.unityFont = outfitSemiBold;
            Debug.Log("Font Outfit-SemiBold aplicada al t�tulo");
        }

        // Bot�n cerrar sin fondo ni bordes
        var btnCerrarProgrammatico = new Button(() => {
            Debug.Log("Cerrando men� program�tico");
            menuOverlayProgrammatico.style.display = DisplayStyle.None;
        })
        { name = "boton_cerrar_programmatico", text = "�" };

        btnCerrarProgrammatico.style.width = 35;
        btnCerrarProgrammatico.style.height = 35;
        btnCerrarProgrammatico.style.backgroundColor = new Color(0, 0, 0, 0); // Completamente transparente
        btnCerrarProgrammatico.style.borderTopWidth = 0;
        btnCerrarProgrammatico.style.borderRightWidth = 0;
        btnCerrarProgrammatico.style.borderBottomWidth = 0;
        btnCerrarProgrammatico.style.borderLeftWidth = 0;
        btnCerrarProgrammatico.style.color = Color.white;
        btnCerrarProgrammatico.style.fontSize = 24; // Un poco m�s grande para compensar la falta de fondo
        btnCerrarProgrammatico.style.unityTextAlign = TextAnchor.MiddleCenter;

        // Efecto hover m�s sutil para el bot�n cerrar (solo cambio de opacidad del texto)
        btnCerrarProgrammatico.RegisterCallback<MouseEnterEvent>(_ => {
            btnCerrarProgrammatico.style.color = new Color(1f, 1f, 1f, 0.7f); // Texto m�s tenue
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

        // Opciones del men� con texto m�s peque�o
        string[] opcionesMenu = {
            "Reconectar Sensores",
            "Reiniciar Tour",
            "Diagn�stico de Conexi�n",
            "Reportar un Problema",
            "Salir a Inicio"
        };

        foreach (string opcion in opcionesMenu)
        {
            var btnOpcion = new Button(() => {
                Debug.Log($"Opci�n clickeada: {opcion}");

                // Cerrar men� primero
                menuOverlayProgrammatico.style.display = DisplayStyle.None;

                // Manejar acci�n espec�fica
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
                    case "Diagn�stico de Conexi�n":
                        Debug.Log("Mostrando diagn�stico...");
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
                Debug.Log($"Font Outfit-Regular aplicada a bot�n: {opcion}");
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

        // Ensamblar el men�
        menuProgrammatico.Add(headerProgrammatico);
        menuProgrammatico.Add(contenedorBotonesProgrammatico);
        menuOverlayProgrammatico.Add(menuProgrammatico);

        // Agregar al root
        root.Add(menuOverlayProgrammatico);

        // Evitar que clics en el men� cierren el overlay
        menuProgrammatico.RegisterCallback<ClickEvent>(evt => evt.StopPropagation());

        // Asignar referencia
        menuHamburguesaVisual = menuOverlayProgrammatico;
        menuInicializado = true;

        Debug.Log("Men� program�tico creado exitosamente");
        Debug.Log($"Men� agregado al root. �ndice: {root.IndexOf(menuOverlayProgrammatico)}");
    }

    private void ConfigurarBotonHamburguesa()
    {
        botonMenuHamburguesa = root.Q<VisualElement>("icono_menu_hamburguesa");
        if (botonMenuHamburguesa != null)
        {
            onHambClickHandler = (ClickEvent evt) => {
                Debug.Log("Click en men� hamburguesa detectado");
                MostrarMenuProgramatico();
            };
            botonMenuHamburguesa.RegisterCallback(onHambClickHandler);
            Debug.Log("Bot�n hamburguesa configurado correctamente");
        }
        else
        {
            Debug.LogError("No se encontr� el VisualElement 'icono_menu_hamburguesa' en el UXML.");
        }
    }

    private void MostrarMenuProgramatico()
    {
        if (!menuInicializado)
        {
            Debug.LogError("El men� no est� inicializado");
            return;
        }

        var menuOverlay = root.Q<VisualElement>("menu_overlay_programmatico");
        if (menuOverlay != null)
        {
            menuOverlay.style.display = DisplayStyle.Flex;
            menuOverlay.BringToFront();
            Debug.Log("Men� program�tico mostrado y tra�do al frente");
        }
        else
        {
            Debug.LogError("No se encontr� el menu_overlay_programmatico");
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
