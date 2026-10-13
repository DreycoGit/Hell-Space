using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

// Script pequeno: liga o painel de Game Over quando o jogador morre.
// NÃO mexe no PlayerHealth. Coloque este script em um objeto SEMPRE LIGADO
// (o Canvas ou o GameOverSistema, nunca no PainelGameOver).
public class TelaGameOver : MonoBehaviour
{
    [Header("Arraste aqui")]
    [Tooltip("O PainelGameOver da CENA (pode estar desligado). Não arraste o arquivo da pasta Prefab.")]
    [SerializeField] private GameObject painel;
    [Tooltip("Botão de voltar. O ENTER aciona ele sozinho.")]
    [SerializeField] private Button botaoVoltar;

    [Header("Configuração")]
    [Tooltip("Nome EXATO da cena do menu (precisa estar nas Build Settings)")]
    [SerializeField] private string nomeCenaMenu = "MenuPrincipal";
    [SerializeField] private bool pausarJogo = true;

    private PlayerHealth jogador;
    private bool jogadorJaFoiVisto;
    private bool mostrando;

    private void Start()
    {
        if (painel == null)
        {
            Debug.LogWarning("TelaGameOver: o campo Painel está vazio! Arraste o PainelGameOver da Hierarchy no Inspector.");
            return;
        }

        if (!painel.scene.IsValid())
            Debug.LogWarning("TelaGameOver: o campo Painel aponta para o ARQUIVO da pasta Prefab, não para o objeto da cena! Arraste o PainelGameOver da Hierarchy.");

        painel.SetActive(false);
    }

    private void Update()
    {
        if (mostrando) return;

        // A nave pode ser criada depois (prefab), então continua procurando.
        if (jogador == null && !jogadorJaFoiVisto)
        {
            jogador = FindAnyObjectByType<PlayerHealth>();
            if (jogador != null)
            {
                jogadorJaFoiVisto = true;
                Debug.Log("TelaGameOver: jogador encontrado (" + jogador.name + ").");
            }
            return;
        }

        // 1) A nave foi destruída (Destroy)
        if (jogador == null)
        {
            Debug.Log("TelaGameOver: a nave sumiu (foi destruída). Mostrando Game Over.");
            Mostrar();
            return;
        }

        // 2) A nave morreu (EstaVivo falso) ou foi desativada (SetActive(false))
        if (!jogador.EstaVivo || !jogador.gameObject.activeInHierarchy)
        {
            Debug.Log("TelaGameOver: jogador morreu. Mostrando Game Over.");
            Mostrar();
        }
    }

    private void Mostrar()
    {
        mostrando = true;

        if (painel == null)
        {
            Debug.LogError("TelaGameOver: não dá para mostrar, o campo Painel está VAZIO. Arraste o PainelGameOver no Inspector.");
        }
        else
        {
            // Liga qualquer objeto "pai" que esteja desligado (senão o painel não aparece)
            Transform t = painel.transform.parent;
            while (t != null)
            {
                if (!t.gameObject.activeSelf)
                {
                    Debug.LogWarning("TelaGameOver: o pai '" + t.name + "' estava DESLIGADO. Ligando.");
                    t.gameObject.SetActive(true);
                }
                t = t.parent;
            }

            painel.SetActive(true);
            painel.transform.SetAsLastSibling(); // desenha por cima dos irmãos

            // Se algum CanvasGroup invisível estiver em volta, deixa visível
            foreach (var grupo in painel.GetComponentsInParent<CanvasGroup>(true))
                if (grupo.alpha < 1f) grupo.alpha = 1f;
        }

        if (pausarJogo) Time.timeScale = 0f;

        // Deixa o botão selecionado: assim o ENTER aperta ele.
        if (botaoVoltar != null && EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(botaoVoltar.gameObject);
    }

    // Ligue este método no On Click () do botão.
    public void Voltar()
    {
        Time.timeScale = 1f;

        // Zera pontos, vida e destrói a nave persistente (método do próprio GameManager).
        if (GameManager.Instancia != null)
            GameManager.Instancia.NovoJogo();

        if (Application.CanStreamedLevelBeLoaded(nomeCenaMenu))
            SceneManager.LoadScene(nomeCenaMenu);
        else
        {
            Debug.LogWarning("TelaGameOver: a cena '" + nomeCenaMenu +
                "' não está nas Build Settings. Reiniciando a fase.");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}