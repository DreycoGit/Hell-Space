using UnityEngine;
using UnityEngine.SceneManagement;

// Lógica comum a todas as fases de inimigos comuns.
// Cada fase (Fase1, Fase4...) herda daqui e só define número, nome, próxima cena e valores.
public abstract class FaseBase : MonoBehaviour
{
    [SerializeField] private FormationManager formationManager;

    [Header("Inimigos desta fase (linha 0 = fileira da frente, mais fraca)")]
    [SerializeField] private GameObject[] linhasDeInimigos;

    protected abstract int NumeroDaFase { get; }
    protected abstract string NomeDaFase { get; }
    protected abstract string ProximaCena { get; } // vazio = última fase
    protected abstract int Colunas { get; }
    protected abstract float OrigemX { get; }
    protected abstract float Velocidade { get; }

    private void Start()
    {
        if (formationManager == null)
        {
            Debug.LogError("FaseBase (" + name + "): o campo 'Formation Manager' está vazio. Arraste o objeto FormationManager da Hierarchy.", this);
            return;
        }

        if (linhasDeInimigos == null || linhasDeInimigos.Length == 0)
        {
            Debug.LogError("FaseBase (" + name + "): 'Linhas De Inimigos' está vazio. Arraste os prefabs dos inimigos (pasta Prefab) para a lista.", this);
            return;
        }

        GameManager.Instancia?.AtualizarFase(NumeroDaFase, NomeDaFase);
        AudioManager.Instancia?.TocarMusicaFase();

        formationManager.OnFormacaoLimpa += IrParaProximaFase;
        formationManager.MontarFormacao(
            linhasDeInimigos, Colunas, 1.5f, 1.2f,
            new Vector3(OrigemX, 4f, 0f), Velocidade);
    }

    private void OnDestroy()
    {
        if (formationManager != null)
            formationManager.OnFormacaoLimpa -= IrParaProximaFase;
    }

    private void IrParaProximaFase()
    {
        if (!string.IsNullOrEmpty(ProximaCena))
            SceneManager.LoadScene(ProximaCena);
        else
            GameManager.Instancia?.FimDeJogoVitoria(); // última fase: tela de vitória
    }
}