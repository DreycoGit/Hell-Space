using UnityEngine;
using TMPro; // se não usar TextMeshPro, troque por UnityEngine.UI.Text

public class GameManager : MonoBehaviour
{
    public static GameManager Instancia { get; private set; }

    [Header("UI")]
    [SerializeField] private TMP_Text textoPontos;
    [SerializeField] private TMP_Text textoOnda;
    [SerializeField] private TMP_Text textoVidas;
    [SerializeField] private GameObject painelFimDeJogo;

    private int pontos;
    private int onda = 1;
    private int vidas = 3;

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
        AtualizarOnda(onda);
        AtualizarVidas(vidas);
        if (painelFimDeJogo != null) painelFimDeJogo.SetActive(false);
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

    public void AtualizarOnda(int novaOnda)
    {
        onda = novaOnda;
        if (textoOnda != null) textoOnda.text = $"ONDA {onda}";
    }

    public void AtualizarVidas(int novasVidas)
    {
        vidas = novasVidas;
        if (textoVidas != null) textoVidas.text = $"VIDAS {Mathf.Max(0, vidas)}";
    }

    public void FimDeJogo()
    {
        Time.timeScale = 0f;
        if (painelFimDeJogo != null) painelFimDeJogo.SetActive(true);
    }

    public void Reiniciar()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}