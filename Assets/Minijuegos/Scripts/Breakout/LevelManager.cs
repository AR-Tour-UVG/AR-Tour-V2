using UnityEngine;

namespace Assets.Minijuegos.Scripts
{
    /// <summary>
    /// Algoritmo de generacion de tiles desarrollado con ChatGPT y modificaciones seg�n comportamiento esperado
    /// </summary>
    public class LevelManager : MonoBehaviour
    {

        [SerializeField]
        private GameObject singleTilePrefab;
        [SerializeField]
        private GameObject doubleTilePrefab;

        [SerializeField]
        private int initialRows = 1;
        [SerializeField]
        private int initialColumns = 1;
        [SerializeField]
        private float offsetX = 0.03f;
        [SerializeField]
        private float offsetY = 0.03f;
        [SerializeField]
        private int maxColumns = 5;
        [SerializeField]
        private int maxRows = 5;

        [SerializeField]
        private float tileWidth;
        [SerializeField]
        private float tileHeight;

        private float containerWidth;
        private float containerHeight;
        private int currentLevel = 1;

        private void Start()
        {
            containerWidth = gameObject.GetComponent<BoxCollider2D>().size.x;
            containerHeight = gameObject.GetComponent<BoxCollider2D>().size.y;
            tileWidth = singleTilePrefab.GetComponent<BoxCollider2D>().size.x;
            tileHeight = singleTilePrefab.GetComponent<BoxCollider2D>().size.y;

            GenerateLevel();
        }

        void GenerateLevel()
        {
            // Aumentar la probabilidad de que aparezcan cuadros dobles
            float doubleTileProbability = 0.1f + (currentLevel - 1) / 10.0f;

            if (doubleTileProbability > 1.0f)
            {
                doubleTileProbability = 1.0f;
            }

            // Determinar la cantidad de filas y columnas basado en el nivel actual
            int rows = Mathf.Min(initialRows + (currentLevel - 1), maxRows);
            int columns = Mathf.Min(initialColumns + (currentLevel - 1), maxColumns);

            //// Calcular la posici�n inicial de los cuadros
            float startX = -(columns * tileWidth + (columns - 1) * offsetX) / 2 + tileWidth / 2;
            float startY = containerHeight / 2 - tileHeight / 2;

            float containerMinX = -(containerWidth / 2);
            float containerMaxX = containerWidth / 2;
            float containerMinY = -(containerHeight / 2) + transform.position.y;
            float containerMaxY = (containerHeight / 2) + transform.position.y;

            // Se suma la posici�n del contenedor para que los cuadros se generen en la posici�n correcta
            startY += transform.position.y;

            // Generar cuadros
            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    // Determinar tipo de cuadro
                    GameObject tilePrefab = Random.value < doubleTileProbability ? doubleTilePrefab : singleTilePrefab; 

                    // Calcular la posici�n del cuadro

                    float posX = column == 0 ? startX : startX + (column * (tileWidth + offsetX));
                    float posY = startY - row * (tileHeight + offsetY);

                    // Asegurarse de que la posici�n est� dentro del contenedor
                    if (posX >= containerMinX 
                        && posX <= containerMaxX 
                        && posY >= containerMinY 
                        && posY <= containerMaxY
                    ){
                        Vector3 position = new Vector3(posX, posY, -3);
                        Instantiate(tilePrefab, position, Quaternion.identity, transform);
                    }
                }
            }
        }

        public void NextLevel()
        {
            currentLevel++;

            GenerateLevel();
        }

        public int GetCurrentLevel()
        {
            return currentLevel;
        }

    }
}