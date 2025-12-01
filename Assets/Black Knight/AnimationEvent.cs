using UnityEngine;
using System.Collections;

public class SimpleEnemyHealth : MonoBehaviour // Nombre de la clase cambiado
{
    public float health = 100f; // Vida del enemigo
    public Animator animator; // Referencia al Animator del enemigo

    // Método para recibir daño
    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
        else
        {
            // Reproducir animación de golpe
            int hitAnim = Random.Range(1, 3); // Elegir aleatoriamente entre hit_1 y hit_2
            animator.SetTrigger("hit_" + hitAnim);
        }
    }

    // Método para la muerte del enemigo
    void Die()
    {
        animator.SetTrigger("death");
        // Aquí puedes agregar lógica adicional para la muerte, como desactivar el objeto, soltar botín, etc.
    }
}