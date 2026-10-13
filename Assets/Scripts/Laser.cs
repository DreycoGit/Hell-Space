using UnityEngine;

// Representa um raio laser (usado pelo BossCrimson da Fase 6 e pelo AncientGodBoss da Fase 10).
// Primeiro pisca um "aviso" sem causar dano (tempo pro jogador se esquivar),
// depois fica sólido e ativo causando dano por um tempo, e então some.
public class Laser : MonoBehaviour
{
    [SerializeField] private float tempoAviso = 1f;
    [SerializeField] private float tempoAtivo = 1.5f;
    [SerializeField] private float dano = 20f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Collider2D colisor; // deixe "Is Trigger" marcado no prefab

    [Header("Seguir o boss")]
    [Tooltip("Marque SÓ no prefab do laser do King Crimson (Fase 6). O laser acompanha o boss mantendo a distância em que nasceu.")]
    [SerializeField] private bool seguirBoss = false;

    private Transform alvo;
    private Vector3 distanciaDoAlvo;
    private bool tinhaAlvo;

    private void Start()
    {
        // Se ninguém mandou seguir e o prefab está marcado, procura o boss sozinho.
        if (seguirBoss && !tinhaAlvo)
        {
            BossBase boss = FindFirstObjectByType<BossBase>();
            if (boss != null) SeguirAlvo(boss.transform);
        }

        StartCoroutine(Rotina());
    }

    // O boss chama isto logo depois de criar o laser. Não precisa marcar nada no prefab.
    public void SeguirAlvo(Transform novoAlvo)
    {
        if (novoAlvo == null) return;
        alvo = novoAlvo;
        distanciaDoAlvo = transform.position - alvo.position;
        tinhaAlvo = true;
    }

    private void LateUpdate()
    {
        if (!tinhaAlvo) return;

        if (alvo != null)
            transform.position = alvo.position + distanciaDoAlvo;
        else
            Destroy(gameObject); // o boss foi destruído: o laser some junto
    }

    private System.Collections.IEnumerator Rotina()
    {
        if (colisor != null) colisor.enabled = false;

        float t = 0f;
        while (t < tempoAviso)
        {
            if (spriteRenderer != null)
                spriteRenderer.color = new Color(1f, 1f, 1f, Mathf.PingPong(t * 6f, 1f) * 0.6f + 0.2f);
            t += Time.deltaTime;
            yield return null;
        }

        if (spriteRenderer != null) spriteRenderer.color = Color.white;
        if (colisor != null) colisor.enabled = true;

        yield return new WaitForSeconds(tempoAtivo);

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Jogador"))
        {
            other.GetComponent<PlayerHealth>()?.ReceberDano(dano);
        }
    }
}
