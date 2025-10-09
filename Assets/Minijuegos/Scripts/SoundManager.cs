using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Minijuegos.Scripts
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }
        [SerializeField]
        private AudioSource musicPlayer;
        [SerializeField]
        private AudioSource sfxPlayer;
        [SerializeField]
        private SoundConfig soundConfig;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            PlayGameMusic();
        }

        public void PlayGameMusic()
        {
            musicPlayer.clip = soundConfig.gameMusic.clip;
            musicPlayer.volume = soundConfig.gameMusic.volume;
            musicPlayer.loop = soundConfig.gameMusic.loop;
            musicPlayer.Play();
        }

        public void PlayImpactSound()
        {
            sfxPlayer.clip = soundConfig.impactSound.clip;
            sfxPlayer.volume = soundConfig.impactSound.volume;
            sfxPlayer.loop = soundConfig.impactSound.loop;
            sfxPlayer.Play();
        }
    }
}