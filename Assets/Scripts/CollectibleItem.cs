using UnityEngine;
using UnityEngine.SceneManagement;

public class CollectibleItem : MonoBehaviour
{
    [Header("Tipo de Gema")]
    public bool esGemaDeVideo = false;
    public string nombreEscenaVideo = "IntroVideo";

    [Header("Puntos")]
    public int pointsValue = 10;

    void Start()
    {
        // Si es la gema del video y ya lo vimos, desaparece
        if (esGemaDeVideo && PlayerPrefs.GetInt("VengoDelVideo") == 1)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // CASO A: GEMA DE VIDEO (Te lleva a la escena)
            if (esGemaDeVideo)
            {
                PlayerPrefs.SetInt("VengoDelVideo", 1);
                SceneManager.LoadScene(nombreEscenaVideo);
            }
            // CASO B: GEMA NORMAL (Puntos y Progreso Jefe)
            else
            {
                // 1. Avisamos al LogicManager (Para que cuente 1/3 para el Jefe)
                if (LogicManager.instance != null)
                {
                    LogicManager.instance.SumarGema();
                }

                // 2. Avisamos al GameManager (Para sumar 10 Puntos al marcador)
                // --- AQUÍ ESTABA EL PROBLEMA, YA LO DESCOMENTÉ ---
                if (GameManager.instance != null)
                {
                    GameManager.instance.AddPoints(pointsValue);
                }
                else
                {
                    // Plan B: Si no usas "instance" en tu GameManager, lo buscamos así:
                    GameManager gm = FindFirstObjectByType<GameManager>();
                    if (gm != null) gm.AddPoints(pointsValue);
                }

                // 3. Destruir la gema
                Destroy(gameObject);
            }
        }
    }
}