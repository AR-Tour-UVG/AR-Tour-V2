using UnityEngine;
using UnityEngine.UIElements;

public class MenuHamburguesaUIController : MonoBehaviour
{
    private VisualElement menuOverlay;
    private VisualElement menu;
    private Button btnCerrar, btnReconectar, btnReiniciar, btnDiagnostico, btnReportar, btnSalir;

    public CambiadorDePantallas cambiador;
    public bool EstaInicializado { get; private set; } = false;

    public bool Inicializar(VisualElement root)
    {
        Debug.Log("MenuHamburguesaUIController: Inicializando...");

        if (root == null)
        {
            Debug.LogError("El VisualElement root es null. No se puede inicializar el men� hamburguesa.");
            EstaInicializado = false;
            return false;
        }

        // Buscar el overlay que contiene todo el men�
        menuOverlay = root.Q<VisualElement>("menu_overlay");
        if (menuOverlay == null)
        {
            Debug.LogError("No se encontr� el elemento 'menu_overlay' en el UXML.");
            EstaInicializado = false;
            return false;
        }

        // Buscar el men� dentro del overlay
        menu = root.Q<VisualElement>("menu_hamburguesa");
        if (menu == null)
        {
            Debug.LogError("No se encontr� el elemento 'menu_hamburguesa' en el UXML.");
            EstaInicializado = false;
            return false;
        }

        // Obtener referencias a los botones
        btnCerrar = root.Q<Button>("boton_cerrar_menu");
        btnReconectar = root.Q<Button>("btn_reconectar");
        btnReiniciar = root.Q<Button>("btn_reiniciar");
        btnDiagnostico = root.Q<Button>("btn_diagnostico");
        btnReportar = root.Q<Button>("btn_reportar");
        btnSalir = root.Q<Button>("btn_salir_inicio");

        // Configurar eventos
        if (btnCerrar != null)
        {
            Debug.Log("Bot�n cerrar encontrado y configurando evento...");
            btnCerrar.clicked += () => {
                Debug.Log("���Bot�n cerrar clickeado!!!");
                OcultarMenu();
            };

            // Tambi�n agregar callback directo como respaldo
            btnCerrar.RegisterCallback<ClickEvent>((evt) => {
                Debug.Log("���Bot�n cerrar ClickEvent capturado!!!");
                OcultarMenu();
                evt.StopPropagation();
            });
        }
        else
            Debug.LogError("No se encontr� el bot�n cerrar men�");

        if (btnSalir != null)
        {
            if (cambiador != null)
                btnSalir.clicked += () => {
                    OcultarMenu();
                    cambiador.MostrarInicio();
                };
            else
                Debug.LogWarning("CambiadorDePantallas no est� asignado en MenuHamburguesaUIController.");
        }

        if (btnReconectar != null)
            btnReconectar.clicked += () => {
                Debug.Log("Reconectando sensores...");
                OcultarMenu();
            };

        if (btnReiniciar != null)
            btnReiniciar.clicked += () => {
                Debug.Log("Reiniciando tour...");
                OcultarMenu();
            };

        if (btnDiagnostico != null)
            btnDiagnostico.clicked += () => {
                Debug.Log("Mostrando diagn�stico...");
                OcultarMenu();
            };

        if (btnReportar != null)
            btnReportar.clicked += () => {
                Debug.Log("Reporte enviado...");
                OcultarMenu();
            };

        // Configurar el overlay para cerrar el men� al hacer click fuera
        menuOverlay.RegisterCallback<ClickEvent>(OnOverlayClicked);

        // Evitar que los clics en el men� se propaguen al overlay
        menu.RegisterCallback<ClickEvent>(evt => evt.StopPropagation());

        // Estado inicial oculto
        menuOverlay.style.display = DisplayStyle.None;

        // Asegurar que el men� capture clics
        menu.pickingMode = PickingMode.Position;
        menuOverlay.pickingMode = PickingMode.Position;

        EstaInicializado = true;
        Debug.Log("MenuHamburguesaUIController: Men� hamburguesa inicializado correctamente.");
        return true;
    }

    private void OnOverlayClicked(ClickEvent evt)
    {
        // Si el click fue exactamente en el overlay (fondo) pero no en el men�, cerrar
        if (evt.target == menuOverlay)
        {
            Debug.Log("Click en overlay detectado - cerrando men�");
            OcultarMenu();
            evt.StopPropagation();
        }
    }

    public void MostrarMenu()
    {
        Debug.Log("MenuHamburguesaUIController: MostrarMenu() llamado");

        if (!EstaInicializado)
        {
            Debug.LogError("El men� hamburguesa no est� inicializado. Llama a Inicializar() primero.");
            return;
        }

        if (menuOverlay == null)
        {
            Debug.LogError("El elemento menuOverlay es null. No se puede mostrar el men�.");
            return;
        }

        // Mostrar primero
        menuOverlay.style.display = DisplayStyle.Flex;

        // FORZAR que aparezca en el frente - m�todo m�s agresivo
        var parent = menuOverlay.parent;
        if (parent != null)
        {
            // Remover y volver a agregar al final (esto lo pone en la posici�n m�s alta)
            parent.Remove(menuOverlay);
            parent.Add(menuOverlay);
            Debug.Log("Men� reposicionado al final de la jerarqu�a");
        }

        // Asegurar estilo de posicionamiento absoluto
        menuOverlay.style.position = Position.Absolute;
        menuOverlay.style.top = 0;
        menuOverlay.style.left = 0;
        menuOverlay.style.right = 0;
        menuOverlay.style.bottom = 0;
        menuOverlay.style.width = Length.Percent(100);
        menuOverlay.style.height = Length.Percent(100);

        // Forzar picking mode para que capture eventos
        menuOverlay.pickingMode = PickingMode.Position;
        menu.pickingMode = PickingMode.Position;

        // Traer al frente despu�s de reposicionar
        menuOverlay.BringToFront();

        Debug.Log("MenuHamburguesaUIController: Men� mostrado correctamente");

        // Debug adicional
        Debug.Log($"Menu overlay display: {menuOverlay.style.display.value}");
        Debug.Log($"Menu hierarchy index DESPU�S: {menuOverlay.parent?.IndexOf(menuOverlay)}");
        Debug.Log($"Total children en parent: {menuOverlay.parent?.childCount}");
    }

    public void OcultarMenu()
    {
        Debug.Log("MenuHamburguesaUIController: OcultarMenu() llamado");

        if (!EstaInicializado || menuOverlay == null)
            return;

        menuOverlay.style.display = DisplayStyle.None;

        Debug.Log("MenuHamburguesaUIController: Men� ocultado correctamente");
    }
}