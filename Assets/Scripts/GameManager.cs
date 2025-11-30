using UnityEngine;
using TMPro; // Necesario para usar TextMeshPro

public class GameManager : MonoBehaviour
{
    // SINGLETON: Esto permite que el script del Jugador encuentre este script fácilmente
    // usando 'GameManager.instance' sin tener que arrastrar referencias.
    public static GameManager instance;

    [Header("Referencias UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;

    [Header("Configuración del Juego")]
    public float timeRemaining = 90; // Tiempo en segundos

    // Variables privadas para controlar la lógica
    private int score = 0;
    private bool timerIsRunning = false;

    void Awake()
    {
        // Configuración del Singleton
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            // Si por error hay otro GameManager (ej. al recargar escena), destruimos el nuevo
            // para mantener la integridad.
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateScoreText();
        timerIsRunning = true;
    }

    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime; // Restar tiempo
                DisplayTime(timeRemaining);
            }
            else
            {
                timeRemaining = 0;
                timerIsRunning = false;
                Debug.Log("¡Se acabó el tiempo!");
                // Aquí puedes agregar lógica de Game Over, como:
                // UnityEngine.SceneManagement.SceneManager.LoadScene("GameOver");
            }
        }
    }

    // Esta función es llamada desde el script PlayerMovement cuando recoge una moneda
    public void AddPoints(int pointsToAdd)
    {
        score += pointsToAdd;
        UpdateScoreText();
    }

    // Actualiza el texto en pantalla
    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Puntos: " + score.ToString();
        }
    }

    // Formatea el tiempo para que se vea como 01:30 en lugar de 90
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
}