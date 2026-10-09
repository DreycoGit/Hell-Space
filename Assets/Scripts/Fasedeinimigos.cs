using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Coloque este script num objeto vazio nas cenas de fase com inimigos comuns
// (fases 1, 2, 4, 5, 7 e 8). Ele monta a formação ao iniciar a cena e carrega
// a próxima cena automaticamente quando todos os inimigos forem derrotados.
public class FaseDeInimigos : MonoBehaviour
{
    [SerializeField] private FormationManager formationManager;

    [Header("Inimigos desta fase (linha 0 = fileira da frente, mais fraca)")]
    [SerializeField] private GameObject[] linhasDeInimigos;
    [SerializeField] private int colunas = 6;
    [SerializeField] private float espacoX = 1.5f;
    [SerializeField] private float espacoY = 1.2f;
    [SerializeField] private Vector3 origemFormacao = new Vector3(-4f, 4f, 0f);
    [SerializeField] private float velocidadeFormacao = 1.5f;

    [Header("Transição de cena")]
    [SerializeField] private string nomeDaFase = "Fase";
    [SerializeField] private int numeroDaFase = 1;
    [SerializeField] private string nomeDaProximaCena; // deixe vazio se for a última
    [SerializeField] private float atrasoParaTrocar = 0.5f;

    private bool trocando;

    private void Start()
    {
        GameManager.Instancia?.AtualizarFase(numeroDaFase, nomeDaFase);
        AudioManager.Instancia?.TocarMusicaFase();

        formationManager.OnFormacaoLimpa += IrParaProximaFase;
        formationManager.MontarFormacao(linhasDeInimigos, colunas, espacoX, espacoY, origemFormacao, velocidadeFormacao);
    }

    private void OnDestroy()
    {
        if (formationManager != null) formationManager.OnFormacaoLimpa -= IrParaProximaFase;
    }

    // O último inimigo morre dentro do OnTriggerEnter2D do tiro (callback de física).
    // Trocar de cena ali mesmo é arriscado, então a troca vai pra uma corrotina,
    // igual ao TrocaDeFaseBoss, e só acontece uma vez.
    private void IrParaProximaFase()
    {
        if (trocando) return;
        trocando = true;
        StartCoroutine(Trocar());
    }

    private IEnumerator Trocar()
    {
        yield return new WaitForSeconds(atrasoParaTrocar);
        Time.timeScale = 1f;
        if (!string.IsNullOrEmpty(nomeDaProximaCena))
            SceneManager.LoadScene(nomeDaProximaCena);
    }
}
