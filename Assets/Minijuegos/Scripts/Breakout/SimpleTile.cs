using UnityEngine;

namespace Assets.Minijuegos.Scripts.Breakout
{
    public class SimpleTile : Tiles
    {
        // Start is called before the first frame update
        void Start()
        {
            hits = 1;
            GetComponentInChildren<SpriteRenderer>().sprite = sprites[0];
        }

        public override bool TakeDamage()
        {
            hits--;
            SoundManager.Instance.PlayImpactSound();
            if (hits <= 0)
            {
                Die();
                return true;
            }
            return false;
        }
    }
}