using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    void Start()
    {
        // Load NavMesh scene first
        SceneManager.LoadScene("TestRoomV2", LoadSceneMode.Single);
        
        // Then load AR scene on top
        SceneManager.LoadScene("TestArrow", LoadSceneMode.Additive);
    }
}