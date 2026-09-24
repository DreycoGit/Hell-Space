using UnityEngine;

// Mesmo script pro tiro do jogador (tag "TiroJogador", direcao 1) e do inimigo/boss
// (tag "TiroInimigo", direcao -1). A aplicação de dano é centralizada aqui.
[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private float velocidade = 12f;
    [SerializeField] private int direcao = 1; // 1 = sobe (jogador), -1 = desce (inimigo)
    [SerializeField] private float tempoDeVida = 3f;
    [SerializeField] private float danoBase = 1f;

    private void Start()
    {
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.up * velocidade * direcao;
        Destroy(gameObject, tempoDeVida);
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
