using UnityEngine;
using UnityEngine.UIElements;

public class EscaneoUIController : MonoBehaviour
{
    private VisualElement root;
    private VisualElement botonMenuHamburguesa;
    private VisualElement menuHamburguesaVisual; // La instancia del menú
    private Button botonSimular;
    [SerializeField] private MenuHamburguesaUIController menuHamburguesaController;
    public VisualTreeAsset menuHamburguesaUXML; // Asigna aquí el MenuHamburguesa.uxml desde inspector
    public CambiadorDePantallas cambiador;

    private bool menuInicializado = false;

    private void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        // Validar que los componentes del menú hamburguesa estén asignados
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

        // Solo advertir sobre CambiadorDePantallas, pero no impedir que continúe
        if (cambiador == null)
        {
            Debug.LogWarning("CambiadorDePantallas no está asignado en el inspector. El botón de simular conexión no funcionará.");
        }

        // Instancia el menú hamburguesa y lo añade al root
        menuHamburguesaVisual = menuHamburguesaUXML.CloneTree();
        root.Add(menuHamburguesaVisual);

        // Inicializa el controlador con el VisualElement del menú instanciado
        menuHamburguesaController.Inicializar(menuHamburguesaVisual);
        menuInicializado = true;

        // Configurar el botón del menú hamburguesa
        botonMenuHamburguesa = root.Q<VisualElement>("icono_menu_hamburguesa");
        if (botonMenuHamburguesa != null)
        {
            botonMenuHamburguesa.RegisterCallback<ClickEvent>(evt => OnBotonMenuHamburguesaClicked());
        }
        else
        {
            Debug.LogError("No se encontró el VisualElement 'icono_menu_hamburguesa' en el UXML.");
        }

        // Configurar el botón de simular conexión
        ConfigurarBotonSimularConexion();
    }

    private void OnDisable()
    {
        if (botonMenuHamburguesa != null)
        {
            botonMenuHamburguesa.UnregisterCallback<ClickEvent>(evt => OnBotonMenuHamburguesaClicked());
        }
    }

    private void ConfigurarBotonSimularConexion()
    {
        botonSimular = root.Q<Button>("BotonSimularSensores");
        if (botonSimular != null)
        {
            // BYPASS TEMPORAL PARA SIMULAR CONEXIÓN DE 3 SENSORES
            // TODO: Reemplazar este bypass por la lógica real de conexión a sensores.
            // Cuando se detecten al menos 3 sensores, llamar a MostrarTour();
            // Ejemplo:
            // cambiador.MostrarTour():
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

        // Validar que todo esté correctamente inicializado antes de mostrar el menú
        if (menuInicializado && menuHamburguesaController != null)
        {
            Debug.Log("EscaneoUIController: Menu inicializado, llamando a MostrarMenu...");
            menuHamburguesaController.MostrarMenu();
        }
        else
        {
            Debug.LogError("El menú hamburguesa no está correctamente inicializado. Verifica las asignaciones en el inspector.");
        }
    }
}