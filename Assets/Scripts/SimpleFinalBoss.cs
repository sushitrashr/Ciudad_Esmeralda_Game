using UnityEngine;
using UnityEngine.Video;
using System.Collections;
using UnityEngine.SceneManagement; // <-- ¡NUEVO IMPORTANTE! Para poder cambiar de escena

public class SimpleFinalBoss : MonoBehaviour
{
    [Header("--- Configuración General ---")]
    public float health = 400f;
    public float moveSpeed = 3f;
    public float detectionRange = 10f;

    [Header("--- Configuración del Final ---")]
    // Nombre exacto de tu escena del menú principal
    public string mainMenuSceneName = "MainMenu"; // <-- ¡NUEVO!
    // Duración exacta de tu animación "Death"
    public float deathAnimationDuration = 2.5f;

    private bool isDead = false;

    [Header("--- Ataque Melee (Cerca) ---")]
    public float meleeRange = 2f;
    public float meleeCooldown = 2f;
    public float meleeDamage = 25f;
    private float nextMeleeTime = 0f;

    [Header("--- Ataque Mágico (Lejos) ---")]
    public float spellCooldown = 6f;
    public float spellDamage = 40f;
    private float nextSpellTime = 0f;
    private bool isCasting = false;
    private bool shouldChase = false;

    [Header("--- Referencias ---")]
    public VideoPlayer videoPlayerEnd;
    public GameObject uiToHide;
    public LayerMask playerLayer;

    private Transform currentTarget;
    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer; // Necesitamos esto para ocultarlo al final

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // Obtenemos el renderer

        rb.freezeRotation = true;
        rb.gravityScale = 1;

        nextSpellTime = Time.time + 3f;

        if (videoPlayerEnd != null) { videoPlayerEnd.Stop(); videoPlayerEnd.gameObject.SetActive(false); }
        if (rb == null) Debug.LogError("¡ERROR! El Jefe necesita Rigidbody2D Dynamic.");
    }

    void Update()
    {
        if (isDead) { shouldChase = false; return; }

        currentTarget = FindClosestLivingPlayer();

        if (currentTarget == null)
        {
            StopMoving();
            animator.SetBool("isMoving", false);
            shouldChase = false;
            return;
        }

        LookAt(currentTarget.position);

        if (isCasting) { shouldChase = false; return; }

        float distanceToPlayer = Vector2.Distance(transform.position, currentTarget.position);

        if (Time.time >= nextSpellTime && distanceToPlayer > meleeRange)
        {
            CastSpellAttack();
            shouldChase = false;
            return;
        }

        if (distanceToPlayer > meleeRange)
        {
            animator.SetBool("isMoving", true);
            shouldChase = true;
        }
        else
        {
            StopMoving();
            animator.SetBool("isMoving", false);
            shouldChase = false;

            if (Time.time >= nextMeleeTime)
            {
                MeleeAttack();
                nextMeleeTime = Time.time + meleeCooldown;
            }
        }
    }

    void FixedUpdate()
    {
        if (isDead || isCasting || currentTarget == null || rb == null) return;

        if (shouldChase)
        {
            Vector2 direction = (currentTarget.position - transform.position).normalized;
            // NOTA: Si usas Unity antiguo, cambia 'linearVelocity' por 'velocity'
            rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);
        }
    }

    Transform FindClosestLivingPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Transform closest = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject player in players)
        {
            PlayerMovement pm = player.GetComponent<PlayerMovement>();
            if (pm != null && !pm.isDead && player.activeInHierarchy)
            {
                float dist = Vector2.Distance(transform.position, player.transform.position);
                if (dist < minDistance && dist <= detectionRange)
                {
                    minDistance = dist;
                    closest = player.transform;
                }
            }
        }
        return closest;
    }

    void StopMoving()
    {
        if (rb != null)
        {
            // NOTA: Si usas Unity antiguo, cambia 'linearVelocity' por 'velocity'
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    void LookAt(Vector3 target)
    {
        if (target.x > transform.position.x)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    void MeleeAttack()
    {
        animator.SetTrigger("triggerAttack");
        if (currentTarget != null)
        {
            DealDamageToTarget(currentTarget, meleeDamage);
        }
        Debug.Log("Jefe usa Ataque Melee.");
    }

    void CastSpellAttack()
    {
        StopMoving();
        animator.SetBool("isMoving", false);
        shouldChase = false;
        isCasting = true;

        animator.SetTrigger("triggerSpell");

        if (currentTarget != null)
        {
            DealDamageToTarget(currentTarget, spellDamage);
        }
        nextSpellTime = Time.time + spellCooldown;

        Debug.Log("¡Jefe lanza HECHIZO MÁGICO!");
        Invoke("FinishCasting", 1.5f);
    }

    void FinishCasting()
    {
        isCasting = false;
    }

    void DealDamageToTarget(Transform target, float amount)
    {
        PlayerMovement playerScript = target.GetComponent<PlayerMovement>();
        if (playerScript != null)
        {
            playerScript.TakeDamage((int)amount);
        }
    }

    public void TakeDamage(float damageAmount)
    {
        if (isDead) return;
        health -= damageAmount;

        Debug.Log("Jefe herido. Vida restante: " + health);

        if (health <= 0)
        {
            Die();
            return;
        }

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        bool isAttacking = stateInfo.IsName("Attack") || stateInfo.IsName("Cast") || stateInfo.IsName("Spell");

        if (!isAttacking && !isCasting)
        {
            animator.SetTrigger("triggerHurt");
        }
    }

    void Die()
    {
        isDead = true;
        health = 0;
        StopMoving();
        shouldChase = false;

        animator.ResetTrigger("triggerHurt");
        animator.ResetTrigger("triggerAttack");
        animator.ResetTrigger("triggerSpell");

        animator.SetBool("isDead", true);
        animator.SetBool("isMoving", false);

        if (rb != null) rb.simulated = false;
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Debug.Log("Jefe muerto. Iniciando secuencia final...");
        // Iniciamos la corrutina que maneja todo el final
        StartCoroutine(EndSequenceCoroutine());
    }

    // --- NUEVA CORRUTINA DE SECUENCIA FINAL ---
    IEnumerator EndSequenceCoroutine()
    {
        // 1. Espera que termine la animación de caer muerto
        yield return new WaitForSeconds(deathAnimationDuration);

        // Verificación de seguridad
        if (videoPlayerEnd == null || videoPlayerEnd.clip == null)
        {
            Debug.LogError("¡ERROR! Falta el VideoPlayer o el VideoClip. Cargando menú directamente.");
            SceneManager.LoadScene(mainMenuSceneName);
            yield break; // Salimos de la corrutina
        }

        // 2. Ocultar la UI y preparar el video
        if (uiToHide != null) uiToHide.SetActive(false);
        Debug.Log("Reproduciendo video final...");

        // Ocultamos visualmente al jefe (pero no lo destruimos aún porque necesitamos este script corriendo)
        if (spriteRenderer != null) spriteRenderer.enabled = false;

        videoPlayerEnd.gameObject.SetActive(true);
        videoPlayerEnd.Play();

        // 3. Esperar exactamente la duración del video
        // (El "clip.length" nos da los segundos que dura el video)
        float videoDuration = (float)videoPlayerEnd.clip.length;
        Debug.Log("Esperando " + videoDuration + " segundos a que termine el video.");
        yield return new WaitForSeconds(videoDuration);

        // 4. Cargar la escena del menú principal
        Debug.Log("Video terminado. Cargando escena: " + mainMenuSceneName);
        // Pequeña espera extra por seguridad antes de cambiar
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeRange);
    }
}