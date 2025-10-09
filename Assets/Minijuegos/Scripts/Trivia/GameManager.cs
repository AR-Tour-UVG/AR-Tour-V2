
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using System.Linq;

namespace Assets.Minijuegos.Scripts.Trivia
{
    public class GameManager : MonoBehaviour
    {

        [SerializeField]
        private TriviaData triviaData;
        [SerializeField]
        private TriviaURL triviaURL;
        [SerializeField]
        private UIDocument uxml;
        [SerializeField]
        private CategoriesManager categoriesManager;

        string saveFilePath;
        private VisualElement mainMenu;
        private VisualElement questionMenu;
        private Label scoreText;
        private Label rachaText;

        private Pregunta preguntaActual;
        private int puntos = 0;
        private int racha = 0;

        private List<RespuestaButton> respuestas = new List<RespuestaButton>();

        private void Start()
        {
            mainMenu = uxml.rootVisualElement.Q<VisualElement>("main-menu");
            questionMenu = uxml.rootVisualElement.Q<VisualElement>("question-menu");

            scoreText = mainMenu.Q<Label>("ScoreText");
            rachaText = mainMenu.Q<Label>("StreakText");

            mainMenu.style.display = DisplayStyle.Flex;
            questionMenu.style.display = DisplayStyle.None;
            saveFilePath = Application.persistentDataPath + "/TriviaData.json";

            StartCoroutine(GetData());
        }

        private void Update()
        {
            scoreText.text = "" + puntos;
            rachaText.text = "" + racha;
        }

        /// <summary>
        /// Prepara el juego para iniciar una nueva categoría.
        /// </summary>
        public void RegresarAMain()
        {
            mainMenu.style.display = DisplayStyle.Flex;
            questionMenu.style.display = DisplayStyle.None;

            categoriesManager.ResetCategoryButtons();
            categoriesManager.currentCategory = null;
        }

        /// <summary>
        /// Carga la categoría seleccionada a la pantalla respectiva
        /// </summary>
        /// <param name="categoria"></param>
        public void StartCategory(Categoria categoria)
        {
            mainMenu.style.display = DisplayStyle.None;
            questionMenu.style.display = DisplayStyle.Flex;

            var pregunta = GetQuestion(categoria.preguntas);
            preguntaActual = pregunta;

            questionMenu.Q<Label>("nombre-categoria").text = categoria.nombre.ToUpper();
            questionMenu.Q<Label>("pregunta").text = pregunta.pregunta;
            
            ResetRespuestas();

            SetRespuestasButtons(preguntaActual.respuestas);
        }

        /// <summary>
        /// Asigna las respuestas a los botones de la pantalla.
        /// </summary>
        /// <param name="respuestasTexto"></param>
        private void SetRespuestasButtons(List<string> respuestasTexto)
        {
            VisualElement respuestasContainer = questionMenu.Q<VisualElement>("question-menu");

            foreach (var respuestaButton in respuestas)
            {
                if (respuestaButton.button != null)
                {
                    respuestaButton.button.clicked -= respuestaButton.Revisar;  // Desvincular el evento de clic
                }
            }

            for (int i = 0; i < respuestasTexto.Count; i++)
            {
                RespuestaButton respuestaButton = new();
                respuestaButton.Initialize(respuestasContainer, i);
                respuestaButton.SetText(respuestasTexto[i]);
                respuestas.Add(respuestaButton);
            }
        }

        /// <summary>
        /// Reinicia los colores de los botones de respuesta. 
        /// </summary>
        private void ResetRespuestas()
        {
            foreach (var respuesta in respuestas)
            {
                string hexColor = "#ECEAF5";
                ColorUtility.TryParseHtmlString(hexColor, out Color color);
                
                respuesta.button.style.backgroundColor = color;
                respuesta.button.clicked -= respuesta.Revisar;
                respuesta.button.SetEnabled(true);
            }
            respuestas.Clear();
        }

        /// <summary>
        /// Obtiene los datos de la API o del archivo local si no hay conexión a internet.
        /// </summary>
        /// <returns></returns>
        private IEnumerator GetData()
        {
            UnityWebRequest request = UnityWebRequest.Get(triviaURL.URL);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;

                File.WriteAllText(saveFilePath, json);
                SaveTriviaData(json);
            }
            else
            {
                string loadPlayerData = File.ReadAllText(saveFilePath);
                SaveTriviaData(loadPlayerData);

                Debug.LogError("Error: " + request.error);
            }
        }

        /// <summary>
        /// Guarda los datos de la trivia en el scriptable object.
        /// </summary>
        /// <param name="data">Data a guardar</param>
        public void SaveTriviaData(string data)
        {
            triviaData = ScriptableObject.CreateInstance<TriviaData>();

            JsonConvert.PopulateObject(data, triviaData);

            categoriesManager.GenerateButtons(triviaData.categorias);
        }

        /// <summary>
        /// Obtiene una pregunta aleatoria de la lista de preguntas.
        /// </summary>
        /// <param name="preguntas">Listado de preguntas</param>
        /// <returns></returns>
        public Pregunta GetQuestion(List<Pregunta> preguntas)
        {
            var pregunta = preguntas.ToArray()[Random.Range(0, preguntas.Count)];

            if(pregunta == preguntaActual && preguntas.Count > 1)
            {
                return GetQuestion(preguntas);
            }

            return pregunta;
        }

        /// <summary>
        /// Revisa si la respuesta es correcta y actualiza los puntos.
        /// </summary>
        /// <param name="index">Indice seleccionado</param>
        /// <param name="boton">Boton de la respuesta seleccionada</param>
        public void Respuesta(int index, Button boton)
        {
            
            var correcta = preguntaActual.correcta == index;
            boton.style.backgroundColor = correcta ? new StyleColor(new Color(0.82f, 0.98f, 0.65f)) : new StyleColor(new Color(0.980f, 0.37f, 0.33f));
            
            foreach (var respuesta in from respuesta in respuestas
                                      where respuesta.button != boton
                                      select respuesta)
            {
                respuesta.button.SetEnabled(false);
            }

            if (correcta)
            {
                puntos += preguntaActual.puntos;
                racha += 1;
            }
            else
            {

                racha = 0;
            }

            StartCoroutine(Regresar());
        }

        /// <summary>
        /// Regresa a la pantalla principal luego de unos segundos.
        /// </summary>
        /// <returns></returns>
        IEnumerator Regresar()
        {
            yield return new WaitForSeconds(5);
            RegresarAMain();
        }
    }
}