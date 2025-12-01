using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("¿Es Jefe Final?")]
    public bool esJefe = false; // <--- ¡IMPORTANTE! Marca esta casilla SOLO en el inspector del Duende Jefe

    [Header("Configuración de Movimiento")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 3.5f;
    public Transform[] patrolPoints;
    public float waitTime = 1f;

    [Header("Combate - Distancias")]
    public float attackRange = 1.0f;      // Rango corto para que se acerque bien
    public float detectionRange = 5f;    // Rango de visión

    [Header("Combate - Tiempos y Daño")]
    public float attackCooldown = 1.5f;
    public float damageDelay = 0.4f;      // Ajusta esto para sincronizar el golpe con la animación
    public int damage = 10;

    [Header("Configuración Técnica")]
    public Transform attackPoint;
    public float attackRadius = 0.5f;
    public LayerMask playerLayer;
    public int maxHealth = 50;

    // Variables Privadas
    private int currentHealth;
    private int currentPointIndex = 0;
    private float waitCounter;
    private float nextAttackTime = 0f;
    private bool isDead = false;
    private bool isAttacking = false;

    // --- NUEVA VARIABLE PARA ESQUELETOS QUIETOS ---
    private bool esPasivo = false;

    // VARIABLE DINÁMICA (Cambia según quién esté vivo)
    private Transform currentTarget;

    private Animator anim;
    private Rigidbody2D rb;

    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        // --- VERIFICACIÓN DEL TAG PARA ESQUELETOS QUIETOS ---
        // Si el objeto tiene el tag "EsqueletosQuietos", se marcará como pasivo.
        if (gameObject.CompareTag("EsqueletosQuietos"))
        {
            esPasivo = true;
            // Opcional: Reducir un poco la velocidad si son pasivos
            patrolSpeed = patrolSpeed * 0.8f;
        }

        // --- CORRECCIÓN DE FÍSICA ---
        // Esto evita que el enemigo se caiga o ruede
        rb.freezeRotation = true;
        rb.gravityScale = 3;

        waitCounter = waitTime;
    }

    void Update()
    {
        if (isDead) return;

        // Si está atacando, frenamos todo movimiento para que no patine
        if (isAttacking)
        {
            // NOTA: Si usas Unity antiguo, cambia 'linearVelocity' por 'velocity'
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // --- LÓGICA PARA ESQUELETOS PASIVOS ---
        if (esPasivo)
        {
            // Si es pasivo, solo patrulla y no hace nada más (no persigue ni ataca)
            Patrol();
            return;
        }

        // --- CEREBRO IA NORMAL (Si NO es pasivo) ---
        currentTarget = FindClosestLivingPlayer();

        // 1. Si no encuentra a nadie vivo, Patrulla
        if (currentTarget == null)
        {
            Patrol();
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, currentTarget.position);

        // 2. Decisión de IA (Atacar, Perseguir o Patrullar)
        if (distanceToPlayer <= attackRange)
        {
            AttackPlayer();
        }
        else if (distanceToPlayer <= detectionRange)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    // --- FUNCIÓN CLAVE: BUSCA AL JUGADOR MÁS CERCANO QUE ESTÉ VIVO ---
    Transform FindClosestLivingPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Transform closest = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject player in players)
        {
            PlayerMovement pm = player.GetComponent<PlayerMovement>();

            // Verificamos: ¿Tiene script? ¿Está vivo? ¿Su collider está activo?
            if (pm != null && !pm.isDead && player.GetComponent<Collider2D>().enabled)
            {
                float dist = Vector2.Distance(transform.position, player.transform.position);
                // Solo nos interesa si está dentro del rango de visión
                if (dist < minDistance && dist <= detectionRange)
                {
                    minDistance = dist;
                    closest = player.transform;
                }
            }
        }
        return closest;
    }

    void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            // Si no tiene puntos de patrulla, se queda quieto con animación de idle
            // NOTA: Si usas Unity antiguo, cambia 'linearVelocity' por 'velocity'
            rb.linearVelocity = Vector2.zero;
            anim.SetBool("IsRunning", false);
            return;
        }

        anim.SetBool("IsRunning", true);
        Transform target = patrolPoints[currentPointIndex];

        Vector2 direction = (target.position - transform.position).normalized;
        // NOTA: Si usas Unity antiguo, cambia 'linearVelocity' por 'velocity'
        rb.linearVelocity = new Vector2(direction.x * patrolSpeed, rb.linearVelocity.y);
        LookAt(target.position);

        if (Vector2.Distance(transform.position, target.position) < 0.5f)
        {
            // NOTA: Si usas Unity antiguo, cambia 'linearVelocity' por 'velocity'
            rb.linearVelocity = Vector2.zero;
            anim.SetBool("IsRunning", false);
            waitCounter -= Time.deltaTime;
            if (waitCounter <= 0)
            {
                currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
                waitCounter = waitTime;
            }
        }
    }

    void ChasePlayer()
    {
        if (currentTarget == null) return;

        anim.SetBool("IsRunning", true);
        Vector2 direction = (currentTarget.position - transform.position).normalized;
        // NOTA: Si usas Unity antiguo, cambia 'linearVelocity' por 'velocity'
        rb.linearVelocity = new Vector2(direction.x * chaseSpeed, rb.linearVelocity.y);
        LookAt(currentTarget.position);
    }

    void AttackPlayer()
    {
        // NOTA: Si usas Unity antiguo, cambia 'linearVelocity' por 'velocity'
        rb.linearVelocity = Vector2.zero;
        anim.SetBool("IsRunning", false);

        if (Time.time >= nextAttackTime)
        {
            isAttacking = true;
            anim.SetTrigger("Attack");

            // Golpeamos con retraso para coincidir con la animación
            Invoke("PerformAttackDamage", damageDelay);

            nextAttackTime = Time.time + attackCooldown;

            // Liberamos el movimiento un poco antes de terminar la animación
            Invoke("ResetAttackState", attackCooldown * 0.9f);
        }
    }

    void ResetAttackState()
    {
        isAttacking = false;
    }

    void PerformAttackDamage()
    {
        if (isDead || attackPoint == null) return;

        // Protección por si el jugador huyó lejos durante la animación
        if (currentTarget != null)
        {
            float dist = Vector2.Distance(transform.position, currentTarget.position);
            if (dist > attackRange + 0.5f) return;
        }

        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, playerLayer);
        foreach (Collider2D player in hitPlayers)
        {
            PlayerMovement playerScript = player.GetComponent<PlayerMovement>();
            if (playerScript != null && !playerScript.isDead)
            {
                playerScript.TakeDamage(damage);
            }
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;
        currentHealth -= damageAmount;
        anim.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        CancelInvoke(); // Cancelar ataques pendientes
        anim.SetTrigger("Dead");

        // NOTA: Si usas Unity antiguo, cambia 'linearVelocity' por 'velocity'
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false; // Ya no tiene física
        GetComponent<Collider2D>().enabled = false; // Ya no se puede golpear ni tocar
        this.enabled = false; // Se apaga la IA

        // --- LÓGICA DE JEFE ---
        // Si este enemigo está marcado como Jefe en el inspector, avisamos al Manager
        if (esJefe && LogicManager.instance != null)
        {
            LogicManager.instance.JefeDerrotado();
        }

        Destroy(gameObject, 2f); // Desaparece a los 2 segundos
    }

    void LookAt(Vector3 target)
    {
        if (target.x > transform.position.x)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    private void OnDrawGizmosSelected()
    {
        // Si es pasivo, no dibujamos los rangos de combate para evitar confusión
        if (esPasivo) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        if (attackPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
    }
}