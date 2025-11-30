using UnityEngine;

public class ChapterManager : MonoBehaviour
{
    [Header("Referencias a las Cámaras")]
    public Camera cameraP1;
    public Camera cameraP2;

    [Header("Scripts de Seguimiento (Para cambiar límites)")]
    public CameraFollow scriptCamaraP1;
    public CameraFollow scriptCamaraP2;

    [Header("Configuración Capítulo Actual")]
    public int capituloActual = 1;

    void Start()
    {
        // Al iniciar, configuramos el capítulo que hayas puesto en el inspector
        ConfigurarCapitulo(capituloActual);
    }

    // Llama a esta función cuando quieras cambiar de nivel, ej: ConfigurarCapitulo(2);
    public void ConfigurarCapitulo(int capitulo)
    {
        capituloActual = capitulo;

        switch (capitulo)
        {
            case 1: // CAPÍTULO 1: Solo Jugador 1 (Pantalla Completa)
                ModoUnJugador(cameraP1, cameraP2);

                // Opcional: Cambiar límites del mapa para el Cap 1
                // scriptCamaraP1.minPosition = new Vector2(-50, 0); 
                // scriptCamaraP1.maxPosition = new Vector2(50, 20);
                break;

            case 2: // CAPÍTULO 2: Dos Jugadores (Pantalla Dividida)
                ModoPantallaDividida();
                break;

            case 3: // CAPÍTULO 3: Solo Jugador 1 otra vez (o el que quieras)
                ModoUnJugador(cameraP1, cameraP2);
                break;
        }
    }

    void ModoUnJugador(Camera activa, Camera inactiva)
    {
        // Apagamos la cámara secundaria
        if (inactiva != null) inactiva.gameObject.SetActive(false);

        // La cámara principal ocupa toda la pantalla
        if (activa != null)
        {
            activa.gameObject.SetActive(true);
            activa.rect = new Rect(0, 0, 1, 1); // X, Y, Ancho, Alto (Full HD)
        }
    }

    void ModoPantallaDividida()
    {
        // Encendemos ambas cámaras
        if (cameraP1 != null) cameraP1.gameObject.SetActive(true);
        if (cameraP2 != null) cameraP2.gameObject.SetActive(true);

        // Configuramos la división (Arriba y Abajo)
        // Cámara 1 (Arriba)
        cameraP1.rect = new Rect(0, 0.5f, 1, 0.5f);

        // Cámara 2 (Abajo)
        cameraP2.rect = new Rect(0, 0f, 1, 0.5f);
    }
}