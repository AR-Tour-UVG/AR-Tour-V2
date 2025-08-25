using UnityEngine;
using UnityEngine.UIElements;

public class MenuHamburguesaUIController : MonoBehaviour
{
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

        menu = root.Q<VisualElement>("menu_hamburguesa");
        if (menu == null)
        {
            Debug.LogError("No se encontró el elemento 'menu_hamburguesa' en el UXML.");
            EstaInicializado = false;
            return false;
        }

        btnCerrar = root.Q<Button>("boton_cerrar_menu");
        btnReconectar = root.Q<Button>("btn_reconectar");
        btnReiniciar = root.Q<Button>("btn_reiniciar");
        btnDiagnostico = root.Q<Button>("btn_diagnostico");
        btnReportar = root.Q<Button>("btn_reportar");
        btnSalir = root.Q<Button>("btn_salir_inicio");

        if (btnCerrar != null) btnCerrar.clicked += OcultarMenu;
        if (btnSalir != null)
        {
            if (cambiador != null) btnSalir.clicked += () => cambiador.MostrarInicio();
            else Debug.LogWarning("CambiadorDePantallas no está asignado en MenuHamburguesaUIController.");
        }
        if (btnReconectar != null) btnReconectar.clicked += () => Debug.Log("Reconectando sensores...");
        if (btnReiniciar != null) btnReiniciar.clicked += () => Debug.Log("Reiniciando tour...");
        if (btnDiagnostico != null) btnDiagnostico.clicked += () => Debug.Log("Mostrando diagnóstico...");
        if (btnReportar != null) btnReportar.clicked += () => Debug.Log("Reporte enviado...");

        // Estado inicial oculto (por código, sin depender de clases USS)
        menu.style.display = DisplayStyle.None;
        menu.style.visibility = Visibility.Hidden;
        menu.style.opacity = 0f;

        // Asegura que el panel capte clics
        menu.pickingMode = PickingMode.Position;

        EstaInicializado = true;
        Debug.Log("MenuHamburguesaUIController: Menú hamburguesa inicializado correctamente.");
        return true;
    }

    public void MostrarMenu()
    {
        Debug.Log("MenuHamburguesaUIController: MostrarMenu() llamado");

        if (!EstaInicializado)
        {
            Debug.LogError("El menú hamburguesa no está inicializado. Llama a Inicializar() primero.");
            return;
        }

        if (menu == null)
        {
            Debug.LogError("El elemento menu es null. No se puede mostrar el menú.");
            return;
        }

        // Traer al frente y forzar overlay lateral
        menu.BringToFront();
        menu.style.position = Position.Absolute;
        // Respeta tu UXML (width:50%) y que pegue a la derecha:
        menu.style.top = 0; menu.style.bottom = 0; menu.style.right = 0;

        menu.style.display = DisplayStyle.Flex;
        menu.style.visibility = Visibility.Visible;
        menu.style.opacity = 1f;
    }

    public void OcultarMenu()
    {
        if (!EstaInicializado) return;
        if (menu == null) return;

        menu.style.display = DisplayStyle.None;
        menu.style.visibility = Visibility.Hidden;
        menu.style.opacity = 0f;
    }
}
