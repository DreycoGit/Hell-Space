using UnityEngine;

// Coloque este script no prefab da bala (com Rigidbody2D + Collider2D marcado como "Is Trigger").
public class Bullet : MonoBehaviour
{
    public float speed = 12f;
    public float lifeTime = 3f; // Destroi a bala automaticamente após esse tempo (evita "balas perdidas")

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // Move a bala sempre para "cima" (eixo Y local). Ajuste conforme a orientação do seu sprite.
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Por enquanto só destrói a bala ao colidir com algo marcado como "Enemy"
        if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject); // (isso vai virar sistema de vida/pontos no passo 4)
            Destroy(gameObject);
        }
    }
}

