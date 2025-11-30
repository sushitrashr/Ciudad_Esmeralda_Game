using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Identificación")]
    public string playerName = "Player 1"; // IMPORTANTE: "Player 1" o "Player 2"

    [Header("Sistema de Vida")]
    public int maxHealth = 100;
    public int currentHealth;
    public bool isDead = false;

    [Header("Configuración de Teclas")]
    public KeyCode leftKey;
    public KeyCode rightKey;
    public KeyCode jumpKey;
    public KeyCode runKey;
    public KeyCode attackKey;
    public KeyCode blockKey;

    [Header("Velocidades")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float jumpForce = 10f;

    [Header("Combate")]
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers; // ¡Asegúrate de seleccionar la layer 'Enemy' aquí!
    public int attackDamage = 20;

    [Header("Suelo")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.2f;

    // Componentes
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private bool isGrounded;
    private Vector3 originalScale;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (isDead) return; // Si está muerto, no hace nada

        // --- 1. SUELO ---
        if (groundCheck != null)
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // --- 2. MOVIMIENTO ---
        float moveInput = 0f;
        if (Input.GetKey(leftKey)) moveInput = -1f;
        if (Input.GetKey(rightKey)) moveInput = 1f;

        bool isRunning = Input.GetKey(runKey);
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        rb.linearVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);

        // --- 3. ANIMACIONES ---
        if (anim != null)
        {
            anim.SetFloat("Speed", Mathf.Abs(moveInput));
            anim.SetBool("IsRunning", isRunning && moveInput != 0);
            anim.SetBool("IsJumping", !isGrounded);
        }

        // --- 4. GIRO (Dirección) ---
        if (moveInput > 0.1f)
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        else if (moveInput < -0.1f)
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);

        // --- 5. ACCIONES ---
        if (Input.GetKeyDown(jumpKey) && isGrounded)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        if (Input.GetKeyDown(attackKey))
            Attack();

        // Bloqueo
        if (Input.GetKeyDown(blockKey) && anim != null) anim.SetBool("IsBlocking", true);
        if (Input.GetKeyUp(blockKey) && anim != null) anim.SetBool("IsBlocking", false);
    }

    // --- FUNCIÓN DE ATAQUE CORREGIDA Y CON DEBUG ---
    void Attack()
    {
        // 1. Activar animación
        if (anim != null) anim.SetTrigger("Attack");

        // 2. Seguridad
        if (attackPoint == null) return;

        // 3. Detectar colisiones en la capa Enemigo
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // DEBUG: Para saber si estamos tocando algo
        if (hitEnemies.Length > 0)
            Debug.Log("¡Golpeaste a " + hitEnemies.Length + " enemigos!");
        else
            Debug.Log("Golpe al aire (Ajusta el rango o acércate más)");

        // 4. Aplicar daño
        foreach (Collider2D enemy in hitEnemies)
        {
            // Buscamos el script 'EnemyAI' en el objeto golpeado
            EnemyAI enemyScript = enemy.GetComponent<EnemyAI>();

            if (enemyScript != null)
            {
                enemyScript.TakeDamage(attackDamage);
            }
        }
    }

    public void TakeDamage(int damage = 10)
    {
        if (isDead) return;

        currentHealth -= damage;
        if (anim != null) anim.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // --- FUNCIÓN DE MUERTE OPTIMIZADA ---
    void Die()
    {
        Debug.Log(playerName + " ha muerto.");
        isDead = true;
        if (anim != null) anim.SetTrigger("Dead");

        // 1. Quitar física
        rb.simulated = false;

        // 2. APAGAR COLLIDER: Para que el enemigo deje de detectar este cadáver y busque al otro jugador
        Collider2D myCollider = GetComponent<Collider2D>();
        if (myCollider != null) myCollider.enabled = false;

        // 3. Desactivar controles
        this.enabled = false;

        // 4. Cambiar cámara (Llama al SplitScreenManager)
        Invoke("TriggerDeathCamera", 1.5f);

        // 5. Destruir el cuerpo después de 3 segundos (Limpieza)
        Destroy(gameObject, 3f);
    }

    void TriggerDeathCamera()
    {
        if (SplitScreenManager.instance != null)
        {
            SplitScreenManager.instance.PlayerDied(playerName);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null) { Gizmos.color = Color.red; Gizmos.DrawWireSphere(attackPoint.position, attackRange); }
        if (groundCheck != null) { Gizmos.color = Color.blue; Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius); }
    }
}