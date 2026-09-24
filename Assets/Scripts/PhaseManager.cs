using UnityEngine;

public enum TipoDeFase { InimigosComuns, Boss }

[System.Serializable]
public class ConfiguracaoDeFase
{
    public string nome;
    public TipoDeFase tipo;

    [Header("Se for InimigosComuns — uma linha do array por fileira, da mais fraca (0) à mais forte")]
    public GameObject[] linhasDeInimigos;
    public int colunas = 6;
    public float espacoX = 1.5f;
    public float espacoY = 1.2f;
    public Vector3 origemFormacao = new Vector3(-4f, 4f, 0f);
    public float velocidadeFormacao = 1.5f;

    [Header("Se for Boss")]
    public GameObject prefabBoss;
    public Vector3 posicaoBoss = new Vector3(0f, 5f, 0f);

    [Header("Cenário (opcional — deixe vazio pra manter o fundo da fase anterior)")]
    public Sprite fundoDeTela;
}

// Orquestra as 10 fases do jogo: chama o FormationManager pras fases de inimigos comuns,
// instancia o boss certo nas fases de boss, troca o fundo e a música, e avança
// automaticamente quando a fase atual termina (formação limpa ou boss derrotado).
public class PhaseManager : MonoBehaviour
{
    [SerializeField] private ConfiguracaoDeFase[] fases;
    [SerializeField] private FormationManager formationManager;
    [SerializeField] private SpriteRenderer fundoRenderer;

    private int indiceFaseAtual = -1;

    private void Start()
    {
        formationManager.OnFormacaoLimpa += AvancarFase;
        AvancarFase();
    }

    private void AvancarFase()
    {
        indiceFaseAtual++;

        if (indiceFaseAtual >= fases.Length)
        {
            GameManager.Instancia?.FimDeJogoVitoria();
            return;
        }

        ConfiguracaoDeFase fase = fases[indiceFaseAtual];
        GameManager.Instancia?.AtualizarFase(indiceFaseAtual + 1, fase.nome);

        if (fase.fundoDeTela != null && fundoRenderer != null)
            fundoRenderer.sprite = fase.fundoDeTela;

        if (fase.tipo == TipoDeFase.InimigosComuns)
        {
            AudioManager.Instancia?.TocarMusicaFase();
            formationManager.MontarFormacao(
                fase.linhasDeInimigos, fase.colunas, fase.espacoX, fase.espacoY,
                fase.origemFormacao, fase.velocidadeFormacao);
        }
        else // Boss
        {
            AudioManager.Instancia?.TocarMusicaBoss();
            GameObject obj = Instantiate(fase.prefabBoss, fase.posicaoBoss, Quaternion.identity);
            BossBase boss = obj.GetComponent<BossBase>();
            if (boss != null)
                boss.OnBossDerrotado += AvancarFase;
        }
    }
}
