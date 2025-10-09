using UnityEngine;

namespace Assets.Minijuegos.Scripts.Breakout
{
    public class DoubleTiles : Tiles
    {
        void Start()
        {
            hits = 2;
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
            else
            {
                GetComponentInChildren<SpriteRenderer>().sprite = sprites[1];
            }
            return false;
        }
    }
}