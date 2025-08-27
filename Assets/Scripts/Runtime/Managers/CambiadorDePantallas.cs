using UnityEngine;

public class CambiadorDePantallas : MonoBehaviour
{
    [Header("Onboarding Screens")]
    public GameObject onboarding1;
    public GameObject onboarding2;
    public GameObject onboarding3;
    public GameObject onboarding4;

    [Header("Pantallas principales")]
    public GameObject pantallaInicio;
    public GameObject pantallaEscaneo;
    public GameObject pantallaTour;

    private int onboardingIndex = 1;

    private void Start()
    {
        // Descomentar en PROD
        //if (PlayerPrefs.GetInt("OnboardingCompletado", 0) == 0)
        //{
        //    MostrarOnboarding(1);
        //}
        //else
        //{
        //    MostrarInicio();
        //}
        PlayerPrefs.DeleteKey("OnboardingCompletado"); // Borrar para probar onboarding siempre
        MostrarOnboarding(1);
    }

    private void ApagarTodas()
    {
        onboarding1.SetActive(false);
        onboarding2.SetActive(false);
        onboarding3.SetActive(false);
        onboarding4.SetActive(false);

        pantallaInicio.SetActive(false);
        pantallaEscaneo.SetActive(false);
        pantallaTour.SetActive(false);
    }

    public void MostrarOnboarding(int index)
    {
        ApagarTodas();

        onboarding1.SetActive(index == 1);
        onboarding2.SetActive(index == 2);
        onboarding3.SetActive(index == 3);
        onboarding4.SetActive(index == 4);

        onboardingIndex = index;
    }

    public void SiguienteOnboarding()
    {
        if (onboardingIndex < 4)
        {
            MostrarOnboarding(onboardingIndex + 1);
        }
        else
        {
            // Termina onboarding
            // PlayerPrefs.SetInt("OnboardingCompletado", 1); DESCOMENTAR LINEA CUANDO SALGA A PROD
            MostrarInicio();
        }
    }

    public void MostrarInicio()
    {
        ApagarTodas();
        pantallaInicio.SetActive(true);
    }

    public void MostrarEscaneo()
    {
        ApagarTodas();
        pantallaEscaneo.SetActive(true);
    }

    public void MostrarTour()
    {
        ApagarTodas();
        pantallaTour.SetActive(true);
    }
}
