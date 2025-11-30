using UnityEngine;
using UnityEngine.Video; // Necesario para controlar videos
using UnityEngine.SceneManagement; // Necesario para cambiar escenas

public class VideoSceneChanger : MonoBehaviour
{
    [Header("Configuración")]
    public VideoPlayer videoPlayer; // Arrastra el componente Video Player aquí (opcional si está en el mismo objeto)
    public string nextSceneName = "Chapter2"; // Escribe aquí el nombre EXACTO de tu nivel 1

    void Start()
    {
        // Si no asignaste el Video Player manualmente en el inspector, lo buscamos en este objeto
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        // Le decimos al video: "Cuando termines (loopPointReached), ejecuta la función 'OnVideoFinished'"
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    // Esta función se ejecuta sola cuando el video termina
    void OnVideoFinished(VideoPlayer vp)
    {
        Debug.Log("Video terminado. Cargando juego...");
        SceneManager.LoadScene(nextSceneName);
    }

    // Opcional: Permitir saltar el video pulsando Espacio
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Intro saltada.");
            SceneManager.LoadScene(nextSceneName);
        }
    }
}