using UnityEngine;

// Mesmo script pro tiro do jogador (tag "TiroJogador", direcao 1) e do inimigo/boss
// (tag "TiroInimigo", direcao -1). A aplicação de dano é centralizada aqui.
[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private float velocidade = 12f;
    [SerializeField] private int direcao = 1; // 1 = sobe (jogador), -1 = desce (inimigo)
    [SerializeField] private float danoBase = 1f;

    [Header("Segurança (valem mesmo que o prefab tenha valores antigos)")]
    [Tooltip("A velocidade nunca passa disso. Evita o tiro sair da tela em 1 frame.")]
    [SerializeField] private float velocidadeMaxima = 20f;
    [Tooltip("Se o tiro passar dessa distância (em Y), ele é destruído.")]
    [SerializeField] private float limiteY = 12f;

    private void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f; // sem gravidade puxando o tiro

        float velocidadeFinal = Mathf.Min(velocidade, velocidadeMaxima);
        rb.linearVelocity = Vector2.up * velocidadeFinal * direcao;
    }

    private void Update()
    {
        // Rede de segurança: se saiu muito da tela, some.
        if (Mathf.Abs(transform.position.y) > limiteY)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // O tiro do jogador acerta inimigos comuns OU bosses; o dano final já
        // considera o multiplicador do power-up "Mais Dano", se estiver ativo.
        if (!CompareTag("TiroJogador")) return;

        float dano = danoBase;
        PlayerCombat combate = FindAnyObjectByType<PlayerCombat>();
        if (combate != null) dano *= combate.MultiplicadorDeDano;

        Enemy inimigo = other.GetComponent<Enemy>();
        if (inimigo != null)
        {
            inimigo.LevarDano(dano);
            Destroy(gameObject);
            return;
        }

        BossBase boss = other.GetComponent<BossBase>();
        if (boss != null)
        {
            boss.ReceberDano(dano);
            Destroy(gameObject);
        }
    }
}