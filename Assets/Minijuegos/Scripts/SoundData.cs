using System;
using UnityEngine;

namespace Assets.Minijuegos.Scripts
{
    [Serializable]
    public class SoundData
    {
        public AudioClip clip;
        public float volume;
        public bool loop;
    }
}