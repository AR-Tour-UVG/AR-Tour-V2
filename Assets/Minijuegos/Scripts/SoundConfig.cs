using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Minijuegos.Scripts
{
    [CreateAssetMenu(menuName = "ScriptableObjects/SoundConfig")]
    public class SoundConfig : ScriptableObject
    {
        public SoundData gameMusic;
        public SoundData impactSound;
    }
}