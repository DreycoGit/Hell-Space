using UnityEngine;
using UnityEngine.InputSystem; // Necessário para usar o novo Input System

// Coloque este script no GameObject do jogador (junto com um Rigidbody2D e um Collider2D).
// Requer o pacote "Input System" instalado (Window > Package Manager).
public class PlayerController : MonoBehaviour
{
    [Header("Movimento")]
    public float speed = 8f;              // Velocidade de deslocamento lateral/vertical
    public float minX = -8f;               // Limite esquerdo da tela
    public float maxX = 8f;                // Limite direito da tela
    public float minY = -4.5f;             // Limite inferior (se quiser permitir mover em Y)
    public float maxY = 4.5f;              // Limite superior

    [Header("Tiro")]
    public GameObject bulletPrefab;        // Arraste o prefab da bala aqui no Inspector
    public Transform firePoint;            // Um objeto filho vazio na "boca" da nave
    public float fireCooldown = 0.3f;      // Tempo mínimo entre tiros
    private float lastFireTime;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        HandleShooting();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        // Lê o teclado diretamente via novo Input System (sem precisar criar um Input Actions asset)
        Vector2 moveInput = Vector2.zero;
        var keyboard = Keyboard.current;

        if (keyboard != null)
        {
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) moveInput.x -= 1f;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) moveInput.x += 1f;
            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) moveInput.y += 1f;
            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) moveInput.y -= 1f;
        }

        Vector2 movement = moveInput * speed;
        Vector2 newPosition = rb.position + movement * Time.fixedDeltaTime;

        // Trava o jogador dentro dos limites da tela
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

        rb.MovePosition(newPosition);
    }

    void HandleShooting()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        // Espaço para atirar (wasPressedThisFrame também funciona bem se preferir tiro único por clique)
        if (keyboard.spaceKey.isPressed && Time.time >= lastFireTime + fireCooldown)
        {
            Shoot();
            lastFireTime = Time.time;
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning("Bullet prefab ou FirePoint não configurado no Inspector!");
            return;
        }

        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }
}
