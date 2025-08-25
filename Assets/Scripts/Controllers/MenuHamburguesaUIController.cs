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
            Debug.LogError("El VisualElement root es null. No se puede inicializar el menú hamburguesa.");
            EstaInicializado = false;
            return false;
        }

        // Buscar el overlay que contiene todo el menú
        menuOverlay = root.Q<VisualElement>("menu_overlay");
        if (menuOverlay == null)
        {
            Debug.LogError("No se encontró el elemento 'menu_overlay' en el UXML.");
            EstaInicializado = false;
            return false;
        }

        // Buscar el menú dentro del overlay
        menu = root.Q<VisualElement>("menu_hamburguesa");
        if (menu == null)
        {
            Debug.LogError("No se encontró el elemento 'menu_hamburguesa' en el UXML.");
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
            Debug.Log("Botón cerrar encontrado y configurando evento...");
            btnCerrar.clicked += () => {
                Debug.Log("¡¡¡Botón cerrar clickeado!!!");
                OcultarMenu();
            };

            // También agregar callback directo como respaldo
            btnCerrar.RegisterCallback<ClickEvent>((evt) => {
                Debug.Log("¡¡¡Botón cerrar ClickEvent capturado!!!");
                OcultarMenu();
                evt.StopPropagation();
            });
        }
        else
            Debug.LogError("No se encontró el botón cerrar menú");

        if (btnSalir != null)
        {
            if (cambiador != null)
                btnSalir.clicked += () => {
                    OcultarMenu();
                    cambiador.MostrarInicio();
                };
            else
                Debug.LogWarning("CambiadorDePantallas no está asignado en MenuHamburguesaUIController.");
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
                Debug.Log("Mostrando diagnóstico...");
                OcultarMenu();
            };

        if (btnReportar != null)
            btnReportar.clicked += () => {
                Debug.Log("Reporte enviado...");
                OcultarMenu();
            };

        // Configurar el overlay para cerrar el menú al hacer click fuera
        menuOverlay.RegisterCallback<ClickEvent>(OnOverlayClicked);

        // Evitar que los clics en el menú se propaguen al overlay
        menu.RegisterCallback<ClickEvent>(evt => evt.StopPropagation());

        // Estado inicial oculto
        menuOverlay.style.display = DisplayStyle.None;

        // Asegurar que el menú capture clics
        menu.pickingMode = PickingMode.Position;
        menuOverlay.pickingMode = PickingMode.Position;

        EstaInicializado = true;
        Debug.Log("MenuHamburguesaUIController: Menú hamburguesa inicializado correctamente.");
        return true;
    }

    private void OnOverlayClicked(ClickEvent evt)
    {
        // Si el click fue exactamente en el overlay (fondo) pero no en el menú, cerrar
        if (evt.target == menuOverlay)
        {
            Debug.Log("Click en overlay detectado - cerrando menú");
            OcultarMenu();
            evt.StopPropagation();
        }
    }

    public void MostrarMenu()
    {
        Debug.Log("MenuHamburguesaUIController: MostrarMenu() llamado");

        if (!EstaInicializado)
        {
            Debug.LogError("El menú hamburguesa no está inicializado. Llama a Inicializar() primero.");
            return;
        }

        if (menuOverlay == null)
        {
            Debug.LogError("El elemento menuOverlay es null. No se puede mostrar el menú.");
            return;
        }

        // Mostrar primero
        menuOverlay.style.display = DisplayStyle.Flex;

        // FORZAR que aparezca en el frente - método más agresivo
        var parent = menuOverlay.parent;
        if (parent != null)
        {
            // Remover y volver a agregar al final (esto lo pone en la posición más alta)
            parent.Remove(menuOverlay);
            parent.Add(menuOverlay);
            Debug.Log("Menú reposicionado al final de la jerarquía");
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

        // Traer al frente después de reposicionar
        menuOverlay.BringToFront();

        Debug.Log("MenuHamburguesaUIController: Menú mostrado correctamente");

        // Debug adicional
        Debug.Log($"Menu overlay display: {menuOverlay.style.display.value}");
        Debug.Log($"Menu hierarchy index DESPUÉS: {menuOverlay.parent?.IndexOf(menuOverlay)}");
        Debug.Log($"Total children en parent: {menuOverlay.parent?.childCount}");
    }

    public void OcultarMenu()
    {
        Debug.Log("MenuHamburguesaUIController: OcultarMenu() llamado");

        if (!EstaInicializado || menuOverlay == null)
            return;

        menuOverlay.style.display = DisplayStyle.None;

        Debug.Log("MenuHamburguesaUIController: Menú ocultado correctamente");
    }
}