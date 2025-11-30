using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Objetivo")]
    // IMPORTANTE: Arrastra aquí a tu Jugador 1 o Jugador 2 desde la Jerarquía
    public Transform target;

    [Header("Configuración")]
    // La distancia de la cámara (Z debe ser negativo para 2D, ej: -10)
    public Vector3 offset = new Vector3(0, 0, -10);

    [Header("Límites del Mapa")]
    public bool activarLimites = true; // Casilla para activar o desactivar los topes

    // Estos valores definen el rectángulo permitido para la cámara
    // Configúralos moviendo la cámara manualmente en la escena para encontrar los bordes
    public Vector2 minPosition; // Límite Izquierda (X) y Abajo (Y)
    public Vector2 maxPosition; // Límite Derecha (X) y Arriba (Y)

    private void LateUpdate()
    {
        // Verificamos si el target existe (por si el jugador muere o se destruye)
        if (target != null)
        {
            // 1. Calculamos la posición donde la cámara DEBERÍA estar (siguiendo al jugador)
            Vector3 desiredPosition = target.position + offset;

            // 2. Si los límites están activados, restringimos esa posición
            if (activarLimites)
            {
                // Mathf.Clamp obliga al número a quedarse entre el mínimo y el máximo.
                // Si el jugador se sale del mapa, la cámara se queda pegada al borde.
                desiredPosition.x = Mathf.Clamp(desiredPosition.x, minPosition.x, maxPosition.x);
                desiredPosition.y = Mathf.Clamp(desiredPosition.y, minPosition.y, maxPosition.y);
            }

            // 3. Aplicamos la posición final a la cámara
            // Mantenemos la Z del offset para no perder de vista los sprites 2D
            transform.position = new Vector3(desiredPosition.x, desiredPosition.y, offset.z);
        }
    }
}