using UnityEngine;

public class CambiadorDePantallas : MonoBehaviour
{
    public GameObject pantallaInicio;
    public GameObject pantallaEscaneo;
    public GameObject pantallaTour;

    private void Start()
    {
        MostrarInicio();
    }

    public void MostrarInicio()
    {
        pantallaInicio.SetActive(true);
        pantallaEscaneo.SetActive(false);
        pantallaTour.SetActive(false);
    }

    public void MostrarEscaneo()
    {
        pantallaInicio.SetActive(false);
        pantallaEscaneo.SetActive(true);
        pantallaTour.SetActive(false);
    }

    public void MostrarTour()
    {
        pantallaInicio.SetActive(false);
        pantallaEscaneo.SetActive(false);
        pantallaTour.SetActive(true);
    }
}
