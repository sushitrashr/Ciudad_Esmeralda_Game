using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Identificación")]
    public string playerName = "Player 1";

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
    public LayerMask enemyLayers;
    public int attackDamage = 40; // Aumentado el daño para que el combate sea más ágil

    [Header("Suelo")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.2f;

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
        if (isDead) return;

        // --- SUELO ---
        if (groundCheck != null)
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // --- MOVIMIENTO ---
        float moveInput = 0f;
        if (Input.GetKey(leftKey)) moveInput = -1f;
        if (Input.GetKey(rightKey)) moveInput = 1f;

        bool isRunning = Input.GetKey(runKey);
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        // NOTA: Si usas Unity antiguo, cambia 'linearVelocity' por 'velocity'
        rb.linearVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);

        // --- ANIMACIONES ---
        if (anim != null)
        {
            anim.SetFloat("Speed", Mathf.Abs(moveInput));
            anim.SetBool("IsRunning", isRunning && moveInput != 0);
            anim.SetBool("IsJumping", !isGrounded);
        }

        // --- GIRO ---
        if (moveInput > 0.1f)
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        else if (moveInput < -0.1f)
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);

        // --- ACCIONES ---
        if (Input.GetKeyDown(jumpKey) && isGrounded)
            // NOTA: Si usas Unity antiguo, cambia 'linearVelocity' por 'velocity'
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        // --- ATAQUE Y BLOQUEO SIMULTÁNEOS ---
        if (Input.GetKeyDown(attackKey))
        {
            // Ahora se puede atacar siempre, incluso si se está bloqueando
            Attack();
        }

        if (Input.GetKeyDown(blockKey) && anim != null) anim.SetBool("IsBlocking", true);
        if (Input.GetKeyUp(blockKey) && anim != null) anim.SetBool("IsBlocking", false);
    }

    // --- FUNCIÓN DE ATAQUE CORREGIDA (Evita golpes dobles) ---
    void Attack()
    {
        if (anim != null) anim.SetTrigger("Attack");
        if (attackPoint == null) return;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // Usamos un HashSet para guardar los objetos raíz únicos que ya hemos golpeado en este ataque
        System.Collections.Generic.HashSet<GameObject> damagedObjects = new System.Collections.Generic.HashSet<GameObject>();

        foreach (Collider2D enemyCollider in hitEnemies)
        {
            // Obtenemos el objeto raíz del enemigo (por si golpeamos un brazo o una pierna)
            GameObject enemyRoot = enemyCollider.attachedRigidbody ? enemyCollider.attachedRigidbody.gameObject : enemyCollider.gameObject;

            // Si ya hemos dañado a este objeto raíz en este mismo ataque, lo saltamos
            if (damagedObjects.Contains(enemyRoot)) continue;

            bool damageDealt = false;

            // Intentamos dañar como enemigo normal
            EnemyAI enemyScript = enemyRoot.GetComponent<EnemyAI>();
            if (enemyScript != null)
            {
                enemyScript.TakeDamage(attackDamage);
                damageDealt = true;
            }
            // Si no es normal, intentamos como JEFE FINAL
            else
            {
                SimpleFinalBoss bossScript = enemyRoot.GetComponent<SimpleFinalBoss>();
                if (bossScript != null)
                {
                    bossScript.TakeDamage(attackDamage);
                    damageDealt = true;
                }
            }

            // Si logramos hacer daño, marcamos el objeto raíz como dañado
            if (damageDealt)
            {
                damagedObjects.Add(enemyRoot);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (anim != null)
        {
            anim.SetTrigger("Hurt");
            // Forzamos dejar de bloquear si nos pegan para evitar bugs
            anim.SetBool("IsBlocking", false);
        }

        Debug.Log(playerName + " recibe daño. Vida restante: " + currentHealth);

        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        Debug.Log(playerName + " ha muerto.");
        isDead = true;
        currentHealth = 0;
        if (anim != null) anim.SetTrigger("Dead");
        rb.simulated = false;
        Collider2D myCollider = GetComponent<Collider2D>();
        if (myCollider != null) myCollider.enabled = false;
        this.enabled = false;
        Invoke("TriggerDeathCamera", 1.5f);
        Destroy(gameObject, 3f);
    }

    void TriggerDeathCamera()
    {
        // Asegúrate de que esta clase exista en tu proyecto
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