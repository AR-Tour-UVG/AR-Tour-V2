using UnityEngine;

namespace Assets.Minijuegos.Scripts.FlappyBird
{
    /// <summary>
    /// Ver referencia de tutorial https://github.com/zigurous/unity-flappy-bird-tutorial?tab=readme-ov-file con el código original y los assets.
    /// Obtenido de https://www.youtube.com/watch?v=ihvBiJ1oC9U&list=WL
    /// </summary>
    public class Spawner : MonoBehaviour
    {
        public GameObject prefab;
        public float spawnRate = 1f;
        public float minHeight = -1f;
        public float maxHeight = 1f;

        private void OnEnable()
        {
            InvokeRepeating(nameof(SpawnPipe), spawnRate, spawnRate);
        }

        private void OnDisable()
        {
            CancelInvoke(nameof(SpawnPipe));
        }

        private void SpawnPipe()
        {
            GameObject pipes = Instantiate(prefab, transform.position, Quaternion.identity);
            pipes.transform.position += Vector3.up * Random.Range(minHeight, maxHeight);
        }

    }
}