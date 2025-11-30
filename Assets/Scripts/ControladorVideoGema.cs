using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ControladorVideoGema : MonoBehaviour
{
    public float duracionVideo = 8f; // Cuánto dura tu video de la gema
    public string nombreEscenaNivel = "Chapter1"; // El nombre de tu nivel

    void Start()
    {
        // Arrancamos el cronómetro nada más empezar la escena
        StartCoroutine(EsperarYVolver());
    }

    IEnumerator EsperarYVolver()
    {
        // Esperamos los segundos del video
        yield return new WaitForSeconds(duracionVideo);

        // Volvemos al nivel para pelear
        SceneManager.LoadScene(nombreEscenaNivel);
    }
}