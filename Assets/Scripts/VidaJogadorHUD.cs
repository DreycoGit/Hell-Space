using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Mostra o retrato de vida do jogador no HUD.
// Verde (cheia) -> Amarelo (média) -> Vermelho (baixa).
// Coloque este script na Image do Canvas. Ele lê a vida do PlayerHealth sozinho.
[RequireComponent(typeof(Image))]
public class VidaJogadorHUD : MonoBehaviour
{
    [Header("Referência do jogador")]
    [Tooltip("Arraste a nave aqui. Se deixar vazio, o script procura sozinho.")]
    [SerializeField] private PlayerHealth jogador;

    [Header("Sprites do retrato")]
    [SerializeField] private Sprite vidaAlta;    // vida_alta.png  (verde)
    [SerializeField] private Sprite vidaMedia;   // vida_media.png (amarelo)
    [SerializeField] private Sprite vidaBaixa;   // vida_baixa.png (vermelho)

    [Header("Limites (0 a 1)")]
    [Tooltip("A partir daqui o retrato fica verde (0.99 = só com vida cheia)")]
    [SerializeField] private float limiteAlta = 0.99f;
    [Tooltip("Acima disso fica amarelo; abaixo ou igual fica vermelho")]
    [SerializeField] private float limiteMedia = 0.34f;

    [Header("Efeito ao trocar de estado")]
    [SerializeField] private bool tremerAoMudar = true;
    [SerializeField] private float duracaoTremida = 0.25f;
    [SerializeField] private float forcaTremida = 8f;

    private Image imagem;
    private Sprite spriteAtual;
    private Vector2 posicaoOriginal;
    private RectTransform rt;
    private Coroutine tremida;

    private void Awake()
    {
        imagem = GetComponent<Image>();
        rt = GetComponent<RectTransform>();
        posicaoOriginal = rt.anchoredPosition;
    }

    private void Start()
    {
        ProcurarJogador();
        Atualizar(false);
    }

    private void Update()
    {
        // Se a nave ainda não existia no Start (ex: é criada por prefab), continua procurando.
        if (jogador == null)
            ProcurarJogador();

        Atualizar(true);
    }

    private void ProcurarJogador()
    {
#if UNITY_2023_1_OR_NEWER
        jogador = FindFirstObjectByType<PlayerHealth>();
#else
        jogador = FindObjectOfType<PlayerHealth>();
#endif
    }

    private void Atualizar(bool comEfeito)
    {
        if (jogador == null) return;

        float p = jogador.VidaPercentual;
        Sprite novo;

        if (p >= limiteAlta)       novo = vidaAlta;
        else if (p > limiteMedia)  novo = vidaMedia;
        else                       novo = vidaBaixa;

        if (novo == spriteAtual) return;

        bool primeiraVez = spriteAtual == null;
        spriteAtual = novo;
        imagem.sprite = novo;

        if (comEfeito && !primeiraVez && tremerAoMudar && gameObject.activeInHierarchy)
        {
            if (tremida != null) StopCoroutine(tremida);
            tremida = StartCoroutine(Tremer());
        }
    }

    private IEnumerator Tremer()
    {
        float t = 0f;
        while (t < duracaoTremida)
        {
            Vector2 offset = Random.insideUnitCircle * forcaTremida;
            rt.anchoredPosition = posicaoOriginal + offset;
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        rt.anchoredPosition = posicaoOriginal;
    }
}