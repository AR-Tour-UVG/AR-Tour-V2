using UnityEngine;

namespace Assets.Minijuegos.Scripts
{
    public abstract class Tiles : MonoBehaviour
    {
        public int hits;
        public Sprite[] sprites;

        public abstract bool TakeDamage();

        // Método para morir
        protected void Die()
        {
            // Destruir el tile
            Destroy(gameObject);
        }
    }
}