using UnityEngine;

public sealed class AppLifetime : MonoBehaviour
{
    void Awake()
    {
        if (transform.parent != null)
            transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
    }
}
