using UnityEngine;

namespace Assets.Minijuegos.Scripts.FlappyBird
{
    /// <summary>
    /// Ver referencia de tutorial https://github.com/zigurous/unity-flappy-bird-tutorial?tab=readme-ov-file con el código original y los assets.
    /// Obtenido de https://www.youtube.com/watch?v=ihvBiJ1oC9U&list=WL
    /// </summary>
    public class Pipes : MonoBehaviour
    {
        public float speed = 5f;
        private float leftEdge;

        private void Start()
        {
            leftEdge = Camera.main.ScreenToWorldPoint(Vector3.zero).x - 1f;
        }

        private void Update()
        {
            transform.position += Vector3.left * speed * Time.deltaTime;

            if (transform.position.x < leftEdge)
            {
                Destroy(gameObject);
            }
        }
    }
}