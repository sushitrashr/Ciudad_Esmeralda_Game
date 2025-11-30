using UnityEngine;

public class SplitScreenManager : MonoBehaviour
{
    public static SplitScreenManager instance;

    [Header("Cámaras de Jugadores")]
    public Camera cameraP1; // Arrastra aquí la Camera_P1
    public Camera cameraP2; // Arrastra aquí la Camera_P2

    // Si tienes una cámara negra de fondo, ponla aquí, si no, déjalo vacío
    public GameObject backgroundCamera;

    void Awake()
    {
        // Esto permite que el script sea accesible desde cualquier lado
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    // Esta función se activará cuando un jugador muera
    public void PlayerDied(string deadPlayerName)
    {
        Debug.Log("Murió el jugador: " + deadPlayerName);

        if (deadPlayerName == "Player 1")
        {
            // Si muere el 1, apagamos su cámara...
            if (cameraP1 != null) cameraP1.gameObject.SetActive(false);

            // ...y la cámara del 2 se hace gigante (pantalla completa)
            if (cameraP2 != null)
            {
                cameraP2.rect = new Rect(0, 0, 1, 1);
            }
        }
        else if (deadPlayerName == "Player 2")
        {
            // Si muere el 2, apagamos su cámara...
            if (cameraP2 != null) cameraP2.gameObject.SetActive(false);

            // ...y la cámara del 1 se hace gigante
            if (cameraP1 != null)
            {
                cameraP1.rect = new Rect(0, 0, 1, 1);
            }
        }

        // Si usabas una cámara de fondo negra, la apagamos para que no estorbe
        if (backgroundCamera != null) backgroundCamera.SetActive(false);
    }
}