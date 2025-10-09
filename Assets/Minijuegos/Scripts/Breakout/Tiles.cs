using UnityEngine;

namespace Assets.Minijuegos.Scripts.Breakout
{
    public abstract class Tiles : MonoBehaviour
    {
        public int hits;
        public Sprite[] sprites;

        public abstract bool TakeDamage();

        // M�todo para morir
        protected void Die()
        {
            // Destruir el tile
            Destroy(gameObject);
        }
    }
}