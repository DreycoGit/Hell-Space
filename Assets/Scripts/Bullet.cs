using UnityEngine;

// Use o mesmo script para o tiro do jogador (direcao = 1, tag "TiroJogador")
// e para o tiro dos inimigos (direcao = -1, tag "TiroInimigo").
[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private float velocidade = 12f;
    [SerializeField] private int direcao = 1; // 1 = sobe (jogador), -1 = desce (inimigo)
    [SerializeField] private float tempoDeVida = 3f;

    private void Start()
    {
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.up * velocidade * direcao;
        Destroy(gameObject, tempoDeVida);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // O tiro do jogador acerta inimigos; a lógica de dano fica no próprio Inimigo.cs,
        // então aqui só cuidamos da limpeza do próprio projétil quando necessário.
        if (CompareTag("TiroJogador") && other.CompareTag("Inimigo"))
        {
            Destroy(gameObject);
        }
    }
}