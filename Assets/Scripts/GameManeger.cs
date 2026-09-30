using UnityEngine;
using TMPro;

// Sobrevive à troca de cena (DontDestroyOnLoad), então guarda o estado do jogo inteiro
// (pontos, vida atual). A UI (textos/painéis) muda a cada cena — por isso ela não fica
// fixa aqui, e sim é "registrada" pelo UIDaFase.cs de cada cena via RegistrarUI().
public class GameManager : MonoBehaviour
{
    public static GameManager Instancia { get; private set; }

    private TMP_Text textoPontos;
    private TMP_Text textoFase;
    private TMP_Text textoVidaJogador;
    private TMP_Text textoVidaBoss;
    private GameObject painelFimDeJogo;
    private GameObject painelVitoria;

    private int pontos;
    private float vidaJogadorPercentual = 1f;
    private int numeroFaseAtual;
    private string nomeFaseAtual = "";
    private float vidaBossPercentual = 1f;

    private void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }
        Instancia = this;
        DontDestroyOnLoad(gameObject);
    }

    // Chamado pelo UIDaFase.cs de cada cena, assim que ela carrega.
    // Reconecta os textos/painéis dessa cena e já atualiza com o estado atual do jogo.
    public void RegistrarUI(TMP_Text pontosUI, TMP_Text faseUI, TMP_Text vidaJogadorUI, TMP_Text vidaBossUI, GameObject fimDeJogoUI, GameObject vitoriaUI)
    {
        textoPontos = pontosUI;
        textoFase = faseUI;
        textoVidaJogador = vidaJogadorUI;
        textoVidaBoss = vidaBossUI;
        painelFimDeJogo = fimDeJogoUI;
        painelVitoria = vitoriaUI;

        if (painelFimDeJogo != null) painelFimDeJogo.SetActive(false);
        if (painelVitoria != null) painelVitoria.SetActive(false);

        AtualizarPontos();
        AtualizarFase(numeroFaseAtual, nomeFaseAtual);
        AtualizarVida(vidaJogadorPercentual);
        AtualizarVidaBoss(vidaBossPercentual);
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

    public void AtualizarFase(int numero, string nome)
    {
        numeroFaseAtual = numero;
        nomeFaseAtual = nome;
        if (textoFase != null) textoFase.text = $"FASE {numero} - {nome}";
    }

    public void AtualizarVida(float percentual)
    {
        vidaJogadorPercentual = Mathf.Clamp01(percentual);
        if (textoVidaJogador != null) textoVidaJogador.text = $"VIDA {Mathf.RoundToInt(vidaJogadorPercentual * 100f)}%";
    }

    public void AtualizarVidaBoss(float percentual)
    {
        vidaBossPercentual = Mathf.Clamp01(percentual);
        if (textoVidaBoss != null) textoVidaBoss.text = $"BOSS {Mathf.RoundToInt(vidaBossPercentual * 100f)}%";
    }

    public void FimDeJogo()
    {
        Time.timeScale = 0f;
        if (painelFimDeJogo != null) painelFimDeJogo.SetActive(true);
    }

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