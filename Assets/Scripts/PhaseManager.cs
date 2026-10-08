/*using UnityEngine;

public enum TipoDeFase { InimigosComuns, Boss, InimigosAleatorios }

[System.Serializable]
public class InimigoNaFase
{
    public GameObject prefab;
    public int quantidade = 5;
}

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

    [Header("Se for InimigosAleatorios")]
    public InimigoNaFase[] inimigosAleatorios;
    public int inimigosPorOnda = 3;
    public float intervaloEntreOndas = 2f;
    public Vector2 areaMin = new Vector2(-6f, 2.5f);
    public Vector2 areaMax = new Vector2(6f, 5f);
    public float distanciaMinima = 1.2f;

    [Header("Se for Boss")]
    public GameObject prefabBoss;
    public Vector3 posicaoBoss = new Vector3(0f, 5f, 0f);

    [Header("Cenário (opcional — deixe vazio pra manter o fundo da fase anterior)")]
    public Sprite fundoDeTela;
}

// Orquestra as 10 fases do jogo: chama o FormationManager pras fases de inimigos comuns,
// o SpawnerAleatorio pras fases aleatórias, instancia o boss certo nas fases de boss,
// troca o fundo e a música, e avança automaticamente quando a fase atual termina.
public class PhaseManager : MonoBehaviour
{
    [SerializeField] private ConfiguracaoDeFase[] fases;
    [SerializeField] private FormationManager formationManager;
    [SerializeField] private SpawnerAleatorio spawnerAleatorio;
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
        else if (fase.tipo == TipoDeFase.InimigosAleatorios)
        {
            AudioManager.Instancia?.TocarMusicaFase();
            spawnerAleatorio.Iniciar(fase, AvancarFase);
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
}*/