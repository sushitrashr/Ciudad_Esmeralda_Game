using UnityEngine;

public class GemPickup : MonoBehaviour
{
    // Cuando alguien entra en el trigger de la gema
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verificamos si el objeto que entró es un jugador (tiene el tag "Player")
        if (other.CompareTag("Player"))
        {
            // Avisamos al GameManager que una gema ha sido recogida.
            // Usamos la instancia estática para acceder fácilmente al GameManager.
            if (GameManager.instance != null)
            {
                GameManager.instance.GemCollected();
                Debug.Log("¡Gema recogida por " + other.name + "!");
            }
            else
            {
                Debug.LogError("¡ERROR! No se encontró el GameManager en la escena.");
            }

            // Destruimos la gema para que desaparezca
            Destroy(gameObject);
        }
    }
}