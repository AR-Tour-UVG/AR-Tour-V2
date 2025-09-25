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
    private VisualElement iconoInfoMinimizado; // Icono "i" que queda visible cuando se minimiza
    private bool footerPOIActivo = false;
    private bool footerPOIMinimizado = false;

    // Botón de prueba
    private Button btnSimularPunto;

    // Variables para guardar el estado original del footer
    private StyleLength alturaOriginalFooter;
    private StyleEnum<FlexDirection> flexDirectionOriginal;
    private Color colorFondoFooter = new Color(0f, 0f, 4f / 255f, 0.95f); // Color de fondo por defecto

    private void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        // Crear menú hamburguesa
        CrearMenuProgramatico();

        // Configurar botón hamburguesa
        botonMenuHamburguesa = root.Q<VisualElement>("icono_menu_hamburguesa");
        if (botonMenuHamburguesa != null)
        {
            onHambClickHandler = (ClickEvent evt) => { MostrarMenuProgramatico(); };
            botonMenuHamburguesa.RegisterCallback(onHambClickHandler);
        }

        // Configurar botón salir
        botonSalir = root.Q<Button>("btn_salir_inicio");
        if (botonSalir != null)
        {
            botonSalir.clicked += () =>
            {
                if (cambiador != null) cambiador.MostrarInicio();
            };
        }

        // Footer normal
        contenedorFooter = root.Q<VisualElement>("ContenedorFooter");
        if (contenedorFooter != null)
        {
            // Obtener el color de fondo actual del footer
            colorFondoFooter = contenedorFooter.resolvedStyle.backgroundColor;
            
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
                        "Makerspace D-Hive:",
                        "D por Diseño, Hive por colmena y colaboración.\n\nEs un espacio de prototipado rápido. Diseñado para desarrollar habilidades de ideación e interacción rápida para proyectos. Es un espacio libre, donde todos los estudiantes pueden venir y usar los equipos.",
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

        Debug.Log("Mostrando footer POI con animación");
        
        // Animar la reducción del footer normal
        AnimarReduccionFooterNormal(() =>
        {
            // Callback que se ejecuta después de la animación de reducción
            CrearFooterPOI(titulo, descripcion, rutaImagen);
            AnimarAparicionFooterPOI();
        });

        footerPOIActivo = true;
    }

    private void AnimarReduccionFooterNormal(System.Action onComplete)
    {
        // Reducir la altura del footer normal a una línea (aproximadamente 4px)
        contenedorFooter.style.height = 4;
        
        // Ocultar el contenido normal
        if (contenidoNormalFooter != null)
        {
            contenidoNormalFooter.style.display = DisplayStyle.None;
        }

        // Simular un delay para la animación (en un proyecto real usarías DOTween o similar)
        StartCoroutine(DelayedCallback(0.3f, onComplete));
    }

    private void CrearFooterPOI(string titulo, string descripcion, string rutaImagen)
    {
        // Crear el footer POI completo
        footerPOI = new VisualElement { name = "FooterPOI" };
        footerPOI.style.position = Position.Absolute;
        footerPOI.style.bottom = 50; // Encima de la línea del footer normal
        footerPOI.style.left = 0;
        footerPOI.style.width = Length.Percent(80);
        footerPOI.style.height = Length.Auto(); // Altura fija en píxeles en lugar de porcentaje
        footerPOI.style.maxHeight = 170; // Altura máxima para evitar que sea demasiado grande
        footerPOI.style.backgroundColor = colorFondoFooter;
        footerPOI.style.flexDirection = FlexDirection.Column;
        footerPOI.style.paddingTop = 10;
        footerPOI.style.paddingBottom = 10;
        footerPOI.style.paddingLeft = 10;
        footerPOI.style.paddingRight = 10;
        footerPOI.style.marginLeft = 25;
        footerPOI.style.borderTopLeftRadius = 12;
        footerPOI.style.borderTopRightRadius = 12;
        footerPOI.style.borderBottomLeftRadius = 12;
        footerPOI.style.borderBottomRightRadius = 12;
        footerPOI.style.display = DisplayStyle.None; // Inicialmente oculto

        // Contenedor principal horizontal: texto a la izquierda, contenedor derecho con icono "i" e imagen
        var contenidoPrincipal = new VisualElement { name = "ContenidoPrincipalPOI" };
        contenidoPrincipal.style.flexDirection = FlexDirection.Row;
        contenidoPrincipal.style.alignItems = Align.FlexStart;
        contenidoPrincipal.style.justifyContent = Justify.SpaceBetween;
        contenidoPrincipal.style.flexGrow = 1;

        // Contenedor de texto (lado izquierdo)
        var contenedorTexto = new VisualElement { name = "ContenedorTextoPOI" };
        contenedorTexto.style.flexDirection = FlexDirection.Column;
        contenedorTexto.style.flexGrow = 1;
        contenedorTexto.style.minWidth = 0;
        contenedorTexto.style.maxWidth = Length.Percent(80);

        var labelTitulo = new Label(titulo) { name = "TituloPOI" };
        labelTitulo.style.fontSize = 10;
        labelTitulo.style.color = new Color(129f / 255f, 209f / 255f, 180f / 255f);
        labelTitulo.style.unityTextAlign = TextAnchor.UpperLeft;
        labelTitulo.style.marginBottom = 0;
        labelTitulo.style.unityFontStyleAndWeight = FontStyle.Bold;
        labelTitulo.style.whiteSpace = WhiteSpace.Normal;

        var labelDescripcion = new Label(descripcion) { name = "DescripcionPOI" };
        labelDescripcion.style.fontSize = 6;
        labelDescripcion.style.color = Color.white;
        labelDescripcion.style.unityTextAlign = TextAnchor.UpperLeft;
        labelDescripcion.style.whiteSpace = WhiteSpace.Normal;

        contenedorTexto.Add(labelTitulo);
        contenedorTexto.Add(labelDescripcion);

        // Contenedor del lado derecho (icono "i" + imagen)
        var contenedorDerecho = new VisualElement { name = "ContenedorDerechoPOI" };
        contenedorDerecho.style.flexDirection = FlexDirection.Column;
        contenedorDerecho.style.alignItems = Align.FlexEnd;
        contenedorDerecho.style.flexShrink = 0;
        contenedorDerecho.style.width = 100; // Ancho fijo igual al de la imagen
        contenedorDerecho.style.marginLeft = Length.Auto(); // Empuja todo el contenedor al extremo derecho

        // Botón de información (icono "i") - en la parte superior derecha
        var btnInfo = new Button(() => MinimizarFooterPOI());
        btnInfo.style.width = 20;
        btnInfo.style.height = 20;
        btnInfo.style.backgroundColor = Color.clear;
        btnInfo.style.borderTopWidth = 0;
        btnInfo.style.borderBottomWidth = 0;
        btnInfo.style.borderLeftWidth = 0;
        btnInfo.style.borderRightWidth = 0;
        btnInfo.style.marginBottom = 15;

        // Cargar el icono de información
        var iconInfoTexture = Resources.Load<Texture2D>("ImagenesUI/Iconos/icon_info");
        if (iconInfoTexture != null)
        {
            btnInfo.style.backgroundImage = iconInfoTexture;
            btnInfo.text = "";
        }
        else
        {
            btnInfo.text = "i"; // Fallback text
            btnInfo.style.fontSize = 8;
            btnInfo.style.color = Color.white;
        }

        // Imagen (debajo del icono "i")
        var imagen = new VisualElement { name = "ImagenPOI" };
        var rutaLimpia = rutaImagen.Replace("Assets/Resources/", "").Replace(".png", "");
        var texture = Resources.Load<Texture2D>(rutaLimpia);
        
        if (texture != null)
            imagen.style.backgroundImage = texture;
        else
            imagen.style.backgroundColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);

        imagen.style.width = 110;
        imagen.style.height = 110;
        imagen.style.flexShrink = 0;
        imagen.style.marginRight = -20; // -25px del borde derecho NO TOCAR
        imagen.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Contain);

        // Agregar icono "i" e imagen al contenedor derecho
        contenedorDerecho.Add(btnInfo);
        contenedorDerecho.Add(imagen);

        // Agregar elementos al contenido principal
        contenidoPrincipal.Add(contenedorTexto);
        contenidoPrincipal.Add(contenedorDerecho);

        // Agregar elementos al footer POI
        footerPOI.Add(contenidoPrincipal);

        // Agregar footer POI al root (no al contenedor footer)
        root.Add(footerPOI);
    }

    private void AnimarAparicionFooterPOI()
    {
        if (footerPOI != null)
        {
            footerPOI.style.display = DisplayStyle.Flex;
        }
    }

    private void MinimizarFooterPOI()
    {
        if (footerPOI == null) return;

        Debug.Log("Minimizando footer POI");

        // Ocultar el footer POI completo
        footerPOI.style.display = DisplayStyle.None;

        // Restaurar el footer normal
        RestaurarFooterNormalYMostrarIcono();

        footerPOIMinimizado = true;
    }

    private void RestaurarFooterNormalYMostrarIcono()
    {
    // Restaurar las propiedades originales del contenedor footer
    contenedorFooter.style.height = alturaOriginalFooter;
    contenedorFooter.style.flexDirection = flexDirectionOriginal;
    

    // Mostrar el contenido normal y resetear layout
        if (contenidoNormalFooter != null)
        {
            contenidoNormalFooter.style.display = DisplayStyle.Flex;
            contenidoNormalFooter.style.flexDirection = FlexDirection.Row;
            foreach (var hijo in contenidoNormalFooter.Children())
            {
                hijo.style.flexShrink = 0;
                hijo.style.flexGrow = 0;
            }
        }

    // Crear el icono minimizado encima del footer normal
    CrearIconoInfoMinimizado();
}

    private void CrearIconoInfoMinimizado()
    {
        // Crear el contenedor del icono minimizado
        iconoInfoMinimizado = new VisualElement { name = "IconoInfoMinimizado" };
        iconoInfoMinimizado.style.position = Position.Absolute;
        iconoInfoMinimizado.style.bottom = contenedorFooter.resolvedStyle.height + 90; // Justo encima del footer normal
        iconoInfoMinimizado.style.left = 25;
        iconoInfoMinimizado.style.width = 40;
        iconoInfoMinimizado.style.height = 30;
        iconoInfoMinimizado.style.backgroundColor = colorFondoFooter;
        iconoInfoMinimizado.style.borderTopLeftRadius = 8; // Bordes menos redondeados (más cuadrados)
        iconoInfoMinimizado.style.borderTopRightRadius = 8;
        iconoInfoMinimizado.style.borderBottomLeftRadius = 8;
        iconoInfoMinimizado.style.borderBottomRightRadius = 8;
        iconoInfoMinimizado.style.justifyContent = Justify.Center;
        iconoInfoMinimizado.style.alignItems = Align.Center;

        // Crear el botón del icono
        var btnIconoInfo = new Button(() => ExpandirFooterPOI());
        btnIconoInfo.style.width = 20;
        btnIconoInfo.style.height = 20;
        btnIconoInfo.style.backgroundColor = Color.clear;
        btnIconoInfo.style.borderTopWidth = 0;
        btnIconoInfo.style.borderBottomWidth = 0;
        btnIconoInfo.style.borderLeftWidth = 0;
        btnIconoInfo.style.borderRightWidth = 0;

        // Cargar el icono de información
        var iconInfoTexture = Resources.Load<Texture2D>("ImagenesUI/Iconos/icon_info");
        if (iconInfoTexture != null)
        {
            btnIconoInfo.style.backgroundImage = iconInfoTexture;
            btnIconoInfo.text = "";
        }
        else
        {
            btnIconoInfo.text = "i";
            btnIconoInfo.style.fontSize = 16;
            btnIconoInfo.style.color = Color.white;
        }

        iconoInfoMinimizado.Add(btnIconoInfo);
        root.Add(iconoInfoMinimizado);
    }

    private void ExpandirFooterPOI()
    {
        Debug.Log("Expandiendo footer POI");

        // Remover el icono minimizado
        if (iconoInfoMinimizado != null)
        {
            root.Remove(iconoInfoMinimizado);
            iconoInfoMinimizado = null;
        }

        // Volver a minimizar el footer normal antes de mostrar el POI
        AnimarReduccionFooterNormal(() =>
        {
            // Mostrar el footer POI completo después de reducir el normal
            if (footerPOI != null)
            {
                footerPOI.style.display = DisplayStyle.Flex;
            }
        });

        footerPOIMinimizado = false;
    }

    private void RestaurarFooterNormal()
    {
        Debug.Log("Restaurando footer normal");

        if (contenedorFooter == null) return;

        // Remover footer POI y icono minimizado
        if (footerPOI != null)
        {
            root.Remove(footerPOI);
            footerPOI = null;
        }

        if (iconoInfoMinimizado != null)
        {
            root.Remove(iconoInfoMinimizado);
            iconoInfoMinimizado = null;
        }

        // Restaurar las propiedades originales del contenedor
        contenedorFooter.style.height = alturaOriginalFooter;
        contenedorFooter.style.flexDirection = flexDirectionOriginal;

        // Mostrar el contenido normal
        if (contenidoNormalFooter != null)
        {
            contenidoNormalFooter.style.display = DisplayStyle.Flex;
        }

        footerPOIActivo = false;
        footerPOIMinimizado = false;

        Debug.Log("Footer normal restaurado");
    }

    // Método auxiliar para simular delays (en un proyecto real usarías DOTween)
    private System.Collections.IEnumerator DelayedCallback(float delay, System.Action callback)
    {
        yield return new WaitForSeconds(delay);
        callback?.Invoke();
    }

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

        var btnCerrar = new Button(() => { menuOverlay.style.display = DisplayStyle.None; })
        {
            text = "✖"
        };
        btnCerrar.style.width = 35;
        btnCerrar.style.height = 35;
        btnCerrar.style.backgroundColor = Color.clear;
        btnCerrar.style.color = Color.white;
        btnCerrar.style.fontSize = 24;

        header.Add(titulo);
        header.Add(btnCerrar);

        var contenedorOpciones = new VisualElement { name = "contenedor_botones_programmatico" };

        string[] opcionesMenu = {
            "Reconectar Sensores",
            "Reiniciar Tour", 
            "Diagnóstico de Conexión",
            "Reportar un Problema",
            "Salir a Inicio"
        };

        foreach (string opcion in opcionesMenu)
        {
            var btnOpcion = new Button(() =>
            {
                menuOverlay.style.display = DisplayStyle.None;
                switch (opcion)
                {
                    case "Salir a Inicio":
                        if (cambiador != null) cambiador.MostrarInicio();
                        break;
                    case "Reconectar Sensores":
                        break;
                    case "Reiniciar Tour":
                        break;
                    case "Diagnóstico de Conexión":
                        break;
                    case "Reportar un Problema":
                        break;
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

    private void OnDisable()
    {
        if (botonMenuHamburguesa != null && onHambClickHandler != null)
        {
            botonMenuHamburguesa.UnregisterCallback(onHambClickHandler);
        }

        // Limpiar estado del footer POI al desactivar la pantalla
        LimpiarEstadoFooterPOI();
    }

    private void LimpiarEstadoFooterPOI()
    {
        // Remover footer POI si existe
        if (footerPOI != null)
        {
            root.Remove(footerPOI);
            footerPOI = null;
        }

        // Remover icono minimizado si existe
        if (iconoInfoMinimizado != null)
        {
            root.Remove(iconoInfoMinimizado);
            iconoInfoMinimizado = null;
        }

        // Restaurar estado original del footer normal
        if (contenedorFooter != null && contenidoNormalFooter != null)
        {
            contenedorFooter.style.height = alturaOriginalFooter;
            contenedorFooter.style.flexDirection = flexDirectionOriginal;
            contenidoNormalFooter.style.display = DisplayStyle.Flex;
        }

        // Resetear flags
        footerPOIActivo = false;
        footerPOIMinimizado = false;
    }
}