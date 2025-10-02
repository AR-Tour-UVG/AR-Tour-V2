// ============================================================================
// ARTourUIController.cs
// ============================================================================
// Controlador principal de la interfaz del Tour AR
// 
// RESPONSABILIDADES:
// - Gestionar el popup inicial de conexión con sensores
// - Controlar la flecha de navegación AR en 3D
// - Manejar el menú hamburguesa lateral
// - Gestionar el footer de información de puntos de interés (POI)
// - Actualizar dinámicamente la ubicación y las instrucciones del tour
//
// FLUJO DE INICIALIZACIÓN:
// 1. OnEnable() → Inicializa la UI
// 2. InicializarPopupYHeader() → Muestra popup "Esperando Conexión"
// 3. En iOS: Espera coordenadas UWB reales
//    En Editor: Usuario presiona botón "Simular Conexión"
// 4. CerrarPopupYMostrarTour() → Inicia el tour
//
// INTEGRACIÓN CON SENSORES (Para Diego):
// - ActualizarUbicacion(string): Actualiza el texto de ubicación actual
// - ActualizarInstruccion(string, string): Actualiza instrucciones de navegación
// ============================================================================

using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using System.Collections;

public class ARTourUIController : MonoBehaviour
{
    // ============================================================================
    // SECCIÓN 1: VARIABLES DE CONFIGURACIÓN
    // ============================================================================
    
    [SerializeField] private CambiadorDePantallas cambiador;
    [SerializeField] private GameObject arrowPrefab; // Prefab de la flecha 3D de navegación
    
    // ============================================================================
    // SECCIÓN 2: REFERENCIAS UI TOOLKIT
    // ============================================================================
    
    private VisualElement root;
    private VisualElement botonMenuHamburguesa;
    private Button botonSalir;
    
    // ============================================================================
    // SECCIÓN 3: SISTEMA DE FLECHA AR (Navegación 3D)
    // ============================================================================
    
    private GameObject arrowInstance;  // Instancia activa de la flecha en la escena
    private Camera arCamera;           // Cámara AR para posicionamiento
    
    // ============================================================================
    // SECCIÓN 4: MENÚ HAMBURGUESA
    // ============================================================================
    
    private VisualElement menuHamburguesaVisual;
    private bool menuInicializado = false;
    private EventCallback<ClickEvent> onHambClickHandler;
    
    // ============================================================================
    // SECCIÓN 5: SISTEMA DE FOOTER DINÁMICO
    // ============================================================================
    
    // Footer normal (información general del tour)
    private VisualElement contenedorFooter;
    private VisualElement contenidoNormalFooter;
    private StyleLength alturaOriginalFooter;
    private StyleEnum<FlexDirection> flexDirectionOriginal;
    private Color colorFondoFooter = new Color(0f, 0f, 4f / 255f, 0.95f);
    
    // Footer POI (Puntos de Interés - información detallada)
    private VisualElement footerPOI;
    private VisualElement iconoInfoMinimizado;
    private bool footerPOIActivo = false;
    private bool footerPOIMinimizado = false;
    
    // Botón de prueba (simula detección de puntos de interés)
    private Button btnSimularPunto;
    
    // ============================================================================
    // SECCIÓN 6: SISTEMA DE POPUP Y ESTADOS DEL HEADER
    // ============================================================================
    
    // Popup de "Esperando Conexión"
    private VisualElement popupEsperandoConexion;
    private Button botonSimularConexion;
    
    // Estados del header (dos modos: "esperando" y "conectado")
    private VisualElement contenedorEsperandoConexion;  // Modo inicial
    private VisualElement contenedorUbicacionReal;      // Modo activo
    private VisualElement contenedorInstruccion;        // Instrucciones de navegación
    
    // IMPORTANTE: Referencias para actualización dinámica desde sensores
    private Label textoUbicacion;      // Texto: "Admisiones y Recepción"
    private Label textoInstruccion;    // Texto: "Dirigete al Nivel 2..."
    
    // Variable de control para iOS
    #if UNITY_IOS && !UNITY_EDITOR
        private bool _connected = false;
    #endif
    
    // ============================================================================
    // SECCIÓN 7: MÉTODOS DE INICIALIZACIÓN DE UNITY
    // ============================================================================
    
    /// <summary>
    /// Inicializa la flecha de navegación AR al iniciar la escena.
    /// La flecha se instancia, configura con material transparente verde y se posiciona
    /// en el centro de la pantalla.
    /// </summary>
    private void Start()
    {
        // Obtener referencia a la cámara AR
        arCamera = Camera.main;
        if (arCamera == null)
        {
            arCamera = FindFirstObjectByType<Camera>();
        }
        
        // Cargar prefab de flecha desde Resources si no está asignado
        if (arrowPrefab == null)
        {
            arrowPrefab = Resources.Load<GameObject>("3D Models/arrow");
        }
        
        // Instanciar y configurar la flecha
        if (arrowPrefab != null)
        {
            arrowInstance = Instantiate(arrowPrefab);
            ConfigurarFlecha();
            PosicionarFlechaEnCentro();
            arrowInstance.SetActive(true); // Visible por defecto (para pruebas)
        }
        else
        {
            Debug.LogError("No se pudo cargar el prefab de la flecha. Verifica la ruta: Resources/3D Models/arrow");
        }
    }
    
    /// <summary>
    /// Inicializa todos los elementos de UI Toolkit cuando se activa el objeto.
    /// Este método se ejecuta cada vez que la pantalla se muestra.
    /// </summary>
    private void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        
        // Inicializar sistemas en orden
        InicializarPopupYHeader();
        CrearMenuProgramatico();
        ConfigurarBotonesHeader();
        ConfigurarFooterNormal();
        ConfigurarBotonPruebaPOI();
    }
    
    /// <summary>
    /// Mantiene la flecha centrada en pantalla mientras está activa.
    /// Se ejecuta cada frame.
    /// </summary>
    private void Update()
    {
        if (arrowInstance != null && arrowInstance.activeInHierarchy)
        {
            PosicionarFlechaEnCentro();
        }
    }
    
    /// <summary>
    /// Limpia eventos y estado del UI al desactivar el objeto.
    /// </summary>
    private void OnDisable()
    {
        if (botonMenuHamburguesa != null && onHambClickHandler != null)
        {
            botonMenuHamburguesa.UnregisterCallback(onHambClickHandler);
        }
        LimpiarEstadoFooterPOI();
    }
    
    /// <summary>
    /// Destruye la instancia de la flecha al destruir el controlador.
    /// </summary>
    private void OnDestroy()
    {
        if (arrowInstance != null)
        {
            Destroy(arrowInstance);
        }
    }
    
    // ============================================================================
    // SECCIÓN 8: SISTEMA DE FLECHA AR (Métodos públicos para sensores)
    // ============================================================================
    
    /// <summary>
    /// Configura el material de la flecha con color verde transparente.
    /// Se aplica tanto al renderer principal como a los hijos si existen.
    /// </summary>
    private void ConfigurarFlecha()
    {
        if (arrowInstance == null) return;
        
        arrowInstance.transform.localScale = Vector3.one * 0.1f;
        
        Renderer renderer = arrowInstance.GetComponent<Renderer>();
        if (renderer != null)
        {
            AplicarMaterialVerde(renderer);
        }
        else
        {
            // Buscar renderers en objetos hijos
            Renderer[] renderersHijos = arrowInstance.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in renderersHijos)
            {
                AplicarMaterialVerde(r);
            }
        }
    }
    
    /// <summary>
    /// Aplica un material verde transparente al renderer especificado.
    /// Configuración: Standard Shader en modo Transparent, alpha 0.6
    /// </summary>
    private void AplicarMaterialVerde(Renderer renderer)
    {
        Material materialVerde = new Material(Shader.Find("Standard"));
        
        // Configurar transparencia
        materialVerde.SetFloat("_Mode", 3);
        materialVerde.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        materialVerde.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        materialVerde.SetInt("_ZWrite", 0);
        materialVerde.DisableKeyword("_ALPHATEST_ON");
        materialVerde.EnableKeyword("_ALPHABLEND_ON");
        materialVerde.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        materialVerde.renderQueue = 3000;
        
        // Color verde con 60% de opacidad
        materialVerde.color = new Color(0f, 1f, 0f, 0.6f);
        materialVerde.SetFloat("_Metallic", 0.2f);
        materialVerde.SetFloat("_Smoothness", 0.8f);
        
        renderer.material = materialVerde;
    }
    
    /// <summary>
    /// Posiciona la flecha en el centro de la pantalla a 2m de la cámara.
    /// La flecha mantiene la misma rotación que la cámara.
    /// </summary>
    private void PosicionarFlechaEnCentro()
    {
        if (arrowInstance == null || arCamera == null) return;
        
        Vector3 puntoMedio = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        float distanciaDelaCamara = 2f;
        Vector3 posicionMundo = arCamera.ScreenToWorldPoint(
            new Vector3(puntoMedio.x, puntoMedio.y, distanciaDelaCamara)
        );
        
        arrowInstance.transform.position = posicionMundo;
        arrowInstance.transform.rotation = arCamera.transform.rotation;
    }
    
    /// <summary>
    /// MÉTODO PÚBLICO - Muestra la flecha en una posición y rotación específicas.
    /// Usar este método cuando se tengan coordenadas exactas desde los sensores.
    /// </summary>
    /// <param name="posicion">Posición mundial donde colocar la flecha</param>
    /// <param name="rotacion">Rotación de la flecha</param>
    public void MostrarFlecha(Vector3 posicion, Quaternion rotacion)
    {
        if (arrowInstance != null)
        {
            arrowInstance.SetActive(true);
            arrowInstance.transform.position = posicion;
            arrowInstance.transform.rotation = rotacion;
        }
    }
    
    /// <summary>
    /// MÉTODO PÚBLICO - Hace que la flecha apunte hacia un destino específico.
    /// La flecha se mantiene centrada pero rota hacia el destino.
    /// Útil cuando conoces las coordenadas del siguiente punto de interés.
    /// </summary>
    /// <param name="destino">Posición mundial del destino</param>
    public void MostrarFlechaHaciaDestino(Vector3 destino)
    {
        if (arrowInstance == null || arCamera == null) return;
        
        arrowInstance.SetActive(true);
        PosicionarFlechaEnCentro();
        
        Vector3 direccion = (destino - arrowInstance.transform.position).normalized;
        if (direccion != Vector3.zero)
        {
            arrowInstance.transform.rotation = Quaternion.LookRotation(direccion);
        }
    }
    
    /// <summary>
    /// MÉTODO PÚBLICO - Oculta la flecha de navegación.
    /// Usar cuando el usuario llegue al destino o no haya ruta activa.
    /// </summary>
    public void OcultarFlecha()
    {
        if (arrowInstance != null)
        {
            arrowInstance.SetActive(false);
        }
    }
    
    // ============================================================================
    // SECCIÓN 9: SISTEMA DE POPUP Y ESTADOS DEL HEADER
    // ============================================================================
    
    /// <summary>
    /// Inicializa el popup de "Esperando Conexión" y los estados del header.
    /// Este método controla el flujo de conexión inicial con los sensores.
    /// 
    /// COMPORTAMIENTO POR PLATAFORMA:
    /// - iOS: Oculta el botón y espera coordenadas UWB reales
    /// - Editor/Otras: Muestra botón "Simular Conexión" para testing
    /// </summary>
    private void InicializarPopupYHeader()
    {
        // Obtener referencias a elementos del UXML
        popupEsperandoConexion = root.Q<VisualElement>("popup_esperando_conexion");
        botonSimularConexion = root.Q<Button>("BotonSimularSensores");
        
        contenedorEsperandoConexion = root.Q<VisualElement>("ContenedorEsperandoConexion");
        contenedorUbicacionReal = root.Q<VisualElement>("ContenedorUbicacionReal");
        contenedorInstruccion = root.Q<VisualElement>("ContenedorInstruccion");
        
        textoUbicacion = root.Q<Label>("TextoUbicacion");
        textoInstruccion = root.Q<Label>("TextoInstruccion");
        
        // Configurar comportamiento según plataforma
        if (botonSimularConexion != null)
        {
#if UNITY_IOS && !UNITY_EDITOR
            // En iOS: ocultar botón y esperar coordenadas UWB reales
            botonSimularConexion.style.display = DisplayStyle.None;
            if (!_connected)
            {
                _connected = true;
                StartCoroutine(WaitForFirstValidCoordinate());
            }
#else
            // En editor: usar botón de simulación para testing
            botonSimularConexion.clicked += CerrarPopupYMostrarTour;
#endif
        }
        
        MostrarEstadoEsperandoConexion();
    }
    
    /// <summary>
    /// Muestra el estado inicial "Esperando Conexión".
    /// - Popup visible
    /// - Header muestra "Esperando Conexión..." con icono de carga
    /// - Ubicación e instrucciones ocultas
    /// </summary>
    private void MostrarEstadoEsperandoConexion()
    {
        if (popupEsperandoConexion != null)
            popupEsperandoConexion.style.display = DisplayStyle.Flex;
        
        if (contenedorEsperandoConexion != null)
            contenedorEsperandoConexion.style.display = DisplayStyle.Flex;
        
        if (contenedorUbicacionReal != null)
            contenedorUbicacionReal.style.display = DisplayStyle.None;
        
        if (contenedorInstruccion != null)
            contenedorInstruccion.style.display = DisplayStyle.None;
    }
    
    /// <summary>
    /// Cierra el popup y activa el tour.
    /// Cambia el header de "Esperando Conexión" a mostrar ubicación real.
    /// Este método se llama automáticamente cuando:
    /// - En iOS: Se recibe la primera coordenada UWB válida
    /// - En Editor: El usuario presiona "Simular Conexión"
    /// </summary>
    private void CerrarPopupYMostrarTour()
    {
        Debug.Log("Conexión establecida - Mostrando tour");
        
        if (popupEsperandoConexion != null)
            popupEsperandoConexion.style.display = DisplayStyle.None;
        
        if (contenedorEsperandoConexion != null)
            contenedorEsperandoConexion.style.display = DisplayStyle.None;
        
        if (contenedorUbicacionReal != null)
            contenedorUbicacionReal.style.display = DisplayStyle.Flex;
        
        if (contenedorInstruccion != null)
            contenedorInstruccion.style.display = DisplayStyle.Flex;
    }
    
    // ============================================================================
    // SECCIÓN 10: MÉTODOS PÚBLICOS PARA INTEGRACIÓN CON SENSORES
    // ============================================================================
    
    /// <summary>
    /// MÉTODO PÚBLICO - Actualiza el texto de ubicación actual en el header.
    /// 
    /// USO DESDE SENSORES:
    /// Llamar este método cuando los sensores detecten que el usuario
    /// ha cambiado de zona o área del campus.
    /// 
    /// EJEMPLO:
    /// ActualizarUbicacion("Biblioteca Central");
    /// ActualizarUbicacion("Cafetería - Nivel 3");
    /// </summary>
    /// <param name="nombreLugar">Nombre del lugar actual</param>
    public void ActualizarUbicacion(string nombreLugar)
    {
        if (textoUbicacion != null)
        {
            textoUbicacion.text = nombreLugar;
            Debug.Log($"[UI] Ubicación actualizada: {nombreLugar}");
        }
        else
        {
            Debug.LogWarning("[UI] No se pudo actualizar ubicación: textoUbicacion es null");
        }
    }
    
    /// <summary>
    /// MÉTODO PÚBLICO - Actualiza las instrucciones de navegación.
    /// 
    /// USO DESDE SENSORES:
    /// Llamar este método para indicarle al usuario hacia dónde debe dirigirse.
    /// Se actualizará automáticamente el texto en la caja de instrucciones.
    /// 
    /// EJEMPLO:
    /// ActualizarInstruccion("Nivel 2", "Admisiones y Recepción");
    /// ActualizarInstruccion("Nivel 3", "Cafetería");
    /// 
    /// RESULTADO EN UI:
    /// "Dirigete al Nivel 2: Admisiones y Recepción"
    /// </summary>
    /// <param name="nivel">Nivel o piso del destino</param>
    /// <param name="lugarDestino">Nombre del lugar de destino</param>
    public void ActualizarInstruccion(string nivel, string lugarDestino)
    {
        if (textoInstruccion != null)
        {
            textoInstruccion.text = $"Dirigete al {nivel}: {lugarDestino}";
            Debug.Log($"[UI] Instrucción actualizada: {nivel} - {lugarDestino}");
        }
        else
        {
            Debug.LogWarning("[UI] No se pudo actualizar instrucción: textoInstruccion es null");
        }
    }
    
    // ============================================================================
    // SECCIÓN 11: CONFIGURACIÓN DE BOTONES Y EVENTOS
    // ============================================================================
    
    /// <summary>
    /// Configura los botones del header (menú hamburguesa y salir).
    /// </summary>
    private void ConfigurarBotonesHeader()
    {
        botonMenuHamburguesa = root.Q<VisualElement>("icono_menu_hamburguesa");
        if (botonMenuHamburguesa != null)
        {
            onHambClickHandler = (ClickEvent evt) => { MostrarMenuProgramatico(); };
            botonMenuHamburguesa.RegisterCallback(onHambClickHandler);
        }
        
        botonSalir = root.Q<Button>("btn_salir_inicio");
        if (botonSalir != null)
        {
            botonSalir.clicked += () =>
            {
                if (cambiador != null) cambiador.MostrarInicio();
            };
        }
    }
    
    /// <summary>
    /// Configura el footer normal y guarda su estado original.
    /// El footer normal muestra información general del tour (avance, distancia).
    /// </summary>
    private void ConfigurarFooterNormal()
    {
        contenedorFooter = root.Q<VisualElement>("ContenedorFooter");
        if (contenedorFooter != null)
        {
            // Guardar estado original para poder restaurarlo después
            colorFondoFooter = contenedorFooter.resolvedStyle.backgroundColor;
            alturaOriginalFooter = contenedorFooter.style.height;
            flexDirectionOriginal = contenedorFooter.style.flexDirection;
            
            // Crear contenedor wrapper para el contenido original
            contenidoNormalFooter = new VisualElement { name = "ContenidoOriginalFooter" };
            contenidoNormalFooter.style.width = Length.Percent(100);
            contenidoNormalFooter.style.height = Length.Percent(100);
            contenidoNormalFooter.style.flexDirection = FlexDirection.Row;
            
            // Mover hijos originales al wrapper
            var hijosOriginales = contenedorFooter.Children().ToList();
            foreach (var hijo in hijosOriginales)
            {
                contenedorFooter.Remove(hijo);
                contenidoNormalFooter.Add(hijo);
            }
            
            contenedorFooter.Add(contenidoNormalFooter);
        }
    }
    
    /// <summary>
    /// Configura el botón de prueba para simular detección de puntos de interés.
    /// Este botón es temporal para testing - en producción será reemplazado
    /// por detección automática desde sensores.
    /// </summary>
    private void ConfigurarBotonPruebaPOI()
    {
        btnSimularPunto = root.Q<Button>("btn_simular_punto");
        if (btnSimularPunto != null)
        {
            btnSimularPunto.clicked += () =>
            {
                if (!footerPOIActivo)
                {
                    // Ejemplo de datos - reemplazar con datos reales de sensores
                    MostrarFooterPOI(
                        "Makerspace D-Hive:",
                        "D por Diseño, Hive por colmena y colaboración.\n\n" +
                        "Es un espacio de prototipado rápido. Diseñado para desarrollar " +
                        "habilidades de ideación e interacción rápida para proyectos. " +
                        "Es un espacio libre, donde todos los estudiantes pueden venir y usar los equipos.",
                        "ImagenesUI/DragonJack/JackGood"
                    );
                }
            };
        }
    }
    
    // ============================================================================
    // SECCIÓN 12: SISTEMA DE FOOTER POI (Puntos de Interés)
    // ============================================================================
    
    /// <summary>
    /// Muestra el footer de Punto de Interés con información detallada.
    /// El footer POI reemplaza temporalmente el footer normal con una animación.
    /// </summary>
    /// <param name="titulo">Título del punto de interés</param>
    /// <param name="descripcion">Descripción detallada</param>
    /// <param name="rutaImagen">Ruta en Resources/ de la imagen (sin extensión)</param>
    private void MostrarFooterPOI(string titulo, string descripcion, string rutaImagen)
    {
        if (contenedorFooter == null) return;
        
        Debug.Log($"[UI] Mostrando POI: {titulo}");
        
        AnimarReduccionFooterNormal(() =>
        {
            CrearFooterPOI(titulo, descripcion, rutaImagen);
            AnimarAparicionFooterPOI();
        });
        
        footerPOIActivo = true;
    }
    
    /// <summary>
    /// Anima la reducción del footer normal antes de mostrar el POI.
    /// </summary>
    private void AnimarReduccionFooterNormal(System.Action onComplete)
    {
        contenedorFooter.style.height = 4;
        if (contenidoNormalFooter != null)
        {
            contenidoNormalFooter.style.display = DisplayStyle.None;
        }
        StartCoroutine(DelayedCallback(0.3f, onComplete));
    }
    
    /// <summary>
    /// Crea programáticamente el footer de POI con toda su estructura visual.
    /// Incluye: título, descripción, imagen del lugar y botón de minimizar.
    /// </summary>
    private void CrearFooterPOI(string titulo, string descripcion, string rutaImagen)
    {
        // Contenedor principal del POI
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
        
        // Contenedor principal (texto + imagen + botón)
        var contenidoPrincipal = new VisualElement { name = "ContenidoPrincipalPOI" };
        contenidoPrincipal.style.flexDirection = FlexDirection.Row;
        contenidoPrincipal.style.alignItems = Align.FlexStart;
        contenidoPrincipal.style.justifyContent = Justify.SpaceBetween;
        contenidoPrincipal.style.flexGrow = 1;
        
        // Sección de texto (título + descripción)
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
        
        // Sección derecha (botón info + imagen)
        var contenedorDerecho = new VisualElement { name = "ContenedorDerechoPOI" };
        contenedorDerecho.style.flexDirection = FlexDirection.Column;
        contenedorDerecho.style.alignItems = Align.FlexEnd;
        contenedorDerecho.style.flexShrink = 0;
        contenedorDerecho.style.width = 100;
        contenedorDerecho.style.marginLeft = Length.Auto();
        
        // Botón de minimizar
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
        
        // Imagen del lugar
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
        
        // Ensamblar estructura
        contenedorDerecho.Add(btnInfo);
        contenedorDerecho.Add(imagen);
        
        // Ensamblar estructura completa
        contenidoPrincipal.Add(contenedorTexto);
        contenidoPrincipal.Add(contenedorDerecho);
        footerPOI.Add(contenidoPrincipal);
        root.Add(footerPOI);
    }
    
    /// <summary>
    /// Muestra el footer POI con una animación de aparición.
    /// </summary>
    private void AnimarAparicionFooterPOI()
    {
        if (footerPOI != null)
        {
            footerPOI.style.display = DisplayStyle.Flex;
        }
    }
    
    /// <summary>
    /// Minimiza el footer POI y muestra un icono pequeño.
    /// El usuario puede volver a expandirlo clickeando el icono.
    /// </summary>
    private void MinimizarFooterPOI()
    {
        if (footerPOI == null) return;
        
        Debug.Log("[UI] Minimizando footer POI");
        
        footerPOI.style.display = DisplayStyle.None;
        RestaurarFooterNormalYMostrarIcono();
        footerPOIMinimizado = true;
    }
    
    /// <summary>
    /// Restaura el footer normal y crea un icono minimizado del POI.
    /// </summary>
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
    
    /// <summary>
    /// Crea el icono minimizado del POI en la esquina inferior izquierda.
    /// </summary>
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
    
    /// <summary>
    /// Expande el footer POI desde su estado minimizado.
    /// </summary>
    private void ExpandirFooterPOI()
    {
        Debug.Log("[UI] Expandiendo footer POI");
        
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
    
    /// <summary>
    /// Restaura completamente el footer normal, eliminando el POI.
    /// </summary>
    private void RestaurarFooterNormal()
    {
        Debug.Log("[UI] Restaurando footer normal");
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
    }
    
    /// <summary>
    /// Limpia el estado del footer POI al desactivar el controlador.
    /// </summary>
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
    
    /// <summary>
    /// Coroutine auxiliar para ejecutar callbacks con delay.
    /// </summary>
    private System.Collections.IEnumerator DelayedCallback(float delay, System.Action callback)
    {
        yield return new WaitForSeconds(delay);
        callback?.Invoke();
    }
    
    // ============================================================================
    // SECCIÓN 13: MENÚ HAMBURGUESA LATERAL
    // ============================================================================
    
    /// <summary>
    /// Crea programáticamente el menú hamburguesa lateral.
    /// El menú incluye opciones como: Reconectar Sensores, Reiniciar Tour,
    /// Diagnóstico, Reportar Problema, y Salir.
    /// 
    /// NOTA: Las opciones están preparadas pero sin implementación completa.
    /// Solo "Salir a Inicio" está funcional actualmente.
    /// </summary>
    private void CrearMenuProgramatico()
    {
        var outfitSemiBold = Resources.Load<Font>("UI Toolkit/Fonts/TTF/Outfit-SemiBold");
        var outfitRegular = Resources.Load<Font>("UI Toolkit/Fonts/TTF/Outfit-Regular");
        
        // Overlay del menú (60% del ancho de la pantalla)
        var menuOverlay = new VisualElement { name = "menu_overlay_programmatico" };
        menuOverlay.style.position = Position.Absolute;
        menuOverlay.style.top = 0;
        menuOverlay.style.right = 0;
        menuOverlay.style.width = Length.Percent(60);
        menuOverlay.style.height = Length.Percent(100);
        menuOverlay.style.backgroundColor = new Color(0f, 0f, 4f / 255f, 0.95f);
        menuOverlay.style.display = DisplayStyle.None;
        menuOverlay.pickingMode = PickingMode.Position;
        
        // Contenedor del menú
        var menu = new VisualElement { name = "menu_hamburguesa_programmatico" };
        menu.style.width = Length.Percent(100);
        menu.style.height = Length.Percent(100);
        menu.style.flexDirection = FlexDirection.Column;
        
        // Header del menú (título + botón cerrar)
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
        
        // Opciones del menú
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
                
                // Manejar acción según la opción seleccionada
                switch (opcion)
                {
                    case "Salir a Inicio":
                        if (cambiador != null) cambiador.MostrarInicio();
                        break;
                    
                    case "Reconectar Sensores":
                        // TODO: Implementar reconexión con sensores
                        Debug.Log("[Menu] Reconectar Sensores - Por implementar");
                        break;
                    
                    case "Reiniciar Tour":
                        // TODO: Implementar reinicio del tour
                        Debug.Log("[Menu] Reiniciar Tour - Por implementar");
                        break;
                    
                    case "Diagnóstico de Conexión":
                        // TODO: Implementar pantalla de diagnóstico
                        Debug.Log("[Menu] Diagnóstico - Por implementar");
                        break;
                    
                    case "Reportar un Problema":
                        // TODO: Implementar sistema de reportes
                        Debug.Log("[Menu] Reportar Problema - Por implementar");
                        break;
                }
            })
            { text = opcion };
            
            btnOpcion.style.color = Color.white;
            btnOpcion.style.backgroundColor = Color.clear;
            if (outfitRegular != null) btnOpcion.style.unityFont = outfitRegular;
            
            contenedorOpciones.Add(btnOpcion);
        }
        
        // Ensamblar menú
        menu.Add(header);
        menu.Add(contenedorOpciones);
        menuOverlay.Add(menu);
        root.Add(menuOverlay);
        
        menuHamburguesaVisual = menuOverlay;
        menuInicializado = true;
    }
    
    /// <summary>
    /// Muestra el menú hamburguesa lateral.
    /// </summary>
    private void MostrarMenuProgramatico()
    {
        if (!menuInicializado) return;
        
        menuHamburguesaVisual.style.display = DisplayStyle.Flex;
        menuHamburguesaVisual.BringToFront();
    }
    
    // ============================================================================
    // SECCIÓN 14: COROUTINE PARA iOS (Integración con UWB)
    // ============================================================================
    
#if UNITY_IOS && !UNITY_EDITOR
    /// <summary>
    /// Coroutine que espera la primera coordenada válida de los sensores UWB.
    /// Una vez recibida, cierra el popup automáticamente y activa el tour.
    /// 
    /// SOLO SE EJECUTA EN iOS - En editor se usa el botón "Simular Conexión".
    /// 
    /// FLUJO:
    /// 1. Se ejecuta cada frame
    /// 2. Intenta obtener posición de UWBLocator
    /// 3. Si es exitoso → Cierra popup y termina
    /// 4. Si falla → Continúa esperando
    /// </summary>
    private IEnumerator WaitForFirstValidCoordinate()
    {
        while (true)
        {
            if (UWBLocator.TryGetPosition(out var pos))
            {
                Debug.Log($"[UWB] Primera coordenada válida recibida: {pos}");
                CerrarPopupYMostrarTour();
                yield break; // Terminar el coroutine
            }
            
            Debug.Log("[UWB] Esperando primera coordenada válida de UWB...");
            yield return null; // Esperar al siguiente frame
        }
    }
#endif
}

// ============================================================================
// FIN DEL ARCHIVO
// ============================================================================

/* 
 * =============================================================================
 * GUÍA RÁPIDA DE INTEGRACIÓN CON SENSORES (Para Diego)
 * =============================================================================
 * 
 * MÉTODOS PÚBLICOS DISPONIBLES:
 * 
 * 1. ActualizarUbicacion(string nombreLugar)
 *    - Actualiza el texto de ubicación en el header
 *    - Llamar cuando el usuario cambie de zona
 *    - Ejemplo: ActualizarUbicacion("Biblioteca Central");
 * 
 * 2. ActualizarInstruccion(string nivel, string lugarDestino)
 *    - Actualiza las instrucciones de navegación
 *    - Llamar cuando cambien las indicaciones de ruta
 *    - Ejemplo: ActualizarInstruccion("Nivel 3", "Cafetería");
 * 
 * 3. MostrarFlecha(Vector3 posicion, Quaternion rotacion)
 *    - Posiciona la flecha en coordenadas específicas
 *    - Usar cuando tengas posición exacta del destino
 * 
 * 4. MostrarFlechaHaciaDestino(Vector3 destino)
 *    - Hace que la flecha apunte hacia un destino
 *    - Más fácil de usar, solo necesitas las coordenadas del destino
 * 
 * 5. OcultarFlecha()
 *    - Oculta la flecha cuando no hay ruta activa
 * 
 * FLUJO DE CONEXIÓN:
 * - iOS: Se conecta automáticamente al recibir primera coordenada UWB
 * - Editor: Usuario presiona "Simular Conexión"
 * 
 * NOTAS IMPORTANTES:
 * - Todos los textos se actualizan dinámicamente
 * - Las coordenadas se pueden actualizar en tiempo real
 * - El sistema maneja automáticamente los estados de UI
 * 
 * =============================================================================
 */