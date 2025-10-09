using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Minijuegos.Scripts.Trivia
{
    [CreateAssetMenu(menuName = "ScriptableObjects/TriviaData")]
    public class TriviaData : ScriptableObject
    {
        public List<Categoria> categorias;
    }

    [Serializable]
    public class Categoria
    {
        public string nombre;
        public string background_color;
        public List<Pregunta> preguntas;
    }

    [Serializable]
    public class Pregunta
    {
        public string pregunta;
        public List<string> respuestas;
        public int correcta;
        public int puntos;
    }
}