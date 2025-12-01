using UnityEngine;
using TMPro; // Necesario para usar TextMeshPro en la UI
using UnityEngine.SceneManagement; // Necesario para cambiar de escena

public class GameManager : MonoBehaviour
{
    // SINGLETON: Permite acceder a este script desde cualquier otro lugar usando 'GameManager.instance'
    public static GameManager instance;

    [Header("--- Configuración de UI (Puntos y Tiempo) ---")]
    // Arrastra aquí los objetos de texto de tu Canvas
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;

    [Header("--- Configuración del Juego (Tiempo) ---")]
    public float timeRemaining = 90; // Tiempo inicial en segundos para el nivel

    [Header("--- Configuración de Nivel (Gemas y Cambio de Escena) ---")]
    // El nombre EXACTO de la siguiente escena a cargar (ej: "Capitulo4")
    public string nextLevelName = "Chapter3";
    // Cuántas gemas se necesitan recoger para pasar de nivel
    public int gemsNeededToWin = 2;

    // --- Variables Privadas ---
    private int score = 0;              // Puntuación actual
    private bool timerIsRunning = false; // Controla si el reloj está corriendo
    private int gemsCollected = 0;      // Contador de gemas recogidas en este nivel

    // --- CICLO DE VIDA DE UNITY ---

    void Awake()
    {
        // Configuración del Patrón Singleton
        if (instance == null)
        {
            // Si es la primera vez que se carga, esta es la instancia principal.
            instance = this;
            // Opcional: Si quieres que el puntaje persista entre escenas, descomenta la siguiente línea:
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Si ya existe una instancia, destruimos la nueva para evitar duplicados y conflictos.
            Debug.LogWarning("¡Cuidado! Se ha detectado y destruido un GameManager duplicado.");
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Al empezar el nivel:
        UpdateScoreText();     // Mostramos la puntuación inicial (0)
        timerIsRunning = true; // Arrancamos el reloj
        gemsCollected = 0;     // Reiniciamos el contador de gemas del nivel
        Debug.Log("Nivel iniciado. Gemas necesarias: " + gemsNeededToWin);
    }

    void Update()
    {
        // Lógica del Temporizador
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                // Restamos el tiempo transcurrido
                timeRemaining -= Time.deltaTime;
                // Actualizamos el texto del reloj
                DisplayTime(timeRemaining);
            }
            else
            {
                // ¡Se acabó el tiempo!
                timeRemaining = 0;
                timerIsRunning = false;
                DisplayTime(timeRemaining); // Aseguramos que muestre 00:00

                Debug.Log("¡Se acabó el tiempo!");
                // AQUÍ PUEDES AGREGAR TU LÓGICA DE "GAME OVER". Por ejemplo:
                // GameOver();
            }
        }
    }

    // --- MÉTODOS PÚBLICOS (Accesibles desde otros scripts) ---

    // Llama a esta función para sumar puntos (desde monedas, enemigos, etc.)
    public void AddPoints(int pointsToAdd)
    {
        score += pointsToAdd;
        UpdateScoreText();
    }

    // Llama a esta función desde el script de las gemas cuando se recoge una.
    public void GemCollected()
    {
        gemsCollected++; // Aumentamos el contador
        Debug.Log("¡Gema recogida! Total: " + gemsCollected + " de " + gemsNeededToWin);

        // Verificamos si se han recogido todas las gemas necesarias
        if (gemsCollected >= gemsNeededToWin)
        {
            LoadNextLevel();
        }
    }

    // --- MÉTODOS PRIVADOS (Lógica interna) ---

    // Actualiza el texto de puntuación en la UI
    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Puntos: " + score.ToString();
        }
    }

    // Formatea el tiempo a MM:SS y actualiza la UI
    void DisplayTime(float timeToDisplay)
    {
        if (timeToDisplay < 0) timeToDisplay = 0;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        if (timerText != null)
        {
            timerText.text = string.Format("Tiempo: {0:00}:{1:00}", minutes, seconds);
        }
    }

    // Carga el siguiente nivel
    void LoadNextLevel()
    {
        Debug.Log("¡Todas las gemas recogidas! Nivel completado. Cargando: " + nextLevelName);
        // Opcional: Aquí podrías detener el tiempo o guardar la puntuación antes de cambiar.
        timerIsRunning = false;
        // Cargar la escena. Asegúrate de que la escena esté añadida en File > Build Settings.
        SceneManager.LoadScene(nextLevelName);
    }

    // Ejemplo de método para Game Over (puedes implementarlo si lo necesitas)
    /*
    private void GameOver() {
        Debug.Log("Juego Terminado");
        // Mostrar pantalla de Game Over, reiniciar nivel, etc.
        // SceneManager.LoadScene("GameOverScene");
    }
    */
}