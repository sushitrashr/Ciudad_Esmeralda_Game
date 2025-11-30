using UnityEngine;
using UnityEngine.SceneManagement;

public class GemaViajera : MonoBehaviour
{
    [Header("A dónde vamos")]
    public string nombreEscenaVideo = "GemVideoScene"; // El nombre de tu escena nueva del video

    void Start()
    {
        // Al iniciar el nivel, leemos la nota.
        // Si dice "1", significa que ya vimos el video, así que esta gema debe desaparecer.
        if (PlayerPrefs.GetInt("VengoDelVideo") == 1)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 1. ESCRIBIMOS LA NOTA: "Vengo de ver el video" (1 = Sí)
            PlayerPrefs.SetInt("VengoDelVideo", 1);

            // 2. Nos vamos a la escena del video
            SceneManager.LoadScene(nombreEscenaVideo);
        }
    }
}