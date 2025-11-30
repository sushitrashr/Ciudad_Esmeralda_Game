using UnityEngine;
using UnityEngine.SceneManagement;

public class LogicManager : MonoBehaviour
{
    public static LogicManager instance; // Singleton para que las gemas lo encuentren

    [Header("Vuelta del Video")]
    public GameObject hordaEsqueletos;
    public AudioSource reproductorMusica;
    public AudioClip cancionBatalla;
    public GameObject jugador;
    public Transform puntoReaparicion;

    [Header("Jefe y Gemas")]
    public GameObject duendeNormal; // El que desaparece
    public GameObject duendeJefe;   // El que aparece
    public int totalGemasNecesarias = 5;
    private int gemasActuales = 0;
    public string nombreSiguienteNivel = "Chapter2";

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        // El Jefe empieza oculto
        if (duendeJefe != null) duendeJefe.SetActive(false);

        // Lógica de vuelta del video
        if (PlayerPrefs.GetInt("VengoDelVideo") == 1)
        {
            Debug.Log("Regreso del video: ¡A PELEAR!");

            if (jugador != null && puntoReaparicion != null)
                jugador.transform.position = puntoReaparicion.position;

            if (hordaEsqueletos != null) hordaEsqueletos.SetActive(true);

            if (reproductorMusica != null && cancionBatalla != null)
            {
                reproductorMusica.Stop();
                reproductorMusica.clip = cancionBatalla;
                reproductorMusica.loop = true;
                reproductorMusica.Play();
            }

            Invoke("BorrarMemoria", 0.5f);
        }
        else
        {
            if (hordaEsqueletos != null) hordaEsqueletos.SetActive(false);
        }
    }

    // --- GEMAS Y JEFE ---
    public void SumarGema()
    {
        gemasActuales++;
        if (gemasActuales >= totalGemasNecesarias)
        {
            InvocarJefe();
        }
    }

    void InvocarJefe()
    {
        if (duendeNormal != null) duendeNormal.SetActive(false);
        if (duendeJefe != null) duendeJefe.SetActive(true);
    }

    public void JefeDerrotado()
    {
        Debug.Log("¡Victoria!");
        Invoke("CargarSiguienteNivel", 2f);
    }

    void CargarSiguienteNivel()
    {
        SceneManager.LoadScene(nombreSiguienteNivel);
    }

    void BorrarMemoria()
    {
        PlayerPrefs.SetInt("VengoDelVideo", 0);
        PlayerPrefs.Save();
    }
}