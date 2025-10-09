using UnityEngine;

namespace Assets.Minijuegos.Scripts.FlappyBird
{
    /// <summary>
    /// Ver referencia de tutorial https://github.com/zigurous/unity-flappy-bird-tutorial?tab=readme-ov-file con el código original y los assets.
    /// Obtenido de https://www.youtube.com/watch?v=ihvBiJ1oC9U&list=WL
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private GameManager gameManager;

        public float gravity = -9.8f;
        public float strength = 5f;

        public SpriteRenderer spriteRenderer;
        public Sprite[] sprites;
        private int spriteIndex;

        private Vector3 direction;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            InvokeRepeating(nameof(AnimateSprite), 0.15f, 0.15f);
        }

        private void OnEnable()
        {
            Vector3 position = transform.position;
            position.y = 0;
            transform.position = position;
            direction = Vector3.zero;
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                direction = Vector3.up * strength;
            }

            if(Input.touchCount > 0)
            {
               Touch touch = Input.GetTouch(0);

                if(touch.phase == TouchPhase.Began)
                {
                    direction = Vector3.up * strength;
                }
            }

            direction.y += gravity * Time.deltaTime;
            transform.position += direction * Time.deltaTime;
        }

        private void AnimateSprite()
        {
            spriteIndex++;

            if (spriteIndex >= sprites.Length)
            {
                spriteIndex = 0;
            }

            spriteRenderer.sprite = sprites[spriteIndex];
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Obstacle"))
            {
                gameManager.GameOver();
            }
            else if (other.CompareTag("ScoreZone"))
            {
                gameManager.IncreaseScore();
            }
        }
    }
}