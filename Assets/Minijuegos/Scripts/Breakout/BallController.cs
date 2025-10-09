using UnityEngine;

namespace Assets.Minijuegos.Scripts.Breakout
{
    public class BallController : MonoBehaviour
    {
        Rigidbody2D rb;

        [SerializeField]
        private GameManager gameManager;
        [SerializeField]
        private float bounceForce;
        [SerializeField]
        private Transform startPosition;


        private bool isPlaying;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            isPlaying = false;

        }

        private void Update()
        {
            if (!isPlaying && Input.anyKeyDown)
            {
                StartBounce();
                isPlaying = true;
            }
        }

        void StartBounce()
        {
            Vector2 randomDirection = new Vector2(Random.Range(-1f, 1f), 1);

            rb.AddForce(randomDirection.normalized * bounceForce, ForceMode2D.Impulse);
        }

        public void ResetBall()
        {
            gameObject.SetActive(true);
            transform.position = startPosition.position;
            rb.linearVelocity = Vector2.zero;
            isPlaying = false;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.CompareTag("Limit"))
            {
                gameManager.LoseLife();
                isPlaying = false;

                gameObject.SetActive(false);
                Invoke("ResetBall", 0.5f);

            }
            else if(collision.collider.CompareTag("Tile"))
            {
                var died = collision.collider.GetComponent<Tiles>().TakeDamage();
                
                if (died && gameManager.UpdateTiles())
                {
                    isPlaying = false;
                }
            }
        }

    }
}