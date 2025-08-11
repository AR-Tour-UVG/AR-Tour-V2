using UnityEngine;

public class CambiadorDePantallas : MonoBehaviour
{
    [Header("Pantallas")]
    public GameObject pantallaOnBoarding;
    public GameObject pantallaInicio;
    public GameObject pantallaEscaneo;
    public GameObject pantallaTour;

    private void Start()
    {
        if (PlayerPrefs.GetInt("OnboardingCompletado", 0) == 0)
        {
            MostrarPantalla("Onboarding");
        }
        else
        {
            MostrarPantalla("Inicio");
        }
    }

    public void MostrarPantalla(string nombrePantalla)
    {
        pantallaOnBoarding.SetActive(false);
        pantallaInicio.SetActive(false);
        pantallaEscaneo.SetActive(false);
        pantallaTour.SetActive(false);

        switch (nombrePantalla)
        {
            case "Onboarding":
                pantallaOnBoarding.SetActive(true);
                break;
            case "Inicio":
                pantallaInicio.SetActive(true);
                break;
            case "Escaneo":
                pantallaEscaneo.SetActive(true);
                break;
            case "Tour":
                pantallaTour.SetActive(true);
                break;
            default:
                Debug.LogWarning("Pantalla no encontrada: " + nombrePantalla);
                break;
        }
    }

    // Métodos antiguos para compatibilidad
    public void MostrarInicio() => MostrarPantalla("Inicio");
    public void MostrarEscaneo() => MostrarPantalla("Escaneo");
    public void MostrarTour() => MostrarPantalla("Tour");
}
