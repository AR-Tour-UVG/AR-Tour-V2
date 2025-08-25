using UnityEngine;
using UnityEngine.UIElements;

public class ARTourUIController : MonoBehaviour
{
    private VisualElement root;
    private VisualElement botonMenuHamburguesa; // Igual que en Escaneo
    private Button botonSalir;

    [SerializeField] private CambiadorDePantallas cambiador;

    // Variables para menú programático
    private VisualElement menuHamburguesaVisual;
    private bool menuInicializado = false;

    private EventCallback<ClickEvent> onHambClickHandler;

    private void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        // Crear menú programático con estilos idénticos
        CrearMenuProgramatico();

        // Configurar botón hamburguesa
        botonMenuHamburguesa = root.Q<VisualElement>("icono_menu_hamburguesa");
        if (botonMenuHamburguesa != null)
        {
            onHambClickHandler = (ClickEvent evt) =>
            {
                Debug.Log("Click detectado en menú hamburguesa (ARTour)");
                MostrarMenuProgramatico();
            };
            botonMenuHamburguesa.RegisterCallback(onHambClickHandler);
            Debug.Log("Botón hamburguesa configurado correctamente en ARTour");
        }
        else
        {
            Debug.LogError("No se encontró 'icono_menu_hamburguesa' en ARTour. Revisa el UXML");
        }

        // Configurar botón salir
        botonSalir = root.Q<Button>("boton_salir");
        if (botonSalir != null)
        {
            botonSalir.clicked += () =>
            {
                Debug.Log("Click en salir desde ARTour ? Volver al inicio");
                if (cambiador != null)
                    cambiador.MostrarInicio();
            };
        }
        else
        {
            Debug.LogWarning("No se encontró 'boton_salir' en ARTour.");
        }
    }

    private void CrearMenuProgramatico()
    {
        // Cargar fonts
        var outfitSemiBold = Resources.Load<Font>("UI Toolkit/Fonts/Outfit-SemiBold");
        var outfitRegular = Resources.Load<Font>("UI Toolkit/Fonts/Outfit-Regular");

        if (outfitSemiBold == null) Debug.LogWarning("No se pudo cargar Outfit-SemiBold");
        if (outfitRegular == null) Debug.LogWarning("No se pudo cargar Outfit-Regular");

        // Overlay del menú
        var menuOverlay = new VisualElement { name = "menu_overlay_programmatico" };
        menuOverlay.style.position = Position.Absolute;
        menuOverlay.style.top = 0;
        menuOverlay.style.right = 0;
        menuOverlay.style.width = Length.Percent(60);
        menuOverlay.style.height = Length.Percent(100);
        menuOverlay.style.backgroundColor = new Color(0f, 0f, 4f / 255f, 0.95f);
        menuOverlay.style.display = DisplayStyle.None;
        menuOverlay.pickingMode = PickingMode.Position;

        // Contenedor principal
        var menu = new VisualElement { name = "menu_hamburguesa_programmatico" };
        menu.style.width = Length.Percent(100);
        menu.style.height = Length.Percent(100);
        menu.style.flexDirection = FlexDirection.Column;
        menu.pickingMode = PickingMode.Position;

        // Header
        var header = new VisualElement { name = "menu_header_programmatico" };
        header.style.flexDirection = FlexDirection.Row;
        header.style.justifyContent = Justify.SpaceBetween;
        header.style.alignItems = Align.Center;
        header.style.paddingTop = 20;
        header.style.paddingBottom = 15;
        header.style.paddingLeft = 20;
        header.style.paddingRight = 20;

        var titulo = new Label("Menú") { name = "titulo_programmatico" };
        titulo.style.color = Color.white;
        titulo.style.fontSize = 22;
        titulo.style.unityTextAlign = TextAnchor.MiddleLeft;
        if (outfitSemiBold != null) titulo.style.unityFont = outfitSemiBold;

        var btnCerrar = new Button(() => { menuOverlay.style.display = DisplayStyle.None; }) { text = "×" };
        btnCerrar.style.width = 35;
        btnCerrar.style.height = 35;
        btnCerrar.style.backgroundColor = Color.clear;
        btnCerrar.style.borderTopWidth = 0;
        btnCerrar.style.borderRightWidth = 0;
        btnCerrar.style.borderBottomWidth = 0;
        btnCerrar.style.borderLeftWidth = 0;
        btnCerrar.style.color = Color.white;
        btnCerrar.style.fontSize = 24;
        btnCerrar.style.unityTextAlign = TextAnchor.MiddleCenter;
        btnCerrar.RegisterCallback<MouseEnterEvent>(_ => { btnCerrar.style.color = new Color(1f, 1f, 1f, 0.7f); });
        btnCerrar.RegisterCallback<MouseLeaveEvent>(_ => { btnCerrar.style.color = Color.white; });

        header.Add(titulo);
        header.Add(btnCerrar);

        // Contenedor de opciones
        var contenedorOpciones = new VisualElement { name = "contenedor_botones_programmatico" };
        contenedorOpciones.style.flexGrow = 1;
        contenedorOpciones.style.width = Length.Percent(100);
        contenedorOpciones.style.paddingLeft = 20;
        contenedorOpciones.style.paddingRight = 20;
        contenedorOpciones.style.paddingTop = 10;

        string[] opcionesMenu = { "Reconectar Sensores", "Reiniciar Tour", "Diagnóstico de Conexión", "Reportar un Problema", "Salir a Inicio" };
        foreach (string opcion in opcionesMenu)
        {
            var btnOpcion = new Button(() =>
            {
                Debug.Log($"Opción clickeada: {opcion}");
                menuOverlay.style.display = DisplayStyle.None;

                switch (opcion)
                {
                    case "Salir a Inicio":
                        if (cambiador != null) cambiador.MostrarInicio();
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

            // Estilo idéntico a Escaneo
            btnOpcion.style.color = Color.white;
            btnOpcion.style.backgroundColor = Color.clear;
            btnOpcion.style.borderTopWidth = 0;
            btnOpcion.style.borderRightWidth = 0;
            btnOpcion.style.borderBottomWidth = 1;
            btnOpcion.style.borderLeftWidth = 0;
            btnOpcion.style.borderBottomColor = new Color(1f, 1f, 1f, 0.3f);
            btnOpcion.style.fontSize = 14;
            btnOpcion.style.paddingTop = 15;
            btnOpcion.style.paddingBottom = 15;
            btnOpcion.style.paddingLeft = 0;
            btnOpcion.style.paddingRight = 5;
            btnOpcion.style.marginBottom = 5;
            btnOpcion.style.unityTextAlign = TextAnchor.MiddleLeft;
            btnOpcion.style.whiteSpace = WhiteSpace.Normal;
            btnOpcion.style.flexWrap = Wrap.Wrap;
            if (outfitRegular != null) btnOpcion.style.unityFont = outfitRegular;

            btnOpcion.RegisterCallback<MouseEnterEvent>(_ => { btnOpcion.style.backgroundColor = new Color(1f, 1f, 1f, 0.1f); });
            btnOpcion.RegisterCallback<MouseLeaveEvent>(_ => { btnOpcion.style.backgroundColor = Color.clear; });

            contenedorOpciones.Add(btnOpcion);
        }

        // Ensamblar
        menu.Add(header);
        menu.Add(contenedorOpciones);
        menuOverlay.Add(menu);
        menu.RegisterCallback<ClickEvent>(evt => evt.StopPropagation());
        root.Add(menuOverlay);

        menuHamburguesaVisual = menuOverlay;
        menuInicializado = true;

        Debug.Log("Menú programático creado exitosamente en ARTour");
    }

    private void MostrarMenuProgramatico()
    {
        if (!menuInicializado)
        {
            Debug.LogError("El menú no está inicializado");
            return;
        }

        menuHamburguesaVisual.style.display = DisplayStyle.Flex;
        menuHamburguesaVisual.BringToFront();
    }

    private void OnDisable()
    {
        if (botonMenuHamburguesa != null && onHambClickHandler != null)
        {
            botonMenuHamburguesa.UnregisterCallback(onHambClickHandler);
        }
    }
}
