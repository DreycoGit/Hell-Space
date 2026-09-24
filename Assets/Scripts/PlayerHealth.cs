using UnityEngine;

// Controla a vida do jogador em porcentagem (0 a 100) e troca o sprite
// da nave conforme o estado: cheia (verde), média (amarela) ou baixa (vermelha).
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerHealth : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField] private float vidaMaxima = 100f;
    [SerializeField] private float vidaAtual;

    [Header("Sprites da nave por estado de vida")]
    [SerializeField] private Sprite spriteVidaCheia;   // fundo verde
    [SerializeField] private Sprite spriteVidaMedia;   // fundo amarelo
    [SerializeField] private Sprite spriteVidaBaixa;   // fundo vermelho

    [Header("Limites (em % da vida máxima)")]
    [SerializeField] private float limiteVidaMedia = 66f; // abaixo disso vira "média"
    [SerializeField] private float limiteVidaBaixa = 33f; // abaixo disso vira "baixa"

    [Header("Invulnerabilidade após dano")]
    [SerializeField] private float tempoInvulneravel = 1.2f;

    private SpriteRenderer sprite;
    private bool invulneravel;
    private bool morto;

    // Outros scripts podem ler isso pra saber a vida atual em % (0 a 1), útil pra uma barra de vida.
    public float VidaPercentual => vidaAtual / vidaMaxima;
    public bool EstaVivo => !morto;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        vidaAtual = vidaMaxima;
    }

    private void Start()
    {
        AtualizarSprite();
        GameManager.Instancia?.AtualizarVida(VidaPercentual);
    }

    public void ReceberDano(float quantidade)
    {
        if (invulneravel || morto) return;

        vidaAtual -= quantidade;
        vidaAtual = Mathf.Clamp(vidaAtual, 0f, vidaMaxima);

        AtualizarSprite();
        GameManager.Instancia?.AtualizarVida(VidaPercentual);

        if (vidaAtual <= 0f)
        {
            Morrer();
        }
        else
        {
            StartCoroutine(FicarInvulneravel());
        }
    }

    // Usado pelo power-up de cura.
    public void Curar(float quantidade)
    {
        if (morto) return;

        vidaAtual += quantidade;
        vidaAtual = Mathf.Clamp(vidaAtual, 0f, vidaMaxima);

        AtualizarSprite();
        GameManager.Instancia?.AtualizarVida(VidaPercentual);
    }

    private void AtualizarSprite()
    {
        float percentual = VidaPercentual * 100f;

        if (percentual <= limiteVidaBaixa)
            sprite.sprite = spriteVidaBaixa;
        else if (percentual <= limiteVidaMedia)
            sprite.sprite = spriteVidaMedia;
        else
            sprite.sprite = spriteVidaCheia;
    }

    private System.Collections.IEnumerator FicarInvulneravel()
    {
        invulneravel = true;
        float tempo = 0f;
        while (tempo < tempoInvulneravel)
        {
            sprite.enabled = !sprite.enabled; // pisca a nave
            tempo += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        sprite.enabled = true;
        invulneravel = false;
    }

    private void Morrer()
    {
        morto = true;
        GameManager.Instancia?.FimDeJogo();
        gameObject.SetActive(false);
    }
}
