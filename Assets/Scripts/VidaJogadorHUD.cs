using UnityEngine;

// Controla a vida do jogador em porcentagem (0 a 100).
// O retrato de vida (verde/amarelo/vermelho) agora é feito pelo VidaJogadorHUD.
// A troca de sprite da própria nave continua disponível, mas fica DESLIGADA por padrão.
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerHealth : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField] private float vidaMaxima = 100f;
    [SerializeField] private float vidaAtual;

    [Header("Visual da nave (opcional)")]
    [Tooltip("Deixe DESLIGADO: o retrato de vida é o VidaJogadorHUD. Ligue só se tiver sprites DA NAVE para cada estado.")]
    [SerializeField] private bool trocarSpriteDaNave = false;

    [Header("Sprites da nave por estado de vida (só usados se a opção acima estiver ligada)")]
    [SerializeField] private Sprite spriteVidaCheia;
    [SerializeField] private Sprite spriteVidaMedia;
    [SerializeField] private Sprite spriteVidaBaixa;

    [Header("Limites (em % da vida máxima)")]
    [SerializeField] private float limiteVidaMedia = 66f; // abaixo disso vira "média"
    [SerializeField] private float limiteVidaBaixa = 33f; // abaixo disso vira "baixa"

    [Header("Invulnerabilidade após dano")]
    [SerializeField] private float tempoInvulneravel = 1.2f;

    private SpriteRenderer sprite;
    private bool invulneravel;
    private bool morto;

    // Outros scripts podem ler isso pra saber a vida atual em % (0 a 1).
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
        // Desligado: a nave mantém o sprite que já tem no Sprite Renderer.
        if (!trocarSpriteDaNave) return;

        float percentual = VidaPercentual * 100f;
        Sprite novo;

        if (percentual <= limiteVidaBaixa)
            novo = spriteVidaBaixa;
        else if (percentual <= limiteVidaMedia)
            novo = spriteVidaMedia;
        else
            novo = spriteVidaCheia;

        // Só troca se o sprite foi atribuído (evita a nave sumir).
        if (novo != null)
            sprite.sprite = novo;
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
