using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using System.Collections;

public class ARTourUIController : MonoBehaviour
{
    private VisualElement root;
    private VisualElement botonMenuHamburguesa;
    private Button botonSalir;
    [SerializeField] private CambiadorDePantallas cambiador;

    // Arrow prefab
    [SerializeField] private GameObject arrowPrefab;
    private GameObject arrowInstance;

    // Referencias para centrar la flecha
    private Camera arCamera;

    // Menú programático
    private VisualElement menuHamburguesaVisual;
    private bool menuInicializado = false;
    private EventCallback<ClickEvent> onHambClickHandler;

    // Footer normal
    private VisualElement contenedorFooter;
    private VisualElement contenidoNormalFooter;

    // Footer punto de interés
    private VisualElement footerPOI;
    private VisualElement iconoInfoMinimizado;
    private bool footerPOIActivo = false;
    private bool footerPOIMinimizado = false;

    // Botón de prueba
    private Button btnSimularPunto;

    // Variables para guardar el estado original del footer
    private StyleLength alturaOriginalFooter;
    private StyleEnum<FlexDirection> flexDirectionOriginal;
    private Color colorFondoFooter = new Color(0f, 0f, 4f / 255f, 0.95f);

    // Popup de conexión
    private VisualElement popupEsperandoConexion;
    private Button botonSimularConexion;

    // Estados del header
    private VisualElement contenedorEsperandoConexion;
    private VisualElement contenedorUbicacionReal;
    private VisualElement contenedorInstruccion;

    // AQUI PARA ACTUALIZAR DINAMICAMENTE LOS TEXTOS DIEGO!!!!
    private Label textoUbicacion;
    private Label textoInstruccion;

    // Variable para iOS
#if UNITY_IOS && !UNITY_EDITOR
        private bool _connected = false;
#endif

    private void Start()
    {
        // Buscar la cámara AR
        arCamera = Camera.main;
        if (arCamera == null)
        {
            arCamera = FindFirstObjectByType<Camera>();
        }

        // Si no asignaste en el inspector, intenta cargar desde Resources
        if (arrowPrefab == null)
        {
            arrowPrefab = Resources.Load<GameObject>("3D Models/arrow");
        }

        // Instancia inicial de la flecha
        if (arrowPrefab != null)
        {
            arrowInstance = Instantiate(arrowPrefab);

            // Configurar la flecha
            ConfigurarFlecha();

            // Posicionar la flecha en el centro de la pantalla
            PosicionarFlechaEnCentro();

            // Mostrar la flecha (para pruebas)
            arrowInstance.SetActive(true);
        }
        else
        {
            Debug.LogError("No se pudo cargar el prefab de la flecha. Asegúrate de que esté en Resources/Modelos/arrow");
        }
    }

    private void ConfigurarFlecha()
    {
        if (arrowInstance == null) return;

        // Escalar la flecha para que sea visible pero no gigante
        arrowInstance.transform.localScale = Vector3.one * 0.1f; // Ajusta este valor según necesites

        // Configurar el material/color de la flecha a verde transparente
        Renderer renderer = arrowInstance.GetComponent<Renderer>();
        if (renderer != null)
        {
            // Crear un material verde transparente usando shader transparente
            Material materialVerde = new Material(Shader.Find("Standard"));
            materialVerde.SetFloat("_Mode", 3); // Transparent mode
            materialVerde.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            materialVerde.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            materialVerde.SetInt("_ZWrite", 0);
            materialVerde.DisableKeyword("_ALPHATEST_ON");
            materialVerde.EnableKeyword("_ALPHABLEND_ON");
            materialVerde.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            materialVerde.renderQueue = 3000;

            // Color verde con transparencia (alpha = 0.6 para semi-transparente)
            materialVerde.color = new Color(0f, 1f, 0f, 0.6f);
            materialVerde.SetFloat("_Metallic", 0.2f);
            materialVerde.SetFloat("_Smoothness", 0.8f);

            renderer.material = materialVerde;
        }
        else
        {
            // Si no tiene renderer, buscar en hijos
            Renderer[] renderersHijos = arrowInstance.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in renderersHijos)
            {
                Material materialVerde = new Material(Shader.Find("Standard"));
                materialVerde.SetFloat("_Mode", 3); // Transparent mode
                materialVerde.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                materialVerde.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                materialVerde.SetInt("_ZWrite", 0);
                materialVerde.DisableKeyword("_ALPHATEST_ON");
                materialVerde.EnableKeyword("_ALPHABLEND_ON");
                materialVerde.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                materialVerde.renderQueue = 3000;

                // Color verde con transparencia
                materialVerde.color = new Color(0f, 1f, 0f, 0.6f);
                materialVerde.SetFloat("_Metallic", 0.2f);
                materialVerde.SetFloat("_Smoothness", 0.8f);

                r.material = materialVerde;
            }
        }
    }

    private void PosicionarFlechaEnCentro()
    {
        if (arrowInstance == null || arCamera == null) return;

        // Calcular la posición en el centro de la pantalla
        Vector3 puntoMedio = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

        // Convertir a posición del mundo a una distancia fija de la cámara
        float distanciaDelaCamara = 2f; // Ajusta esta distancia según necesites
        Vector3 posicionMundo = arCamera.ScreenToWorldPoint(new Vector3(puntoMedio.x, puntoMedio.y, distanciaDelaCamara));

        // Posicionar la flecha
        arrowInstance.transform.position = posicionMundo;

        // Hacer que la flecha mire hacia adelante (misma dirección que la cámara)
        arrowInstance.transform.rotation = arCamera.transform.rotation;

        Debug.Log($"Flecha posicionada en: {posicionMundo}");
    }

    // Método para mostrar la flecha en una dirección específica (para uso futuro con sensores)
    public void MostrarFlecha(Vector3 posicion, Quaternion rotacion)
    {
        if (arrowInstance != null)
        {
            arrowInstance.SetActive(true);
            arrowInstance.transform.position = posicion;
            arrowInstance.transform.rotation = rotacion;
        }
    }

    // Método para mostrar la flecha apuntando hacia una dirección desde el centro
    public void MostrarFlechaHaciaDestino(Vector3 destino)
    {
        if (arrowInstance == null || arCamera == null) return;

        arrowInstance.SetActive(true);

        // Posicionar en el centro
        PosicionarFlechaEnCentro();

        // Calcular la dirección hacia el destino
        Vector3 direccion = (destino - arrowInstance.transform.position).normalized;

        // Rotar la flecha para que apunte hacia el destino
        if (direccion != Vector3.zero)
        {
            arrowInstance.transform.rotation = Quaternion.LookRotation(direccion);
        }
    }

    // Método para ocultar la flecha
    public void OcultarFlecha()
    {
        if (arrowInstance != null)
        {
            arrowInstance.SetActive(false);
        }
    }

    // Método para actualizar la posición de la flecha cuando la cámara se mueva
    private void Update()
    {
        // Si la flecha está activa, mantenerla centrada
        if (arrowInstance != null && arrowInstance.activeInHierarchy)
        {
            PosicionarFlechaEnCentro();
        }
    }

    private void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        // Inicializar referencias del popup y header
        InicializarPopupYHeader();

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
            colorFondoFooter = contenedorFooter.resolvedStyle.backgroundColor;
            alturaOriginalFooter = contenedorFooter.style.height;
            flexDirectionOriginal = contenedorFooter.style.flexDirection;

            contenidoNormalFooter = new VisualElement { name = "ContenidoOriginalFooter" };
            contenidoNormalFooter.style.width = Length.Percent(100);
            contenidoNormalFooter.style.height = Length.Percent(100);
            contenidoNormalFooter.style.flexDirection = FlexDirection.Row;

            var hijosOriginales = contenedorFooter.Children().ToList();
            foreach (var hijo in hijosOriginales)
            {
                contenedorFooter.Remove(hijo);
                contenidoNormalFooter.Add(hijo);
            }

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
    // ===================== POPUP Y HEADER =====================
    private void InicializarPopupYHeader()
    {
        // Referencias del popup
        popupEsperandoConexion = root.Q<VisualElement>("popup_esperando_conexion");
        botonSimularConexion = root.Q<Button>("BotonSimularSensores");

        // Referencias de los estados del header
        contenedorEsperandoConexion = root.Q<VisualElement>("ContenedorEsperandoConexion");
        contenedorUbicacionReal = root.Q<VisualElement>("ContenedorUbicacionReal");
        contenedorInstruccion = root.Q<VisualElement>("ContenedorInstruccion");

        // Referencias de textos para actualización dinámica
        textoUbicacion = root.Q<Label>("TextoUbicacion");
        textoInstruccion = root.Q<Label>("TextoInstruccion");

        // Configurar botón de simular conexión
        if (botonSimularConexion != null)
        {
#if UNITY_IOS && !UNITY_EDITOR
            // En iOS, ocultar el botón y esperar coordenadas reales
            botonSimularConexion.style.display = DisplayStyle.None;
            if (!_connected)
            {
                _connected = true;
                StartCoroutine(WaitForFirstValidCoordinate());
            }
#else
            // En editor/otras plataformas, usar el botón de simulación
            botonSimularConexion.clicked += CerrarPopupYMostrarTour;
#endif
        }

        // Estado inicial: mostrar popup y estado de "esperando conexión"
        MostrarEstadoEsperandoConexion();
    }

    private void MostrarEstadoEsperandoConexion()
    {
        // Mostrar popup
        if (popupEsperandoConexion != null)
            popupEsperandoConexion.style.display = DisplayStyle.Flex;

        // Mostrar "Esperando Conexión" en header
        if (contenedorEsperandoConexion != null)
            contenedorEsperandoConexion.style.display = DisplayStyle.Flex;

        // Ocultar ubicación real
        if (contenedorUbicacionReal != null)
            contenedorUbicacionReal.style.display = DisplayStyle.None;

        // Ocultar instrucciones
        if (contenedorInstruccion != null)
            contenedorInstruccion.style.display = DisplayStyle.None;
    }

    private void CerrarPopupYMostrarTour()
    {
        Debug.Log("Conexión establecida - Mostrando tour");

        // Cerrar popup
        if (popupEsperandoConexion != null)
            popupEsperandoConexion.style.display = DisplayStyle.None;

        // Ocultar "Esperando Conexión"
        if (contenedorEsperandoConexion != null)
            contenedorEsperandoConexion.style.display = DisplayStyle.None;

        // Mostrar ubicación real
        if (contenedorUbicacionReal != null)
            contenedorUbicacionReal.style.display = DisplayStyle.Flex;

        // Mostrar instrucciones
        if (contenedorInstruccion != null)
            contenedorInstruccion.style.display = DisplayStyle.Flex;
    }

    // METODO PUBLICO PARA @DIEGO ACTUALIZA LA UBICACION DESDE SENSORES!!!!!!
    public void ActualizarUbicacion(string nombreLugar)
    {
        if (textoUbicacion != null)
        {
            textoUbicacion.text = nombreLugar;
            Debug.Log($"Ubicación actualizada a: {nombreLugar}");
        }
    }

    // Método público para actualizar las instrucciones desde sensores @DIEGO!!!!!
    public void ActualizarInstruccion(string nivel, string lugarDestino)
    {
        if (textoInstruccion != null)
        {
            textoInstruccion.text = $"Dirigete al {nivel}: {lugarDestino}";
            Debug.Log($"Instrucción actualizada: {nivel} - {lugarDestino}");
        }
    }

    // ===================== FOOTER PUNTO DE INTERÉS =====================
    private void MostrarFooterPOI(string titulo, string descripcion, string rutaImagen)
    {
        if (contenedorFooter == null) return;
        Debug.Log("Mostrando footer POI con animación");

        AnimarReduccionFooterNormal(() =>
        {
            CrearFooterPOI(titulo, descripcion, rutaImagen);
            AnimarAparicionFooterPOI();
        });
        footerPOIActivo = true;
    }

    private void AnimarReduccionFooterNormal(System.Action onComplete)
    {
        contenedorFooter.style.height = 4;
        if (contenidoNormalFooter != null)
        {
            contenidoNormalFooter.style.display = DisplayStyle.None;
        }
        StartCoroutine(DelayedCallback(0.3f, onComplete));
    }

    private void CrearFooterPOI(string titulo, string descripcion, string rutaImagen)
    {
        footerPOI = new VisualElement { name = "FooterPOI" };
        footerPOI.style.position = Position.Absolute;
        footerPOI.style.bottom = 50;
        footerPOI.style.left = 0;
        footerPOI.style.width = Length.Percent(80);
        footerPOI.style.height = Length.Auto();
        footerPOI.style.maxHeight = 170;
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
        footerPOI.style.display = DisplayStyle.None;

        var contenidoPrincipal = new VisualElement { name = "ContenidoPrincipalPOI" };
        contenidoPrincipal.style.flexDirection = FlexDirection.Row;
        contenidoPrincipal.style.alignItems = Align.FlexStart;
        contenidoPrincipal.style.justifyContent = Justify.SpaceBetween;
        contenidoPrincipal.style.flexGrow = 1;

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

        var contenedorDerecho = new VisualElement { name = "ContenedorDerechoPOI" };
        contenedorDerecho.style.flexDirection = FlexDirection.Column;
        contenedorDerecho.style.alignItems = Align.FlexEnd;
        contenedorDerecho.style.flexShrink = 0;
        contenedorDerecho.style.width = 100;
        contenedorDerecho.style.marginLeft = Length.Auto();

        var btnInfo = new Button(() => MinimizarFooterPOI());
        btnInfo.style.width = 20;
        btnInfo.style.height = 20;
        btnInfo.style.backgroundColor = Color.clear;
        btnInfo.style.borderTopWidth = 0;
        btnInfo.style.borderBottomWidth = 0;
        btnInfo.style.borderLeftWidth = 0;
        btnInfo.style.borderRightWidth = 0;
        btnInfo.style.marginBottom = 15;

        var iconInfoTexture = Resources.Load<Texture2D>("ImagenesUI/Iconos/icon_info");
        if (iconInfoTexture != null)
        {
            btnInfo.style.backgroundImage = iconInfoTexture;
            btnInfo.text = "";
        }
        else
        {
            btnInfo.text = "i";
            btnInfo.style.fontSize = 8;
            btnInfo.style.color = Color.white;
        }

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
        imagen.style.marginRight = -20;
        imagen.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Contain);

        contenedorDerecho.Add(btnInfo);
        contenedorDerecho.Add(imagen);

        contenidoPrincipal.Add(contenedorTexto);
        contenidoPrincipal.Add(contenedorDerecho);

        footerPOI.Add(contenidoPrincipal);
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

        footerPOI.style.display = DisplayStyle.None;
        RestaurarFooterNormalYMostrarIcono();
        footerPOIMinimizado = true;
    }

    private void RestaurarFooterNormalYMostrarIcono()
    {
        contenedorFooter.style.height = alturaOriginalFooter;
        contenedorFooter.style.flexDirection = flexDirectionOriginal;

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

        CrearIconoInfoMinimizado();
    }

    private void CrearIconoInfoMinimizado()
    {
        iconoInfoMinimizado = new VisualElement { name = "IconoInfoMinimizado" };
        iconoInfoMinimizado.style.position = Position.Absolute;
        iconoInfoMinimizado.style.bottom = contenedorFooter.resolvedStyle.height + 90;
        iconoInfoMinimizado.style.left = 25;
        iconoInfoMinimizado.style.width = 40;
        iconoInfoMinimizado.style.height = 30;
        iconoInfoMinimizado.style.backgroundColor = colorFondoFooter;
        iconoInfoMinimizado.style.borderTopLeftRadius = 8;
        iconoInfoMinimizado.style.borderTopRightRadius = 8;
        iconoInfoMinimizado.style.borderBottomLeftRadius = 8;
        iconoInfoMinimizado.style.borderBottomRightRadius = 8;
        iconoInfoMinimizado.style.justifyContent = Justify.Center;
        iconoInfoMinimizado.style.alignItems = Align.Center;

        var btnIconoInfo = new Button(() => ExpandirFooterPOI());
        btnIconoInfo.style.width = 20;
        btnIconoInfo.style.height = 20;
        btnIconoInfo.style.backgroundColor = Color.clear;
        btnIconoInfo.style.borderTopWidth = 0;
        btnIconoInfo.style.borderBottomWidth = 0;
        btnIconoInfo.style.borderLeftWidth = 0;
        btnIconoInfo.style.borderRightWidth = 0;

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

        if (iconoInfoMinimizado != null)
        {
            root.Remove(iconoInfoMinimizado);
            iconoInfoMinimizado = null;
        }

        AnimarReduccionFooterNormal(() =>
        {
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

        contenedorFooter.style.height = alturaOriginalFooter;
        contenedorFooter.style.flexDirection = flexDirectionOriginal;

        if (contenidoNormalFooter != null)
        {
            contenidoNormalFooter.style.display = DisplayStyle.Flex;
        }

        footerPOIActivo = false;
        footerPOIMinimizado = false;
        Debug.Log("Footer normal restaurado");
    }

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
            })
            { text = opcion };
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
        LimpiarEstadoFooterPOI();
    }

    private void LimpiarEstadoFooterPOI()
    {
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

        if (contenedorFooter != null && contenidoNormalFooter != null)
        {
            contenedorFooter.style.height = alturaOriginalFooter;
            contenedorFooter.style.flexDirection = flexDirectionOriginal;
            contenidoNormalFooter.style.display = DisplayStyle.Flex;
        }

        footerPOIActivo = false;
        footerPOIMinimizado = false;
    }

    private void OnDestroy()
    {
        // Limpiar la flecha al destruir el objeto
        if (arrowInstance != null)
        {
            Destroy(arrowInstance);
        }
    }
    
        // ========== COROUTINE DE IOS ==========
    #if UNITY_IOS && !UNITY_EDITOR
        private IEnumerator WaitForFirstValidCoordinate()
        {
            while (true)
            {
                if (UWBLocator.TryGetPosition(out var pos))
                {
                    Debug.Log($"[UWB] Primera coordenada válida recibida: {pos}");
                    CerrarPopupYMostrarTour();
                    yield break;
                }
                Debug.Log("[UWB] Esperando primera coordenada válida de UWB...");
                yield return null;
            }
        }
    #endif
}   