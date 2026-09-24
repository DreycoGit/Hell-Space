using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instancia { get; private set; }

    [Header("UI")]
    [SerializeField] private TMP_Text textoPontos;
    [SerializeField] private TMP_Text textoFase;
    [SerializeField] private TMP_Text textoVidaJogador;
    [SerializeField] private TMP_Text textoVidaBoss; // deixe vazio se não tiver boss visível ainda
    [SerializeField] private GameObject painelFimDeJogo;
    [SerializeField] private GameObject painelVitoria;

    private int pontos;
    private float vidaJogadorPercentual = 1f;

    private void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }
        Instancia = this;
    }

    private void Start()
    {
        AtualizarPontos();
        AtualizarVida(vidaJogadorPercentual);
        if (painelFimDeJogo != null) painelFimDeJogo.SetActive(false);
        if (painelVitoria != null) painelVitoria.SetActive(false);
    }

    public void AdicionarPontos(int valor)
    {
        pontos += valor;
        AtualizarPontos();
    }

    private void AtualizarPontos()
    {
        if (textoPontos != null) textoPontos.text = $"PONTOS {pontos}";
    }

    // Chamado pelo PhaseManager toda vez que uma nova fase começa.
    public void AtualizarFase(int numero, string nome)
    {
        if (textoFase != null) textoFase.text = $"FASE {numero} - {nome}";
    }

    // Chamado pelo PlayerHealth sempre que a vida do jogador muda. percentual vai de 0 a 1.
    public void AtualizarVida(float percentual)
    {
        vidaJogadorPercentual = Mathf.Clamp01(percentual);
        if (textoVidaJogador != null) textoVidaJogador.text = $"VIDA {Mathf.RoundToInt(vidaJogadorPercentual * 100f)}%";
    }

    // Chamado pelo BossBase sempre que a vida do boss atual muda.
    public void AtualizarVidaBoss(float percentual)
    {
        if (textoVidaBoss != null) textoVidaBoss.text = $"BOSS {Mathf.RoundToInt(Mathf.Clamp01(percentual) * 100f)}%";
    }

    public void FimDeJogo()
    {
        Time.timeScale = 0f;
        if (painelFimDeJogo != null) painelFimDeJogo.SetActive(true);
    }

    // Chamado quando o jogador vence a Fase 10 (boss final derrotado).
    public void FimDeJogoVitoria()
    {
        Time.timeScale = 0f;
        if (painelVitoria != null) painelVitoria.SetActive(true);
    }

    public void Reiniciar()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
