using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

// Script pequeno: liga o painel de Game Over quando o jogador morre.
// NÃO mexe no PlayerHealth. Coloque este script no CANVAS (objeto sempre ativo).
// Considera "morto" se: EstaVivo for falso, a nave foi desativada ou a nave foi destruída.
public class TelaGameOver : MonoBehaviour
{
    [Header("Arraste aqui")]
    [Tooltip("O PainelGameOver (pode estar desligado)")]
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
        if (painel != null) painel.SetActive(false);
        else Debug.LogWarning("TelaGameOver: o campo Painel está vazio! Arraste o PainelGameOver no Inspector do Canvas.");
    }

    private void Update()
    {
        // Se o ecrã já estiver a mostrar o Game Over, aguarda pelo Enter
        if (mostrando) 
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                Voltar();
            }
            return;
        }

        // A nave pode ser criada depois (prefab), então continua procurando.
        if (jogador == null && !jogadorJaFoiVisto)
        {
            jogador = FindFirstObjectByType<PlayerHealth>();
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

        if (painel != null) painel.SetActive(true);
        if (pausarJogo) Time.timeScale = 0f;

        // Deixa o botão selecionado: assim o ENTER aperta ele pelo sistema UI.
        if (botaoVoltar != null && EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(botaoVoltar.gameObject);
    }

    // Ligue este método no On Click () do botão.
    public void Voltar()
    {
        Time.timeScale = 1f;

        // Força a ida para o menu em vez de reiniciar a cena atual
        SceneManager.LoadScene(nomeCenaMenu);
    }
}