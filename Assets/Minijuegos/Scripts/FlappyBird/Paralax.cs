using UnityEngine;

namespace Assets.Minijuegos.Scripts.FlappyBird
{
    /// <summary>
    /// Ver referencia de tutorial https://github.com/zigurous/unity-flappy-bird-tutorial?tab=readme-ov-file con el código original y los assets.
    /// Obtenido de https://www.youtube.com/watch?v=ihvBiJ1oC9U&list=WL
    /// </summary>
    public class Paralax : MonoBehaviour
    {
        private MeshRenderer meshRenderer;
        public float animationSpeed = 1f;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        private void Update()
        {
            meshRenderer.material.mainTextureOffset += new Vector2(animationSpeed * Time.deltaTime, 0f);
        }
    }
}