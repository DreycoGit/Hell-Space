using UnityEngine;

// Boss da Fase 3: um meteoro grande e lento que avança em direção ao jogador.
// Bastante vida, mas sem padrão de ataque especial, só avança e causa dano por colisão.
public class MeteoroBoss : BossBase
{
    [SerializeField] private float velocidade = 0.6f;
    [SerializeField] private float danoPorColisao = 30f;

    protected override void Awake()
    {
        vidaMaxima = 800f;
        base.Awake();
    }

    protected override void IniciarBatalha()
    {
        // Sem rotina de ataque — o próprio avanço já é a ameaça.
    }

    private void Update()
    {
        if (!EstaVivo) return;
        transform.position += Vector3.down * velocidade * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Jogador"))
        {
            other.GetComponent<PlayerHealth>()?.ReceberDano(danoPorColisao);
        }
    }
}