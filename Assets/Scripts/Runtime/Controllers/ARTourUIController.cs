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
    private VisualElement contenidoNormalFooter; // Esto guardará solo el contenido original
    // Footer punto de interés
    private VisualElement footerPOI;
    private bool footerPOIActivo = false;
    // Botón de prueba
    private Button btnSimularPunto;
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
        // Footer normal - AQUÍ ESTÁ LA CORRECCIÓN
        contenedorFooter = root.Q<VisualElement>("ContenedorFooter");
        
        // Crear un wrapper para todo el contenido original del footer
        contenidoNormalFooter = new VisualElement { name = "ContenidoOriginalFooter" };
        
        // Mover todos los hijos existentes al wrapper
        var hijosOriginales = contenedorFooter.Children().ToList();
        foreach (var hijo in hijosOriginales)
        {
            contenedorFooter.Remove(hijo);
            contenidoNormalFooter.Add(hijo);
        }
        
        // Agregar el wrapper de vuelta al contenedor
        contenedorFooter.Add(contenidoNormalFooter);
        
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
                        "Assets/Resources/ImagenesUI/ImagenesDeTour/AdmisionesYRecepcion.png"
                    );
                }
            };
        }
    }
    // ===================== FOOTER PUNTO DE INTERÉS =====================
    private void MostrarFooterPOI(string titulo, string descripcion, string rutaImagen)
    {
        if (contenedorFooter == null) return;
        
        Debug.Log("Mostrando footer POI"); // Para debug
        
        // Ocultar footer normal (solo el contenido original)
        contenidoNormalFooter.style.display = DisplayStyle.None;
        
        // Crear footer dinámico POI
        footerPOI = new VisualElement { name = "FooterPOI" };
        footerPOI.style.flexDirection = FlexDirection.Column;
        footerPOI.style.height = 200; // Aumenté la altura para mejor visualización
        footerPOI.style.backgroundColor = new Color(4f / 255f, 8f / 255f, 19f / 255f, 0.9f);
        footerPOI.style.paddingTop = 15;
        footerPOI.style.paddingRight = 15;
        footerPOI.style.paddingBottom = 15;
        footerPOI.style.paddingLeft = 15;
        footerPOI.style.borderTopWidth = 2;
        footerPOI.style.borderTopColor = new Color(129f / 255f, 209f / 255f, 180f / 255f);
        
        // Contenedor horizontal para la información
        var contenidoHorizontal = new VisualElement { name = "ContenidoHorizontalPOI" };
        contenidoHorizontal.style.flexDirection = FlexDirection.Row;
        contenidoHorizontal.style.alignItems = Align.FlexStart;
        contenidoHorizontal.style.flexGrow = 1;
        
        // Imagen
        var imagen = new VisualElement { name = "ImagenPOI" };
        var rutaLimpia = rutaImagen.Replace("Assets/Resources/", "").Replace(".png", "");
        var texture = Resources.Load<Texture2D>(rutaLimpia);
        if (texture != null)
        {
            imagen.style.backgroundImage = texture;
        }
        else
        {
            Debug.LogWarning($"No se pudo cargar la imagen: {rutaLimpia}");
            imagen.style.backgroundColor = new Color(0.3f, 0.3f, 0.3f); // Color de placeholder
        }
        imagen.style.width = 80;
        imagen.style.height = 80;
        imagen.style.marginRight = 15;
        
        // Contenedor de texto
        var contenedorTexto = new VisualElement { name = "ContenedorTextoPOI" };
        contenedorTexto.style.flexDirection = FlexDirection.Column;
        contenedorTexto.style.flexGrow = 1;
        
        // Título
        var labelTitulo = new Label(titulo) { name = "TituloPOI" };
        labelTitulo.style.fontSize = 16;
        labelTitulo.style.color = new Color(129f / 255f, 209f / 255f, 180f / 255f);
        labelTitulo.style.unityTextAlign = TextAnchor.UpperLeft;
        labelTitulo.style.marginBottom = 8;
        labelTitulo.style.unityFontStyleAndWeight = FontStyle.Bold;
        
        // Descripción
        var labelDescripcion = new Label(descripcion) { name = "DescripcionPOI" };
        labelDescripcion.style.fontSize = 12;
        labelDescripcion.style.color = Color.white;
        labelDescripcion.style.flexWrap = Wrap.Wrap;
        labelDescripcion.style.whiteSpace = WhiteSpace.Normal;
        labelDescripcion.style.flexGrow = 1;
        
        contenedorTexto.Add(labelTitulo);
        contenedorTexto.Add(labelDescripcion);
        
        contenidoHorizontal.Add(imagen);
        contenidoHorizontal.Add(contenedorTexto);
        
        // Botón cerrar
        var btnCerrar = new Button(() => RestaurarFooterNormal()) { text = "✖ Cerrar" };
        btnCerrar.style.width = 100;
        btnCerrar.style.height = 35;
        btnCerrar.style.marginTop = 10;
        btnCerrar.style.backgroundColor = new Color(129f / 255f, 209f / 255f, 180f / 255f, 0.2f);
        btnCerrar.style.color = new Color(129f / 255f, 209f / 255f, 180f / 255f);
        btnCerrar.style.alignSelf = Align.Center;
        
        // Agregar elementos al footer POI
        footerPOI.Add(contenidoHorizontal);
        footerPOI.Add(btnCerrar);
        
        // Agregar footer dinámico al contenedor
        contenedorFooter.Add(footerPOI);
        footerPOIActivo = true;
        
        Debug.Log("Footer POI creado y agregado"); // Para debug
    }
    
    private void RestaurarFooterNormal()
    {
        Debug.Log("Restaurando footer normal"); // Para debug
        
        if (footerPOI != null)
        {
            contenedorFooter.Remove(footerPOI);
            footerPOI = null;
        }
        
        // Mostrar el contenido normal del footer
        contenidoNormalFooter.style.display = DisplayStyle.Flex;
        footerPOIActivo = false;
        
        Debug.Log("Footer normal restaurado"); // Para debug
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