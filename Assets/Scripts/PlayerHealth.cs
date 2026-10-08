using UnityEngine;

// Vida do jogador por HITS: cheia (verde) -> média (amarelo) -> baixa (vermelho) -> morte.
// O retrato de vida é trocado pelo VidaJogadorHUD e a tela de Game Over pelo TelaGameOver.
// Este script só controla a vida.
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerHealth : MonoBehaviour
{
    [Header("Modo de dano")]
    [Tooltip("Ligado: cada hit tira 1 estágio (3 hits = verde, amarelo, vermelho, morte). Desligado: usa a vida em porcentagem.")]
    [SerializeField] private bool usarSistemaPorHits = true;
    [SerializeField] private int hitsMaximos = 3;

    [Header("Vida")]
    [SerializeField] private float vidaMaxima = 100f;
    [SerializeField] private float vidaAtual;

    [Header("Visual da nave (opcional)")]
    [Tooltip("Deixe desmarcado: o retrato de vida já é trocado pelo VidaJogadorHUD")]
    [SerializeField] private bool trocarSpriteDaNave = false;

    [Header("Sprites da nave por estado de vida")]
    [SerializeField] private Sprite spriteVidaCheia;
    [SerializeField] private Sprite spriteVidaMedia;
    [SerializeField] private Sprite spriteVidaBaixa;

    [Header("Limites (em % da vida máxima) - só no modo porcentagem")]
    [SerializeField] private float limiteVidaMedia = 66f;
    [SerializeField] private float limiteVidaBaixa = 33f;

    [Header("Invulnerabilidade após dano")]
    [SerializeField] private float tempoInvulneravel = 1.2f;

    private SpriteRenderer sprite;
    private bool invulneravel;
    private bool morto;
    private int hitsRestantes;

    // Outros scripts leem isso (VidaJogadorHUD, TelaGameOver, barra de vida).
    public float VidaPercentual => vidaAtual / vidaMaxima;
    public bool EstaVivo => !morto;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        hitsMaximos = Mathf.Max(1, hitsMaximos);
        hitsRestantes = hitsMaximos;
        vidaAtual = vidaMaxima;
    }

    private void Start()
    {
        AtualizarSprite();
        GameManager.Instancia?.AtualizarVida(VidaPercentual);
    }

    // No modo por hits, o valor de "quantidade" é ignorado: todo dano conta como 1 hit.
    public void ReceberDano(float quantidade)
    {
        if (invulneravel || morto) return;

        if (usarSistemaPorHits)
        {
            hitsRestantes = Mathf.Clamp(hitsRestantes - 1, 0, hitsMaximos);
            vidaAtual = vidaMaxima * hitsRestantes / hitsMaximos;
        }
        else
        {
            vidaAtual = Mathf.Clamp(vidaAtual - quantidade, 0f, vidaMaxima);
        }

        AtualizarSprite();
        GameManager.Instancia?.AtualizarVida(VidaPercentual);

        if (vidaAtual <= 0f) Morrer();
        else StartCoroutine(FicarInvulneravel());
    }

    // Usado pelo power-up de cura. No modo por hits, recupera 1 hit.
    public void Curar(float quantidade)
    {
        if (morto) return;

        if (usarSistemaPorHits)
        {
            hitsRestantes = Mathf.Clamp(hitsRestantes + 1, 0, hitsMaximos);
            vidaAtual = vidaMaxima * hitsRestantes / hitsMaximos;
        }
        else
        {
            vidaAtual = Mathf.Clamp(vidaAtual + quantidade, 0f, vidaMaxima);
        }

        AtualizarSprite();
        GameManager.Instancia?.AtualizarVida(VidaPercentual);
    }

    private void AtualizarSprite()
    {
        if (!trocarSpriteDaNave) return;

        Sprite escolhido;

        if (usarSistemaPorHits)
        {
            if (hitsRestantes >= hitsMaximos) escolhido = spriteVidaCheia;
            else if (hitsRestantes > 1)       escolhido = spriteVidaMedia;
            else                              escolhido = spriteVidaBaixa;
        }
        else
        {
            float p = VidaPercentual * 100f;
            if (p <= limiteVidaBaixa)      escolhido = spriteVidaBaixa;
            else if (p <= limiteVidaMedia) escolhido = spriteVidaMedia;
            else                           escolhido = spriteVidaCheia;
        }

        if (escolhido != null) sprite.sprite = escolhido;
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

    [ContextMenu("Testar Dano")]
    private void TestarDano() => ReceberDano(10f);
}