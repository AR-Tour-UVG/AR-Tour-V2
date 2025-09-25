using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public class ARTourUIController : MonoBehaviour
{
    private VisualElement root;
    private VisualElement botonMenuHamburguesa;
    private Button botonSalir;
    [SerializeField] private CambiadorDePantallas cambiador;
    
    // Menú programático
    private VisualElement menuHamburguesaVisual;
    private bool menuInicializado = false;
    private EventCallback<ClickEvent> onHambClickHandler;
    
    // Footer normal
    private VisualElement contenedorFooter;
    private VisualElement contenidoNormalFooter;
    
    // Footer punto de interés
    private VisualElement footerPOI;
    private bool footerPOIActivo = false;
    
    // Botón de prueba
    private Button btnSimularPunto;
    
    // Variables para guardar el estado original del footer
    private StyleLength alturaOriginalFooter;
    private StyleEnum<FlexDirection> flexDirectionOriginal;
    
    private void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        
        // Crear menú hamburguesa
        CrearMenuProgramatico();
        
        // Configurar botón hamburguesa
        botonMenuHamburguesa = root.Q<VisualElement>("icono_menu_hamburguesa");
        if (botonMenuHamburguesa != null)
        {
            onHambClickHandler = (ClickEvent evt) =>
            {
                MostrarMenuProgramatico();
            };
            botonMenuHamburguesa.RegisterCallback(onHambClickHandler);
        }
        
        // Configurar botón salir
        botonSalir = root.Q<Button>("btn_salir_inicio");
        if (botonSalir != null)
        {
            botonSalir.clicked += () =>
            {
                if (cambiador != null)
                    cambiador.MostrarInicio();
            };
        }
        
        // Footer normal - CORRECCIÓN MEJORADA
        contenedorFooter = root.Q<VisualElement>("ContenedorFooter");
        
        if (contenedorFooter != null)
        {
            // Guardar las propiedades originales del contenedor
            alturaOriginalFooter = contenedorFooter.style.height;
            flexDirectionOriginal = contenedorFooter.style.flexDirection;
            
            // Crear un wrapper que mantenga la flexDirection Row para el contenido normal
            contenidoNormalFooter = new VisualElement { name = "ContenidoOriginalFooter" };
            contenidoNormalFooter.style.width = Length.Percent(100);
            contenidoNormalFooter.style.height = Length.Percent(100);
            // IMPORTANTE: Mantener flexDirection Row para el contenido normal
            contenidoNormalFooter.style.flexDirection = FlexDirection.Row;
            
            // Mover todos los hijos existentes al wrapper
            var hijosOriginales = contenedorFooter.Children().ToList();
            foreach (var hijo in hijosOriginales)
            {
                contenedorFooter.Remove(hijo);
                contenidoNormalFooter.Add(hijo);
            }
            
            // Agregar el wrapper de vuelta al contenedor
            contenedorFooter.Add(contenidoNormalFooter);
        }
        
        // Botón de prueba
        btnSimularPunto = root.Q<Button>("btn_simular_punto");
        if (btnSimularPunto != null)
        {
            btnSimularPunto.clicked += () =>
            {
                if (!footerPOIActivo)
                {
                    MostrarFooterPOI(
                        "Admisiones y Recepción",
                        "Aquí se gestionan inscripciones y recepción de visitantes.",
                        "ImagenesUI/DragonJack/JackGood"
                    );
                }
            };
        }
    }
    
    // ===================== FOOTER PUNTO DE INTERÉS =====================
    private void MostrarFooterPOI(string titulo, string descripcion, string rutaImagen)
{
    if (contenedorFooter == null) return;

    Debug.Log("Mostrando footer POI");

    // Expandir el contenedor del footer
    contenedorFooter.style.height = Length.Percent(50);
    contenedorFooter.style.flexDirection = FlexDirection.Column;
    contenedorFooter.Clear(); // Limpiar contenido existente

    // Crear footer POI independiente
    footerPOI = new VisualElement { name = "FooterPOI" };
    footerPOI.style.flexDirection = FlexDirection.Column;
    footerPOI.style.width = Length.Percent(100);
    footerPOI.style.height = Length.Percent(100);
    footerPOI.style.paddingTop = 20;
    footerPOI.style.paddingBottom = 20;
    footerPOI.style.paddingLeft = 20;
    footerPOI.style.paddingRight = 20;

    // Header con botón de cerrar
    var header = new VisualElement { name = "HeaderPOI" };
    header.style.flexDirection = FlexDirection.Row;
    header.style.justifyContent = Justify.FlexEnd;
    header.style.alignItems = Align.Center;
    header.style.marginBottom = 10;

    var btnCerrar = new Button(() => RestaurarFooterNormal());
        btnCerrar.style.width = 30;
        btnCerrar.style.height = 30;
        btnCerrar.style.backgroundColor = Color.clear;
        btnCerrar.style.borderTopWidth = 0;
        btnCerrar.style.borderBottomWidth = 0;
        btnCerrar.style.borderLeftWidth = 0;
        btnCerrar.style.borderRightWidth = 0;
        btnCerrar.style.paddingLeft = 0;
        btnCerrar.style.paddingRight = 0;

var iconTexture = Resources.Load<Texture2D>("ImagenesUI/Iconos/icon_exit");
        if (iconTexture != null)
        {
            btnCerrar.style.backgroundImage = iconTexture;
            btnCerrar.text = ""; // sin texto
        }

    header.Add(btnCerrar);

    // Contenedor principal horizontal: texto a la izquierda, imagen a la derecha
    var contenidoPrincipal = new VisualElement { name = "ContenidoPrincipalPOI" };
    contenidoPrincipal.style.flexDirection = FlexDirection.Row;
    contenidoPrincipal.style.alignItems = Align.Center;
    contenidoPrincipal.style.justifyContent = Justify.FlexStart;
    contenidoPrincipal.style.flexGrow = 1;

    // Contenedor de texto (lado izquierdo)
    var contenedorTexto = new VisualElement { name = "ContenedorTextoPOI" };
    contenedorTexto.style.flexDirection = FlexDirection.Column;
    contenedorTexto.style.flexGrow = 1;
    contenedorTexto.style.minWidth = 0;
    contenedorTexto.style.maxWidth = Length.Percent(70); // para que no se encoja demasiado

    var labelTitulo = new Label(titulo) { name = "TituloPOI" };
    labelTitulo.style.fontSize = 12;
    labelTitulo.style.color = new Color(129f / 255f, 209f / 255f, 180f / 255f);
    labelTitulo.style.unityTextAlign = TextAnchor.MiddleLeft;
    labelTitulo.style.marginBottom = 4;
    labelTitulo.style.unityFontStyleAndWeight = FontStyle.Bold;
    labelTitulo.style.unityTextAlign = TextAnchor.UpperLeft;
    labelTitulo.style.whiteSpace = WhiteSpace.Normal;

    var labelDescripcion = new Label(descripcion) { name = "DescripcionPOI" };
    labelDescripcion.style.fontSize = 8;
    labelDescripcion.style.color = Color.white;
    labelDescripcion.style.unityTextAlign = TextAnchor.UpperLeft;
    labelDescripcion.style.whiteSpace = WhiteSpace.Normal;

    contenedorTexto.Add(labelTitulo);
    contenedorTexto.Add(labelDescripcion);

    // Imagen (lado derecho)
    var imagen = new VisualElement { name = "ImagenPOI" };
    var rutaLimpia = rutaImagen.Replace("Assets/Resources/", "").Replace(".png", "");
    var texture = Resources.Load<Texture2D>(rutaLimpia);
    if (texture != null)
        imagen.style.backgroundImage = texture;
    else
        imagen.style.backgroundColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);

    imagen.style.width = 100;
    imagen.style.height = 100;
    imagen.style.marginLeft = Length.Auto(); // Empuja la imagen a la derecha
    imagen.style.flexShrink = 0;
    imagen.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Contain);

    // Agregar texto primero, imagen después
    contenidoPrincipal.Add(contenedorTexto);
    contenidoPrincipal.Add(imagen);

    // Agregar elementos al footer POI
    footerPOI.Add(header);
    footerPOI.Add(contenidoPrincipal);

    // Agregar footer POI al contenedor
    contenedorFooter.Add(footerPOI);

    footerPOIActivo = true;
    Debug.Log("Footer POI creado y agregado");
}

    
    private void RestaurarFooterNormal()
    {
        Debug.Log("Restaurando footer normal");
        
        if (contenedorFooter == null) return;
        
        // Restaurar las propiedades originales del contenedor
        contenedorFooter.style.height = alturaOriginalFooter;
        contenedorFooter.style.flexDirection = flexDirectionOriginal;
        
        // Limpiar completamente el contenedor
        contenedorFooter.Clear();
        
        // Restaurar el contenido original (que mantiene su flexDirection Row)
        contenedorFooter.Add(contenidoNormalFooter);
        
        footerPOI = null;
        footerPOIActivo = false;
        
        Debug.Log("Footer normal restaurado");
    }
    
    // ================================================================
    
    // ===================== MENÚ PROGRAMÁTICO ===================
    private void CrearMenuProgramatico()
    {
        var outfitSemiBold = Resources.Load<Font>("UI Toolkit/Fonts/TTF/Outfit-SemiBold");
        var outfitRegular = Resources.Load<Font>("UI Toolkit/Fonts/TTF/Outfit-Regular");
        
        var menuOverlay = new VisualElement { name = "menu_overlay_programmatico" };
        menuOverlay.style.position = Position.Absolute;
        menuOverlay.style.top = 0;
        menuOverlay.style.right = 0;
        menuOverlay.style.width = Length.Percent(60);
        menuOverlay.style.height = Length.Percent(100);
        menuOverlay.style.backgroundColor = new Color(0f, 0f, 4f / 255f, 0.95f);
        menuOverlay.style.display = DisplayStyle.None;
        menuOverlay.pickingMode = PickingMode.Position;
        
        var menu = new VisualElement { name = "menu_hamburguesa_programmatico" };
        menu.style.width = Length.Percent(100);
        menu.style.height = Length.Percent(100);
        menu.style.flexDirection = FlexDirection.Column;
        
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
        if (outfitSemiBold != null) titulo.style.unityFont = outfitSemiBold;
        
        var btnCerrar = new Button(() => { menuOverlay.style.display = DisplayStyle.None; }) { text = "✖" };
        btnCerrar.style.width = 35;
        btnCerrar.style.height = 35;
        btnCerrar.style.backgroundColor = Color.clear;
        btnCerrar.style.color = Color.white;
        btnCerrar.style.fontSize = 24;
        
        header.Add(titulo);
        header.Add(btnCerrar);
        
        var contenedorOpciones = new VisualElement { name = "contenedor_botones_programmatico" };
        string[] opcionesMenu = { "Reconectar Sensores", "Reiniciar Tour", "Diagnóstico de Conexión", "Reportar un Problema", "Salir a Inicio" };
        
        foreach (string opcion in opcionesMenu)
        {
            var btnOpcion = new Button(() =>
            {
                menuOverlay.style.display = DisplayStyle.None;
                switch (opcion)
                {
                    case "Salir a Inicio": if (cambiador != null) cambiador.MostrarInicio(); break;
                    case "Reconectar Sensores": break;
                    case "Reiniciar Tour": break;
                    case "Diagnóstico de Conexión": break;
                    case "Reportar un Problema": break;
                }
            }) { text = opcion };
            btnOpcion.style.color = Color.white;
            btnOpcion.style.backgroundColor = Color.clear;
            if (outfitRegular != null) btnOpcion.style.unityFont = outfitRegular;
            contenedorOpciones.Add(btnOpcion);
        }
        
        menu.Add(header);
        menu.Add(contenedorOpciones);
        menuOverlay.Add(menu);
        root.Add(menuOverlay);
        menuHamburguesaVisual = menuOverlay;
        menuInicializado = true;
    }
    
    private void MostrarMenuProgramatico()
    {
        if (!menuInicializado) return;
        menuHamburguesaVisual.style.display = DisplayStyle.Flex;
        menuHamburguesaVisual.BringToFront();
    }
    // ===========================================================
    
    private void OnDisable()
    {
        if (botonMenuHamburguesa != null && onHambClickHandler != null)
        {
            botonMenuHamburguesa.UnregisterCallback(onHambClickHandler);
        }
    }
}